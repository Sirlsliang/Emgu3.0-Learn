using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmguShapDetection
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CameraCaptureDemo fc = new CameraCaptureDemo();
            this.CheckAndOpened(fc);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageBoxTest img = new ImageBoxTest();
            CheckAndOpened(img);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShapeDetection shapDet = new ShapeDetection();
            CheckAndOpened(shapDet);

        }

        private void CheckAndOpened(Form form) {
            
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
