using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace C_SHARP_LAB_2_2
{
    public partial class Form1 : Form
    {
        private Bin bin = new Bin();
        private View view = new View();
        private bool loaded = false;
        private int currentLayer;
        private int frameCount;
        DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);
        bool needReload = false;
        public static int minBarValue;
        public static int widthBarValue;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                glControl1.Width = (int)(glControl1.Height * Bin.X / Bin.Y);
                //glControl1.Location.X = (this.Size.Width - glControl1.Width) / 2;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = Bin.Z;
                needReload = true;
                minBarValue = trackBar2.Value;
                widthBarValue = trackBar3.Value;
            }
        }

        private void glControl1_Paint(object sender, /*PaintEventArgs*/ EventArgs e)
        {
            if (loaded)
            {
                if (needReload)
                {
                    view.GenerateTextureImage(currentLayer);
                    view.Load2DTexture();
                    needReload = false;
                }

                if (checkBox1.Checked)
                {
                    view.DrawTexture();
                }
                else if (checkBox2.Checked)
                {
                    view.DrawQuadStrip(currentLayer);
                }
                else
                {
                    view.DrawQuads(currentLayer);
                }

                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;

            if (currentLayer == trackBar1.Maximum)
                currentLayer--;

            needReload = true;

            label5.Text = Convert.ToString(trackBar1.Value);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                DisplayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void DisplayFPS()
        {
            if (DateTime.Now >= nextFPSUpdate)
            {
                this.Text = String.Format("CT Visulizer (fps={0})", frameCount);
                nextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            minBarValue = trackBar2.Value;
            label3.Text = Convert.ToString(trackBar2.Value);
            glControl1_Paint(sender, e);
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            widthBarValue = trackBar3.Value;
            label4.Text = Convert.ToString(trackBar3.Value);
            glControl1_Paint(sender, e);
            needReload = true;
        }

        /*private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (!loaded)
            {
                GL.Begin(BeginMode.Quads);

                GL.Color3(Color.Black);

                GL.Vertex2(glControl1.Location.X, glControl1.Location.Y);
                GL.Vertex2(glControl1.Location.X, glControl1.Location.Y + glControl1.Height);
                GL.Vertex2(glControl1.Location.X + glControl1.Width, glControl1.Location.Y + glControl1.Height);
                GL.Vertex2(glControl1.Location.X + glControl1.Width, glControl1.Location.Y);

                GL.End();

                //glControl1.Width = this.Size.Width;

                label6.Text = "OK";
            }
        }*/
    }
}
