using System.Buffers.Binary;
using System.Security.Cryptography;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fixed AK47 layout reproduction is diagnostic only, never a general weapon processor or publication gate.
internal static class WeaponTokenReproduction
{
    // Reborn: descriptors encode reviewed field/tag/offset relationships, not a copied stock prefix buffer.
    private readonly record struct Descriptor(int At, uint Name, uint Tag, int NativeOffset, int TableTarget);
    private static readonly Descriptor[] Descriptors = {
        new(0, 0xA8A3BBFEu, 0x645864F3u, 4, 0), // Reborn: AttackRange
        new(12, 0x9797D067u, 0x645864F3u, 8, 0), // Reborn: MinimumAttackRange
        new(24, 0x03646BDEu, 0x645864F3u, 12, 0), // Reborn: RangeBonusMinHeight
        new(36, 0x02E3525Eu, 0x645864F3u, 16, 0), // Reborn: RangeBonus
        new(48, 0x9FCEE103u, 0x645864F3u, 20, 0), // Reborn: RangeBonusPerFoot
        new(60, 0x95FF8A6Cu, 0x645864F3u, 24, 0), // Reborn: RequestAssistRange
        new(72, 0x1066C317u, 0x4E16EA4Du, 28, 0), // Reborn: AcceptableAimDelta
        new(84, 0x7EF694B1u, 0x4E16EA4Du, 32, 0), // Reborn: AimDirection
        new(96, 0x7A72CBE9u, 0x645864F3u, 36, 0), // Reborn: ScatterRadius
        new(108, 0xCB2EC959u, 0x645864F3u, 40, 0), // Reborn: ScatterLength
        new(120, 0x0FEB4E73u, 0x77E3F4F7u, 300, 0), // Reborn: ScatterIndependently
        new(132, 0x2CA1C843u, 0x77E3F4F7u, 301, 0), // Reborn: ScatterAlways
        new(144, 0xE16286CBu, 0x645864F3u, 44, 0), // Reborn: WeaponSpeed
        new(156, 0x388A03C9u, 0x645864F3u, 48, 0), // Reborn: MinWeaponSpeed
        new(168, 0x490E4588u, 0x645864F3u, 52, 0), // Reborn: MaxWeaponSpeed
        new(180, 0x3BA0478Fu, 0x77E3F4F7u, 302, 0), // Reborn: ScaleWeaponSpeed
        new(192, 0x61128F5Cu, 0x77E3F4F7u, 303, 0), // Reborn: IgnoresContactPoints
        new(204, 0xB6E40781u, 0x77E3F4F7u, 304, 0), // Reborn: ScaleAttackRangeByAmmoRemaining
        new(216, 0xF1DF59ACu, 0x77E3F4F7u, 305, 0), // Reborn: CanBeDodged
        new(228, 0x5D5C6183u, 0x998669E8u, 56, 0), // Reborn: IdleAfterFiringDelaySeconds
        new(240, 0x6B1FF4E9u, 0x998669E8u, 60, 0), // Reborn: HoldAfterFiringDelaySeconds
        new(252, 0x00A9BE33u, 0x77E3F4F7u, 306, 0), // Reborn: HoldDuringReload
        new(264, 0xBBCBE27Eu, 0x77E3F4F7u, 307, 0), // Reborn: CanFireWhileMoving
        new(276, 0x1319E3B1u, 0x4E16EA4Du, 64, 0), // Reborn: WeaponRecoil
        new(288, 0x553B6C95u, 0x4E16EA4Du, 68, 0), // Reborn: MinTargetPitch
        new(300, 0xB7737023u, 0x4E16EA4Du, 72, 0), // Reborn: MaxTargetPitch
        new(312, 0x4C932F32u, 0x1A2933AAu, 76, 0), // Reborn: PreferredTargetBone
        new(324, 0x120A78E8u, 0x63D7B468u, -1, 0), // Reborn: FireSound
        new(336, 0x3819F796u, 0x63D7B468u, 340, 0), // Reborn: FireSoundPerClip
        new(348, 0x86CE6D08u, 0x63D7B468u, -1, 0), // Reborn: FiringLoopSound
        new(360, 0xCA62E481u, 0x63D7B468u, -1, 0), // Reborn: ImpactLoopSound
        new(372, 0x4EBFA6D5u, 0x63D7B468u, -1, 0), // Reborn: RetargetedWhileLoopingSound
        new(384, 0x8E9F06FBu, 0x77E3F4F7u, 308, 0), // Reborn: FiringLoopSoundContinuesDuringReload
        new(396, 0x9F1BCA25u, 0x0D1C8E83u, 104, 0), // Reborn: FireFX
        new(408, 0x6DBBA63Eu, 0x0D1C8E83u, 108, 0), // Reborn: FireVeteranFX
        new(420, 0x1459E5BDu, 0x0D1C8E83u, 112, 0), // Reborn: FireFlankFX
        new(432, 0x17F17220u, 0x0D1C8E83u, 116, 0), // Reborn: PreAttackFX
        new(444, 0xB68DBC28u, 0x23E650B8u, 120, 0), // Reborn: ClipSize
        new(456, 0x9B8145E9u, 0x23E650B8u, 124, 0), // Reborn: ContinuousFireOne
        new(468, 0xCC9D6D26u, 0x23E650B8u, 128, 0), // Reborn: ContinuousFireTwo
        new(480, 0x97FDA959u, 0x998669E8u, 132, 0), // Reborn: ContinuousFireCoastSeconds
        new(492, 0x88099007u, 0x998669E8u, 136, 0), // Reborn: AutoReloadWhenIdleSeconds
        new(504, 0x95C56669u, 0x23E650B8u, 140, 0), // Reborn: ShotsPerBarrel
        new(516, 0x104CDFAAu, 0x77E3F4F7u, 309, 0), // Reborn: DamageDealtAtSelfPosition
        new(528, 0x4D73E039u, 0xA7BD811Eu, -1, 0), // Reborn: RequiredFiringObjectStatus
        new(540, 0x600D69FBu, 0xA7BD811Eu, -1, 0), // Reborn: ForbiddenFiringObjectStatus
        new(552, 0x6A4BDF67u, 0x77E3F4F7u, 310, 0), // Reborn: CheckStatusFlagsInRangeChecks
        new(564, 0x9BDB3BC4u, 0x77E3F4F7u, 311, 0), // Reborn: ProjectileSelf
        new(576, 0x6FDD1A18u, 0x77E3F4F7u, 312, 0), // Reborn: ProjectileSelfUsesPathfinder
        new(588, 0x3F081EA7u, 0x77E3F4F7u, 313, 0), // Reborn: MeleeWeapon
        new(600, 0x0A8779F5u, 0x77E3F4F7u, 314, 0), // Reborn: ChaseWeapon
        new(612, 0x844F87FAu, 0x77E3F4F7u, 315, 0), // Reborn: LeechRangeWeapon
        new(624, 0x8DF650A3u, 0x77E3F4F7u, 316, 0), // Reborn: HitStoredTarget
        new(636, 0x252225FAu, 0x77E3F4F7u, 317, 0), // Reborn: CapableOfFollowingWaypoints
        new(648, 0xB1598108u, 0x77E3F4F7u, 318, 0), // Reborn: ShowsAmmoPips
        new(660, 0x1F46C311u, 0x77E3F4F7u, 319, 0), // Reborn: AllowAttackGarrisonedBldgs
        new(672, 0x5B25DE0Du, 0x77E3F4F7u, 320, 0), // Reborn: PlayFXWhenStealthed
        new(684, 0x4131B666u, 0x645864F3u, 152, 0), // Reborn: ContinueAttackRange
        new(696, 0x1C66E1FCu, 0x998669E8u, 156, 0), // Reborn: SuspendFXDelaySeconds
        new(708, 0x8566AEF8u, 0x77E3F4F7u, 321, 0), // Reborn: IsAimingWeapon
        new(720, 0x4013983Bu, 0x77E3F4F7u, 322, 0), // Reborn: NoVictimNeeded
        new(732, 0xFFB6DE81u, 0x645864F3u, 160, 0), // Reborn: HitPercentage
        new(744, 0xA09C5D99u, 0x645864F3u, 164, 0), // Reborn: HitPassengerPercentage
        new(756, 0xFBB273E4u, 0x77E3F4F7u, 323, 0), // Reborn: PassengerProportionalAttack
        new(768, 0x87D285A7u, 0x645864F3u, 168, 0), // Reborn: HealthProportionalResolution
        new(780, 0xAF845003u, 0x23E650B8u, 172, 0), // Reborn: MaxAttackPassengers
        new(792, 0x447F0E34u, 0x77E3F4F7u, 324, 0), // Reborn: FinishAttackOnceStarted
        new(804, 0xCA7889B7u, 0x645864F3u, 176, 0), // Reborn: RestrictedHeightRange
        new(816, 0x7D34A061u, 0x77E3F4F7u, 325, 0), // Reborn: CannotTargetCastleVictims
        new(828, 0x5031D40Bu, 0x77E3F4F7u, 326, 0), // Reborn: RequireFollowThru
        new(840, 0x5BE81F7Cu, 0x77E3F4F7u, 327, 0), // Reborn: ShareTimers
        new(852, 0x68DD1107u, 0x77E3F4F7u, 328, 0), // Reborn: ShouldPlayUnderAttackEvaEvent
        new(864, 0x35890A12u, 0x77E3F4F7u, 329, 0), // Reborn: InstantLoadClipOnActivate
        new(876, 0xF345058Au, 0x47FC1D9Au, 180, 0), // Reborn: Flags
        new(888, 0x62679E18u, 0x77E3F4F7u, 330, 0), // Reborn: LockWhenUsing
        new(900, 0xD51F15E0u, 0x77E3F4F7u, 331, 0), // Reborn: BombardType
        new(912, 0xAFB534D7u, 0x77E3F4F7u, 332, 0), // Reborn: UseInnateAttributes
        new(924, 0x15B123C5u, 0x826ACC69u, 184, 0), // Reborn: PreAttackType
        new(936, 0xEF029607u, 0x3061BA53u, 188, 0), // Reborn: ReAcquireDetailType
        new(948, 0x2CF21F66u, 0x266EF25Au, 192, 0), // Reborn: AutoReloadsClip
        new(960, 0xD9D3C0EEu, 0x63D7B468u, 196, 0), // Reborn: SingleAmmoReloadedNotFullSound
        new(972, 0xB1C14901u, 0x63D7B468u, 200, 0), // Reborn: ClipReloadedSound
        new(984, 0xE3731062u, 0x5208E1B9u, 204, 0), // Reborn: RadiusDamageAffects
        new(996, 0xC4274C12u, 0x808B85BEu, 208, 0), // Reborn: FXTrigger
        new(1008, 0x6E2FE0B6u, 0x11E28116u, 212, 0), // Reborn: ProjectileCollidesWith
        new(1020, 0x475B437Du, 0xC8C69390u, 216, 0), // Reborn: RequiredAntiMask
        new(1032, 0x4DD6D922u, 0xC8C69390u, 220, 0), // Reborn: ForbiddenAntiMask
        new(1044, 0xE1BD34FEu, 0x77E3F4F7u, 333, 0), // Reborn: StopFiringOnCanBeInvisible
        new(1056, 0x8A3BDF92u, 0x148ED75Du, 224, 0), // Reborn: ProjectileStreamName
        new(1068, 0xF40F232Du, 0x77E3F4F7u, 334, 0), // Reborn: ContactWeapon
        new(1080, 0x4C86C5A2u, 0x77E3F4F7u, 335, 0), // Reborn: UseCenterForRangeCheck
        new(1092, 0xD297044Fu, 0x2AFA9122u, 228, 0), // Reborn: VirtualDamage
        new(1104, 0x3BD61BC1u, 0xD046DD22u, -1, 0), // Reborn: PreAttackWeapon
        new(1116, 0x1BC662CCu, 0x77E3F4F7u, 336, 0), // Reborn: RevealShroudOnFire
        new(1128, 0xD9EEFC1Fu, 0x77E3F4F7u, 337, 0), // Reborn: ShouldPlayTargetDeadEvaEvent
        new(1140, 0xE312300Du, 0x77E3F4F7u, 338, 0), // Reborn: UpdateBarrelModelConditions
        new(1152, 0x7E74DFCCu, 0x1AE6B6B7u, -1, 0), // Reborn: OverrideVoiceAttackSound
        new(1164, 0xAE8F7512u, 0x1AE6B6B7u, -1, 0), // Reborn: OverrideVoiceEnterStateAttackSound
        new(1176, 0xCF9D76E0u, 0x9CED3BFAu, -1, 1320), // Reborn: PreAttackDelay
        new(1188, 0x508A6678u, 0x9CED3BFAu, -1, 1356), // Reborn: FiringDuration
        new(1200, 0x51A6820Du, 0x9CED3BFAu, -1, 0), // Reborn: CoolDownDelayBetweenShots
        new(1212, 0x0C7FF035u, 0x9CED3BFAu, -1, 1392), // Reborn: ClipReloadTime
        new(1224, 0xD3A7C780u, 0xD3A7C780u, -1, 0), // Reborn: WeaponAiHintInfo
        new(1236, 0x5752BD3Cu, 0xA5C43252u, -1, 1428), // Reborn: Nuggets
        new(1248, 0xA64A0E8Fu, 0xE46EA3B2u, -1, 0), // Reborn: SurpriseAttackObjectFilter
        new(1260, 0xAB96B610u, 0xE46EA3B2u, -1, 0), // Reborn: CombinedAttackObjectFilter
        new(1272, 0xC0F5DA4Cu, 0xE46EA3B2u, -1, 0), // Reborn: HitStoredObjectFilter
        new(1284, 0xDBEA8EFDu, 0xE1EBA7C4u, -1, 1956), // Reborn: ScatterRadiusVsType
        new(1296, 0xDF062E0Du, 0x09DDCFF3u, -1, 1968), // Reborn: IncompatibleAttributeModifier
        new(1320, 0x387EF9A1u, 0x998669E8u, 344, 0), // Reborn: nested table/count/class descriptor
        new(1332, 0x35642BEFu, 0x998669E8u, 348, 0), // Reborn: nested table/count/class descriptor
        new(1356, 0x387EF9A1u, 0x998669E8u, 352, 0), // Reborn: nested table/count/class descriptor
        new(1368, 0x35642BEFu, 0x998669E8u, 356, 0), // Reborn: nested table/count/class descriptor
        new(1392, 0x387EF9A1u, 0x998669E8u, 360, 0), // Reborn: nested table/count/class descriptor
        new(1404, 0x35642BEFu, 0x998669E8u, 364, 0), // Reborn: nested table/count/class descriptor
        new(1428, 0x6C827073u, 0x5D448FD8u, 264, 0), // Reborn: nested table/count/class descriptor
        new(1440, 0xB151A0EBu, 0x00000000u, -1, 1452), // Reborn: nested table/count/class descriptor
        new(1452, 0x00000002u, 0xC0F53E99u, -1, 1464), // Reborn: nested table/count/class descriptor
        new(1464, 0x00000001u, 0xC7C2CC04u, -1, 1812), // Reborn: nested table/count/class descriptor
        new(1476, 0x7C33E4E9u, 0x645864F3u, 412, 0), // Reborn: Damage
        new(1488, 0x1182EA06u, 0x645864F3u, 416, 0), // Reborn: DamageTaperOff
        new(1500, 0xEC392B69u, 0x645864F3u, 420, 0), // Reborn: MinRadius
        new(1512, 0x1CA750FCu, 0x4E16EA4Du, 424, 0), // Reborn: DamageArc
        new(1524, 0x0CFD3B28u, 0x77E3F4F7u, 520, 0), // Reborn: DamageArcInverted
        new(1536, 0x06298750u, 0x645864F3u, 428, 0), // Reborn: DamageMaxHeight
        new(1548, 0x229B6AD7u, 0x645864F3u, 432, 0), // Reborn: DamageMaxHeightAboveTerrain
        new(1560, 0x792EA9B5u, 0x77E3F4F7u, 521, 0), // Reborn: AcceptDamageAdd
        new(1572, 0x827AD6D4u, 0x645864F3u, 436, 0), // Reborn: FlankingBonus
        new(1584, 0x6BA7B51Au, 0x645864F3u, 440, 0), // Reborn: FlankedScalar
        new(1596, 0x8AC7E66Au, 0x998669E8u, 444, 0), // Reborn: DelayTimeSeconds
        new(1608, 0x46486FF1u, 0x46486FF1u, 448, 0), // Reborn: DamageType
        new(1620, 0xC608ED36u, 0xC608ED36u, 452, 0), // Reborn: DeathType
        new(1632, 0x2FAA4E62u, 0x2FAA4E62u, 456, 0), // Reborn: DamageFXType
        new(1644, 0x62BB1CE7u, 0x62BB1CE7u, 460, 0), // Reborn: DamageSubType
        new(1656, 0x6FBB66BAu, 0x77E3F4F7u, 522, 0), // Reborn: OnlyKillOwnerWhenTriggered
        new(1668, 0xF37C08FCu, 0x645864F3u, 464, 0), // Reborn: DrainLifeMultiplier
        new(1680, 0x722223EDu, 0x77E3F4F7u, 523, 0), // Reborn: DrainLife
        new(1692, 0x31A847B4u, 0x645864F3u, 468, 0), // Reborn: DamageSpeed
        new(1704, 0xFA8DAB3Fu, 0xC4EC43B1u, 472, 0), // Reborn: UnderAttackOverrideEvaEvent
        new(1716, 0x2DFA364Cu, 0xBFAB9075u, 476, 0), // Reborn: VictimShroudRevealer
        new(1728, 0x53FC2B18u, 0x77E3F4F7u, 524, 0), // Reborn: NotifyOwnerOnVictimDeath
        new(1740, 0x063C4040u, 0x77E3F4F7u, 525, 0), // Reborn: NotifyObserversOnPreDamageEffectPosition
        new(1752, 0x45C3978Cu, 0x77E3F4F7u, 526, 0), // Reborn: ForceFXPositionToVictim
        new(1764, 0x21494542u, 0x77E3F4F7u, 527, 0), // Reborn: RadiusAffectsBridges
        new(1776, 0xDE8679A4u, 0xA7BD811Eu, 480, 0), // Reborn: InvalidTargetStatus
        new(1788, 0x919720CDu, 0xE4A6E6E4u, -1, 1944), // Reborn: DamageScalarDetails
        new(1812, 0x19B042A0u, 0x23E650B8u, 372, 0), // Reborn: TypeId
        new(1824, 0xF85D2910u, 0x9F0DA607u, 376, 0), // Reborn: PartitionFilterTestType
        new(1836, 0xC2C48A88u, 0xA7BD811Eu, -1, 0), // Reborn: ForbiddenTargetObjectStatus
        new(1848, 0xBE80EE1Eu, 0x67DBBFC9u, -1, 0), // Reborn: ForbiddenTargetModelCondition
        new(1860, 0x4F31C6C5u, 0x645864F3u, 388, 0), // Reborn: Radius
        new(1872, 0xCE173B85u, 0xE46EA3B2u, -1, 0), // Reborn: SpecialObjectFilter
        new(1884, 0x7F719729u, 0x26FACFC7u, -1, 1920), // Reborn: RequiredUpgrade
        new(1896, 0x2B3C1BF6u, 0x26FACFC7u, -1, 1932), // Reborn: ForbiddenUpgrade
        new(1920, 0x6C827073u, 0x5D448FD8u, 396, 0), // Reborn: nested table/count/class descriptor
        new(1932, 0x6C827073u, 0x5D448FD8u, 404, 0), // Reborn: nested table/count/class descriptor
        new(1944, 0x6C827073u, 0x5D448FD8u, 512, 0), // Reborn: nested table/count/class descriptor
        new(1956, 0x6C827073u, 0x5D448FD8u, 284, 0), // Reborn: nested table/count/class descriptor
        new(1968, 0x6C827073u, 0x5D448FD8u, 292, 0), // Reborn: nested table/count/class descriptor
    };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse detached shape, scalar, relocation and import corruptions while proving no caller-buffer mutation. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckControls(Chunk native)
    {
        byte[] original = native.InstanceBuffer.ToArray();
        var first = Encode(native); var second = Encode(native);
        if (!first.InstanceBuffer.SequenceEqual(second.InstanceBuffer) || !first.RelocationBuffer.SequenceEqual(second.RelocationBuffer))
            throw new InvalidDataException("AK47 token reproduction nondeterministic.");
        foreach (int fault in Enumerable.Range(0, 7))
        {
            var changed = new Chunk { InstanceBuffer = native.InstanceBuffer.ToArray(),
                RelocationBuffer = native.RelocationBuffer.ToArray(), ImportsBuffer = native.ImportsBuffer.ToArray() };
            switch (fault)
            {
                case 0: changed.InstanceBuffer = changed.InstanceBuffer[..^4]; break;
                case 1: Put(changed.InstanceBuffer, 412, 0x7FC00000u); break;
                case 2: BinaryPrimitives.WriteSingleLittleEndian(changed.InstanceBuffer.AsSpan(412, 4), -1f); break;
                case 3: BinaryPrimitives.WriteSingleLittleEndian(changed.InstanceBuffer.AsSpan(412, 4), 10001f); break;
                case 4: changed.InstanceBuffer[324] ^= 1; break;
                case 5: Put(changed.RelocationBuffer, 0, 0); break;
                case 6: Put(changed.ImportsBuffer, 0, 0); break;
            }
            try { Encode(changed); }
            catch (InvalidDataException) { continue; }
            throw new InvalidDataException("AK47 token reproduction accepted corruption " + fault);
        }
        if (!native.InstanceBuffer.SequenceEqual(original)) throw new InvalidDataException("AK47 token reproduction mutated caller bytes.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reconstruct the observed fixed token graph from actual native compilation; admit only a Damage scalar difference. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Encode(Chunk native)
    {
        const int prefix = 1980;
        if (native.InstanceBuffer.Length != 528 || native.RelocationBuffer.Length != 28 || native.ImportsBuffer.Length != 16)
            throw new InvalidDataException($"AK47 diagnostic native shape differs: BIN={native.InstanceBuffer.Length}, RELO={native.RelocationBuffer.Length}, IMP={native.ImportsBuffer.Length}.");
        var canonical = native.InstanceBuffer.ToArray();
        float damage = BinaryPrimitives.ReadSingleLittleEndian(canonical.AsSpan(412, 4));
        if (!float.IsFinite(damage) || damage < 0 || damage > 10000)
            throw new InvalidDataException("AK47 diagnostic damage outside admitted finite range.");
        BinaryPrimitives.WriteSingleLittleEndian(canonical.AsSpan(412, 4), 5f);
        if (Convert.ToHexString(SHA256.HashData(canonical)) != "3DA83658B59755C6F5FDFDA98E2AE740E51798CC33CB85CBB7CB28BE8D70A841")
            throw new InvalidDataException("AK47 diagnostic native fields outside Damage changed.");
        // Reborn: enforce the independently recovered pointer and import locations before changing their representation.
        uint[] locations = { 88, 244, 248, 256, 268, 368, uint.MaxValue };
        for (int index = 0; index < locations.Length; index++)
            if (Word(native.RelocationBuffer, index * 4) != locations[index])
                throw new InvalidDataException("AK47 diagnostic relocation shape differs: " + string.Join(",", Enumerable.Range(0, 7).Select(at => Word(native.RelocationBuffer, at * 4))));
        var imports = Enumerable.Range(0, 4).Select(index => Word(native.ImportsBuffer, index * 4)).ToArray();
        if (!imports.Order().SequenceEqual(new uint[] { 104, 108, 340, uint.MaxValue }))
            throw new InvalidDataException("AK47 diagnostic imports differ.");
        var result = new Chunk { InstanceBuffer = new byte[prefix + canonical.Length], RelocationBuffer = new byte[28] };
        native.InstanceBuffer.CopyTo(result.InstanceBuffer, prefix);
        foreach (var descriptor in Descriptors)
        {
            Put(result.InstanceBuffer, descriptor.At, descriptor.Name);
            Put(result.InstanceBuffer, descriptor.At + 4, descriptor.Tag);
            Put(result.InstanceBuffer, descriptor.At + 8,
                checked((uint)(descriptor.NativeOffset >= 0 ? prefix + descriptor.NativeOffset : descriptor.TableTarget)));
        }
        // Reborn: native pointer values and relocation locations shift together; token-table pointers are not native relocations.
        for (int index = 0; index < locations.Length - 1; index++)
        {
            int location = checked((int)locations[index]);
            uint pointer = Word(native.InstanceBuffer, location);
            if (pointer == 0 || pointer > native.InstanceBuffer.Length - 4)
                throw new InvalidDataException("AK47 diagnostic native pointer outside allocation.");
            Put(result.InstanceBuffer, prefix + location, checked(pointer + prefix));
            Put(result.RelocationBuffer, index * 4, checked((uint)(prefix + location)));
        }
        Put(result.RelocationBuffer, 24, uint.MaxValue);
        // Reborn: this observed tokenized profile has zero IMP bytes but retains all three encoded dependency indices in BIN.
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only owned diagnostic output buffers in explicit little-endian form. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded canonical native words for the fixed diagnostic shape. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
}

