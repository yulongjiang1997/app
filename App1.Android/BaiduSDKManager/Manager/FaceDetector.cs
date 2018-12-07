using Android.Content;
using Com.Baidu.Idl.Facesdk;

namespace App1.Droid.BaiduSDKManager.Manager
{
    public class FaceDetector
    {

        /// <summary>
        /// 检测结果代码 成功
        /// </summary>
        public static readonly int OK = FaceTracker.ErrCode.Ok.Ordinal();
        public static readonly int PITCH_OUT_OF_DOWN_MAX_RANGE = FaceTracker.ErrCode.PitchOutOfDownMaxRange.Ordinal();
        public static readonly int PITCH_OUT_OF_UP_MAX_RANGE = FaceTracker.ErrCode.PitchOutOfUpMaxRange.Ordinal();
        public static readonly int YAW_OUT_OF_LEFT_MAX_RANGE = FaceTracker.ErrCode.YawOutOfLeftMaxRange.Ordinal();
        public static readonly int YAW_OUT_OF_RIGHT_MAX_RANGE = FaceTracker.ErrCode.YawOutOfRightMaxRange.Ordinal();
        public static readonly int POOR_ILLUMINATION = FaceTracker.ErrCode.PoorIllumination.Ordinal();
        public static readonly int HIT_LAST = FaceTracker.ErrCode.DataHitLast.Ordinal();
        /// <summary>
        /// 检测结果代码 没有检测到人脸， 人脸太小（必须打于最小检测人脸minFaceSize），或者人脸角度太大，人脸不是朝上
        /// </summary>
        public static readonly int NO_FACE_DETECTED = FaceTracker.ErrCode.NoFaceDetected.Ordinal();
        public static readonly int DATA_NOT_READY = FaceTracker.ErrCode.DataNotReady.Ordinal();
        public static readonly int DATA_HIT_ONE = FaceTracker.ErrCode.DataHitOne.Ordinal();
        public static readonly int DATA_HIT_LAST = FaceTracker.ErrCode.DataHitLast.Ordinal();
        public static readonly int IMG_BLURED = FaceTracker.ErrCode.ImgBlured.Ordinal();
        public static readonly int OCCLUSION_LEFT_EYE = FaceTracker.ErrCode.OcclusionLeftEye.Ordinal();
        public static readonly int OCCLUSION_RIGHT_EYE = FaceTracker.ErrCode.OcclusionRightEye.Ordinal();
        public static readonly int OCCLUSION_NOSE = FaceTracker.ErrCode.OcclusionNose.Ordinal();
        public static readonly int OCCLUSION_MOUTH = FaceTracker.ErrCode.OcclusionMouth.Ordinal();
        public static readonly int OCCLUSION_LEFT_CONTOUR = FaceTracker.ErrCode.OcclusionLeftContour.Ordinal();
        public static readonly int OCCLUSION_RIGHT_CONTOUR = FaceTracker.ErrCode.OcclusionRightContour.Ordinal();
        public static readonly int OCCLUSION_CHIN_CONTOUR = FaceTracker.ErrCode.OcclusionChinContour.Ordinal();
        public static readonly int FACE_NOT_COMPLETE = FaceTracker.ErrCode.FaceNotComplete.Ordinal();
        public static readonly int UNKNOW_TYPE = FaceTracker.ErrCode.UnknowType.Ordinal();

        private Context context;
        private FaceTracker mFaceTracker;
        private int initStatus = FaceSDKManager.SDK_UNINIT;
        private FaceEnvironment faceEnvironment = new FaceEnvironment();

        /// <summary>
        /// FaceSDK 初始化，用户可以根据自己的需求实例化FaceTracker 和 FaceRecognize ，具体功能参考文档
        /// </summary>
        /// <param name="context"> </param>
        public void init(Context context)
        {
            this.context = context;
            init(context, faceEnvironment);
        }

