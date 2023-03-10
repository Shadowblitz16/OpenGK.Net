
namespace OpenGK;
public abstract class Scene
{
    public Shader DefaultShader  { get; private set; } = new();

    public Scene()
    {

    }

    public virtual void OnStart()
    {
        DefaultShader = new();
    }
    public virtual void OnUpdate(float dt)
    {

    }
    public virtual void OnEnd()
    {

    }
}