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
using Emgu.CV.Structure;
using Emgu.CV.UI;
namespace EmguShapDetection
{
    public partial class CameraCaptureDemo : Form
    {
        public CameraCaptureDemo()
        {
            InitializeComponent();
            ImageViewer viewer = new ImageViewer();
            Capture capture = new Capture();
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {
                viewer.Image = capture.QueryFrame();
            });
            viewer.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
