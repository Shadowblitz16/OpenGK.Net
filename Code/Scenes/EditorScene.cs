using OpenTK.Graphics.ES30;
namespace OpenGK;
public class EditorScene : Scene
{
    Texture testTexture = new();

    float[] vArray = 
    {
        // position                  // color                       // uvs
         100.5f,  000.5f, 0.0f,      1.0f, 0.0f, 0.0f, 1.0f,        1, 0, // 0:Bottom right
         000.5f,  100.5f, 0.0f,      0.0f, 1.0f, 0.0f, 1.0f,        0, 1, // 1:Top left
         100.5f,  100.5f, 0.0f,      0.0f, 0.0f, 1.0f, 1.0f,        1, 1, // 2:Top right
         000.5f,  000.5f, 0.0f,      1.0f, 1.0f, 0.0f, 1.0f,        0, 0, // 3:Bottom left
    };

    // Important: Must be in counter-clockwise order
    int[] eArray =
    {
        2,1,0,
        0,1,3
    };

    int vao, vbo, ebo;
    public override void OnStart()
    {
        base.OnStart();
        
        testTexture = new("Assets/Images/testImage.png");

        // Create vao and bind it
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Create vbo and upload the vertex buffer
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vao);
        GL.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float) * vArray.Length, vArray, BufferUsageHint.StaticDraw);

        // Create ebo and upload the element buffer
        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData<int>(BufferTarget.ElementArrayBuffer, sizeof(int) * eArray.Length, eArray, BufferUsageHint.StaticDraw);

        // Add the vertex attribute pointers
        var sizePositions   = 3;
        var sizeColors      = 4;
        var sizeUv          = 2;
        var sizeFloatBytes  = sizeof(float);
        var sizeVertexBytes = (sizePositions + sizeColors + sizeUv) * sizeFloatBytes;

        // Setup and enable vertex attribute 0
        GL.VertexAttribPointer(0, sizePositions, VertexAttribPointerType.Float, false, sizeVertexBytes, 0);
        GL.EnableVertexAttribArray(0);

        // Setup and enable vertex attribute 1
        GL.VertexAttribPointer(1, sizeColors, VertexAttribPointerType.Float, false, sizeVertexBytes, sizePositions * sizeFloatBytes);
        GL.EnableVertexAttribArray(1);

        // Setup and enable vertex attribute 2
        GL.VertexAttribPointer(2, sizeUv, VertexAttribPointerType.Float, false, sizeVertexBytes, (sizePositions + sizeColors) * sizeFloatBytes);
        GL.EnableVertexAttribArray(2);
    }
    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);

        DefaultCamera.Translate(-dt * 50f, 0);

        // Bind shader
        DefaultCamera.Begin();
        DefaultCamera.GetShader().UploadSampler2D("texture0", 0);
        testTexture.Begin();
        GL.ActiveTexture(TextureUnit.Texture0);


        // Bind the vao
        GL.BindVertexArray(vao);

        // Enable vertex attribute 0 and 1 and 2
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);

        // Draw elements
        GL.DrawElements(PrimitiveType.Triangles, eArray.Length, DrawElementsType.UnsignedInt, 0);

        // Disable vertex attribute 0 and 1 and 2
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.DisableVertexAttribArray(2);

        // Unbind the vao
        GL.BindVertexArray(0);

        // Unbind shader
        testTexture.End();
        DefaultCamera.End();
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
}