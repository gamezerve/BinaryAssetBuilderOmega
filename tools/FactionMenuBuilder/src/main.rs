// Reborn: preserve the original Uprising controller and import RA3's original faction art as an isolated library.
use apt::{
    actions::{ActionStream, DictRefKind, Instruction as I, PushStringKind},
    *,
};
use std::{collections::HashMap, path::Path};

// Reborn: assemble ordinary AVM1 instructions without changing existing branch targets or constant pools.
#[derive(Default)]
struct Code(Vec<I>);
impl Code {
    // Reborn: the shipped interpreter uses inline literals and dictionary references, not nested synthesized Push pools.
    fn string(&mut self, s: &str) {
        self.0.push(I::PushString {
            kind: PushStringKind::Push,
            value: s.into(),
        });
    }
    fn int(&mut self, n: i32) {
        self.0.push(if (0..=255).contains(&n) {
            I::PushByte(n as u8)
        } else {
            I::PushDWord(n as u32)
        });
    }
    fn float(&mut self, n: f32) {
        self.0.push(I::PushFloat(n));
    }
    fn bool(&mut self, b: bool) {
        self.op(if b { 0x73 } else { 0x74 });
    }
    fn op(&mut self, op: u8) {
        self.0.push(I::Simple(op));
    }
    fn path(&mut self, p: &[&str]) {
        self.string(p[0]);
        self.op(0x1c);
        for part in &p[1..] {
            self.string(part);
            self.op(0x4e);
        }
    }
    // Reborn: methods receive the live screen controller as preloaded this; m_thisClass on the constructor is not that instance.
    fn owner(&mut self) {
        self.0.push(I::PushRegister(1));
    }
    // Reborn: the stock base constructor stores m_screen on Cafe2_BaseUIScreen, not on the controller instance.
    fn screen(&mut self) {
        self.path(&["_global", "Cafe2_BaseUIScreen", "m_screen"]);
    }
    fn clip(&mut self, name: &str) {
        self.screen();
        for part in name.split('.') {
            self.string(part);
            self.op(0x4e);
        }
    }
    fn owner_prop(&mut self, name: &str) {
        self.owner();
        self.string(name);
        self.op(0x4e);
    }
    fn set_owner_bool(&mut self, name: &str, value: bool) {
        self.owner();
        self.string(name);
        self.bool(value);
        self.op(0x4f);
    }
    fn visible(&mut self, name: &str, value: bool) {
        self.clip(name);
        self.string("_visible");
        self.bool(value);
        self.op(0x4f);
    }
    fn call(&mut self, name: &str) {
        self.int(0);
        self.owner();
        self.string(name);
        self.op(0x52);
        self.op(0x17);
    }
    fn clip_call(&mut self, clip: &str, name: &str) {
        self.int(0);
        self.clip(clip);
        self.string(name);
        self.op(0x52);
        self.op(0x17);
    }
    // Reborn: the original screen plays faction and completion sounds through the global UI sound helper.
    fn sound(&mut self, sound: &str) {
        self.string(sound);
        self.int(1);
        self.path(&["_global"]);
        self.string("playSound");
        self.op(0x52);
        self.op(0x17);
    }
    fn title(&mut self, key: &str) {
        for (clip, suffix) in [
            ("rebornTitleBar.titleTF", ""),
            ("rebornTitleBar.shadowTF", "&Outline"),
        ] {
            self.clip(clip);
            self.string("text");
            self.string(&format!("{key}{suffix}"));
            self.op(0x4f);
        }
    }
    fn bind(&mut self, clip: &str, method: &str, event: &str) {
        self.owner_prop(method);
        self.owner();
        self.int(2);
        // Reborn: use the inherited bind0 implementation explicitly defined on Cafe2_BaseUIScreen.prototype.
        self.owner();
        self.string("bind0");
        self.op(0x52);
        self.int(1);
        self.clip(clip);
        self.string(event);
        self.op(0x52);
        self.op(0x17);
    }
    fn body(mut self) -> ActionStream {
        self.0.push(I::End);
        ActionStream {
            instructions: self.0,
        }
    }
    // Reborn: function bodies are length-delimited; an embedded End prematurely stops the native relocation scan.
    fn function_body(self) -> ActionStream {
        ActionStream {
            instructions: self.0,
        }
    }
}

// Reborn: locate the original named placement rather than depending on its character index.
fn placement(file: &AptFile, name: &str) -> PlaceObject {
    file.movie.frames[0]
        .controls
        .iter()
        .find_map(|c| match c {
            Control::PlaceObject(p) if p.name.as_deref() == Some(name) => Some(p.clone()),
            _ => None,
        })
        .unwrap_or_else(|| panic!("Missing original placement: {name}"))
}

// Reborn: change only literal labels/export names inside copied clip initialization and sprite actions.
fn strings(actions: &mut ActionStream, changes: &HashMap<String, String>) {
    for instruction in &mut actions.instructions {
        match instruction {
            I::Push(v) | I::DefineDictionary(v) => {
                for value in v {
                    if let Value::String(s) = value {
                        if let Some(new) = changes.get(s) {
                            *s = new.clone();
                        }
                    }
                }
            }
            I::PushString { value, .. } => {
                if let Some(new) = changes.get(value) {
                    *value = new.clone();
                }
            }
            I::DefineFunction { body, .. } | I::DefineFunction2 { body, .. } => {
                strings(body, changes)
            }
            _ => (),
        }
    }
}

// Reborn: walk existing clip actions while preserving the original timeline, layout and animation code.
fn frame_strings(frames: &mut [Frame], changes: &HashMap<String, String>) {
    for frame in frames {
        for control in &mut frame.controls {
            match control {
                Control::Action(a) | Control::InitAction { actions: a, .. } => strings(a, changes),
                Control::PlaceObject(p) => {
                    if let Some(events) = &mut p.clip_actions {
                        for event in events {
                            strings(&mut event.actions, changes);
                        }
                    }
                }
                _ => (),
            }
        }
    }
}

