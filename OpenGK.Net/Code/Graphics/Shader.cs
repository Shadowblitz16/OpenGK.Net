using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using System.Text.RegularExpressions;
namespace OpenGK;

public class Shader
{
    #region Fields       

    private static string vDefaultShaderSource = @$"
#version 330 core

layout (location=0) in vec3 aPos;
layout (location=1) in vec4 aColor;

uniform mat4  uProjection;
uniform mat4  uView;
uniform float uTime;

out vec4  fColor;
out float fTime;

void main()
{{
    fColor = aColor;
    fTime  = uTime;
    gl_Position = uProjection * uView * vec4(aPos, 1.0);
}}";
    private static string fDefaultShaderSource = @$"
#version 330 core

in vec4  fColor;
in float fTime;

out vec4 color;

void main()
{{
    float noise = fract(sin(dot(fColor.xy, vec2(12.9898, 78.233))) * 43758.5453);
    color = fColor * noise;
}}";
    private int    sId, vId, fId;
    private bool   bound;
    private string vSource = vDefaultShaderSource; 
    private string fSource = fDefaultShaderSource; 

    #endregion

    #region ID           

    public IntPtr GetID() => new(sId);


    #endregion
    #region Bind         
    
    public void Begin(Camera camera)
    {
        Begin();
        UploadMatrix4("uProjection", camera.GetProjection());
        UploadMatrix4("uView"      , camera.GetView());
    }
    public void Begin()
    {
        if (sId == 0 || bound) return;
        bound = true;
        GL.UseProgram(sId);
        UploadFloat1("uTime", Time.GetTime());


    }
    public void End()
    {
        GL.UseProgram(0);
        bound = false;
    }
    
    #endregion
    #region Utility      
   
