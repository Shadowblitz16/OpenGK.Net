
namespace OpenGK;
public abstract class Scene
{
    public Scene()
    {

    }

    public abstract void OnStart();
    public abstract void OnUpdate(float dt);
    public abstract void OnEnd();
}