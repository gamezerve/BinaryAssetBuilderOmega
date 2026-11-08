using System;
using System.Linq;
using System.Reflection;
using Relo;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: match the complete writer signature exactly so int/uint/float pointer overloads cannot become ambiguous. */
    //-------------------------------------------------------------------------------------------------
    internal static MethodInfo ExactMarshalMethod(Type pointer)
    {
        var signature=new[]{typeof(Node),pointer,typeof(Tracker)};
        var exact=typeof(Marshaler).GetMethods(BindingFlags.Public|BindingFlags.Static)
            .Where(method=>method.Name==nameof(Marshal)&&!method.IsGenericMethod)
            .SingleOrDefault(method=>method.GetParameters().Select(parameter=>parameter.ParameterType).SequenceEqual(signature));
        if(exact!=null)return exact;
        var element=pointer.GetElementType();
        if(element?.IsEnum!=true)return null;
        // Reborn: close only the existing Node/T*/Tracker enum overload; pointer-to-pointer and unrelated generic writers stay excluded.
        return typeof(Marshaler).GetMethods(BindingFlags.NonPublic|BindingFlags.Static)
            .Where(method=>method.Name==nameof(Marshal)&&method.IsGenericMethodDefinition&&method.GetGenericArguments().Length==1)
            .Single(method=>{var parameters=method.GetParameters();return parameters.Length==3&&parameters[0].ParameterType==typeof(Node)&&parameters[1].ParameterType.IsPointer&&parameters[1].ParameterType.GetElementType()!.IsGenericParameter&&parameters[2].ParameterType==typeof(Tracker);})
            .MakeGenericMethod(element);

    }
}
