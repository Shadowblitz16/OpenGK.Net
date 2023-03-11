using OpenTK.Mathematics;
using OpenTK.Graphics.ES30;
using StbImageSharp;
namespace OpenGK;


public class Texture
{
    #region Fields

    private int id;
    private bool bound;

    private byte[] data;
    private int width;
    private int height;
    private int channels;
    
    #endregion

    #region Bind

    public void Begin()
    {
        if (id == 0 || bound)
        bound = true;
        GL.BindTexture(TextureTarget.Texture2D, id);
    }

    public void End()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        bound = false;
    }

    #endregion
    #region Utility
    
    private void Init()
    {
        if (width == 0 || height == 0) return;

        var buffer = new byte[width*height];
        Array.Copy(data, buffer, Math.Min(data.Length, buffer.Length));

        // Generate texture;
        id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);

        // Set texture parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS    , (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT    , (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap  , 0);


        // Load Image
        switch (channels) 
        {
            case 4:
                GL.TexImage2D
                (
                    TextureTarget2d.Texture2D, 
                    0, 
                    TextureComponentCount.Bgra8Ext,
                    this.width, 
                    this.height, 
                    0, 
                    PixelFormat.Rgba, 
                    PixelType.Byte, 
                    this.data
                );
                break;
            case 3:
                GL.TexImage2D
                (
                    TextureTarget2d.Texture2D, 
                    0, 
                    TextureComponentCount.Bgra8Ext, 
                    this.width, 
                    this.height, 
                    0, 
                    PixelFormat.Rgb, 
                    PixelType.Byte,
                    this.data
                );
                break;
            default:
                throw new InvalidOperationException("ERROR: (Texture) Unknown number of channels '" +channels + "'" );
                break;
        }
    }

    #endregion
    #region Constructors
    
    public Texture()
    {
        this.data     = Array.Empty<byte>();
        this.width    = 0;
        this.height   = 0;
        this.channels = 0;
    }
    public Texture(string file) : this()
    {
        if (!File.Exists(file)) return;
        using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            var result = StbImageSharp.ImageResult.FromStream(stream);
            if (result is ImageResult iResult)
            {
                data     = iResult.Data;
                width    = iResult.Width;
                height   = iResult.Height;
                channels = iResult.Comp switch
                {
                    ColorComponents.RedGreenBlueAlpha => 4,
                    ColorComponents.RedGreenBlue      => 3,
                    ColorComponents.GreyAlpha         => 2,
                    ColorComponents.Grey              => 1,
                    _                                 => 0
                };
            }
        }
        Init();
    }
    #endregion
}