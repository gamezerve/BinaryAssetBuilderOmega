namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: real builds have no fixture-sized source graph quotas; isolated diagnostic profiles retain their original refusal controls.
internal static class SdkSourceScale
{
    [ThreadStatic] private static int scopes;
    internal static bool Unrestricted=>scopes!=0;
    // Reborn: source XML can use the process's representable byte array size instead of a small diagnostic file allowance.
    internal static int XmlLimit(int diagnosticLimit)=>Unrestricted?int.MaxValue:diagnosticLimit;
    // Reborn: nested source entry points restore their previous mode even after a located XML/include error.
    internal static IDisposable Enter()=>new Scope();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schedule dependencies before consumers without recursive calls; reject actual active cycles and reuse shared subgraphs. */
    //-------------------------------------------------------------------------------------------------
    internal static IEnumerable<string> PostOrder(string root,Func<string,IEnumerable<string>> children,StringComparer comparer,string kind)
    {
        var state=new Dictionary<string,byte>(comparer);var pending=new Stack<(string Name,bool Exit)>();pending.Push((root,false));
        while(pending.Count!=0){
            var frame=pending.Pop();state.TryGetValue(frame.Name,out byte status);
            if(frame.Exit){state[frame.Name]=2;yield return frame.Name;continue;}
            if(status==2)continue;if(status==1)throw new InvalidDataException(kind+" cycle: "+frame.Name);
            state[frame.Name]=1;pending.Push((frame.Name,true));foreach(string child in children(frame.Name).Reverse())pending.Push((child,false));
        }
    }
    private sealed class Scope:IDisposable
    {
        private bool disposed;
        // Reborn: source preparation and routing opt in automatically; no UI setting or artificial replacement cap is required.
        internal Scope(){scopes++;}
        // Reborn: do not leak production source behavior into subsequent diagnostic work on the same compiler thread.
        public void Dispose(){if(!disposed){scopes--;disposed=true;}}
    }
}
