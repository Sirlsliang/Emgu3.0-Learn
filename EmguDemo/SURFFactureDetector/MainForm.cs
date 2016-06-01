using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURFFactureDetector
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            this.CheckAndOpened(f);
        }   

        private void button2_Click(object sender, EventArgs e)
        {
            camForm cf = new camForm();
            this.CheckAndOpened(cf);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FaceDetection fD = new FaceDetection();
            this.CheckAndOpened(fD);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            faceRecognition faRe = new faceRecognition();
            this.CheckAndOpened(faRe);
        }

        private void CheckAndOpened(Form form)
        {

            bool isExitFormConfig = false; //判断配置窗口是否已经打开，放置打开多个配置窗口
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name == form.Name)
                {
                    openForm.Visible = true;
                    openForm.Activate();
                    isExitFormConfig = true;
                    break;
                }
            }
            if (!isExitFormConfig)
            {
                form.Show();
                form.TopMost = true;
            }
        }
    }
}
