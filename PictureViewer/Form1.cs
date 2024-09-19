using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picture_viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private float zoom = 1.0f;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image=Image.FromFile(openFileDialog1.FileName);
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            zoom = 1.0f;
            trackBar1.Value=(int)(zoom*100);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            zoom=(float)trackBar1.Value/100;
            pictureBox1.Height=(int)(pictureBox1.Image.Height*zoom);
            pictureBox1.Width=(int)(pictureBox1.Image.Width*zoom);
        }
    }
}