// Reborn: add a controller method using normal AVM1 functions, retaining all original native-facing callbacks.
fn method(patch: &mut Code, name: &str, code: Code) {
    parameter_method(patch, name, code, vec![]);
}

// Reborn: explicit parameter registers keep the intercepted stock completion callback's faction argument intact.
fn parameter_method(patch: &mut Code, name: &str, code: Code, params: Vec<(u32, String)>) {
    patch.path(&["_global", "fes_m_factionMenu", "prototype"]);
    patch.string(name);
    // Reborn: match the shipped screen-method ABI instead of the old unregistered DefineFunction representation.
    patch.0.push(I::DefineFunction2 {
        name: String::new(),
        register_count: 2 + params.len() as i16,
        params,
        flags: 1,
        body: code.function_body(),
    });
    patch.op(0x4f);
}

// Reborn: capture one dictionary for new methods, so function bodies contain no deferred literal pointers or Push arrays.
fn dictionary_code(stream: &mut ActionStream, dictionary: &mut Vec<Value>) {
    for instruction in &mut stream.instructions {
        match instruction {
            I::PushString {
                kind: PushStringKind::Push,
                value,
            } => {
                let value = Value::String(value.clone());
                let index = dictionary
                    .iter()
                    .position(|v| *v == value)
                    .unwrap_or_else(|| {
                        dictionary.push(value);
                        dictionary.len() - 1
                    });
                *instruction = I::DictRef {
                    kind: if index <= 255 {
                        DictRefKind::PushByte
                    } else {
                        DictRefKind::PushWord
                    },
                    index: index as u16,
                };
            }
            I::DefineFunction2 { body, .. } => dictionary_code(body, dictionary),
            _ => (),
        }
    }
}

// Reborn: the game scans inline function bytecode during relocation; only the outer action stream may contain End.
fn verify_function_boundaries(stream: &ActionStream) {
    for instruction in &stream.instructions {
        if let I::DefineFunction2 { body, .. } | I::DefineFunction { body, .. } = instruction {
            assert!(
                !body.instructions.iter().any(|i| matches!(i, I::End)),
                "Embedded End stops native APT relocation before the remaining code"
            );
            verify_function_boundaries(body);
        }
    }
}

// Reborn: install helper methods in the original class initializer, then call setup from the original initGUI itself.
fn install_lifecycle_hook(file: &mut AptFile, patch: ActionStream) {
    let mut changed = 0;
    let mut completion_hooked = 0;
    for control in &mut file.movie.frames[0].controls {
        let Control::InitAction { actions, .. } = control else {
            continue;
        };
        let Some(dictionary) = actions.instructions.iter().find_map(|i| match i {
            I::DefineDictionary(v) => Some(v.clone()),
            _ => None,
        }) else {
            continue;
        };
        if !dictionary.contains(&Value::String("fes_m_factionMenu".into())) {
            continue;
        }
        for index in 1..actions.instructions.len() {
            let name = match &actions.instructions[index - 1] {
                I::DictRef { index, .. } => dictionary.get(*index as usize),
                _ => None,
            };
            if name == Some(&Value::String("configureCompleteAnimation".into())) {
                // Reborn: extra campaigns use the RA3-sized animation after page setup; retain the stock implementation for indices 0..3.
                actions.instructions[index - 1] = I::PushString {
                    kind: PushStringKind::Push,
                    value: "rebornOriginalCompleteAnimation".into(),
                };
                completion_hooked += 1;
                continue;
            }
            if name != Some(&Value::String("initGUI".into())) {
                continue;
            }
            let I::DefineFunction2 { body, flags, .. } = &mut actions.instructions[index] else {
                continue;
            };
            assert_ne!(
                *flags & 1,
                0,
                "Original initGUI must preload this in register 1"
            );
            assert!(
                !body
                    .instructions
                    .iter()
                    .any(|i| matches!(i, I::Simple(0x3e) | I::End)),
                "Review early-return original initialization before hooking it"
            );
            // Reborn: no dictionary change inside the stock method; the inline method name uses the native relocation path.
            body.instructions.extend([
                I::PushByte(0),
                I::PushRegister(1),
                I::PushString {
                    kind: PushStringKind::Push,
                    value: "rebornInitializePages".into(),
                },
                I::Simple(0x5d),
            ]);
            changed += 1;
        }
        if changed != 0 {
            assert!(matches!(actions.instructions.last(), Some(I::End)));
            actions.instructions.pop();
            // Reborn: retain the old end index as the new patch entry, including the class-already-defined branch.
            actions.instructions.extend(patch.instructions.clone());
        }
    }
    assert_eq!(
        changed, 1,
        "Expected the original faction menu initGUI exactly once"
    );
    assert_eq!(
        completion_hooked, 1,
        "Expected the original completion animation callback exactly once"
    );
}

// Reborn: use the original painted flag size to carry the stock frame's complete relative transform onto RA3 artwork.
fn flag_bounds(file: &AptFile, export: &str) -> Rect {
    let id = file
        .movie
        .exports
        .iter()
        .find(|e| e.name == export)
        .unwrap()
        .character_id;
    fn first_shape(file: &AptFile, id: u32) -> Rect {
        match &file.movie.characters[id as usize] {
            CharacterSlot::Character(Character::Shape(shape)) => shape.bounds,
            CharacterSlot::Character(Character::Sprite(sprite)) => {
                let p = sprite.frames[0]
                    .controls
                    .iter()
                    .find_map(|c| match c {
                        Control::PlaceObject(p) if p.character_id.is_some() => Some(p),
                        _ => None,
                    })
                    .unwrap();
                let b = first_shape(file, p.character_id.unwrap() as u32);
                let m = p.matrix.unwrap_or_default();
                assert_eq!(m.b, 0.0);
                assert_eq!(m.c, 0.0);
                Rect {
                    left: b.left * m.a + m.tx,
                    right: b.right * m.a + m.tx,
                    top: b.top * m.d + m.ty,
                    bottom: b.bottom * m.d + m.ty,
                }
            }
            _ => panic!("Expected original flag artwork"),
        }
    }
    first_shape(file, id)
}

