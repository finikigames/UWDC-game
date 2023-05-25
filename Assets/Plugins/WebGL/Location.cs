// FILE: Assets/Plugins/WebGL/Location.cs
using System.Runtime.InteropServices;
 
public class Location
{
#if UNITY_WEBGL && !UNITY_EDITOR
    // get href from javascript.
    [DllImport("__Internal")]
    public static extern string href();
#else
    // for test
    public static string href() { return @"https://example.com/?product=shirt"; }
#endif
}