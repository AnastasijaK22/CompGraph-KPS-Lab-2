using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace KUZENKOVA_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        bool needReload = false;
        Bin bin = new Bin();
        View view = new View();
        int currentLayer = 0;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.Z - 1;
                trackBar2.Maximum = 1999;
                trackBar3.Minimum = 1;
                trackBar3.Maximum = 2000;
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(loaded)
            {
                if (radioButton1.Checked == true)
                {
                    view.DrawQuads(currentLayer);
                }
                if (radioButton2.Checked == true)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                if (radioButton3.Checked == true)
                {
                    view.DrawQuadsStrip(currentLayer);
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            trackBar3.Maximum = 2000 - trackBar2.Value;
            view.SetupMin(trackBar2.Value);
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.SetupWidth(trackBar3.Value);
            needReload = true;
        }
    }
}