// Reborn: compare the generated native movie model after serialization before writing either output file.
fn save(file: &AptFile, output: &Path) {
    let (apt, constants) = file
        .write(&apt::write::WriteOptions::from_header(&file.header))
        .unwrap();
    let reread = AptFile::read(&apt, &constants).unwrap();
    assert_eq!(reread.movie, file.movie, "Generated APT did not round-trip");
    std::fs::write(output.with_extension("apt"), apt).unwrap();
    std::fs::write(output.with_extension("const"), constants).unwrap();
}

// Reborn: derive all campaign indices from the mod's seven-entry layout: original four, SovietRA3, AlliesRA3, JapanRA3.
fn main() {
    let args: Vec<String> = std::env::args().collect();
    assert_eq!(
        args.len(),
        5,
        "usage: reborn-faction-menu <EP1.apt> <RA3.apt> <output.apt> <art-library.apt>"
    );
    let mut ep1 = AptFile::load(Path::new(&args[1])).unwrap();
    let mut ra3 = AptFile::load(Path::new(&args[2])).unwrap();
    let mut ra3_positions = [
        placement(&ra3, "sovietFaction"),
        placement(&ra3, "alliesFaction"),
        placement(&ra3, "japaneseFaction"),
    ];
    let ep1_positions = [
        placement(&ep1, "sovietFaction"),
        placement(&ep1, "alliesFaction"),
        placement(&ep1, "japanFaction"),
    ];
    let ep1_frames = [
        placement(&ep1, "sovietCompleteFrame"),
        placement(&ep1, "alliesCompleteFrame"),
        placement(&ep1, "japanCompleteFrame"),
    ];
    let mut frame_matrices = Vec::new();
    for (index, symbol) in [
        "SovietFactionButtonContent Symbol",
        "AlliedFactionButtonContent Symbol",
        "JapaneseFactionButtonContent Symbol",
    ]
    .iter()
    .enumerate()
    {
        let before = flag_bounds(&ep1, symbol);
        let after = flag_bounds(&ra3, symbol);
        // Reborn: reduce each larger RA3 banner around its own center so both the banner and earned-star frame remain inside the campaign panel.
        let mut target = ra3_positions[index].matrix.unwrap();
        let local_x = (after.left + after.right) * 0.5;
        let local_y = (after.top + after.bottom) * 0.5;
        let center_x = target.tx + local_x * target.a;
        let center_y = target.ty + local_y * target.d;
        target.a *= 0.94;
        target.d *= 0.94;
        target.tx = center_x - local_x * target.a;
        target.ty = center_y - local_y * target.d;
        ra3_positions[index].matrix = Some(target);
        let stock = ep1_positions[index].matrix.unwrap();
        let target = ra3_positions[index].matrix.unwrap();
        let sx = (after.right - after.left) * target.a / ((before.right - before.left) * stock.a);
        let sy = (after.bottom - after.top) * target.d / ((before.bottom - before.top) * stock.d);
        let border = ep1_frames[index].matrix.unwrap();
        frame_matrices.push(Matrix {
            a: border.a * sx,
            d: border.d * sy,
            tx: target.tx + (border.tx - stock.tx) * sx,
            ty: target.ty + (border.ty - stock.ty) * sy,
            ..Default::default()
        });
    }
    // Reborn: name the original page-only backdrop and shared title without changing their original positions or artwork.
    let title_id = ep1
        .movie
        .imports
        .iter()
        .find(|i| i.name == "ScreenTitleBarSymbol")
        .unwrap()
        .character_id as i32;
    // Reborn: keep the original lower bar above both campaign backdrops and below the navigation buttons.
    let lower_bar_id = ep1
        .movie
        .imports
        .iter()
        .find(|i| i.name == "lowerScreenBar Symbol")
        .unwrap()
        .character_id as i32;
    // Reborn: the RA3 backdrop already contains its own full-screen artwork.
    let uprising_background_id = ep1
        .movie
        .imports
        .iter()
        .find(|i| i.name == "Background_mc")
        .unwrap()
        .character_id as i32;
    for control in &mut ep1.movie.frames[0].controls {
        if let Control::PlaceObject(p) = control {
            if p.depth == 20 {
                p.name = Some("rebornUprisingBackdrop".into());
            }
            if p.character_id == Some(title_id) {
                p.name = Some("rebornTitleBar".into());
            }
            if p.character_id == Some(lower_bar_id) {
                p.depth = 22;
            }
            if p.character_id == Some(uprising_background_id) {
                p.name = Some("rebornUprisingBackground".into());
            }
        }
    }
    let mut changes = HashMap::new();
    for symbol in [
        "SovietFactionButtonContent Symbol",
        "AlliedFactionButtonContent Symbol",
        "JapaneseFactionButtonContent Symbol",
    ] {
        changes.insert(symbol.to_string(), format!("RebornRA3{symbol}"));
    }
    // Reborn: the art library has no screen constructor or game-state side effects; only the three original flag widgets are instantiated.
    let mut page = Frame::default();
    // Reborn: the original RA3 backdrop has no Yuriko separator; keep it behind the shared description and title widgets.
    let ra3_backdrop = ra3.movie.frames[0]
        .controls
        .iter()
        .find_map(|c| match c {
            Control::PlaceObject(p) if p.depth == 2 => Some(p.clone()),
            _ => None,
        })
        .unwrap();
    page.controls.push(Control::PlaceObject(ra3_backdrop));
    for (index, name) in [
        "sovietFaction",
        "alliesFaction",
        "japaneseFaction",
        "factionButtonController",
    ]
    .iter()
    .enumerate()
    {
        let p = if index < 3 {
            ra3_positions[index].clone()
        } else {
            placement(&ra3, name)
        };
        page.controls.push(Control::PlaceObject(p));
    }
    frame_strings(std::slice::from_mut(&mut page), &changes);
    let page_id = ra3.movie.characters.len() as u32;
    ra3.movie
        .characters
        .push(CharacterSlot::Character(Character::Sprite(Sprite {
            frames: vec![page],
        })));
    for export in &mut ra3.movie.exports {
        if let Some(name) = changes.get(&export.name) {
            export.name = name.clone();
        }
    }
    ra3.movie.exports.push(Export {
        name: "RebornRA3FactionPage".into(),
        character_id: page_id,
    });
    // Reborn: composite the exact RA3 background over the right end of the shared animated title bar.
    let fade_bitmap = ra3.movie.characters.len();
    assert_eq!(fade_bitmap, 35, "Update the fade DAT and geometry IDs if the stock movie changes");
    ra3.movie.characters.push(CharacterSlot::Character(Character::Bitmap));
    let fade_shape = ra3.movie.characters.len() as i32;
    ra3.movie.characters.push(CharacterSlot::Character(Character::Shape(Shape {
        bounds: Rect { left: 955.0, top: 0.0, right: 1365.0, bottom: 83.0 },
        bitmap_character_id: None,
    })));
    let fade_sprite = ra3.movie.characters.len() as u32;
    ra3.movie.characters.push(CharacterSlot::Character(Character::Sprite(Sprite {
        frames: vec![Frame { controls: vec![Control::PlaceObject(PlaceObject {
            depth: 1, character_id: Some(fade_shape), matrix: Some(Matrix::default()), ..Default::default()
        })] }],
    })));
    ra3.movie.exports.push(Export { name: "RebornRA3TitleFade".into(), character_id: fade_sprite });
    ra3.movie.frames = vec![Frame::default()];

    // Reborn: keep the Uprising stage and all existing placements intact, with the separate RA3 page initially hidden.
    let imported = ep1.movie.characters.len() as u32;
    ep1.movie.characters.push(CharacterSlot::Empty);
    ep1.movie.imports.push(Import {
        movie: "screens/fes_m_ra3FactionAssets".into(),
        name: "RebornRA3FactionPage".into(),
        character_id: imported,
    });
    let mut hidden = Code::default();
    hidden.path(&["this"]);
    hidden.string("_visible");
    hidden.bool(false);
    hidden.op(0x4f);
    let hide_on_construct = hidden.body();
    ep1.movie.frames[0]
        .controls
        .push(Control::PlaceObject(PlaceObject {
            // Reborn: cover the Uprising panel at depth 20 while keeping the shared lower bar above this page.
            depth: 21,
            character_id: Some(imported as i32),
            matrix: Some(Matrix::default()),
            name: Some("rebornRa3Page".into()),
            clip_actions: Some(vec![EventAction {
                triggers: 1,
                key_code: 0,
                actions: hide_on_construct.clone(),
            }]),
            ..Default::default()
        }));
    let imported_fade = ep1.movie.characters.len() as u32;
    ep1.movie.characters.push(CharacterSlot::Empty);
    ep1.movie.imports.push(Import {
        movie: "screens/fes_m_ra3FactionAssets".into(),
        name: "RebornRA3TitleFade".into(),
        character_id: imported_fade,
    });
    ep1.movie.frames[0].controls.push(Control::PlaceObject(PlaceObject {
        depth: 68, character_id: Some(imported_fade as i32), matrix: Some(Matrix::default()),
        name: Some("rebornRa3TitleFade".into()),
        clip_actions: Some(vec![EventAction { triggers: 1, key_code: 0, actions: hide_on_construct }]),
        ..Default::default()
    }));
    for (i, matrix) in frame_matrices.iter().enumerate() {
        let mut frame = placement(&ep1, "sovietCompleteFrame");
        frame.name = Some(format!("rebornRa3Frame{i}"));
        frame.depth = 77 + i as i32 * 2;
        frame.matrix = Some(*matrix);
        let mut a = Code::default();
        a.path(&["this"]);
        a.string("_visible");
        a.bool(false);
        a.op(0x4f);
        frame.clip_actions = Some(vec![EventAction {
            triggers: 1,
            key_code: 0,
            actions: a.body(),
        }]);
        ep1.movie.frames[0]
            .controls
            .push(Control::PlaceObject(frame));
    }
    // Reborn: play the retail completion timeline on the same transformed coordinate system as each static RA3 frame.
    let anchor_id = placement(&ep1, "completeAnimationAnchor").character_id;
    ep1.movie.frames[0]
        .controls
        .push(Control::PlaceObject(PlaceObject {
            depth: 84,
            character_id: anchor_id,
            matrix: Some(Matrix::default()),
            name: Some("rebornRa3EffectAnchor".into()),
            ..Default::default()
        }));
    // Reborn: align Back and Continue to the campaign panel edges and distribute both page tabs through the three equal intervening gaps.
    let navigation_x = [75.0, 425.0, 775.0, 1125.0];
    for name in ["backButton", "playButton"] {
        for control in &mut ep1.movie.frames[0].controls {
            if let Control::PlaceObject(p) = control {
                if p.name.as_deref() == Some(name) {
                    let mut matrix = p.matrix.unwrap();
                    matrix.tx = if name == "backButton" {
                        navigation_x[0]
                    } else {
                        navigation_x[3]
                    };
                    p.matrix = Some(matrix);
                }
            }
        }
    }
    for (i, label) in ["$RebornUI:CampaignUprising", "$RebornUI:CampaignRA3"]
        .iter()
        .enumerate()
    {
        let mut tab = placement(&ep1, "backButton");
        tab.name = Some(format!("rebornPageTab{i}"));
        tab.depth = 90 + i as i32 * 2;
        tab.matrix = Some(Matrix {
            a: 1.0,
            d: 1.0,
            tx: navigation_x[1 + i],
            ty: 693.0,
            ..Default::default()
        });
        if let Some(events) = &mut tab.clip_actions {
            let replacements = HashMap::from([
                ("$Back".into(), label.to_string()),
                ("$BACK".into(), label.to_string()),
            ]);
            for event in events {
                strings(&mut event.actions, &replacements);
            }
        }
        ep1.movie.frames[0].controls.push(Control::PlaceObject(tab));
    }
    let originals = [
        "rebornUprisingBackground",
        "rebornUprisingBackdrop",
        "sovietFaction",
        "alliesFaction",
        "japanFaction",
        "yurikoFaction",
        "sovietCompleteFrame",
        "alliesCompleteFrame",
        "japanCompleteFrame",
        "yurikoCompleteFrame",
        "alliesLocked",
        "japanLocked",
        "yurikoLocked",
        "revealAnimation",
        "sovietCompleteAnimation",
        "alliesCompleteAnimation",
        "japanCompleteAnimation",
        "yurikoCompleteAnimation",
    ];
    let mut patch = Code::default();
    // Reborn: the stock initGUI calls this setup directly; do not replace a method after the screen has already initialized.
    let mut init = Code::default();
    init.set_owner_bool("rebornRa3Active", false);
    // Reborn: bind page navigation before preparing hidden imported widgets.
    for i in 0..2 {
        init.clip_call(&format!("rebornPageTab{i}"), "show");
        init.clip_call(&format!("rebornPageTab{i}"), "enable");
        init.bind(
            &format!("rebornPageTab{i}"),
            if i == 0 {
                "rebornShowUprising"
            } else {
                "rebornShowRA3"
            },
            "setOnMouseUpFunction",
        );
    }
    // Reborn: Uprising is the initial page, so its tab stays visibly disabled while the RA3 tab retains stock hover and click behavior.
    init.clip_call("rebornPageTab0", "disable");
    init.visible("rebornRa3Page", false);
    for i in 0..3 {
        let clip = format!(
            "rebornRa3Page.{}",
            ["sovietFaction", "alliesFaction", "japaneseFaction"][i]
        );
        init.clip_call(&clip, "noIntro");
        init.clip_call(&clip, "enable");
        init.bind(
            &clip,
            &format!("rebornClickRa3{i}"),
            "setOnMouseDownFunction",
        );
    }
    // Reborn: use the native completion result to return to the matching RA3 page and faction, while ordinary entry stays on Uprising.
    for i in 0..3 {
        init.owner_prop("m_justUnlockedCampaign");
        init.int(4 + i);
        init.op(0x49);
        let skip = init.0.len();
        init.0.push(I::BranchIfFalse { target: 0 });
        init.call("rebornShowRA3");
        init.call(&format!("rebornChooseRa3{i}"));
        init.call(&format!("rebornCompleteRa3{i}"));
        let end = init.0.len();
        init.0[skip] = I::BranchIfFalse { target: end };
    }
    method(&mut patch, "rebornInitializePages", init);
    for (i, description) in [
        "$RebornUI:RA3SovietDescription",
        "$RebornUI:RA3AlliesDescription",
        "$RebornUI:RA3JapanDescription",
    ]
    .iter()
    .enumerate()
    {
        let mut select = Code::default();
        select.path(&["GAME"]);
        select.string("MODE");
        select.path(&["ENUM", "GAME_MODE_CAMPAIGN"]);
        select.op(0x4f);
        select.path(&["GAME"]);
        select.string("CAMPAIGN_TYPE");
        select.int(4 + i as i32);
        select.op(0x4f);
        for (name, suffix) in [
            ("factionDescriptionTF", ""),
            ("factionDescriptionShadowTF", "&Outline"),
        ] {
            select.clip(name);
            select.string("text");
            select.string(&format!("{description}{suffix}"));
            select.op(0x4f);
        }
        select.clip_call("playButton", "enable");
        // Reborn: the shared Continue callback uses GAME.CAMPAIGN_TYPE; button selection uses RA3's original radio controller.
        select.clip(&format!(
            "rebornRa3Page.{}",
            ["sovietFaction", "alliesFaction", "japaneseFaction"][i]
        ));
        select.int(1);
        select.clip("rebornRa3Page.factionButtonController");
        select.string("SetSelectedRadioButton");
        select.op(0x52);
        select.op(0x17);
        method(&mut patch, &format!("rebornChooseRa3{i}"), select);
        let mut click = Code::default();
        click.call(&format!("rebornChooseRa3{i}"));
        click.sound(
            [
                "GuiSovietFactionSelection",
                "GuiAlliedFactionSelection",
                "GuiJapanFactionSelection",
            ][i],
        );
        method(&mut patch, &format!("rebornClickRa3{i}"), click);
    }
    let mut show = Code::default();
    // Reborn: keep both full-size page buttons enabled for their stock hover animation; selecting the already-open RA3 page is a safe no-op.
    show.owner_prop("rebornRa3Active");
    let already_ra3 = show.0.len();
    show.0.push(I::BranchIfFalse { target: 0 });
    show.op(0x76);
    show.op(0x3e);
    let begin_ra3 = show.0.len();
    show.0[already_ra3] = I::BranchIfFalse { target: begin_ra3 };
    // Reborn: retain each live Uprising visibility state for restoration after browsing the RA3 page.
    show.owner();
    show.string("rebornUprisingFaction");
    show.int(1);
    show.op(0x4f);
    for faction in 1..=4 {
        show.owner_prop("m_currentSelectedFaction");
        show.int(faction);
        show.op(0x49);
        let skip = show.0.len();
        show.0.push(I::BranchIfFalse { target: 0 });
        show.owner();
        show.string("rebornUprisingFaction");
        show.int(faction);
        show.op(0x4f);
        let end = show.0.len();
        show.0[skip] = I::BranchIfFalse { target: end };
    }
    show.set_owner_bool("rebornRa3Active", true);
    show.title("$RebornUI:RA3CampaignTitle");
    for name in originals {
        show.clip(name);
        show.string("rebornPreviousVisibility");
        show.clip(name);
        show.string("_visible");
        show.op(0x4e);
        show.op(0x4f);
        show.visible(name, false);
    }
    show.visible("rebornRa3Page", true);
    show.visible("rebornRa3TitleFade", true);
    show.visible("rebornRa3EffectAnchor", true);
    show.call("rebornRefreshRa3Frames");
    // Reborn: the active RA3 tab is grey and inert; only the Uprising destination tab remains hoverable and clickable.
    show.clip_call("rebornPageTab1", "disable");
    show.clip_call("rebornPageTab0", "enable");
    show.call("rebornChooseRa30");
    method(&mut patch, "rebornShowRA3", show);
    let mut refresh = Code::default();
    // Reborn: query completion for the extra campaigns, never reuse the original faction's progress.
    for i in 0..3 {
        refresh.string("rebornQuery");
        refresh.int(0);
        refresh.string("Object");
        refresh.op(0x40);
        refresh.op(0x3c);
        refresh.string(&format!(
            "QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex={}",
            4 + i
        ));
        refresh.path(&["rebornQuery"]);
        refresh.op(0x9a);
        refresh.clip(&format!("rebornRa3Frame{i}"));
        refresh.string("_visible");
        refresh.path(&["rebornQuery", "IS_CAMPAIGN_COMPLETE"]);
        refresh.string("1");
        refresh.op(0x49);
        refresh.op(0x4f);
    }
    method(&mut patch, "rebornRefreshRa3Frames", refresh);
    let mut back = Code::default();
    // Reborn: preserve stock hover feedback on both page buttons while preventing an already-open Uprising page from restoring uncaptured state.
    back.owner_prop("rebornRa3Active");
    back.op(0x12);
    let already_uprising = back.0.len();
    back.0.push(I::BranchIfFalse { target: 0 });
    back.op(0x76);
    back.op(0x3e);
    let begin_uprising = back.0.len();
    back.0[already_uprising] = I::BranchIfFalse {
        target: begin_uprising,
    };
    back.set_owner_bool("rebornRa3Active", false);
    back.visible("rebornRa3Page", false);
    back.visible("rebornRa3TitleFade", false);
    back.title("$FACTION_SELECT_SCREEN_TITLE");
    back.visible("rebornRa3EffectAnchor", false);
    for i in 0..3 {
        back.visible(&format!("rebornRa3Frame{i}"), false);
    }
    for name in originals {
        back.clip(name);
        back.string("_visible");
        back.clip(name);
        back.string("rebornPreviousVisibility");
        back.op(0x4e);
        back.op(0x4f);
    }
    // Reborn: the active Uprising tab is grey and inert; only the RA3 destination tab remains hoverable and clickable.
    back.clip_call("rebornPageTab0", "disable");
    back.clip_call("rebornPageTab1", "enable");
    back.owner_prop("rebornUprisingFaction");
    back.int(1);
    back.owner();
    back.string("factionButtonClicked");
    back.op(0x52);
    back.op(0x17);
    method(&mut patch, "rebornShowUprising", back);
    // Reborn: defer only the extra campaigns' completion animation until their page and frame transforms are ready.
    let mut intercept = Code::default();
    for campaign in 4..=6 {
        intercept.0.push(I::PushRegister(2));
        intercept.int(campaign);
        intercept.op(0x49);
        let skip = intercept.0.len();
        intercept.0.push(I::BranchIfFalse { target: 0 });
        intercept.op(0x76);
        intercept.op(0x3e);
        let end = intercept.0.len();
        intercept.0[skip] = I::BranchIfFalse { target: end };
    }
    intercept.0.push(I::PushRegister(2));
    intercept.int(1);
    intercept.owner();
    intercept.string("rebornOriginalCompleteAnimation");
    intercept.op(0x52);
    intercept.op(0x17);
    parameter_method(
        &mut patch,
        "configureCompleteAnimation",
        intercept,
        vec![(2, "faction".into())],
    );
    for (i, matrix) in frame_matrices.iter().enumerate() {
        let mut animate = Code::default();
        animate.visible(&format!("rebornRa3Frame{i}"), false);
        animate.sound("GuiCampaignFactionFlagComplete");
        animate.owner();
        animate.string("rebornRa3CompleteEffect");
        animate.int(1);
        animate.string("Reborn_RA3_Complete");
        animate.string("Complete_animation");
        animate.int(3);
        animate.clip("rebornRa3EffectAnchor");
        animate.string("attachMovie");
        animate.op(0x52);
        animate.op(0x4f);
        for (key, value) in [
            ("_x", matrix.tx),
            ("_y", matrix.ty),
            ("_xscale", matrix.a * 100.0),
            ("_yscale", matrix.d * 100.0),
        ] {
            animate.owner_prop("rebornRa3CompleteEffect");
            animate.string(key);
            animate.float(value);
            animate.op(0x4f);
        }
        animate.owner_prop("rebornRa3CompleteEffect");
        animate.string("content");
        animate.op(0x4e);
        animate.string("onUnload");
        animate.owner_prop("rebornRa3CompletionFinished");
        animate.owner();
        animate.int(2);
        animate.owner();
        animate.string("bind0");
        animate.op(0x52);
        animate.op(0x4f);
        animate.string(["_sovietFlagEffect", "_alliesFlagEffect", "_japanFlagEffect"][i]);
        animate.int(1);
        animate.owner_prop("rebornRa3CompleteEffect");
        animate.string("content");
        animate.op(0x4e);
        animate.string("completeFlagEffect");
        animate.op(0x4e);
        animate.string("gotoAndPlay");
        animate.op(0x52);
        animate.op(0x17);
        method(&mut patch, &format!("rebornCompleteRa3{i}"), animate);
    }
    let mut finished = Code::default();
    finished.owner_prop("rebornRa3Active");
    let skip = finished.0.len();
    finished.0.push(I::BranchIfFalse { target: 0 });
    finished.call("rebornRefreshRa3Frames");
    let end = finished.0.len();
    finished.0[skip] = I::BranchIfFalse { target: end };
    method(&mut patch, "rebornRa3CompletionFinished", finished);
    let mut patch = patch.body();
    let mut dictionary = Vec::new();
    dictionary_code(&mut patch, &mut dictionary);
    patch
        .instructions
        .insert(0, I::DefineDictionary(dictionary));
    verify_function_boundaries(&patch);
    install_lifecycle_hook(&mut ep1, patch);
    save(&ra3, Path::new(&args[4]));
    save(&ep1, Path::new(&args[3]));
    println!("Reborn: original EP1 page + original RA3 flag page generated; both APT/CONST models round-trip exactly.");
}

