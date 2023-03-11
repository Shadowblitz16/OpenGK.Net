
namespace OpenGK;
public abstract class Scene
{
    public Camera DefaultCamera  { get; private set; } = new();

    public Scene()
    {

    }

    public virtual void OnStart()
    {
        DefaultCamera = new();
    }
    public virtual void OnUpdate(float dt)
    {

    }
    public virtual void OnEnd()
    {

    }
}