namespace OpenGK;
public class EngineScene : Scene
{
    public override void OnStart()
    {
        Console.WriteLine("Engine");
        Window.Instance.R = 1;
        Window.Instance.G = 1;
        Window.Instance.B = 1;
        
    }
    public override void OnUpdate(float dt)
    {
        
    }

    public override void OnEnd()
    {
        
    }
}