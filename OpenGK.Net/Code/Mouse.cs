using OpenTK.Input;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode  = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
using GLFWWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;
namespace OpenGK;

/// <summary>
///     Specifies the buttons of a mouse.
/// </summary>
public enum MouseCode
{

    /// <summary>
    ///     Any button.
    /// </summary>
    Any = -2,   
    /// <summary>
    ///     The first button.
    /// </summary>
    Button1 = 0,

    /// <summary>
    ///     The second button.
    /// </summary>
    Button2 = 1,

    /// <summary>
    ///     The third button.
    /// </summary>
    Button3 = 2,

    /// <summary>
    ///     The fourth button.
    /// </summary>
    Button4 = 3,

    /// <summary>
    ///     The fifth button.
    /// </summary>
    Button5 = 4,

    /// <summary>
    ///     The sixth button.
    /// </summary>
    Button6 = 5,

    /// <summary>
    ///     The seventh button.
    /// </summary>
    Button7 = 6,

    /// <summary>
    ///     The eighth button.
    /// </summary>
    Button8 = 7,

    /// <summary>
    ///     The left mouse button. This corresponds to <see cref="Button1"/>.
    /// </summary>
    Left = Button1,

    /// <summary>
    ///     The right mouse button. This corresponds to <see cref="Button2"/>.
    /// </summary>
    Right = Button2,

    /// <summary>
    ///     The middle mouse button. This corresponds to <see cref="Button3"/>.
    /// </summary>
    Middle = Button3,

}

public static class Mouse
{
    public static readonly int Count = (int)Enum.GetValues<MouseCode>().Max()+1;

    private static double currSX, currSY, lastSX, lastSY;
    private static double currPX, currPY, lastPX, lastPY;
    private static bool[] pressed  = new bool[Count];
    private static bool[] released = new bool[Count];
    private static bool[] dragging = new bool[Count];



    internal static unsafe void MoveCallback  (GLFWWindow* window, double x, double y)
    {
        lastPX = currPX;
        lastPY = currPY;
        currPX = x;
        currPY = y;

        for (var i=0; i<pressed.Length; i++)
        {
            if (pressed[i]) dragging[i] = true;
        }
    }
    
    internal static unsafe void ScrollCallback(GLFWWindow* window, double x, double y)
    {
        lastSX = currSX;
        lastSY = currSY;
        currSX = x;
        currSY = y;
    }
    internal static unsafe void ButtonCallback(GLFWWindow* window, MouseButton button, InputAction action, KeyModifiers mods)
    {
        if      (action == InputAction.Press  )
        {
            var index = ((int)button);
            if (Enum.IsDefined<MouseButton>((MouseButton)index)) 
            {
                pressed [index] = true;
                released[index] = false;
            }
        }
        else if (action == InputAction.Release)
        {
            var index = ((int)button);
            if (Enum.IsDefined<MouseButton>((MouseButton)index))
            {
                pressed [index] = false;
                released[index] = true;
                dragging[index] = false;
            }  
        }
    }
    internal static unsafe void EndFrame()
    {
        lastSY = currSX;
        lastSY = currSY;
        lastPX = currPX;
        lastPY = currPY;

        foreach (var button in Enum.GetValues<MouseButton>())
        {
            var index = (int)button;
            if (index < 0) continue;
            released[index] = pressed[index];
        }
    }
    
    public static float GetScrollX   () => (float)currSX;
    public static float GetScrollY   () => (float)currSY;
    public static float GetScrollDX  () => (float)(lastSX - currSX);
    public static float GetScrollDY  () => (float)(lastSY - currSY);
    public static float GetPositionX () => (float)currPX;
    public static float GetPositionY () => (float)currPY;
    public static float GetPositionDX() => (float)(lastPX - currPX);
    public static float GetPositionDY() => (float)(lastPY - currPY);

    public static bool IsDragging (MouseCode button)
    {
        var index = ((int)button);
        var code  = (MouseCode)index;
        if (code == MouseCode.Any) return dragging.Any();
        if (Enum.IsDefined<MouseButton>((MouseButton)index)) return dragging[index];
        return false;
    }
    public static bool IsPressed  (MouseCode button)
    {
        var index = ((int)button);
        var code  = (MouseCode)index;
        if (code == MouseCode.Any)
        {
            foreach (var item in Enum.GetValues<MouseButton>())
            {
                var i = (int)item;
                if (i < 0) continue;
                if (pressed[i] && released[i]) return true;
            }
            return false;
        }
        if (Enum.IsDefined<MouseButton>((MouseButton)index)) return  pressed[index] &&  released[index];
        return false;    
    }
    public static bool IsReleased (MouseCode button)
    {
        var index = ((int)button);
        var code  = (MouseCode)index;
        if (code == MouseCode.Any)
        {
            foreach (var item in Enum.GetValues<MouseButton>())
            {
                var i = (int)item;
                if (i < 0) continue;
                if (!pressed[i] && !released[i]) return true;
            }
            return false;
        }
        if (Enum.IsDefined<MouseButton>((MouseButton)index)) return !pressed[index] && !released[index];
        return false;    
    }   
    public static bool WasPressed (MouseCode button)
    {
        var index = ((int)button);
        var code  = (MouseCode)index;
        if (code == MouseCode.Any) 
        {
            foreach (var item in Enum.GetValues<MouseButton>())
            {
                var i = (int)item;
                if (i < 0) continue;
                if (pressed[i] && !released[i]) return true;
            }
            return false;
        }
        if (Enum.IsDefined<MouseButton>((MouseButton)index)) return  pressed[index] && !released[index];
        return false;    
    }
    public static bool WasReleased(MouseCode button)
    {
        var index = ((int)button);
        var code  = (MouseCode)index;
        if (code == MouseCode.Any)
        {
            foreach (var item in Enum.GetValues<MouseButton>())
            {
                var i = (int)item;
                if (i < 0) continue;
                if (!pressed[i] && released[i]) return true;
            }
            return false;
        }
        if (Enum.IsDefined<MouseButton>((MouseButton)index)) return !pressed[index] &&  released[index];
        return false;    
    }
}