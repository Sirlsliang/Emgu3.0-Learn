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
using System.Data.SQLite;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;

namespace SURFFactureDetector
{

    public partial class faceRecognition : Form
    {
        private CascadeClassifier cascadeClassifier;

        private ReconizerEngine recognizerEngine;

        private bool bCamProgress = false;
        private Capture camCap;

        public faceRecognition()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitTheCam();
        }

        private void InitTheCam()
        {
            #region  处理开启或者关闭摄像头
            if (bCamProgress)
            {
                button1.Text = "开启摄像头";
                //Application.Idle -= ProgressFrame;
                ReleaseTheResource();
            }
            else {
                //开启摄像头
                InitTheResource();
                button1.Text = "关闭摄像头";
                //Application.Idle += ProgressFrame;

            }
            bCamProgress = !bCamProgress;
            #endregion
        }
        private void InitTheResource()
        {
            camCap = new Capture();
            string faceXml = ConfigurationManager.AppSettings["faceXml"];
            cascadeClassifier = new CascadeClassifier(faceXml);
            recognizerEngine = new ReconizerEngine();
        }

        private void ProgressFrame(object sender, EventArgs e)
        {
            imageBox1.Image = camCap.QueryFrame();
        }

        private void ReleaseTheResource()
        {
            if (null != camCap)
            {
                camCap.Dispose();
            }
            if (null != cascadeClassifier)
            {
                cascadeClassifier.Dispose();
            }
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bCamProgress)
            {

                using (Image<Bgr, byte> frameImage = camCap.QueryFrame().ToImage<Bgr, byte>())
                {
                    if (null != frameImage)
                    {
                        Image<Gray, byte> grayImage = frameImage.Convert<Gray, byte>();
                        var faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 12, Size.Empty);
                       
                        foreach (var face in faces)
                        {
                            frameImage.GrabCut(face, 1);
                            frameImage.Draw(face, new Bgr(Color.Red), 3);
                        }
                    }
                   
                    imageBox1.Image = frameImage;
                }
            }
        }

   


        private void button2_Click(object sender, EventArgs e)
        {
            if (bCamProgress)
            {
                if (textBox1.Text.Length > 0)
                {
                    using (Image<Gray, byte> faceToSave =new Image<Gray, byte>(camCap.QueryFrame().Bitmap))
                    {
                        Byte[] file;
                    
                        IDataStoreAccess dataStore = new DataStoreAccess();
                        var username = textBox1.Text.Trim();
                        var filePath = Application.StartupPath + String.Format("/config/image/{0}.bmp",username);
                        faceToSave.ToBitmap().Save(filePath);
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                            using (var reader = new BinaryReader(stream)) {
                                file = reader.ReadBytes((int)stream.Length);
                            }
                        }
                        var result = dataStore.SaveFace(username,file);
                        MessageBox.Show(result,"Save Result",MessageBoxButtons.OK);

                    }
                }
                else {
                    MessageBox.Show("请输入用户名");
                    imageBox1.Focus();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (recognizerEngine.TrainRecognizer()) {
                MessageBox.Show("训练成功");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bCamProgress)
            {

                using (Image<Gray, byte> frameImage = new Image<Gray, byte>(camCap.QueryFrame().Bitmap))
                {
                    if (null != frameImage)
                    {
                        string username = recognizerEngine.RecognizerUser(frameImage);
                        MessageBox.Show(username);

                        //label2.Text =String.Format("当前用户是:{0}",username);
                    }
                   
                }
            }
        }
    }
}
