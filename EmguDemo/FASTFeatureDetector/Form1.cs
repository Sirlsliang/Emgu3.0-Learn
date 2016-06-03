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
namespace FASTFeatureDetector
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> image1;
        Image<Bgr, byte> image2;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK) {
                image1 = new Image<Bgr,byte>(file.FileName);
            }
            imageBox1.Size = image1.Size;
            imageBox1.Image = image1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
               image2 = new Image<Bgr, byte>(file.FileName);
            }
            imageBox2.Size = image2.Size;
            imageBox2.Image = image2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
          Image<Gray, byte> gImage1 = image1.Convert<Gray, byte>();
          Image<Gray, byte> gImage2 = image2.Convert<Gray, byte>();
          imageBox1.Image = FastFeatureDetector.Draw(gImage1,gImage2);
          
                
        }
    }
}
