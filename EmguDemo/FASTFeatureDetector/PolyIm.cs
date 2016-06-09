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
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace FASTFeatureDetector
{
    public partial class PolyIm : Form
    {
        public PolyIm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat polyImage = Draw(800f, 1000);
            imageBox1.Image = polyImage;
            imageBox1.Size = polyImage.Size;
        }

        private void CreateSubdivision(float maxValue, int pointCount, out Triangle2DF[] delaunayTriangles, out VoronoiFacet[] voronoiFacets)
        {

            #region 在0-maxValue 之间创建随机点
            PointF[] pts = new PointF[pointCount];
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = new PointF((float)r.NextDouble() * maxValue, (float)r.NextDouble() * maxValue);
            }
            #endregion
            using (Subdiv2D subDivision = new Subdiv2D(pts))
            {
                delaunayTriangles = subDivision.GetDelaunayTriangles();
                voronoiFacets = subDivision.GetVoronoiFacets();
            }
        }

        public Mat Draw(float maxValue, int pointCount)
        {
            Triangle2DF[] delaunayTriangles;
            VoronoiFacet[] voronoiFacets;

            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            CreateSubdivision(maxValue, pointCount, out delaunayTriangles, out voronoiFacets);

            Mat img = new Mat((int)maxValue, (int)maxValue, DepthType.Cv8U, 3);

            foreach (VoronoiFacet facet in voronoiFacets)
            {
                Point[] polyLine = Array.ConvertAll<PointF, Point>(facet.Vertices, Point.Round);

                using (VectorOfPoint vp = new VectorOfPoint(polyLine))
                using (VectorOfVectorOfPoint vvp = new VectorOfVectorOfPoint(vp))
                {
                    CvInvoke.FillPoly(img, vvp, new Bgr(r.NextDouble() * 120, r.NextDouble() * 120, r.NextDouble() * 120).MCvScalar);
                    CvInvoke.Polylines(img, vp, true, new Bgr(0, 0, 0).MCvScalar, 2);
                }
               // CvInvoke.Circle(img, Point.Round(facet.Point), 5, new Bgr(0, 0, 255).MCvScalar, -1);
            }

            foreach (Triangle2DF triangle in delaunayTriangles)
            {
                Point[] vertices = Array.ConvertAll<PointF, Point>(triangle.GetVertices(), Point.Round);
                using (VectorOfPoint vp = new VectorOfPoint(vertices))
                {
                   // CvInvoke.Polylines(img, vp, true, new Bgr(255, 255, 255).MCvScalar);
                }
            }
            return img;
        }
    }
}

