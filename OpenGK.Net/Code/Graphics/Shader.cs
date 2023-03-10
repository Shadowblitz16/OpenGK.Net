using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using System.Text.RegularExpressions;
namespace OpenGK;
public class Shader
{
    private static string vDefaultShaderSource = @$"
#version 330 core

layout (location=0) in vec3 aPos;
layout (location=1) in vec4 aColor;

uniform mat4 uProjection;
uniform mat4 uView;

out vec4 fColor;

void main()
{{
    fColor = aColor;
    gl_Position = uProjection * uView * vec4(aPos, 1.0);
}}";
    private static string fDefaultShaderSource = @$"
#version 330 core

in vec4 fColor;
out vec4 color;

void main()
{{
    color = fColor;
}}";
    private int    sId, vId, fId;
    private bool   bound;
    private string vSource = vDefaultShaderSource; 
    private string fSource = fDefaultShaderSource; 
    public IntPtr GetID() => new(sId);

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

    public void Begin(Camera camera)
    {
        Begin();
        UploadMatrix("uProjection", camera.GetProjection());
        UploadMatrix("uView"      , camera.GetView());
    }
    public void Begin()
    {
        if (sId == 0 || bound) return;
        GL.UseProgram(sId);
        bound = true;
    }

    public void End()
    {
        GL.UseProgram(0);
        bound = false;
    }

    public void UploadMatrix(string varName, Matrix4 matrix)
    {
        var varLocation = GL.GetUniformLocation(sId, varName);
        GL.UniformMatrix4(varLocation, false, ref matrix);
    }
}