        public void init(Context context, FaceEnvironment environment)
        {
            this.context = context;
            this.faceEnvironment = environment;

            if (mFaceTracker == null)
            {
                mFaceTracker = new FaceTracker(context);
                mFaceTracker.Set_isFineAlign(false);
                mFaceTracker.Set_isVerifyLive(false);
                mFaceTracker.Set_isCheckQuality(environment.isCheckQuality());
                mFaceTracker.Set_DetectMethodType(1);
                mFaceTracker.Set_isCheckQuality(environment.isCheckQuality());
                mFaceTracker.Set_notFace_thr(environment.getNotFaceThreshold());
                mFaceTracker.Set_min_face_size(environment.getMiniFaceSize());
                mFaceTracker.Set_cropFaceSize(FaceEnvironment.VALUE_CROP_FACE_SIZE);
                mFaceTracker.Set_illum_thr(environment.getIlluminationThreshold());
                mFaceTracker.Set_blur_thr(environment.getBlurrinessThreshold());
                mFaceTracker.Set_occlu_thr(environment.getOcclulationThreshold());
                mFaceTracker.Set_max_reg_img_num(FaceEnvironment.VALUE_MAX_CROP_IMAGE_NUM);
                mFaceTracker.Set_eulur_angle_thr(environment.getPitch(), environment.getYaw(), environment.getRoll());
                // mFaceTracker.set_track_by_detection_interval(50);
            }
        }
        public virtual FaceEnvironment FaceEnvironment
        {
            get
            {
                return faceEnvironment;
            }
            set
            {
                this.faceEnvironment = value;
            }
        }


        public virtual int InitStatus
        {
            set
            {
                this.initStatus = value;
            }
        }


        /// <summary>
        /// 进行人脸检测。返回检测结果代码。如果返回值为DETECT_CODE_OK 可调用 getTrackedFaces 获取人脸相关信息。
        /// </summary>
        /// <param name="argb">   人脸argb_8888图片。 </param>
        /// <param name="width">  图片宽度 </param>
        /// <param name="height"> 图片高度 </param>
        /// <returns> 检测结果代码。 </returns>
        public virtual int detect(int[] argb, int width, int height)
        {
            if (initStatus != FaceSDKManager.SDK_INITED)
            {
                return UNKNOW_TYPE;
            }
            int minDetectFace = FaceEnvironment.getMiniFaceSize();
            if (width < minDetectFace || height < minDetectFace)
            {
                return NO_FACE_DETECTED;
            }
            return this.mFaceTracker.Prepare_max_face_data_for_verify(argb, height, width, FaceSDK.ImgType.Argb.Ordinal(), FaceTracker.ActionType.Recognize.Ordinal());
        }

        /// <summary>
        /// 进行人脸检测。返回检测结果代码。如果返回值为DETECT_CODE_OK 可调用 getTrackedFaces 获取人脸相关信息。
        /// </summary>
        /// <param name="imageFrame"> 人脸图片帧 </param>
        /// <returns> 检测结果代码。 </returns>
        public virtual int detect(ImageFrame imageFrame)
        {
            if (initStatus != FaceSDKManager.SDK_INITED)
            {
                return UNKNOW_TYPE;
            }
            return detect(imageFrame.getArgb(), imageFrame.getWidth(), imageFrame.getHeight());
        }

        public virtual void detectMultiFace(ImageFrame imageFrame, int multiFaceNumber)
        {
            detectMultiFace(imageFrame.getArgb(), imageFrame.getWidth(), imageFrame.getHeight(), multiFaceNumber);
        }

        public virtual void detectMultiFace(int[] argb, int width, int height, int multiFaceNumber)
        {
            this.mFaceTracker.Track(argb, height, width, FaceSDK.ImgType.Argb.Ordinal(), multiFaceNumber);
        }

        /// <summary>
        /// yuv图片转换为相应的argb;
        /// </summary>
        /// <param name="yuv">      yuv_420p图片 </param>
        /// <param name="width">    图片宽度 </param>
        /// <param name="height">   图片高度 </param>
        /// <param name="argb">     接收argb用得 int数组 </param>
        /// <param name="rotation"> yuv图片的旋转角度 </param>
        /// <param name="mirror">   是否为镜像 </param>
        public static void yuvToARGB(byte[] yuv, int width, int height, int[] argb, int rotation, int mirror)
        {
            FaceSDK.GetARGBFromYUVimg(yuv, argb, width, height, rotation, mirror);
        }

        /// <summary>
        /// 获取当前跟踪的人脸信息。
        /// </summary>
        /// <returns> 返回人脸信息，没有时返回null </returns>
        public virtual FaceInfo[] TrackedFaces
        {
            get
            {
                return mFaceTracker.Get_TrackedFaceInfo();
            }
        }

        /// <summary>
        /// 获取当前跟踪的人脸信息。只返回一个。
        /// </summary>
        /// <returns> 返回人脸信息，没有时返回null </returns>
        public virtual FaceInfo TrackedFace
        {
            get
            {
                FaceInfo[] faces = mFaceTracker.Get_TrackedFaceInfo();
                if (faces != null && faces.Length > 0)
                {
                    return mFaceTracker.Get_TrackedFaceInfo()[0];
                }
                return null;
            }
        }
        /// <summary>
        /// 重置跟踪人脸。下次将重新开始跟踪。
        /// </summary>
        public virtual void clearTrackedFaces()
        {
            if (mFaceTracker != null)
            {
                mFaceTracker.ClearTrackedFaces();
            }
        }