#[cfg(test)]
mod tests {
    use super::*;

    // Reborn: verify the emitted completion timeline uses exactly the same transform as its corresponding static frame.
    #[test]
    fn emitted_completion_matches_each_ra3_frame() {
        let path = Path::new(env!("CARGO_MANIFEST_DIR"))
            .join("../../alpha/mods/LanMod5/Data/AptUI/fes_m_factionMenu/fes_m_factionMenu.apt");
        let file = AptFile::load(&path).unwrap();
        for index in 0..3 {
            let matrix = placement(&file, &format!("rebornRa3Frame{index}"))
                .matrix
                .unwrap();
            let mut found = false;
            for control in &file.movie.frames[0].controls {
                if let Control::InitAction { actions, .. } = control {
                    let mut dictionary = vec![];
                    for (position, instruction) in actions.instructions.iter().enumerate() {
                        if let I::DefineDictionary(values) = instruction {
                            dictionary = values.clone();
                        }
                        if let I::DefineFunction2 { body, .. } = instruction {
                            let name = match actions.instructions.get(position.wrapping_sub(1)) {
                                Some(I::DictRef { index, .. }) => dictionary.get(*index as usize),
                                _ => None,
                            };
                            if name != Some(&Value::String(format!("rebornCompleteRa3{index}"))) {
                                continue;
                            }
                            for (property, expected) in [
                                ("_x", matrix.tx),
                                ("_y", matrix.ty),
                                ("_xscale", matrix.a * 100.0),
                                ("_yscale", matrix.d * 100.0),
                            ] {
                                assert!(body.instructions.windows(3).any(|w| match w {
                                    [I::DictRef{index,..},I::PushFloat(actual),I::Simple(0x4f)]=>dictionary.get(*index as usize)==Some(&Value::String(property.into())) && (*actual-expected).abs()<0.001,
                                    _=>false,
                                }),"Animation/frame mismatch: {property}");
                            }
                            found = true;
                        }
                    }
                }
            }
            assert!(found, "Missing emitted RA3 completion method");
            assert!(
                (matrix.tx - [256.0, 551.0, 839.0][index]).abs() < 15.0,
                "Completion frame moved into another faction column"
            );
        }
    }

