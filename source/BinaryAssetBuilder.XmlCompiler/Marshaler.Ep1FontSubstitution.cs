using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the requested source size and replacement font without imposing a host font installation dependency. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FontSubstitute* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FontSubstitute.Size),null),&target->Size,state);
        Marshal(node.GetChildNode(nameof(Ep1FontSubstitute.Font),null),&target->Font,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the original locale font substitution name and ordered interpolation entries. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FontSubstitution* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FontSubstitution.Name),null),&target->Name,state);
        Marshal(node.GetChildNodes(nameof(Ep1FontSubstitution.Substitute)),&target->Substitute,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }
}
