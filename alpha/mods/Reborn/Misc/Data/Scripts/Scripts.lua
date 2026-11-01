--- define lua functions 
function NoOp(self, source)
end


function kill(self) -- Kill unit self.
	ExecuteAction("NAMED_KILL", self);
end

function RadiateUncontrollableFear( self )
	ObjectBroadcastEventToEnemies( self, "BeUncontrollablyAfraid", 350 )
end

function RadiateGateDamageFear(self)
	ObjectBroadcastEventToAllies(self, "BeAfraidOfGateDamaged", 200)
end

function OnNeutralGarrisonableBuildingCreated(self)
	ObjectHideSubObjectPermanently( self, "ARMOR", true )
end

function OnGDITechCenterCreated(self)
	ObjectHideSubObjectPermanently( self, "UG_Boost", true )
	ObjectHideSubObjectPermanently( self, "UG_Mortar", true )
	ObjectHideSubObjectPermanently( self, "B_MortarRound_1", true )
	ObjectHideSubObjectPermanently( self, "UG_Rail", true )
	ObjectHideSubObjectPermanently( self, "UG_Scan", true )
end

function OnGDIMedicalBayCreated(self)
	ObjectHideSubObjectPermanently( self, "UG_Armor", true )
	ObjectHideSubObjectPermanently( self, "UG_StealthDetector", true )
	ObjectHideSubObjectPermanently( self, "UG_StealthDetector01", true )
	ObjectHideSubObjectPermanently( self, "UG_Injector", true )
end

function OnGDIPowerPlantCreated(self)
	ObjectHideSubObjectPermanently( self, "Turbines", true )
	ObjectHideSubObjectPermanently( self, "TurbineGlows", true )
end

function OnGDIZoneTrooperCreated(self)
	ObjectHideSubObjectPermanently( self, "UGSCANNER", true )
	ObjectHideSubObjectPermanently( self, "UGJUMP", true )
	ObjectHideSubObjectPermanently( self, "UGINJECTOR", true )
end

function OnGDIPredatorCreated(self)
	ObjectHideSubObjectPermanently( self, "UGRAIL_01", true )
end

function OnGDIMammothCreated(self)
	ObjectHideSubObjectPermanently( self, "UGRAIL_01", true )
	ObjectHideSubObjectPermanently( self, "UGRAIL_02", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_01", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_02", true )
end

function OnGDIJuggernaughtCreated(self)
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_01", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_02", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_03", true )
	
end

function OnGDIWatchTowerCreated(self)
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_01", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_02", true )
end

function OnGDIFirehawkCreated(self)
	-- bomb load by default.
	ObjectGrantUpgrade( self, "Upgrade_SelectLoad_02" )
	ObjectHideSubObjectPermanently( self, "Plane04", true )
end

function OnGDIPitbullCreated(self)
	ObjectHideSubObjectPermanently( self, "MortorTube", true )
end

function OnGDIOrcaCreated(self)
	ObjectHideSubObjectPermanently( self, "UG_PROBE", true )
end

function OnGDISniperSquadCreated(self)
	ObjectSetObjectStatus( self, "CAN_SPOT_FOR_BOMBARD" )
end

function OnGDIOrcaClipEmpty(self)
	ObjectHideSubObjectPermanently( self, "MISSILE01", true )
end

function OnGDIOrcaClipFull(self)
	ObjectHideSubObjectPermanently( self, "MISSILE01", false )
end

function OnGDIV35Ox_SummonedForVehicleCreated(self)
	ObjectHideSubObjectPermanently( self, "LOADREF", true )
end

function OnNODShredderCreated(self)

end

function OnNODRaiderTankCreated(self)
	ObjectHideSubObjectPermanently( self, "Gun_Upgrade", true )
	ObjectHideSubObjectPermanently( self, "Turret2_Gun", true )
	ObjectHideSubObjectPermanently( self, "Turret2", true )
	ObjectHideSubObjectPermanently( self, "MuzzleFlash_01", true )
	ObjectHideSubObjectPermanently( self, "DOZERBLADE", true )
end

