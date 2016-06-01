using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;


namespace SURFFactureDetector
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> modelImage;
        Image<Bgr, byte> observedImage;
   
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadImage1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK) {
                modelImage = new Image<Bgr, byte>(fileDialog.FileName);
                imageBox1.Height = modelImage.Height;
                imageBox1.Width = modelImage.Width;
                imageBox1.Image = modelImage;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                observedImage = new Image<Bgr, byte>(fileDialog.FileName);
                imageBox2.Height = observedImage.Height;
                imageBox2.Width = observedImage.Width;
                imageBox2.Image = observedImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           

        }

        private void imageBox2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            long matchTime;
            if (modelImage != null && observedImage != null)
            {
                textBox1.Text = "vaule=" + trackBar1.Value;
                Mat m3 = new Mat();
                m3 = DrawMatched.Draw(modelImage.Mat, observedImage.Mat, out matchTime,(double)trackBar1.Value);
                imageBox3.Width = m3.Width;
                imageBox3.Height = m3.Height;
                imageBox3.Image = m3.ToImage<Bgr, byte>();
            }

        }
    }
}