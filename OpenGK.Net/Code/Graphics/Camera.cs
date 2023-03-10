using OpenTK.Mathematics;
namespace OpenGK;

public class Camera
{
    private Shader shader = new();
    private Matrix4 projectionMatrix, viewMatrix;
    private Vector2 position;

    public Camera()
    {
        if (Window.Instance.IsReady())
            shader = new Shader();

        AdjustProjection();
    }
    public Camera (float x, float y)
    {
        this.position   = new(x,y);
        this.projectionMatrix = new();
        this.viewMatrix = new();
    }

    public void AdjustProjection()
    {
        projectionMatrix  = Matrix4.Identity;
        projectionMatrix *= Matrix4.CreateOrthographicOffCenter(0f, 32f * 40f, 0f, 32f*21f, 0f, 100f);
    }   

    public Matrix4 GetView      ()
    {
        var cameraFront   = new Vector3(0f, 0f, -1f);
        var cameraUp      = new Vector3(0f, 1f,  0f);
        this.viewMatrix   = Matrix4.Identity;
        this.viewMatrix  *= Matrix4.LookAt
        (
            new Vector3(position.X, position.Y, 20), 
            cameraFront + new Vector3(position.X, position.Y, 0), 
            cameraUp
        );

        return this.viewMatrix;
    }
    public Matrix4 GetProjection()
    {
        return this.projectionMatrix;
    }

    public void Begin()
    {
        shader.Begin(this);
    }

    public void End()
    {
        shader.End();
    }

    public void Translate(float x, float y)
    {
        position += new Vector2(x,y);
    }
}