    private static string ReadToEnd(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
    private void CompileProgram()
    {
        // Create program
        sId = GL.CreateProgram();

        // Compile shaders
        CompileShaders();

        // Attach the shaders and link the program
        GL.AttachShader(sId, vId);
        GL.AttachShader(sId, fId);
        GL.LinkProgram (sId);

        // Check for errors
        GL.GetProgram(sId, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var log = GL.GetProgramInfoLog(sId);
            Console.WriteLine($"ERROR:: 'shader program");
            Console.WriteLine($"\t linking failed");
            Console.WriteLine($"\t {log.Replace(Environment.NewLine, Environment.NewLine+"\t")}");   

            GL.DetachShader (sId, vId);
            GL.DetachShader (sId, fId);
            GL.DeleteShader (vId);
            GL.DeleteShader (fId);
            GL.DeleteProgram(sId);
        }
    }
    private void CompileShaders()
    {
        // Create shader
        vId = GL.CreateShader(ShaderType.VertexShader);

        // Pass the shader source to the GPU and compile
        GL.ShaderSource (vId, vSource);
        GL.CompileShader(vId);

        // Check for errors
        GL.GetShader(vId, ShaderParameter.CompileStatus, out var success1);
        if (success1 == 0)
        {
            var log = GL.GetShaderInfoLog(vId);
            Console.WriteLine($"ERROR:: 'vertex' shader");
            Console.WriteLine($"\t compilation failed");
            Console.WriteLine($"\t {log.Replace(Environment.NewLine, Environment.NewLine+"\t")}");   
        }

        // Create shader
        fId = GL.CreateShader(ShaderType.FragmentShader);

        // Pass the shader source to the GPU and compile
        GL.ShaderSource (fId, fSource);
        GL.CompileShader(fId);

        // Check for errors
        GL.GetShader(fId, ShaderParameter.CompileStatus, out var success2);
        if (success2 == 0)
        {
            var log = GL.GetShaderInfoLog(fId);
            Console.WriteLine($"ERROR:: 'fragment' shader");
            Console.WriteLine($"\t compilation failed");
            Console.WriteLine($"\t {log.Replace(Environment.NewLine, Environment.NewLine+"\t")}");   
        }
    }

    #endregion
    #region Constructors 
    
    public Shader() 
    {
        if (Window.Instance.IsReady())
        {
            CompileProgram();
        }
        else
        {
            this.sId = 0;
            this.vId = 0;
            this.fId = 0;
        }
    }
    public Shader(string source)
    {
        if (Window.Instance.IsReady())
        {
            string[] split = Regex.Split(source, "#type +[a-zA-Z]+");

            // Find first match
            int index1 = source.IndexOf("#type")+6;
            int index2 = source.IndexOf(Environment.NewLine, index1);
            var match1 = source.Substring(index1, index2-index1).Trim();

            // Find second match
            int index3 = source.IndexOf("#type", index2)+6;
            int index4 = source.IndexOf(Environment.NewLine, index3);
            var match2 = source.Substring(index3, index4-index3).Trim();

            // check if first match is vertex, fragment or error
            if      (match1.Equals("vertex"  ))
            {
                vSource = split[0];
            }
            else if (match1.Equals("fragment"))
            {
                fSource = split[0];
            }
            else 
            {
                throw new FormatException($"Unexpected token '{match1}'");
            }
            
            // check if second match is vertex, fragment or error
            if      (match2.Equals("vertex"  ))
            {
                vSource = split[1];
            }
            else if (match2.Equals("fragment"))
            {
                fSource = split[1];
            } 
            else 
            {
                throw new FormatException($"Unexpected token '{match2}'");
            }
        
            CompileProgram();
        }
        else
        {
            this.sId = 0;
            this.vId = 0;
            this.fId = 0;
        }
    }
    public Shader(Stream stream) : this(ReadToEnd(stream))
    {

    }

    #endregion

    #region Upload Bool  

    public void UploadBool1  (string varName, bool x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x ? 1 : 0);
    }
    public void UploadBool2  (string varName, bool x, bool y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x ? 1 : 0, y ? 1 : 0);
    }
    public void UploadBool3  (string varName, bool x, bool y, bool z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x ? 1 : 0, y ? 1 : 0, z ? 1 : 0);
    }
    public void UploadBool4  (string varName, bool x, bool y, bool z, bool w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x ? 1 : 0, y ? 1 : 0, z ? 1 : 0, w ? 1 : 0);
    }
   
    #endregion
    #region Upload Int   
    
    // Byte overloads 
    public void UploadInt1  (string varName, sbyte x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadInt2  (string varName, sbyte x, sbyte y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadInt3  (string varName, sbyte x, sbyte y, sbyte z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadInt4  (string varName, sbyte x, sbyte y, sbyte z, sbyte w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }
   
    // Short overloads 
    public void UploadInt1  (string varName, short x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadInt2  (string varName, short x, short y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadInt3  (string varName, short x, short y, short z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadInt4  (string varName, short x, short y, short z, short w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }
   
    // Int overloads 
    public void UploadInt1  (string varName, int x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadInt2  (string varName, int x, int y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadInt3  (string varName, int x, int y, int z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadInt4  (string varName, int x, int y, int z, int w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }
   
    #endregion
    #region Upload UInt  

    // Byte overload
    public void UploadUInt1  (string varName, byte x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadUInt2  (string varName, byte x, byte y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadUInt3  (string varName, byte x, byte y, byte z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadUInt4  (string varName, byte x, byte y, byte z, byte w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }

    // Short overloads
    public void UploadUInt1  (string varName, ushort x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadUInt2  (string varName, ushort x, ushort y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadUInt3  (string varName, ushort x, ushort y, ushort z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadUInt4  (string varName, ushort x, ushort y, ushort z, ushort w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }

    // Int overloads
    public void UploadUInt1  (string varName, uint x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadUInt2  (string varName, uint x, uint y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadUInt3  (string varName, uint x, uint y, uint z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadUInt4  (string varName, uint x, uint y, uint z, uint w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }

    #endregion
    #region Upload Float 

    public void UploadFloat1 (string varName, float x)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform1(varLocation,  x);
    }
    public void UploadFloat2 (string varName, float x, float y)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform2(varLocation,  x, y);
    }
    public void UploadFloat3 (string varName, float x, float y, float z)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform3(varLocation,  x, y, z);
    }
    public void UploadFloat4 (string varName, float x, float y, float z, float w)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.Uniform4(varLocation,  x, y, z, w);
    }
    
    #endregion
    #region Upload Matrix

    public void UploadMatrix2(string varName, Matrix2 matrix)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.UniformMatrix2(varLocation, false, ref matrix);
    }
    public void UploadMatrix3(string varName, Matrix3 matrix)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.UniformMatrix3(varLocation, false, ref matrix);
    }
    public void UploadMatrix4(string varName, Matrix4 matrix)
    {
        Begin();
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.UniformMatrix4(varLocation, false, ref matrix);
    }
    
    #endregion
}