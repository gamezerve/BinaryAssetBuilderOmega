using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain ordinary Core import selectors for every dependency, with schema defaults and no artificial point/list quota. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTech* target,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("TechPointsRequired","0"),&target->TechPointsRequired,state);Marshal(node.GetAttributeValue("GrantFX",null),&target->GrantFX,state);Marshal(node.GetChildNodes("TechDependency"),&target->TechDependency,state);}
}