    // Reborn: prove the real stock method calls setup and helper definitions live in its class initializer, before screen construction.
    #[test]
    fn stock_initialization_directly_calls_page_setup() {
        let original = Path::new(env!("CARGO_MANIFEST_DIR"))
            .join("../../alpha/Original EP1 UI Source SWF/fes_m_factionMenu/fes_m_factionMenu.apt");
        let mut file = AptFile::load(&original).unwrap();
        let original_root_actions: Vec<_> = file.movie.frames[0]
            .controls
            .iter()
            .filter(|c| matches!(c, Control::Action(_)))
            .cloned()
            .collect();
        let mut patch = Code::default();
        method(&mut patch, "rebornInitializePages", Code::default());
        install_lifecycle_hook(&mut file, patch.body());
        let root_actions: Vec<_> = file.movie.frames[0]
            .controls
            .iter()
            .filter(|c| matches!(c, Control::Action(_)))
            .cloned()
            .collect();
        assert_eq!(
            root_actions, original_root_actions,
            "Do not install setup after screen construction"
        );
        let mut calls = 0;
        for control in &file.movie.frames[0].controls {
            if let Control::InitAction { actions, .. } = control {
                for instruction in &actions.instructions {
                    if let I::DefineFunction2 { body, .. } = instruction {
                        if body.instructions.windows(4).any(|window| matches!(window,
                            [I::PushByte(0), I::PushRegister(1), I::PushString { value, .. }, I::Simple(0x5d)] if value == "rebornInitializePages")) { calls += 1; }
                    }
                }
            }
        }
        assert_eq!(calls, 1);
        let (bytes, constants) = file
            .write(&apt::write::WriteOptions::from_header(&file.header))
            .unwrap();
        assert_eq!(AptFile::read(&bytes, &constants).unwrap().movie, file.movie);
    }

