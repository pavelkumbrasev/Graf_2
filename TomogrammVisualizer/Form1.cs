using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TomogrammVisualizer
{
    public partial class Form1 : Form
    {
        private Bin bin=new Bin();
        private View view= new View();
        private bool loaded=false;
        private int currentLayer;
        private int frameCount;
        DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);
        public static int minBarValue;
        public static int widthBarValue;
        bool nReload = false;
        public Form1()
        {
            InitializeComponent();
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                minBarValue = trackBar2.Value;
                widthBarValue = trackBar3.Value;
                


            }

        }

        private void glControl1_Paint(object sender, EventArgs e)
        {
            if (loaded)
            {
                if (nReload)
                {
                    view.GenerateTextureImage(currentLayer);
                    view.Load2DTexture();
                    nReload = false;
                }
                if (radioButton1.Checked) view.DrawQuadStrip(currentLayer);
                if (radioButton2.Checked) view.DrawTexture();

                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            if (currentLayer == trackBar1.Maximum)
                currentLayer--;
            glControl1_Paint(sender, e);
            nReload = true;
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

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            minBarValue = trackBar2.Value;
            glControl1_Paint(sender, e);
            nReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            widthBarValue = trackBar3.Value;
            glControl1_Paint(sender, e);
            nReload = true;
        }
    }
}