function OnNODAvatarCreated(self)
	ObjectHideSubObjectPermanently( self, "NUBEAM", true )
	ObjectHideSubObjectPermanently( self, "FLAMETANK", true )
	ObjectHideSubObjectPermanently( self, "S_DETECTOR", true )
	ObjectHideSubObjectPermanently( self, "S_GENERATOR", true )
end

function OnNODAvatarGenericEvent(self, data)

	local str = tostring( data )

	if str == "upgrades_copied" then
		ObjectRemoveUpgrade( self, "Upgrade_Veterancy_VETERAN" );
		ObjectRemoveUpgrade( self, "Upgrade_Veterancy_ELITE" );
		ObjectRemoveUpgrade( self, "Upgrade_Veterancy_HEROIC" );
	end
end

function OnNODScorpionBuggyCreated(self)
	ObjectHideSubObjectPermanently( self, "EMP", true )
end

function OnNODVenomCreated(self)
	ObjectHideSubObjectPermanently( self, "SigGen", true )
end

function OnNODTechAssembleyPlantCreated(self)
	ObjectHideSubObjectPermanently( self, "UG_EMP", true )
	ObjectHideSubObjectPermanently( self, "UG_Lasers", true )
	ObjectHideSubObjectPermanently( self, "UG_SigGen", true )
	ObjectHideSubObjectPermanently( self, "UG_DozerBlades", true )
end

function OnNODSecretShrineCreated(self)
	ObjectHideSubObjectPermanently( self, "GLOWS", true )	
	ObjectHideSubObjectPermanently( self, "ConfUpgrd", true )
end

function OnNODSecretShrinePowerOutage(self)	
	if ObjectHasUpgrade( self, "Upgrade_NODConfessorUpgrade" ) == 1 then
		ObjectHideSubObjectPermanently( self, "GLOWS", true )	
	end
end

function OnNODSecretShrinePowerRestored(self)		 
	if ObjectHasUpgrade( self, "Upgrade_NODConfessorUpgrade" ) == 1 then
		ObjectHideSubObjectPermanently( self, "GLOWS", false )	
	end
end


function OnAlliedAntiVehicleInfantryGenericEvent(self, data)
	local str = tostring( data )

	if str == "laser_on" then
		ObjectHideSubObjectPermanently( self, "FX_LASER", false )
	elseif str == "laser_off" then
		ObjectHideSubObjectPermanently( self, "FX_LASER", true )
	end
end

