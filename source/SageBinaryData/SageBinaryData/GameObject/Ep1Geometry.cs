using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 adds RING without altering legacy geometry enum consumers.
public enum Ep1GeometryType { SPHERE, BOX, CYLINDER, RING }

// Reborn: stock Shape remains a 48-byte record; the enum is scoped so new EP1 literals cannot shift old users.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1Shape
{
    public Ep1GeometryType Type;
    public ContactPointGenerationType ContactPointGeneration;
    public AnsiString Name;
    public float MajorRadius;
    public float MinorRadius;
    public AnsiString Other;
    public float Height;
    public Angle FrontAngle;
    public unsafe Coord3D* Offset;
    public SageBool Active;
    public SageBool UsedForHealthBox;
}

// Reborn: EP1 Geometry inserts PlacementShape at +4; the subsequent count/items pairs and IsSmall are four bytes later than the legacy KW model.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1Geometry
{
    public unsafe Coord2D* RotationAnchorOffset;
    public unsafe Ep1Shape* PlacementShape;
    public List<Ep1Shape> Shape;
    public List<ContactPoint> ContactPoint;
    public SageBool IsSmall;
}
