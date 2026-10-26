using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData;
// Reborn: EP1 jets extend the 76-byte AI record with five words and four byte flags, without KW takeoff/lockon fields.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1JetAIUpdateModuleData
{
    public Ep1AIUpdateModuleData Base;
    public Percentage OutOfAmmoDamagePerSecond;
    public Time OutOfAmmoDamageDelay;
    public float ParkingOffset;
    public AssetReference<WeaponTemplate> UseForOutOfAmmoCheck;
    public AssetReference<BaseAssetType> OutOfAmmoEvaEvent;
    public SageBool NeedsRunway,ReturnToBaseWhenVictimDead,KeepsParkingSpaceWhenAirborne,CirclesForAttack;
}
