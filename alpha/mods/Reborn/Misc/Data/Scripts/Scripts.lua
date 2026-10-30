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


-- Reborn: show a bounded, non-pausing test caption through the retail HUD action with its verified two-parameter signature.
function RebornParadropCooldownCaption(key)
    ExecuteAction("SHOW_MILITARY_CAPTION", key, 6)
end

-- Reborn: record the real callback/grant boundary through the engine's existing script-debug action, without modal dialogs.
function RebornParadropCooldownLog(message)
    ExecuteAction("DEBUG_STRING", "Reborn Paradrop: " .. message)
end

-- Reborn: accept full team paths and the engine's bare default-team names, without assuming a particular player slot.
function RebornParadropCooldownPlayer(self)
    local team = ObjectTeamName(self)
    if not team then return nil end
    local separator = strfind(team, "/")
    if separator then
        return strsub(team, 1, separator - 1)
    end
    if strsub(team, 1, 19) == "defaultSkirmishTeam" and strsub(team, 20) ~= "" then
        return strsub(team, 20)
    end
    if strfind(team, "^teamPlayer_%d+$") or strfind(team, "^teamPlyr[%w_]+$") then
        return strsub(team, 5)
    end
    return nil
end

-- Reborn: accept both numeric and boolean condition results without treating numeric zero as true.
function RebornParadropHasTech(player, tech)
    local result = EvaluateCondition("PLAYER_HAS_PLAYER_TECH", player, tech)
    return result and result ~= 0
end

-- Reborn: grant each transfer marker once through the engine, preserving protocol points and making the guard savegame-persistent.
function RebornParadropGrantTransfer(player, level)
    local tech = "PlayerTech_RebornParadropTransfer" .. level
    if not RebornParadropHasTech(player, tech) then
        RebornParadropCooldownLog("grant " .. tech .. " to " .. player)
        ExecuteAction("PLAYER_GRANT_SPECIAL_POWER", "SpecialPower_RebornParadropGrantTransfer" .. level, player)
        RebornParadropCooldownLog("grant result=" .. tostring(RebornParadropHasTech(player, tech)))
    end
    return RebornParadropHasTech(player, tech)
end

-- Reborn: any Allied protocol column can promote the power; transfer level 2 before level 3 and ignore later purchases in the same row.
function RebornParadropCooldownRefresh(self, event)
    local player = RebornParadropCooldownPlayer(self)
    if not player then
        RebornParadropCooldownLog("owner unresolved; team=" .. tostring(ObjectTeamName(self)))
        RebornParadropCooldownCaption("RebornDebug:ParadropOwnerMissing")
        return
    end
    RebornParadropCooldownLog("callback owner=" .. player .. "; team=" .. tostring(ObjectTeamName(self)))
    -- Reborn: a row model-condition event already proves its bound upgrade fired; do not gate it on a second purchase query.
    local row3 = event == "row3" or RebornParadropHasTech(player, "PlayerTech_Allied_TimeBomb_Rank1")
        or RebornParadropHasTech(player, "PlayerTech_Allied_ChronoRift_Rank2")
        or RebornParadropHasTech(player, "PlayerTech_Allied_CryoSatellite_Rank1")
    local row5 = event == "row5" or RebornParadropHasTech(player, "PlayerTech_Allied_TimeBomb_Rank3")
        or RebornParadropHasTech(player, "PlayerTech_ProductionBonus_Allies")
        or RebornParadropHasTech(player, "PlayerTech_Allied_CryoSatellite_Rank3")
    if row5 then
        if RebornParadropGrantTransfer(player, "2") then
            if RebornParadropGrantTransfer(player, "3") then
                RebornParadropCooldownCaption("RebornDebug:ParadropTransfer3Granted")
            else
                RebornParadropCooldownCaption("RebornDebug:ParadropTransfer3Failed")
            end
        else
            RebornParadropCooldownCaption("RebornDebug:ParadropTransfer2Failed")
        end
    elseif row3 then
        if RebornParadropGrantTransfer(player, "2") then
            RebornParadropCooldownCaption("RebornDebug:ParadropTransfer2Granted")
        else
            RebornParadropCooldownCaption("RebornDebug:ParadropTransfer2Failed")
        end
    else
        RebornParadropCooldownCaption("RebornDebug:ParadropCallbackReady")
    end
end

-- Reborn: identify the row-3 model-condition callback independently of the purchase condition result.
function RebornParadropCooldownRow3(self)
    RebornParadropCooldownLog("row 3 event received")
    RebornParadropCooldownRefresh(self, "row3")
end

-- Reborn: identify the row-5 model-condition callback independently of the purchase condition result.
function RebornParadropCooldownRow5(self)
    RebornParadropCooldownLog("row 5 event received")
    RebornParadropCooldownRefresh(self, "row5")
end

-- Reborn: report global script execution once per Lua state, without a map-local override or cooldown changes.
if not RebornParadropGlobalLoadReported then
    RebornParadropGlobalLoadReported = true
    RebornParadropCooldownCaption("RebornDebug:ParadropGlobalLoaded")
    RebornParadropCooldownLog("global Scripts.lua loaded")
end
