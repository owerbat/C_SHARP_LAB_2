using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace C_SHARP_LAB_2_2
{
    class View
    {
        Bitmap textureImage;
        int VBOtexture;

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public int clamp(int value, int min = 0, int max = 255)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        /*public Color TransferFunction(short value)
        {
            int min = 0, max = 2000;
            int newVal = clamp((value - min) * 255 / (max - min));
            return Color.FromArgb(255, newVal, newVal, newVal);
        }*/

        public Color TransferFunction(short value)
        {
            int min = Form1.minBarValue, max = Form1.widthBarValue;
            int newVal = clamp((value - min) * 255 / (max - min));
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void DrawQuads(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);

            for (int curX = 0; curX < Bin.X - 1; curX++)
                for (int curY = 0; curY < Bin.Y - 1; curY++)
                {
                    short value;

                    value = Bin.array[curX + curY * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX, curY);

                    value = Bin.array[curX + (curY + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX, curY + 1);

                    value = Bin.array[curX + 1 + (curY + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX + 1, curY + 1);

                    value = Bin.array[curX + 1 + curY * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX + 1, curY);
                }

            GL.End();
        }

        public void DrawQuadStrip(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int curX = 0; curX < Bin.X - 1; curX++)
            {
                GL.Begin(BeginMode.QuadStrip);

                for (int curY = 0; curY < Bin.Y - 1; curY++)
                {
                    short value;

                    value = Bin.array[curX + curY * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX, curY);

                    value = Bin.array[curX + 1 + curY * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(curX + 1, curY);
                }

                GL.End();
            }
        }

        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, 
                                                    textureImage.Height), ImageLockMode.ReadOnly,
                                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                          PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);

            ErrorCode er = GL.GetError();
            string str = er.ToString();
        }

        public void GenerateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);
            for(int i = 0; i < Bin.X; i++)
            {
                for(int j = 0; j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
                }
            }
        }

        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Light0);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

        public void DrawLightingTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 1.0f, 1.0f, 3.0f, 0.0f });
            //GL.Light(LightName.Light0, LightParameter.Ambient, OpenTK.Graphics.Color4.Red);
            GL.Light(LightName.Light0, LightParameter.Diffuse, OpenTK.Graphics.Color4.Red);
            //GL.Light(LightName.Light0, LightParameter.Specular, OpenTK.Graphics.Color4.Red);
            GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, 1.0f);
            /*GL.Light(LightName.Light0, LightParameter.SpotCutoff, 10.0f);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] {0.0f, 0.0f, 1.0f, 0.0f});
            GL.Light(LightName.Light0, LightParameter.SpotExponent, 0.0f);*/


            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}

/*GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);

            Vector4 lightPosition = new Vector4(1.0f, 1.0f, 4.0f, 0.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            Vector4 ambientColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambientColor);
            Vector4 diffuseColor = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuseColor);

            Vector4 materialSpecular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, materialSpecular);
            float materialShininess = 100;
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, materialShininess);*/