    // Reborn: derive the static stage owner and inherited binder from the actual EA initializer, rather than a guessed mock layout.
    #[test]
    fn stock_base_owns_the_stage_and_binder() {
        let original = Path::new(env!("CARGO_MANIFEST_DIR"))
            .join("../../alpha/Original EP1 UI Source SWF/fes_m_factionMenu/fes_m_factionMenu.apt");
        let file = AptFile::load(&original).unwrap();
        let actions = file.movie.frames[0].controls.iter().find_map(|c| match c {
            Control::InitAction { actions, .. } if matches!(actions.instructions.first(), Some(I::DefineDictionary(d)) if d.get(1)==Some(&Value::String("Cafe2_BaseUIScreen".into()))) => Some(actions), _ => None,
        }).unwrap();
        let I::DefineDictionary(dictionary) = &actions.instructions[0] else {
            unreachable!()
        };
        assert_eq!(dictionary[2], Value::String("m_screen".into()));
        assert_eq!(dictionary[5], Value::String("bind0".into()));
        let constructor = actions
            .instructions
            .iter()
            .find_map(|i| match i {
                I::DefineFunction2 { body, params, .. }
                    if params.iter().any(|(_, name)| name == "screen") =>
                {
                    Some(body)
                }
                _ => None,
            })
            .unwrap();
        assert!(matches!(
            &constructor.instructions[..4],
            [
                I::DictRef {
                    kind: DictRefKind::ByteGetVar,
                    index: 1
                },
                I::DictRef { index: 2, .. },
                I::PushRegister(3),
                I::Simple(0x4f)
            ]
        ));
        assert!(actions.instructions.windows(3).any(|w| matches!(w, [I::PushRegister(2), I::DictRef { index: 5, .. }, I::DefineFunction { params, .. }] if params==&["obj".to_string(),"func".to_string()])));
    }

