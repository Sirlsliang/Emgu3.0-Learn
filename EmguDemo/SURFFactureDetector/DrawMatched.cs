using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System.Diagnostics;
#if !__IOS__
using Emgu.CV.Cuda;
#endif
//CUDA（Computer Unifed Device Architecture）显卡NVIDA推出的运算平台
namespace SURFFactureDetector
{

    public static class DrawMatched
    {

        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints,
            out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography,double hessianThresh) {

            int k = 2;
            double uniquenessThreshold = 0.8;
            //double hessianThresh = 300;设置阈值，这个值越大，最终的特征点越少

            Stopwatch sw ;
            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

#if !__IOS__
            //判断是否存在NVIDIA显卡，如果存在就是使用GPU进行计算
            if (CudaInvoke.HasCuda) 
            {
                //SURF算法
                //创建一个CudaSurf 侦测器
                CudaSURF surfCuda = new CudaSURF((float)hessianThresh);
                //在Gpu中 使用GpuMat 来替代cv::Mat
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
               
                //从图像中提取特征点
                using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage, null))
                //创建特征点描述器
                using (GpuMat gupModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                //创建匹配器
                using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                {

                    surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    sw = Stopwatch.StartNew();

                    using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))

                    //using (GpuMat tmp = new GpuMat())
                    //using (Stream stream = new Stream())
                    {
                        matcher.KnnMatch(gpuObservedDescriptors, gpuObservedDescriptors, matches, k);

                        surfCuda.DownloadKeypoints(gpuModelKeyPoints, observedKeyPoints);
                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));

                        //过滤匹配特征，，如果匹配点是比较罕见，那么就剔除
                        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);
                        //返回数组中的非零元素
                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= 4)
                        {
                            //剔除
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                        }
                    }
                    sw.Stop();
                }
            }
            else
#endif
            {
                
                using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
                using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read)) {
                    
                    //创建surf算法器
                    SURF surfCPU = new SURF(hessianThresh);
                    
                    //从源的图像提取描述符
                    UMat modelDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uModelImage,null,modelKeyPoints,modelDescriptors,false);
                    sw = Stopwatch.StartNew();

                    //从观察图像中提取描述器
                    UMat observedDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uObservedImage,null,observedKeyPoints,observedDescriptors,false);
                   
                    //Brute Force匹配
                    BFMatcher matcher = new BFMatcher(DistanceType.L2);
                    matcher.Add(modelDescriptors);
                    //matches:VectorOfVectorOfDMatch
                    //observedDescriptors:VectorOfKeyPoint
                    matcher.KnnMatch(observedDescriptors,matches,k,null);

                    mask = new Mat(matches.Size,1,DepthType.Cv8U,1);
                    mask.SetTo(new MCvScalar(255));
                    //过滤匹配特征，，如果匹配点是比较罕见，那么就剔除
                    Features2DToolbox.VoteForUniqueness(matches,uniquenessThreshold,mask);
                    //返回数组中的非零元素
                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4) {
                        //剔除那些旋转和缩放不与大多数匹配和旋转统一的特征点
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints,observedKeyPoints,matches,mask,1.5,20);
                        if (nonZeroCount >= 4)
                            //使用RANDSAC算法获取单应性矩阵，如果矩阵不能恢复，返回null
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,observedKeyPoints,matches,mask,2);
                    }
                    sw.Stop();
                }
            }
            matchTime = sw.ElapsedMilliseconds;
        }


        public static Mat Draw(Mat modelImage, Mat observedImage, out long matchTime,double hessianThresh)
        {
            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, matches, out mask, out homography, hessianThresh);

                //Draw the matched points
                Mat result = new Mat();

                //在两个图像之间画匹配的关键点
                Features2DToolbox.DrawMatches(modelImage,modelKeyPoints,observedImage,observedKeyPoints,
                        matches,result,new MCvScalar(255,255,255),new MCvScalar(255,255,255),mask);
                
                //画出投射区域
                #region draw the projected region on the image
                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
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
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                    }

                }
                #endregion
                return result;

            }

        }
    } 
}

