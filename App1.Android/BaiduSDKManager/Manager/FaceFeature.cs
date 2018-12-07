using Android.Content;
using App1.Droid.BaiduSDKManager.Entity;
using Com.Baidu.Idl.Facesdk;
using System.Collections;
using System.Linq;

namespace App1.Droid.BaiduSDKManager.Manager
{
    public class FaceFeature
    {

        private FaceRecognize faceRecognize;
        private FeatureCallbak callbak;

        public void init(Context context)
        {
            if (faceRecognize == null)
            {
                faceRecognize = new FaceRecognize(context);
                // RECOGNIZE_LIVE普通生活照、视频帧识别模型（包含特征抽取）
                // RECOGNIZE_ID_PHOTO 身份证芯片模型（包含特征抽取）
                // RECOGNIZE_NIR 近红外图片识别模型（包含特征抽取）
                // 两张图片的识别需要使用相同的模型
                faceRecognize.InitModel(FaceSDK.RecognizeType.RecognizeLive);
                faceRecognize.InitModel(FaceSDK.RecognizeType.RecognizeIdPhoto);
                // faceRecognize.initModel(FaceSDK.RecognizeType.RECOGNIZE_NIR);
            }
        }

        public void init(Context context, FaceSDK.RecognizeType type)
        {
            if (faceRecognize == null)
            {
                faceRecognize = new FaceRecognize(context);
                faceRecognize.InitModel(type);
            }
        }

        public void initWithIdCardRecognizeModel(Context context, FaceSDK.RecognizeType type)
        {
            if (faceRecognize == null)
            {
                faceRecognize = new FaceRecognize(context);
                faceRecognize.InitModel(FaceSDK.RecognizeType.RecognizeIdPhoto);
            }
        }

        /**
         * 当初始化为模型为身份证芯片照模型RECOGNIZE_ID_PHOTO， 指定最小检测人脸大小，里面逻辑已带人脸检测，不需要在前面进行人脸检测，使用其关键点
         *
         * @param argb
         * @param height
         * @param width
         * @param minfaceSize
         * @param feature
         * @return
         */
        public int extractIdCardFeature(int[] argb, int height, int width, int minfaceSize, byte[] feature)
        {

            if (faceRecognize == null)
            {
                return -1;
            }
            int ret = faceRecognize.ExtractFeature(argb, height, width, FaceSDK.ImgType.Argb, minfaceSize, feature,
                    FaceSDK.RecognizeType.RecognizeIdPhoto);
            return ret;
        }

        /**
         * @param argb
         * @param height
         * @param width
         * @param feature
         * @param landmarks
         * @return
         */
        public int extractFeatureWithDetect(int[] argb, int height, int width, byte[] feature, int[] landmarks)
        {

            if (faceRecognize == null)
            {
                return -1;
            }
            //int ret = faceRecognize.extractFeature(argb, height, width, FaceSDK.ImgType.ARGB, feature, landmarks,
            //        FaceSDK.RecognizeType.RECOGNIZE_LIVE);
            int ret = faceRecognize.ExtractFeature(argb, height, width, FaceSDK.ImgType.Argb, feature, landmarks, FaceSDK
                    .RecognizeType.RecognizeIdPhoto);
            return ret;
            //
        }

        public int extractFeature(int[] argb, int height, int width, byte[] feature, int[] landmarks)
        {

            if (faceRecognize == null)
            {
                return -1;
            }
            int ret = faceRecognize.ExtractFeature(argb, height, width, FaceSDK.ImgType.Argb, feature, landmarks,
                    FaceSDK.RecognizeType.RecognizeLive);
            return ret;
        }

        public int extractFeatureForIDPhoto(int[] argb, int height, int width, byte[] feature, int[] landmarks)
        {

            if (faceRecognize == null)
            {
                return -1;
            }
            int ret = faceRecognize.ExtractFeature(argb, height, width, FaceSDK.ImgType.Argb, feature, landmarks,
                    FaceSDK.RecognizeType.RecognizeIdPhoto);
            return ret;
        }

        public int faceFeature(ARGBImg argbImg, byte[] feature, int minFaceSize)
        {
            return faceRecognize.ExtractFeature(argbImg.data, argbImg.height, argbImg.width, FaceSDK.ImgType.Argb,
                    minFaceSize, feature, FaceSDK.RecognizeType.RecognizeLive);
        }


        public int faceFeatureForIDPhoto(ARGBImg argbImg, byte[] feature, int minFaceSize)
        {
            return faceRecognize.ExtractFeature(argbImg.data, argbImg.height, argbImg.width, FaceSDK.ImgType.Argb,
                    minFaceSize, feature, FaceSDK.RecognizeType.RecognizeIdPhoto);
        }


        public int faceFeature(ARGBImg argbImg, byte[] feature)
        {

            FaceSDKManager.getInstance().getFaceDetector().clearTrackedFaces();
            int ret = FaceSDKManager.getInstance().getFaceDetector().detect(argbImg.data, argbImg.width, argbImg.height);
            // Log.i("wtf", "feature detect from image->" + ret + " " + argbImg.width + " " + argbImg.height);

            FaceInfo[] faceInfos = FaceSDKManager.getInstance().getFaceDetector().TrackedFaces;
            // Log.i("wtf", "feature detect from image faceInfos ->" + faceInfos);
            if (faceInfos != null && faceInfos.Length > 0)
            {
                FaceInfo faceInfo = faceInfos[0];
                // Log.i("wtf", "feature detect from image faceInfos ->" + faceInfo);
                // 可以ret FaceDetector.DETECT_CODE_OK和 FaceDetector.DETECT_CODE_HIT_LAST才进行特征抽取
                // if (faceInfo != null && (ret == FaceDetector.DETECT_CODE_OK || ret == FaceDetector
                // .DETECT_CODE_HIT_LAST)) {
                if (faceInfo != null && (ret != FaceDetector.NO_FACE_DETECTED && ret != FaceDetector.UNKNOW_TYPE))
                {
                    return faceRecognize.ExtractFeature(argbImg.data, argbImg.height, argbImg.width, FaceSDK.ImgType.Argb,
                            feature, PerformQuery<int>(faceInfo.Landmarks), FaceSDK.RecognizeType.RecognizeLive);
                }
            }
            FaceSDKManager.getInstance().getFaceDetector().clearTrackedFaces();
            return ret;
        }

        /// <summary>
        /// IList转Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static T[] PerformQuery<T>(IEnumerable enumerable)
        {
            T[] array = enumerable.Cast<T>().ToArray();
            return array;
        }

        public float getFaceFeatureDistance(byte[] firstFeature, byte[] secondFeature)
        {
            return faceRecognize.GetFaceSimilarity(firstFeature, secondFeature, FaceSDK.RecognizeType.RecognizeLive);
        }

        public float getFaceFeatureDistanceForIDPhoto(byte[] firstFeature, byte[] secondFeature)
        {
            return faceRecognize.GetFaceSimilarity(firstFeature, secondFeature, FaceSDK.RecognizeType.RecognizeIdPhoto);
        }

        public float getFaceFeatureDistance(byte[] firstFeature, byte[] secondFeature, FaceSDK.RecognizeType type)
        {
            return faceRecognize.GetFaceSimilarity(firstFeature, secondFeature, type);
        }


        public interface FeatureCallbak
        {

            void callback(byte[] feature);

        }
    }
}