    // Reborn: reject the exact premature-End encoding that caused both observed native crashes.
    #[test]
    #[should_panic(expected = "Embedded End stops native APT relocation")]
    fn rejects_terminated_function_body() {
        verify_function_boundaries(&ActionStream {
            instructions: vec![
                I::DefineFunction2 {
                    name: String::new(),
                    params: vec![],
                    register_count: 1,
                    flags: 0,
                    body: ActionStream {
                        instructions: vec![I::End],
                    },
                },
                I::End,
            ],
        });
    }

    // Reborn: ordinary function completion leaves the outer scanner free to relocate the next function.
    #[test]
    fn accepts_length_delimited_functions() {
        let mut patch = Code::default();
        method(&mut patch, "first", Code::default());
        method(&mut patch, "second", Code::default());
        verify_function_boundaries(&patch.body());
    }

    // Reborn: initialize and bind through the actual method receiver rather than an absent static m_thisClass property.
    #[test]
    fn methods_preload_the_live_receiver() {
        let mut body = Code::default();
        body.call("rebornOriginalInitGUI");
        let mut patch = Code::default();
        method(&mut patch, "initGUI", body);
        let function = patch
            .0
            .iter()
            .find_map(|i| match i {
                I::DefineFunction2 {
                    flags,
                    register_count,
                    body,
                    ..
                } => Some((*flags, *register_count, body)),
                _ => None,
            })
            .unwrap();
        assert_eq!(function.0 & 1, 1);
        assert!(function.1 >= 2);
        assert!(function
            .2
            .instructions
            .iter()
            .any(|i| matches!(i, I::PushRegister(1))));
        assert!(!format!("{:?}", function.2).contains("m_thisClass"));
    }