        /// <summary>
        /// 根据设备的cpu核心数设定人脸sdk使用的线程数，如双核设置为2，四核设置为4
        /// </summary>
        /// <param name="numberOfThreads"> </param>
        public virtual int NumberOfThreads
        {
            set
            {
                FaceSDK.SetNumberOfThreads(value);
            }
        }

        /// <summary>
        /// 设置人脸概率阈值。范围是0-1。1是最严格，基本不存在？
        /// </summary>
        /// <param name="threshold"> 人脸概率阈值 </param>
        public virtual float NotFaceThreshold
        {
            set
            {
                this.faceEnvironment.setNotFaceThreshold(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_notFace_thr(value);
                }
            }
        }

        /// <summary>
        /// 设置最小检测人脸（两个眼睛之间的距离）小于此值的人脸检测不出来。范围为80-200。该值会严重影响检测性能。 </summary>
        /// 设置为100的性能损耗大概是200的4倍。所以在满足要求的前提下尽量设置大一些。默认值为 <seealso cref= (DEFAULT_MIN_FACE_SIZE)
        /// </seealso>
        /// <param name="faceSize"> 最小可检测人脸大小。 </param>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: public void setMinFaceSize(@IntRange(from = 80, to = 200) int faceSize)
        public virtual int MinFaceSize
        {
            set
            {
                this.faceEnvironment.setMinFaceSize(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_min_face_size(value);
                }
            }
        }

        /// <summary>
        /// 设置最低光照强度（YUV中的Y分量）取值范围0-255，建议值大于40.
        /// </summary>
        /// <param name="threshold"> 最低光照强度。 </param>
        public virtual float IlluminationThreshold
        {
            set
            {
                this.faceEnvironment.setIlluminationThreshold(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_illum_thr(value);
                }
            }
        }

        /// <summary>
        /// 设置模糊度。取值范围为0-1;0表示特别清晰，1表示，特别模糊。默认值为 @see(DEFAULT_BLURRINESS_THRESHOLD)。
        /// </summary>
        /// <param name="threshold"> 模糊度 </param>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: public void setBlurrinessThreshold(@FloatRange(from = 0, to = 1) float threshold)
        public virtual float BlurrinessThreshold
        {
            set
            {
                this.faceEnvironment.setBlurrinessThreshold(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_blur_thr(value);
                }
            }
        }
        /// <summary>
        /// 人脸遮挡阀值
        /// </summary>
        /// <param name="threshold"> </param>
        public virtual float OcclulationThreshold
        {
            set
            {
                this.faceEnvironment.setOcclulationThreshold(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_occlu_thr(value);
                }
            }
        }

        /// <summary>
        /// 设置是否检测质量
        /// </summary>
        /// <param name="checkQuality"> 是否检测质量 </param>
        public virtual bool CheckQuality
        {
            set
            {
                this.faceEnvironment.setCheckQuality(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_isCheckQuality(value);
                }
            }
        }

        // yaw 左右
        // pitch 上下
        // roll 扭头

        /// <summary>
        /// 设置头部欧拉角，大于这个值的人脸将不能识别。
        /// </summary>
        /// <param name="yaw">   左右摇头的角度。 </param>
        /// <param name="roll">  顺时针扭头的角度 </param>
        /// <param name="pitch"> 上下点头的角度。 </param>
        public virtual void setEulerAngleThreshold(int yaw, int roll, int pitch)
        {
            this.faceEnvironment.setYaw(yaw);
            this.faceEnvironment.setPitch(pitch);
            this.faceEnvironment.setRoll(roll);
        }

        /// <summary>
        /// 检测间隔设置，单位ms.该值控制检测间隔。值越大，检测时间越长，性能消耗越低。值越小，能更快的检测到人脸。
        /// </summary>
        /// <param name="interval"> 间隔时间，单位ms; </param>
        public virtual int DetectInterval
        {
            set
            {
                this.faceEnvironment.setDetectInterval(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_detect_in_video_interval(value);
                }
            }
        }

        public virtual int TrackInterval
        {
            set
            {
                this.faceEnvironment.setTrackInterval(value);
                if (mFaceTracker != null)
                {
                    mFaceTracker.Set_track_by_detection_interval(value);
                }
            }
        }


    }
}
