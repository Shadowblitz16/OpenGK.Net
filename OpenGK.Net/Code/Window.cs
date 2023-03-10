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

    private Scene? scene = null;
    private string title  = "Mario";
    private int    width  = 1920;
    private int    height = 1080;
    public float R = 1, G = 1, B = 1, A = 1;

    
    #endregion
    
    public void GoTo(Scene scene)
    {
        this?.scene?.OnEnd();
        this.scene = scene;
        this?.scene?.OnStart();
    }

    public void Quit()
    {
        unsafe
        {
            GLFW.SetWindowShouldClose(window, true);
        }
    }

    public void Clear()
    {
        unsafe 
        {
            GL.ClearColor(R,G,B,A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
    }

    public void Run(Scene scene)
    {
        this.scene = scene;
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

        // Set mouse callbacks
        GLFW.SetCursorPosCallback  (window, Mouse.MoveCallback  );
        GLFW.SetScrollCallback     (window, Mouse.ScrollCallback);
        GLFW.SetMouseButtonCallback(window, Mouse.ButtonCallback);

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
        var deltaTime = -1f;
        var beginTime = Time.GetTime();
        var endTime   = 0f;
        while(!GLFW.WindowShouldClose(window))
        {
            // Poll events
            GLFW.PollEvents();
            
            // Clear
            Clear();

            // Update game
            if (deltaTime >= 0)
                this.scene?.OnUpdate(deltaTime);

            // End input frame
            Keyboard.EndFrame();
            Mouse.EndFrame();

            // Swap backbuffer
            GLFW.SwapBuffers(window);

            // Delta time
            endTime   = Time.GetTime();
            deltaTime = endTime - beginTime;
            beginTime = endTime;
        }
    }
    private unsafe void Free()
    {
        // End scene
        this?.scene?.OnEnd();

        // Free the key callbacks
        GLFW.SetKeyCallback(window, null);

        // Free mouse callbacks
        GLFW.SetCursorPosCallback  (window, null);
        GLFW.SetScrollCallback     (window, null);
        GLFW.SetMouseButtonCallback(window, null);

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


    public static unsafe implicit operator GLFWWindow*(Window w)
    {
        return w.window;
    }
}