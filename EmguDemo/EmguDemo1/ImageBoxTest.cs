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
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EmguDemo1
{
    public partial class ImageBoxTest : Form
    {
        public ImageBoxTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            imageBox1.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Everything;
            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == DialogResult.OK) {
                Image<Bgr, byte> myImage = new Image<Bgr, byte>(openfile.FileName);
                imageBox1.Width = imageBox2.Width =  myImage.Width;
                imageBox1.Height = imageBox2.Height= myImage.Height;
                imageBox1.Image = myImage;
                Image<Rgb, byte> grayImage = myImage.Convert<Rgb, byte>();
                imageBox2.Image =grayImage;
                for (int row = 0; row < grayImage.Height; row++) {
                    for (int col = 0; col < grayImage.Width; col++) {
                        for (int dep = 0; dep < 3; dep++) {
                            Console.Write(grayImage.Data[row,col,dep]+"   ");
                        }
                    }
                    Console.WriteLine("\n");
                }
            }
            
            
        }

      
    }
}
