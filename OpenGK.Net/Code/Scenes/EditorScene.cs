
namespace OpenGK;
public class EditorScene : Scene
{
    bool changingScene;
    float timeToChangeScene = 2.0f;
    public override void OnStart()
    {
        Console.WriteLine("Editor");
    }
    public override void OnUpdate(float dt)
    {
        Console.WriteLine("" + (1.0 / dt) + " FPS");

        if (!changingScene && Keyboard.IsPressed(KeyCode.Space))
        {
            changingScene = true;
        }

        if (changingScene && timeToChangeScene > 0)
        {
            timeToChangeScene -= dt;
            Window.Instance.R -= dt * 5.0f;
            Window.Instance.G -= dt * 5.0f;
            Window.Instance.B -= dt * 5.0f;
            
        }
        else if (changingScene)
        {
            Window.Instance.GoTo(new EngineScene());
        }
    }

    public override void OnEnd()
    {
        
    }
}