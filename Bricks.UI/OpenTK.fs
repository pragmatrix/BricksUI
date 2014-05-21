module BricksUIOpenTK

open Bricks
open BricksUI
open BricksHost
open OpenTK
open OpenTK.Graphics.OpenGL
open System
open System.Drawing
open System.Drawing.Imaging
open System.Windows.Forms
open System.Diagnostics
open System.Runtime.InteropServices

type TextureId = int

type Texture = { id: TextureId; width: int; height: int }

type Size = SizeF

type Surface = { background: Texture; size: Size; content: Surface bricks }




let private make8BitComponent f = Math.Max(255., f * 256.) |> int

let makeTextBoxSurface (text : Text) = 
    let style = text.style
    use font = new Font(style.fontName, float32(style.size))
    let size = bitmapSizeOfText font text.text
    let bitmapWidth = size.Width |> float |> Math.Ceiling |> int
    let bitmapHeight = size.Height |> float |> Math.Ceiling |> int
    use bitmap = new Bitmap(bitmapWidth, bitmapHeight)
    use graphics = Graphics.FromImage bitmap
    let color = style.color
    let color = Color.FromArgb(color.red |> make8BitComponent, color.green |> make8BitComponent, color.blue |> make8BitComponent)
    use brush = new SolidBrush(color)
    graphics.DrawString(text.text, font, brush, PointF())


    let data = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
    assert (data.Stride = bitmapWidth * 4)
    let byteSize = bitmapWidth * 4 * bitmapHeight;
    let bytes = Array.zeroCreate<byte>(data.Stride * bitmapHeight)
    Marshal.Copy(data.Scan0, bytes, 0, byteSize)
    bitmap.UnlockBits(data)

    let texture = GL.GenTexture()
    let target = TextureTarget.Texture2D
    GL.BindTexture(target, texture)
    GL.TexParameter(target, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear |> int)
    GL.TexParameter(target, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear |> int)
    GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, bitmapWidth, bitmapHeight, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bytes)
    // let surfaceSize = Size(bitmapWidth |> float32, bitmapHeight |> float32 )
    { background = { id = texture; width = bitmapWidth; height = bitmapHeight }; size = size ; content = Seq.empty }

let makeSurface box = 
    brick {
        let! b = box
        return 
            match b with
            | TextBox tb -> makeTextBoxSurface tb
    }

type _Window(model: Window, host: ProgramHost) as this = 
    inherit GameWindow(model.width, model.height)
    let mutable model = model
    do
        this.update(model) |> ignore

    interface IDisposable with
        member this.Dispose() = 
            base.Dispose()

    member this.update(u: Window) =
        model <- u
        this.Width <- u.width
        this.Height <- u.height
        this.Title <- u.title
        this

    override this.OnResize _ = 
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadIdentity();
        let r = this.ClientRectangle
        GL.Ortho(r.Left |> float, r.Right |> float, r.Bottom |> float, r.Top |> float, -1.0, 1.0)
        GL.Viewport(r.Size)

        GL.Disable(EnableCap.DepthTest)

        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
        GL.AlphaFunc(AlphaFunction.Greater, 0.f)


    override this.OnRenderFrame _ =
        GL.ClearColor(0.f, 0.f, 0.f, 0.f);
        GL.Clear(ClearBufferMask.ColorBufferBit (* ||| ClearBufferMask.DepthBufferBit *))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()


        GL.Begin PrimitiveType.Polygon
        GL.Color4(1.f, 1.f, 1.f, 0.5f)
        GL.Vertex3(100., 10., 0.)
        GL.Vertex3(10., 10., 0.)
        GL.Vertex3(100., 100., 0.)
        GL.End()


        this.SwapBuffers()

    override this.OnClosing args =
        args.Cancel <- true
        base.OnClosing args
        host.dispatch model.eventHandler CloseWindow
