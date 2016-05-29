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
using Emgu.CV.CvEnum;
using System.Threading;
using Emgu.CV.UI;
using System.Diagnostics;

namespace EmguShapDetection
{
    public partial class ShapeDetection : Form
    {
        private Image<Bgr, byte> srcImage;//源图片 第一步加载
        private UMat uImage;//灰度图 在第二步中加载
        private Stopwatch sw = new Stopwatch();
        private UMat cannyEdges; //canny边，在button5中初始化

        private double cannyThreshold = 180.0;
        public ShapeDetection()
        {
            InitializeComponent();
        }

        private void ShowImage(IImage image)
        {
            imageBox2.Image = image;
        }
        private void SWReset() {
            sw.Reset();
            sw.Start();
        }
        private void SWStop(string name) {
            sw.Stop();
            textBox1.Text = String.Format("{0} 耗时 {1} ms",name, sw.ElapsedMilliseconds);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //加载图片并显示在imageBox1中
            //将imageBox 设置为图片大小
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK) {
               srcImage = new Image<Bgr, byte>(fileDialog.FileName).Resize(400,400,Emgu.CV.CvEnum.Inter.Linear,true);
            }
            //srcImage = new Image<Bgr, byte>("d://test/1.png").Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            
            if (null == srcImage) {
                MessageBox.Show("图片加载不成功，请重新加载！！");
                
            }
            imageBox1.Image = srcImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SWReset();
            //将图片转换成灰度图
             uImage= new UMat();
            //转换颜色模式
            CvInvoke.CvtColor(srcImage, uImage, ColorConversion.Bgr2Gray);
            ShowImage(uImage);
            SWStop("转换为灰度");
            //使用图像的Pyr来移除噪音
           
            //imageBox1.Image = uImage.ToImage<Gray, byte>();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SWReset();
            UMat pydownImage = new UMat();
            //uImage.ToImage<Gray, byte>().PyrDown();
            CvInvoke.PyrDown(uImage, pydownImage);
            CvInvoke.PyrUp(pydownImage, uImage);
            
            ShowImage(uImage);
            SWStop("去噪");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SWReset();
            double circleAccumulatorThreshold = 120;
            //利用霍夫变换来寻找圆
            CircleF[] circles = CvInvoke.HoughCircles(uImage,HoughType.Gradient,2.0,20.0, cannyThreshold, circleAccumulatorThreshold,5);
            SWStop("检测圆形 ");

            //draw circles
            Image<Bgr, byte> circleImage = srcImage.CopyBlank();
            foreach (CircleF circle in circles) {
                circleImage.Draw(circle,new Bgr(Color.Brown),2);
            }
            imageBox2.Image = circleImage;
        }

        private void button5_Click(object sender, EventArgs e)
        {

            SWReset();
            double cannyThresholdLinking  = 120;
            cannyEdges = new UMat();
            //使用Canny算法寻找边
            CvInvoke.Canny(uImage,cannyEdges,cannyThreshold,cannyThresholdLinking);
            
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, // 像素相关单位上的距离解析度
                Math.PI/45.0,//用弧度表示的角度解析度
                20, //散列值
                30,//最小的线宽
                10); //线的间隙 
            SWStop("检测线");
            Image<Bgr, byte> lineImage = srcImage.CopyBlank();
            foreach (LineSegment2D line in lines)
                lineImage.Draw(line, new Bgr(Color.Green),2);
            imageBox2.Image = cannyEdges;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SWReset();
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            //一个box 就是一个旋转的长方形
            List<RotatedRect> boxList = new List<RotatedRect>();
            //contours 轮廓线
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
                CvInvoke.FindContours(cannyEdges,contours,null,RetrType.List,ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++) {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint()) {
                        CvInvoke.ApproxPolyDP(contour,approxContour,CvInvoke.ArcLength(contour,true)*0.05,true);
                        if (CvInvoke.ContourArea(approxContour, false) > 10) {//只用面积大于250的？
                            if (approxContour.Size == 3) { //轮廓有三个顶点，那就是一个三角形
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                    pts[0],
                                    pts[1],
                                    pts[2]
                                    ));
                            } else if(approxContour.Size == 4){ //轮廓有四个顶点
                                //检测轮廓内所有的角度是否为在[80,100]度之间
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray(); //将向量转化为数组
                                LineSegment2D[] edges = PointCollection.PolyLine(pts,true);//将点转化为线

                                for (int j = 0; j < edges.Length; j++) {
                                    double angle = Math.Abs(edges[(j+1)%edges.Length].GetExteriorAngleDegree(edges[j]));//获取线之间的角度
                                    if (angle < 80 || angle > 100) {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
            SWStop("检测长方形和三角形");
           

            //画三角形和长方形
            Image<Bgr, byte> triangleRectangleImage = srcImage.CopyBlank();
            foreach (Triangle2DF triangle in triangleList)
                triangleRectangleImage.Draw(triangle,new Bgr(Color.DarkBlue),2);
            foreach (RotatedRect box in boxList)
                triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
            imageBox2.Image = triangleRectangleImage;
          
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                srcImage = new Image<Bgr, byte>(fileDialog.FileName).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            }
            //srcImage = new Image<Bgr, byte>("d://test/1.png").Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

            if (null == srcImage)
            {
                MessageBox.Show("图片加载不成功，请重新加载！！");

            }
            imageBox1.Image = srcImage;
            uImage = new UMat();
            //转换颜色模式
            CvInvoke.CvtColor(srcImage, uImage, ColorConversion.Bgr2Gray);
            ShowImage(uImage);
            UMat pydownImage = new UMat();

            CvInvoke.PyrDown(uImage, pydownImage);
            CvInvoke.PyrUp(pydownImage, uImage);
            ShowImage(uImage);
            double circleAccumulatorThreshold = 120;
            //利用霍夫变换来寻找圆
            CircleF[] circles = CvInvoke.HoughCircles(uImage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);
           
            double cannyThresholdLinking = 120;
            cannyEdges = new UMat();
            //使用Canny算法寻找边
            CvInvoke.Canny(uImage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, // 像素相关单位上的距离解析度
                Math.PI / 45.0,//用弧度表示的角度解析度
                20, //散列值
                30,//最小的线宽
                10); //线的间隙 
            SWReset();
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            //一个box 就是一个旋转的长方形
            List<RotatedRect> boxList = new List<RotatedRect>();
            //contours 轮廓线
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 10)
                        {//只用面积大于250的？
                            if (approxContour.Size == 3)
                            { //轮廓有三个顶点，那就是一个三角形
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                    pts[0],
                                    pts[1],
                                    pts[2]
                                    ));
                            }
                            else if (approxContour.Size == 4)
                            { //轮廓有四个顶点
                                //检测轮廓内所有的角度是否为在[80,100]度之间
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray(); //将向量转化为数组
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);//将点转化为线

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));//获取线之间的角度
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
            SWStop("检测长方形和三角形");


            //画三角形和长方形
            Image<Bgr, byte> triangleRectangleImage = srcImage.CopyBlank();
            foreach (Triangle2DF triangle in triangleList)
                triangleRectangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
            foreach (RotatedRect box in boxList)
                triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
            imageBox2.Image = triangleRectangleImage;
        }
    }
}
