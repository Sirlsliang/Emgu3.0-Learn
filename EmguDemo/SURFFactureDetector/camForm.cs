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
    public partial class camForm : Form
    {
        //将摄像头的图像帧设置为图片
        private Capture camCapture=null;
        
        //检查是否捕获 
        private bool captureInProgress;
        public camForm()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region if camCapture is not cteated ,create it now
            if (camCapture == null) {
                try
                {
                    camCapture = new Capture();
                }
                catch (NullReferenceException except)
                {
                    MessageBox.Show(except.Message);
                }
            }

            #endregion
            if (camCapture != null) {
                if (captureInProgress)
                {
                    button1.Text = "Start";
                    Application.Idle -= ProcessFrame;
                }
                else {
                    button1.Text = "Stop";

                    //当应用程序完成处理并即将进入空闲状态时发生
                    Application.Idle += ProcessFrame;
                }
                captureInProgress = !captureInProgress;
            }
        }



        private void ProcessFrame(object sende,EventArgs arg) {
            Image<Bgr, byte> imageFrame = camCapture.QueryFrame().ToImage<Bgr,byte>();
            imageBox1.Image = imageFrame;
        }
        private void ReleaseData() {
            if (camCapture != null) {
                camCapture.Dispose();
            }
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