    // Reborn: execute the generated binding stack against distinct global/screen-manager scopes and verify the installed callback.
    #[test]
    fn binding_installs_a_callback_on_the_widget() {
        #[derive(Clone, Debug, PartialEq)]
        enum V {
            Object(&'static str),
            Text(String),
            Number(i32),
            Handler,
            Bound,
            Undefined,
        }
        let mut code = Code::default();
        code.bind("rebornPageTab1", "rebornShowRA3", "setOnMouseUpFunction");
        let mut stack: Vec<V> = vec![];
        let mut installed = false;
        for instruction in code.0 {
            match instruction {
                I::PushRegister(1) => stack.push(V::Object("controller")),
                I::PushString { value, .. } => stack.push(V::Text(value)),
                I::PushByte(n) => stack.push(V::Number(n as i32)),
                I::Simple(0x1c) => {
                    let variable = stack.pop().unwrap();
                    stack.push(match variable {
                        V::Text(s) if s == "_global" => V::Object("global"),
                        V::Text(s) if s == "gSM" => V::Object("screen-manager"),
                        _ => V::Undefined,
                    });
                }
                I::Simple(0x4e) => {
                    let member = stack.pop().unwrap();
                    let object = stack.pop().unwrap();
                    stack.push(match (object, member) {
                        (V::Object("controller"), V::Text(s)) if s == "rebornShowRA3" => V::Handler,
                        (V::Object("global"), V::Text(s)) if s == "Cafe2_BaseUIScreen" => {
                            V::Object("base-class")
                        }
                        (V::Object("base-class"), V::Text(s)) if s == "m_screen" => {
                            V::Object("stage")
                        }
                        (V::Object("stage"), V::Text(s)) if s == "rebornPageTab1" => {
                            V::Object("tab")
                        }
                        _ => V::Undefined,
                    });
                }
                I::Simple(0x52) => {
                    let name = stack.pop().unwrap();
                    let object = stack.pop().unwrap();
                    let V::Number(count) = stack.pop().unwrap() else {
                        panic!("Missing argument count");
                    };
                    let args: Vec<_> = (0..count).map(|_| stack.pop().unwrap()).collect();
                    let result = match (object, name) {
                        (V::Object("controller"), V::Text(s)) if s == "bind0" => {
                            assert_eq!(args, vec![V::Object("controller"), V::Handler]);
                            V::Bound
                        }
                        (V::Object("tab"), V::Text(s)) if s == "setOnMouseUpFunction" => {
                            assert_eq!(args, vec![V::Bound]);
                            installed = true;
                            V::Undefined
                        }
                        _ => V::Undefined,
                    };
                    stack.push(result);
                }
                I::Simple(0x17) => {
                    stack.pop().unwrap();
                }
                other => panic!("Unexpected binding instruction: {other:?}"),
            }
        }
        assert!(installed);
        assert!(stack.is_empty());
    }
}
