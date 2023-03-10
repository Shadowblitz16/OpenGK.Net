
using System.Diagnostics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGK;

public static class Time
{
    public readonly static float TimeStarted = (float)GLFW.GetTime(); 
    public static float GetTime() => (float)((GLFW.GetTime() - TimeStarted)); 

}