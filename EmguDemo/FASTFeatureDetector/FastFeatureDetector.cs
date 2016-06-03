using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace FASTFeatureDetector
{
    class FastFeatureDetector
    {
        public static Image<Bgr, byte> Draw(Image<Gray, byte> modelImage, Image<Gray, byte> observedImage) {
            Mat homography = null;
            FastDetector fastCpu = new FastDetector(10, true);
            VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
            VectorOfKeyPoint observedPoints = new VectorOfKeyPoint();
           
            BriefDescriptorExtractor descriptors = new BriefDescriptorExtractor();

            UMat modelDescriptors = new UMat();
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
            Mat mask;
            int k = 2;
            double uniquenessThreshold = 0.8;
            try
            {
                //extract features from object image(不能直接使用fastCpu 提取关键点特征。)
                fastCpu.DetectRaw(modelImage,modelKeyPoints,null);
                descriptors.DetectAndCompute(modelImage, null, modelKeyPoints, modelDescriptors, false);
                
                //fastCpu.DetectAndCompute(modelImage, null, modelKeyPoints, descriptors, false);
            }
            catch (Exception e)
            {
                Console.Write("debug" + e.Message);
            }
            finally {
                Console.Write("");
            }
           
            

            BFMatcher matcher = new BFMatcher(DistanceType.L2);
            matcher.Add(modelDescriptors);
           
            using (Matrix<float> dist = new Matrix<float>(observedImage.Rows, k)) {
                matcher.KnnMatch(modelDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);
            }
            int nonZeroCount = CvInvoke.CountNonZero(mask);
            if (nonZeroCount >= 4) {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints,observedPoints,matches,mask,1.5,20);
                if (nonZeroCount >= 4) {
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,observedPoints,matches,mask,2);
                }
            }
            Mat result = new Mat();
            //Draw the matched keypoints
            Features2DToolbox.DrawKeypoints(modelImage,modelKeyPoints, result, new Bgr(255,255,255),Features2DToolbox.KeypointDrawType.Default);
            
            #region draw the projected region on the image
            if (homography != null) {
                Rectangle rect = modelImage.ROI;
                //与point 的区别是不是一个就是f
                PointF[] pts = new PointF[] {
                    new PointF(rect.Left,rect.Bottom),
                    new PointF(rect.Right,rect.Bottom),
                    new PointF(rect.Right,rect.Top),
                    new PointF(rect.Left,rect.Top)
                };
                pts = CvInvoke.PerspectiveTransform(pts, homography);
                //将一种类型的数组转换成另一种类型
                Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);

                using (VectorOfPoint vp = new VectorOfPoint(points))
                {
                    //画出一个或多个多边形曲线
                    CvInvoke.Polylines(modelImage, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                }

            }
            #endregion

            return result.ToImage<Bgr,byte>();
        }
    }
}
