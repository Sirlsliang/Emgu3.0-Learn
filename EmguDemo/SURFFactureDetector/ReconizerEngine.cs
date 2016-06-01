using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace SURFFactureDetector
{
    class ReconizerEngine
    {
        private FaceRecognizer faceRecognizer;
        private DataStoreAccess dataStoreAccess;
        private String recognizeFilePath;

        public ReconizerEngine() {
            this.recognizeFilePath = Environment.CurrentDirectory+ConfigurationManager.AppSettings["recognizerPath"]; 
            this.dataStoreAccess = new DataStoreAccess();
            this.faceRecognizer = new EigenFaceRecognizer(80,double.PositiveInfinity);
        }

        public bool TrainRecognizer() {         
            var allFaces = dataStoreAccess.CallFaces("ALL_USERS");
            if (allFaces != null) {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++) {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }
                faceRecognizer.Train(faceImages, faceLabels);
                faceRecognizer.Save(recognizeFilePath);
                return true;
            }
            return false;
        }

        public void LoadRecognizerData() {
            faceRecognizer.Load(recognizeFilePath);
        }

        public string RecognizerUser(Image<Gray, byte> userImage) {
            faceRecognizer.Load(recognizeFilePath);
            var result = faceRecognizer.Predict(userImage.Resize(100,100,Inter.Cubic));
            Console.WriteLine(result.Label);
            string name = dataStoreAccess.GetUserName(result.Label);
            return name;
           
        }
    }
}
