using OpenTK.Graphics.ES30;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode  = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
using GLFWWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace OpenGK;
public class Window
{
    #region Singleton
    
    private static readonly Lazy<Window> _lazy = new Lazy<Window>(() => new Window());
    public static Window Instance => _lazy.Value;
    private Window()
    {

    }

    #endregion
    #region Fields
   
    private unsafe GLFWWindow* window;
    private string title  = "Mario";
    private int    width  = 1920;
    private int    height = 1080;
    
    #endregion
    
    public void Run()
    {
        unsafe 
        {
            Init();
            Loop();
            Free();
        }
    }

    private unsafe void Init()
    {
        // Set error callback
        GLFW.SetErrorCallback(OnError);

        // Initalize GLFW
        if (!GLFW.Init()) throw new InvalidOperationException("Unable to initailize GLFW.");

        // Configure GLFW
        GLFW.DefaultWindowHints();
        GLFW.WindowHint(WindowHintBool.Visible  , false);
        GLFW.WindowHint(WindowHintBool.Resizable, true );
        GLFW.WindowHint(WindowHintBool.Maximized, true );

        // Create the window
        window = GLFW.CreateWindow(this.width, this.height, this.title, null, null);
        if (window == null) throw new InvalidOperationException("Unable to create GLFW window.");

        // Set key callbacks
        GLFW.SetKeyCallback(window, Keyboard.KeyCallback);

        // Make the opengl context current
        GLFW.MakeContextCurrent(window);

        // Enable vsync and Show window
        GLFW.SwapInterval(1);
        GLFW.ShowWindow(window);

        // Load the GL bindings and Clear to default color
        GL.LoadBindings(new GLFWBindingsContext());
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    private unsafe void Loop()
    {
        while(!GLFW.WindowShouldClose(window))
        {
            // Poll events
            GLFW.PollEvents();
            
            // Clear to black
            GL.ClearColor(1,0,0,0);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Update game
            // TODO

            // End input frame
            Keyboard.EndFrame();

            // Swap backbuffer
            GLFW.SwapBuffers(window);
        }
    }
    private unsafe void Free()
    {
        // Free the key callbacks
        GLFW.SetKeyCallback(window, null);

        // Free the memory
        GLFW.DestroyWindow(window);

        // Terminate GLFW and free the error callback
        GLFW.Terminate();
        GLFW.SetErrorCallback(null);
    }

    private void OnError(ErrorCode error, string description)
    {
        Console.WriteLine(description);
    }
}