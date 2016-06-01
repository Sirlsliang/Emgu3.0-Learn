using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;


using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;

namespace SURFFactureDetector
{
    public partial class FaceDetection : Form
    {
        private Capture camCap;


        private CascadeClassifier cascadeClassifier;
        //判断摄像头是否在工作
        private bool bCamProgress =false;

      
       
        public FaceDetection()
        {
            InitializeComponent();
            
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bCamProgress) {
                using (Image<Bgr, byte> imageFrame = camCap.QueryFrame().ToImage<Bgr, byte>())
                {
                    if (null != imageFrame)
                    {
                        var grayFrame = imageFrame.Convert<Gray, byte>();


                        var faces = cascadeClassifier.DetectMultiScale(grayFrame,1.1,3,Size.Empty);


                        foreach (var face in faces) {
                            imageFrame.Draw(face,new Bgr(Color.BurlyWood),3);
                        }
                    }
                    imageBox1.Image = imageFrame;
                }

            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 没有检测到摄像头就开启摄像头
            //C# 中一个初始化了的对象调用了dispose 方法后，该对象仍然是非null的，即用null==该对象得到的是false
            if (!bCamProgress)
            {
                try
                {
                    camCap = new Capture();
                    imageBox1.Width = camCap.Width;
                    imageBox1.Height = camCap.Height;
                }
                catch (NullReferenceException except)
                {
                    MessageBox.Show(except.Message);
                }
            }
            #endregion
            if (bCamProgress)
            {
                button1.Text = "开启摄像头";
               // Application.Idle -= CamProgress;
                Realease();
            }
            else {
                button1.Text = "关闭摄像头";
               // Application.Idle += CamProgress;
                //初始化人脸检测算法器
                InitDetector();
            }
            bCamProgress = !bCamProgress;
            
        }

        private void InitDetector() {
            string path = ConfigurationManager.AppSettings["faceXml"];
            cascadeClassifier = new CascadeClassifier(path);
            if (null == cascadeClassifier)
            {
                MessageBox.Show("算法器没有正确初始化");

            }
            else {
                Console.WriteLine("成功初始化人脸检测算法器");
            }
        }


        private void CamProgress(object sender, EventArgs e) {
            
            imageBox1.Image = camCap.QueryFrame();

        }
        private void Realease() {

            if (camCap != null) {
                camCap.Dispose();
            }
            if (cascadeClassifier != null) {
                cascadeClassifier.Dispose();
            }
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
