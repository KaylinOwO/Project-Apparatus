using UnityEngine;

public static partial class Extensions
{
    public static T Unfake<T>(this T obj) where T : Object
    {
        return obj == null || obj.Equals(null) ? default(T) : obj;
    }
}