function OnAlliedBarracksCreated(self)
	ObjectHideSubObjectPermanently( self, "Upgrade_02", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_03", true )
end

function OnAlliedConstructionYardCreated(self)
	ObjectHideSubObjectPermanently( self, "Upgrade_02", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_03", true )
end

function onCreatedControlPointFunctions(self)
	ObjectHideSubObjectPermanently( self, "TB_CP_ALN", true )
	ObjectHideSubObjectPermanently( self, "TB_CP_GDI", true )
	ObjectHideSubObjectPermanently( self, "TB_CP_NOD", true )
	ObjectHideSubObjectPermanently( self, "LIGHTSF01", true )
	ObjectHideSubObjectPermanently( self, "100", false)
	ObjectHideSubObjectPermanently( self, "75", false)
	ObjectHideSubObjectPermanently( self, "50", false)
	ObjectHideSubObjectPermanently( self, "25", false )
end

function onBuildingPowerOutage(self)
	ObjectHideSubObjectPermanently( self, "LIGHTS", true )
	ObjectHideSubObjectPermanently( self, "FXLIGHTS05", true )
	ObjectHideSubObjectPermanently( self, "FXLIGHTS", true )
	ObjectHideSubObjectPermanently( self, "FXGLOWS", true )
	ObjectHideSubObjectPermanently( self, "FLASHINGLIGHTS", true )
	ObjectHideSubObjectPermanently( self, "MESH01", true )
	ObjectHideSubObjectPermanently( self, "POWERPLANTGLOWS", true )
	ObjectHideSubObjectPermanently( self, "LIGHTL", true )
	ObjectHideSubObjectPermanently( self, "LIGHTR", true )
	ObjectHideSubObjectPermanently( self, "LIGHTS1", true )
	ObjectHideSubObjectPermanently( self, "NBCHEMICALPTE1", true )
	ObjectHideSubObjectPermanently( self, "LINKS", true )
	ObjectHideSubObjectPermanently( self, "MESH28", true )
	ObjectHideSubObjectPermanently( self, "TURBINEGLOWS", true )
	ObjectHideSubObjectPermanently( self, "GLOWS", true )
end

function onBuildingPowerRestored(self)
	ObjectHideSubObjectPermanently( self, "LIGHTS", false )
	ObjectHideSubObjectPermanently( self, "FXLIGHTS05", false )
	ObjectHideSubObjectPermanently( self, "FXLIGHTS", false )
	ObjectHideSubObjectPermanently( self, "FXGLOWS", false )
	ObjectHideSubObjectPermanently( self, "FLASHINGLIGHTS", false )
	ObjectHideSubObjectPermanently( self, "MESH01", false )
	ObjectHideSubObjectPermanently( self, "POWERPLANTGLOWS", false )
	ObjectHideSubObjectPermanently( self, "LIGHTL", false )
	ObjectHideSubObjectPermanently( self, "LIGHTR", false )
	ObjectHideSubObjectPermanently( self, "LIGHTS1", false )
	ObjectHideSubObjectPermanently( self, "NBCHEMICALPTE1", false )
	ObjectHideSubObjectPermanently( self, "LINKS", false )
	ObjectHideSubObjectPermanently( self, "MESH28", false )
	ObjectHideSubObjectPermanently( self, "TURBINEGLOWS", false )
	ObjectHideSubObjectPermanently( self, "GLOWS", false )
end







function OnGenericJapanBuildingCreated(self)
	ObjectHideSubObjectPermanently( self, "Upgrade_02", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_03", true )
end

function OnNeutralGarrisonableBuildingGenericEvent(self,data)
end

function onCreatedGDIOrcaAirstrike(self)
	ObjectForbidPlayerCommands( self, true )
end

function onCreatedAlienMCVUnpacking(self)
	ObjectForbidPlayerCommands( self, true )
end

function MakeMeAlert(self)
	ObjectEnterAlertState(self)
end

function BecomeUncontrollablyAfraid(self, other)
	if not ObjectTestCanSufferFear(self) then
		return
	end

	ObjectEnterUncontrollableCowerState(self, other)
end

function RadiateTerror(self, other)
	ObjectBroadcastEventToEnemies(self, "BeTerrified", 180)
end
	
function RadiateTerrorEx(self, other, terrorRange)
	ObjectBroadcastEventToEnemies(self, "BeTerrified", terrorRange)
end
	

function BecomeTerrified(self, other)
	ObjectEnterRunAwayPanicState(self, other)
end

function BecomeAfraidOfGateDamaged(self, other)
	if not ObjectTestCanSufferFear(self) then
		return
	end

	ObjectEnterCowerState(self,other)
end


function ChantForUnit(self) -- Used by units to broadcast the chant event to their own side.
	ObjectBroadcastEventToAllies(self, "BeginChanting", 9999)
end

function StopChantForUnit(self) -- Used by units to stop the chant event to their own side.
	ObjectBroadcastEventToAllies(self, "StopChanting", 9999)
end

function SpyMoving(self, other)
	print(ObjectDescription(self).." spying movement of "..ObjectDescription(other));
end

function OnGarrisonableCreated(self)
	ObjectHideSubObjectPermanently( self, "GARRISON01", true )
	ObjectHideSubObjectPermanently( self, "GARRISON02", true )
end

function OnRubbleDropshipCreated(self)
	ObjectHideSubObjectPermanently( self, "Loadref", true )
end

function OnSovietAntiVehicleVehicleTech1Created(self)
	ObjectHideSubObjectPermanently( self, "Upgrade_Athena_Cannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_AutoCannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Big_Cannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Cryo_Weapon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_FlakCannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_FlakMissile", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_GaussCannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Laser_Cannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_PrismCannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Small_Cannon", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_TeslaCoil", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_V4Rocket_A", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_V4Rocket_Missile", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Waveforce", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_FutureTank", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_GigaFortress", true )
		
end

function OnAlliedInfantryFightingCreated(self)
	ObjectHideSubObjectPermanently( self, "NUBEAM", true )
	ObjectHideSubObjectPermanently( self, "FLAMETANK", true )
	ObjectHideSubObjectPermanently( self, "S_DETECTOR", true )
	ObjectHideSubObjectPermanently( self, "S_GENERATOR", true )
end


function OnAlliedBaseDefenseCreated(self)

	ObjectHideSubObjectPermanently( self, "Upgrade_Blaster_Rifles", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Engineer", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Flak", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_GoldenGun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_MachineGun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_MissileLauncher", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Natasha_Sniper", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Star", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Plasma", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Shotgun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Speakers", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tanya_Pistols", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tesla_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tesla", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Turret", true )
	ObjectHideSubObjectPermanently( self, "Tanya", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Cryo", true )	
	ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaiden", true )	
	ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaidenString", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Desolator", true )	
	
end

function OnAlliedAntiAirVehicleTech1Created(self)

	ObjectHideSubObjectPermanently( self, "Upgrade_Blaster_Rifles", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Engineer", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Flak", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_GoldenGun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_MachineGun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_MissileLauncher", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Natasha_Sniper", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Star", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Plasma", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Shotgun", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Speakers", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tanya_Pistols", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tesla_Turret", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Tesla", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Turret", true )
	ObjectHideSubObjectPermanently( self, "Tanya", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Cryo", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaiden", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaidenString", true )
	ObjectHideSubObjectPermanently( self, "Upgrade_Desolator", true )
	
end

function OnAlliedInfantryFightingCreated(self)
	ObjectHideSubObjectPermanently( self, "NUBEAM", true )
	ObjectHideSubObjectPermanently( self, "FLAMETANK", true )
	ObjectHideSubObjectPermanently( self, "S_DETECTOR", true )
	ObjectHideSubObjectPermanently( self, "S_GENERATOR", true )
end

function OnJapanAntiStructureShipCreated(self)
	ObjectHideSubObjectPermanently( self, "Upgrade_01", true )
end

function OnSovietGrinderVehicleClipEmpty(self)
	ObjectDoSpecialPower( self, "SpecialPower_SlowSelfWhileGrinding" )
end

-- this function expects an eventType of 'onDisguiseAs' and checks for various assetIDs, hiding the appropriate subObjects
function OnJapanLightTransportVehicleGenericEvent(self, eventType, data)
	local eTpye = tostring( eventType )
	
	if not eType == "onDisguiseAs" then
		return
	end
	
	local str = tostring( data )
	
	-- this is the assetID of an AlliedAntiAirVehicleTech1
	if str == "3137747290" then
		ObjectHideSubObjectPermanently( self, "Upgrade_Blaster_Rifles", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Engineer", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Flak", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_GoldenGun", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_MachineGun", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_MissileLauncher", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Natasha_Sniper", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Star", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Ninja_Turret", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Plasma", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Psyonic_Turret", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Shotgun", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Speakers", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Tanya_Pistols", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Tesla_Turret", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Tesla", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Turret", true )
		ObjectHideSubObjectPermanently( self, "Tanya", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Cryo", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaiden", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_ArcherMaidenString", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Desolator", true )
	-- this is the assetID of a SovietAntiVehicleVehicleTech1
	elseif str == "2494781707" then
		ObjectHideSubObjectPermanently( self, "Upgrade_Athena_Cannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_AutoCannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Big_Cannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Cryo_Weapon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_FlakCannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_FlakMissile", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_GaussCannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Laser_Cannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_PrismCannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Small_Cannon", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_TeslaCoil", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_V4Rocket_A", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_V4Rocket_Missile", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_Waveforce", true )	
		ObjectHideSubObjectPermanently( self, "Upgrade_FutureTank", true )
		ObjectHideSubObjectPermanently( self, "Upgrade_GigaFortress", true )
	end
end


-- Reborn: all persistent clock state lives in native OBJECT upgrades, never in a Lua-only timer or player-script context.
function RebornParadropHasUpgrade(self, name)
    -- Reborn: retail ObjectHasUpgrade handles PLAYER templates only; UNIT_HAS_UPGRADE reads object-local clock bits.
    local result
    if strfind(name, "^Upgrade_RebornParadropClock") then
        -- Reborn: EvaluateCondition rejects userdata; bind the direct object through the native unit-reference action first.
        ExecuteAction("SET_UNIT_REFERENCE", "__RebornParadropClockRead", self)
        result = EvaluateCondition("UNIT_HAS_UPGRADE", "__RebornParadropClockRead", name)
    else
        result = ObjectHasUpgrade(self, name)
    end
    return result and result ~= 0
end

-- Reborn: these transient role tags are reconstructed by every XML-bound callback; saved clock state stays in native upgrades.
RebornParadropRoles = {}
-- Reborn: separate callbacks wrap one native object in different Lua tables; native description IDs provide a stable actor key.
RebornParadropActors = {}

-- Reborn: ObjectDescription starts with the native object ID in both named and unnamed retail formatters.
function RebornParadropActor(self)
    local description = ObjectDescription(self)
    if description then
        local first, last, id = strfind(description, "^Object (%d+) ")
        if id then
            local actor = RebornParadropActors[id]
            if actor then return actor end
            RebornParadropActors[id] = self
        end
    end
    return self
end
-- Reborn: failed native state reads must never overwrite a real cooldown with an invented ready state.
RebornParadropClockInvalid = {}
-- Reborn: defer only lifecycle synchronization, never ordinary clock ticks.
RebornParadropDeferred = {}

-- Reborn: player upgrades are queried through the owning object; names of map teams are irrelevant.
function RebornParadropOwner(self)
    for i = 1, 8 do
        if RebornParadropHasUpgrade(self, "Upgrade_RebornParadropOwner" .. i) then return i end
    end
    return nil
end

-- Reborn: assign one shared player identity when its first logic clock is created; reuse it for replacement clocks.
function RebornParadropBindOwner(clock)
    if RebornParadropOwner(clock) then return true end
    local used = {}
    local actor, role = next(RebornParadropRoles)
    while actor do
        local owner = RebornParadropOwner(actor)
        if owner then used[owner] = true end
        actor, role = next(RebornParadropRoles, actor)
    end
    for i = 1, 8 do
        if not used[i] then
            ObjectGrantUpgrade(clock, "Upgrade_RebornParadropOwner" .. i)
            return RebornParadropOwner(clock) == i
        end
    end
    return false
end

-- Reborn: allied players must not inherit another player's Paradrop clock.
function RebornParadropSameOwner(self, other)
    local owner = RebornParadropOwner(self)
    return owner and other and owner == RebornParadropOwner(other)
end

-- Reborn: only the logic-only actor owns the saved clock; Tower and spellbook are receivers.
function RebornParadropIsClock(self)
    return RebornParadropRoles[self] == 0
end

-- Reborn: read nine saved native upgrade bits as remaining simulation seconds (0..360).
function RebornParadropRemaining(self)
    local value = 0
    local weight = 1
    for i = 0, 8 do
        if RebornParadropHasUpgrade(self, "Upgrade_RebornParadropClockBit" .. i) then value = value + weight end
        weight = weight * 2
    end
    return value
end

-- Reborn: change only differing bits, keeping pause/save behavior in native object state.
function RebornParadropSetRemaining(self, value)
    local weight = 256
    for i = 8, 0, -1 do
        local name = "Upgrade_RebornParadropClockBit" .. i
        local wanted = value >= weight
        if wanted then value = value - weight end
        local present = RebornParadropHasUpgrade(self, name)
        if wanted and not present then ObjectGrantUpgrade(self, name)
        elseif present and not wanted then ObjectRemoveUpgrade(self, name) end
        weight = weight / 2
    end
end

-- Reborn: the current clock rank is persisted separately from the player's cumulative row unlocks.
function RebornParadropClockRank(self)
    if RebornParadropHasUpgrade(self, "Upgrade_RebornParadropClockRank3") then return 3 end
    if RebornParadropHasUpgrade(self, "Upgrade_RebornParadropClockRank2") then return 2 end
    return 1
end

-- Reborn: store one active rank without resetting the remaining native clock bits.
function RebornParadropSetRank(self, level)
    for i = 2, 3 do
        local name = "Upgrade_RebornParadropClockRank" .. i
        local present = RebornParadropHasUpgrade(self, name)
        if level == i and not present then ObjectGrantUpgrade(self, name)
        elseif level ~= i and present then ObjectRemoveUpgrade(self, name) end
    end
end

-- Reborn: native PLAYER upgrades can be inspected through the object API without ScriptEngine's current-player context.
function RebornParadropUnlockedRank(self)
    if RebornParadropHasUpgrade(self, "Upgrade_RebornParadrop_Row5") then return 3 end
    if RebornParadropHasUpgrade(self, "Upgrade_RebornParadrop_Row3") then return 2 end
    return 1
end

-- Reborn: preserve the three authored reload durations; round up fractional remaining seconds to avoid early readiness.
function RebornParadropDuration(level)
    return 270 + level * 30
end

-- Reborn: integer ceiling division keeps this compatible with the game's older Lua runtime.
function RebornParadropScale(value, targetDuration, sourceDuration)
    local result = 0
    local numerator = value * targetDuration
    while numerator > 0 do
        numerator = numerator - sourceDuration
        result = result + 1
    end
    return result
end

-- Reborn: the roster contains native object references; only role 0 owns persistent clock bits.
function RebornParadropClockFor(self)
    if RebornParadropIsClock(self) then return self end
    local actor, role = next(RebornParadropRoles)
    while actor do
        if role == 0 and RebornParadropSameOwner(self, actor) then return actor end
        actor, role = next(RebornParadropRoles, actor)
    end
    return nil
end

-- Reborn: schedule a native timed condition without executing a hidden special power on a visible building.
function RebornParadropArm(clock)
    local active = ObjectTestModelCondition(clock, "USER_8")
    if RebornParadropRemaining(clock) > 0 and not (active and active ~= 0) then
        ExecuteAction("UNIT_SET_MODELCONDITION_FOR_DURATION", clock, "USER_8", 1, 100)
    end
end

-- Reborn: native power timers advance themselves; this function is called only on an event, never on a clock tick.
function RebornParadropApply(actor, clock)
    if RebornParadropClockInvalid[clock] then return end
    local remaining = RebornParadropRemaining(clock)
    local duration = RebornParadropDuration(RebornParadropClockRank(clock))
    for i = 1, 3 do
        local seconds = RebornParadropScale(remaining, RebornParadropDuration(i), duration)
        ExecuteAction("NAMED_SET_SPECIAL_POWER_COUNTDOWN", actor, "SpecialPowerParadropLvl" .. i, seconds)
    end
end

-- Reborn: direct saved object references also reach the non-spatial spellbook; spatial broadcasts are unnecessary.
function RebornParadropPublish(clock)
    if RebornParadropClockInvalid[clock] then return end
    local actor, role = next(RebornParadropRoles)
    while actor do
        if (role == 4 or role == 5) and RebornParadropSameOwner(actor, clock) then
            RebornParadropApply(actor, clock)
        end
        actor, role = next(RebornParadropRoles, actor)
    end
end

-- Reborn: promote one authoritative clock once; repeated native row callbacks never rescale it again.
function RebornParadropPromote(clock, requested)
    if RebornParadropClockInvalid[clock] then return end
    local level = RebornParadropUnlockedRank(clock)
    if requested and requested > level then level = requested end
    local previous = RebornParadropClockRank(clock)
    if level > previous then
        local remaining = RebornParadropScale(RebornParadropRemaining(clock), RebornParadropDuration(level), RebornParadropDuration(previous))
        RebornParadropSetRemaining(clock, remaining)
        RebornParadropSetRank(clock, level)
        if RebornParadropRemaining(clock) ~= remaining or RebornParadropClockRank(clock) ~= level then
            RebornParadropClockInvalid[clock] = true
            ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockStoreFailed", 6)
            return
        end
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockPromoted", 6)
    end
end

-- Reborn: apply once after the engine finishes upgrading command sets and power availability.
function RebornParadropDefer(clock)
    RebornParadropDeferred[clock] = true
    ExecuteAction("UNIT_SET_MODELCONDITION_FOR_DURATION", clock, "USER_11", 1, 100)
end

-- Reborn: the negative-condition event also fires at creation, so require an explicit pending request.
function RebornParadropDeferredSync(clock)
    clock = RebornParadropActor(clock)
    RebornParadropRoles[clock] = 0
    if not RebornParadropDeferred[clock] then return end
    RebornParadropDeferred[clock] = nil
    if RebornParadropClockInvalid[clock] then return end
    RebornParadropPublish(clock)
    ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropDeferredSync", 6)
end

-- Reborn: creation and purchases synchronize now and once after native upgrade processing settles.
function RebornParadropCooldownRefresh(self, requested)
    local clock = RebornParadropClockFor(self)
    if not clock then
        if requested or RebornParadropHasUpgrade(self, "Upgrade_RebornParadrop_Row1") then
            ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockMissing", 6)
        end
        return
    end
    if RebornParadropClockInvalid[clock] then return end
    RebornParadropPromote(clock, requested)
    RebornParadropPublish(clock)
    RebornParadropDefer(clock)
    if not RebornParadropClockInvalid[clock] then RebornParadropArm(clock) end
end

-- Reborn: any branch's row-three purchase selects the same level-two timer.
function RebornParadropCooldownRow3(self) RebornParadropCooldownRefresh(self, 2) end
-- Reborn: row five promotes the same saved clock to level three.
function RebornParadropCooldownRow5(self) RebornParadropCooldownRefresh(self, 3) end

-- Reborn: a successful cast records the shared state once, then updates spellbook and towers at that event only.
function RebornParadropStartCast(clock, level)
    RebornParadropSetRank(clock, level)
    RebornParadropSetRemaining(clock, RebornParadropDuration(level))
    if RebornParadropRemaining(clock) ~= RebornParadropDuration(level) or RebornParadropClockRank(clock) ~= level then
        RebornParadropClockInvalid[clock] = true
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockStoreFailed", 6)
        return
    end
    RebornParadropClockInvalid[clock] = nil
    ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropCastRecorded", 6)
    RebornParadropPublish(clock)
    RebornParadropArm(clock)
end

-- Reborn: both interfaces address the same canonical powers and one owner-scoped logic clock.
function RebornParadropCast(self, level)
    local clock = RebornParadropClockFor(self)
    if clock then RebornParadropStartCast(clock, level)
    else ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockMissing", 6) end
end
-- Reborn: canonical level-one cast event.
function RebornParadropCast1(self) RebornParadropCast(self, 1) end
-- Reborn: canonical level-two cast event.
function RebornParadropCast2(self) RebornParadropCast(self, 2) end
-- Reborn: canonical level-three cast event.
function RebornParadropCast3(self) RebornParadropCast(self, 3) end

-- Reborn: only the logic object's saved bits change here; visible objects receive no periodic timer or upgrade writes.
function RebornParadropClockTick(clock)
    if not RebornParadropIsClock(clock) or RebornParadropClockInvalid[clock] then return end
    local remaining = RebornParadropRemaining(clock)
    if remaining <= 0 then return end
    if remaining == RebornParadropDuration(RebornParadropClockRank(clock)) then
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockTicking", 6)
    end
    RebornParadropSetRemaining(clock, remaining - 1)
    if RebornParadropRemaining(clock) ~= remaining - 1 then
        RebornParadropClockInvalid[clock] = true
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockStoreFailed", 6)
        return
    end
    if remaining > 1 then RebornParadropArm(clock) end
end

-- Reborn: remove dead roster entries so direct references never target a destroyed object.
function RebornParadropDestroyed(self)
    -- Reborn: avoid retaining a stale userdata wrapper after this native actor is destroyed.
    local id, actor = next(RebornParadropActors)
    while id do
        if actor == self then RebornParadropActors[id] = nil; break end
        id, actor = next(RebornParadropActors, id)
    end
    RebornParadropRoles[self] = nil
    RebornParadropClockInvalid[self] = nil
    RebornParadropDeferred[self] = nil
end

-- Reborn: discard duplicate logic clocks before they can publish a ready state; the surviving one outlives all towers.
function RebornParadropCreated(self)
    if RebornParadropIsClock(self) then
        local other, role = next(RebornParadropRoles)
        while other do
            if other ~= self and role == 0 and RebornParadropSameOwner(self, other) then
                RebornParadropDestroyed(self)
                ExecuteAction("NAMED_DELETE", self)
                return
            end
            other, role = next(RebornParadropRoles, other)
        end
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockActorActive", 6)
    elseif RebornParadropRoles[self] == 4 then
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropSpellbookActive", 6)
    else
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropTowerClockActive", 6)
    end
    -- Reborn: a new Tower's child is created on a later native update; this expected interval is not a missing-clock failure.
    if RebornParadropIsClock(self) or RebornParadropClockFor(self) then RebornParadropCooldownRefresh(self) end
end

-- Reborn: XML binds role 0 before OnCreated.
function RebornParadropClockCreated(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 0
    -- Reborn: never publish a ready timer if the native player identity grant did not take effect.
    if not RebornParadropBindOwner(self) then
        RebornParadropClockInvalid[self] = true
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockStoreFailed", 6)
        return
    end
    RebornParadropCreated(self, other)
end

-- Reborn: XML binds role 0 before OnDestroyed.
function RebornParadropClockDestroyed(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 0
    RebornParadropDestroyed(self, other)
end

-- Reborn: XML binds role 0 before RebornParadropCooldownRow3.
function RebornParadropClockRow3(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 0
    RebornParadropCooldownRow3(self, other)
end

-- Reborn: XML binds role 0 before RebornParadropCooldownRow5.
function RebornParadropClockRow5(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 0
    RebornParadropCooldownRow5(self, other)
end

-- Reborn: XML binds role 0 before RebornParadropClockExpired.
function RebornParadropClockTickEvent(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 0
    RebornParadropClockTick(self, other)
end

-- Reborn: XML binds role 5 before OnCreated.
function RebornParadropTowerCreated(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    -- Reborn: the known-working creation callback starts the retail OBJECT-triggered spawn path, independently of protocol purchase order.
    if not RebornParadropHasUpgrade(self, "Upgrade_RebornParadropClockRequest") then
        ObjectGrantUpgrade(self, "Upgrade_RebornParadropClockRequest")
    end
    -- Reborn: prove the object-local request was written before relying on the later OCL update.
    if not RebornParadropHasUpgrade(self, "Upgrade_RebornParadropClockRequest") then
        ExecuteAction("SHOW_MILITARY_CAPTION", "RebornDebug:ParadropClockRequestFailed", 6)
        return
    end
    RebornParadropCreated(self, other)
end

-- Reborn: XML binds role 5 before OnDestroyed.
function RebornParadropTowerDestroyed(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropDestroyed(self, other)
end

-- Reborn: XML binds role 5 before RebornParadropCooldownRow3.
function RebornParadropTowerRow3(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropCooldownRow3(self, other)
end

-- Reborn: XML binds role 5 before RebornParadropCooldownRow5.
function RebornParadropTowerRow5(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropCooldownRow5(self, other)
end

-- Reborn: XML binds role 5 before RebornParadropCastCondition1.
function RebornParadropTowerCast1(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropCast1(self, other)
end

-- Reborn: XML binds role 5 before RebornParadropCastCondition2.
function RebornParadropTowerCast2(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropCast2(self, other)
end

-- Reborn: XML binds role 5 before RebornParadropCastCondition3.
function RebornParadropTowerCast3(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 5
    RebornParadropCast3(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCooldownRow1.
function RebornParadropSpellbookRow1(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCooldownRefresh(self)
end

-- Reborn: XML binds role 4 before OnCreated.
function RebornParadropSpellbookCreated(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCreated(self, other)
end

-- Reborn: XML binds role 4 before OnDestroyed.
function RebornParadropSpellbookDestroyed(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropDestroyed(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCooldownRow3.
function RebornParadropSpellbookRow3(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCooldownRow3(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCooldownRow5.
function RebornParadropSpellbookRow5(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCooldownRow5(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCastCondition1.
function RebornParadropSpellbookCast1(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCast1(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCastCondition2.
function RebornParadropSpellbookCast2(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCast2(self, other)
end

-- Reborn: XML binds role 4 before RebornParadropCastCondition3.
function RebornParadropSpellbookCast3(self, other)
    self = RebornParadropActor(self)
    RebornParadropRoles[self] = 4
    RebornParadropCast3(self, other)
end
