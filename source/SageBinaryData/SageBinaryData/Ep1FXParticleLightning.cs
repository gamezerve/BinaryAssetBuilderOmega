using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: Uprising stores changes to the ending lightning parameters, using the original schema field names.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FXParticleDrawLightningRandomSet
{
    public ClientRandomVariable StartAmplitude;
    public ClientRandomVariable EndAmplitudeChange;
    public ClientRandomVariable StartFrequency;
    public ClientRandomVariable EndFrequencyChange;
    public ClientRandomVariable StartPhase;
    public ClientRandomVariable EndPhaseChange;
}

// Reborn: the EP1 lightning header retains two coordinates and a list of 72-byte random sets.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FXParticleDrawLightning
{
    public FXParticleDrawBase Base;
    public Coord3D StartPoint;
    public Coord3D EndPoint;
    public List<Ep1FXParticleDrawLightningRandomSet> RandomSet;
}
