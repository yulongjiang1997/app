using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager;
using App1.Droid.BaiduSDKManager.Face;
using App1.Droid.BaiduSDKManager.Face.Camera;
using App1.Droid.BaiduSDKManager.Manager;
using App1.Droid.BaiduSDKManager.Utils;
using Com.Baidu.Idl.Facesdk;
using Java.Lang;
using Java.Util.Concurrent;

namespace App1.Droid.Activitys
{
    public class RgbVideoAttributeTrackActivity : Activity, View.IOnClickListener
    {
        public RgbVideoAttributeTrackActivity()
        {
            paint.Color = (Color.Yellow);
            paint.SetStyle(Paint.Style.Stroke);
            paint.TextSize = (30);
        }

        private static int PICK_PHOTO = 100;
        // 预览View;
        private PreviewView previewView;
        // textureView用于绘制人脸框等。
        private TextureView textureView;
        // 用于检测人脸。
        private FaceDetectManager faceDetectManager;
        private TextView textAttr;
        private TextView rgbLivenessScoreTv;
        private TextView rgbLivenssDurationTv;
        private TextView tipTv;

        private FaceAttribute mFaceAttribute;

        // 为了方便调式。
        private Handler handler = new Handler();
        private byte[] photoFeature = new byte[2048];
        private volatile bool matching = false;
        private IExecutorService es = Executors.NewSingleThreadExecutor();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_attribute_track);
            findView();
            init();
            addListener();

        }

        private void findView()
        {
            previewView = (PreviewView)FindViewById(Resource.Id.preview_view);
            textureView = (TextureView)FindViewById(Resource.Id.texture_view);
            textAttr = (TextView)FindViewById(Resource.Id.text_attr);
            rgbLivenessScoreTv = (TextView)FindViewById(Resource.Id.text_rgb_liveness_score);
            rgbLivenssDurationTv = (TextView)FindViewById(Resource.Id.text_rgb_livenss_duration);
            tipTv = (TextView)FindViewById(Resource.Id.text_tip);
        }

        private void init()
        {

            faceDetectManager = new FaceDetectManager(this);
            FaceSDK.FaceAttributeModelInit(this);
            // 从系统相机获取图片帧。
            CameraImageSource cameraImageSource = new CameraImageSource(this);
            // 图片越小检测速度越快，闸机场景640 * 480 可以满足需求。实际预览值可能和该值不同。和相机所支持的预览尺寸有关。
            // 可以通过 camera.getParameters().getSupportedPreviewSizes()查看支持列表。
            cameraImageSource.getCameraControl().setPreferredPreviewSize(1280, 720);

            // 设置最小人脸，该值越小，检测距离越远，该值越大，检测性能越好。范围为80-200

            // 设置预览
            cameraImageSource.setPreviewView(previewView);
            // 设置图片源
            faceDetectManager.setImageSource(cameraImageSource);

            textureView.SetOpaque(false);
            // 不需要屏幕自动变黑。
            textureView.KeepScreenOn = (true);
            bool isPortrait = Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait;
            if (isPortrait)
            {
                previewView.setScaleType(ScaleType.FIT_WIDTH);
                // 相机坚屏模式
                cameraImageSource.getCameraControl().setDisplayOrientation(App1.Droid.BaiduSDKManager.Face.Camera.CameraView.ORIENTATION_PORTRAIT);
            }
            else
            {
                previewView.setScaleType(ScaleType.FIT_HEIGHT);
                // 相机横屏模式
                cameraImageSource.getCameraControl().setDisplayOrientation(CameraView.ORIENTATION_HORIZONTAL);
            }

            setCameraType(cameraImageSource);
        }

        private void setCameraType(CameraImageSource cameraImageSource)
        {
            // TODO 选择使用前置摄像头
            // cameraImageSource.getCameraControl().setCameraFacing(ICameraControl.CAMERA_FACING_FRONT);

            // TODO 选择使用usb摄像头
            cameraImageSource.getCameraControl().setCameraFacing(2);
            //        // 如果不设置，人脸框会镜像，显示不准
            previewView.getTextureView().ScaleX=(-1);

            // TODO 选择使用后置摄像头
            //        cameraImageSource.getCameraControl().setCameraFacing(ICameraControl.CAMERA_FACING_BACK);
            //        previewView.getTextureView().setScaleX(-1);
        }

        private void addListener()
        {
            // 设置回调，回调人脸检测结果。
            faceDetectManager.setUseDetect(true);
            //        faceDetectManager.setOnFaceDetectListener(new FaceDetectManager.OnFaceDetectListener() {
            //        @Override
            //        public void onDetectFace(int retCode, FaceInfo[] infos, ImageFrame frame)
            //        {
            //            // TODO 显示检测的图片。用于调试，如果人脸sdk检测的人脸需要朝上，可以通过该图片判断
            //            Bitmap bitmap =
            //                  Bitmap.createBitmap(frame.getArgb(), frame.getWidth(),
            //                          frame.getHeight(), Bitmap.Config.ARGB_8888);
            //            handler.post(new Runnable() {
            //                @Override
            //                public void run()
            //            {
            //                // testView.setImageBitmap(bitmap);
            //            }
            //        });
            //        checkFace(retCode, infos, frame);
            //        showFrame(frame, infos);

            //    }
            //});
        }


        protected override void OnStart()
        {
            base.OnStart();
            // 开始检测
            faceDetectManager.start();
        }


        protected override void OnStop()
        {
            base.OnStop();
            // 结束检测。
            faceDetectManager.stop();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            faceDetectManager.stop();
        }


        public  void OnClick(View v)
        {

        }

        private void checkFace(int retCode, FaceInfo[] faceInfos, ImageFrame frame)
        {
            if (retCode == FaceTracker.ErrCode.Ok.Ordinal() && faceInfos != null)
            {
                FaceInfo faceInfo = faceInfos[0];
                string tip = filter(faceInfo, frame);
                displayTip(tip);
            }
            else
            {
                string tip = checkFaceCode(retCode);
                displayTip(tip);
            }
        }

        private string filter(FaceInfo faceInfo, ImageFrame imageFrame)
        {

            string tip = "";
            if (faceInfo.MConf < 0.6)
            {
                tip = "人脸置信度太低";
                return tip;
            }

            float[] headPose = faceInfo.HeadPose.ToArray();
            if (Java.Lang.Math.Abs(headPose[0]) > 20 || Java.Lang.Math.Abs(headPose[1]) > 20 || Java.Lang.Math.Abs(headPose[2]) > 20)
            {
                tip = "人脸置角度太大，请正对屏幕";
                return tip;
            }

            int width = imageFrame.getWidth();
            int height = imageFrame.getHeight();
            // 判断人脸大小，若人脸超过屏幕二分一，则提示文案“人脸离手机太近，请调整与手机的距离”；
            // 若人脸小于屏幕三分一，则提示“人脸离手机太远，请调整与手机的距离”
            float ratio = (float)faceInfo.MWidth / (float)height;
            Log.Info("liveness_ratio", "ratio=" + ratio);
            if (ratio > 0.6)
            {
                tip = "人脸离屏幕太近，请调整与屏幕的距离";
                return tip;
            }
            else if (ratio < 0.2)
            {
                tip = "人脸离屏幕太远，请调整与屏幕的距离";
                return tip;
            }
            else if (faceInfo.MCenterX > width * 3 / 4)
            {
                tip = "人脸在屏幕中太靠右";
                return tip;
            }
            else if (faceInfo.MCenterX < width / 4)
            {
                tip = "人脸在屏幕中太靠左";
                return tip;
            }
            else if (faceInfo.MCenterY > height * 3 / 4)
            {
                tip = "人脸在屏幕中太靠下";
                return tip;
            }
            else if (faceInfo.MCenterX < height / 4)
            {
                tip = "人脸在屏幕中太靠上";
                return tip;
            }

            int liveType = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                    .TYPE_NO_LIVENSS);
            if (liveType == LivenessSettingActivity.TYPE_NO_LIVENSS)
            {
                //asyncMath(photoFeature, faceInfo, imageFrame);
                attrCheck(faceInfo, imageFrame);
            }
            else if (liveType == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {
                float rgbLivenessScore = rgbLiveness(imageFrame, faceInfo);
                if (rgbLivenessScore > 0.9)
                {
                    //asyncMath(photoFeature, faceInfo, imageFrame);
                    attrCheck(faceInfo, imageFrame);
                }
                else
                {
                    toast("rgb活体分数过低");
                }
            }

            return tip;
        }

        private string checkFaceCode(int errCode)
        {
            string tip = "";
            if (errCode == FaceTracker.ErrCode.NoFaceDetected.Ordinal())
            {
                //            tip = "未检测到人脸";
            }
            else if (errCode == FaceTracker.ErrCode.ImgBlured.Ordinal() ||
                  errCode == FaceTracker.ErrCode.PitchOutOfDownMaxRange.Ordinal() ||
                  errCode == FaceTracker.ErrCode.PitchOutOfUpMaxRange.Ordinal() ||
                  errCode == FaceTracker.ErrCode.YawOutOfLeftMaxRange.Ordinal() ||
                  errCode == FaceTracker.ErrCode.YawOutOfRightMaxRange.Ordinal())
            {
                tip = "请静止平视屏幕";
            }
            else if (errCode == FaceTracker.ErrCode.PoorIllumination.Ordinal())
            {
                tip = "光线太暗，请到更明亮的地方";
            }
            else if (errCode == FaceTracker.ErrCode.UnknowType.Ordinal())
            {
                tip = "未检测到人脸";
            }
            return tip;
        }

        private float rgbLiveness(ImageFrame imageFrame, FaceInfo faceInfo)
        {

            long starttime = DateTime.Now.Millisecond;
            float rgbScore = FaceLiveness.getInstance().rgbLiveness(imageFrame.getArgb(), imageFrame
                  .getWidth(), imageFrame.getHeight(), faceInfo.Landmarks.ToArray());
            long duration = DateTime.Now.Millisecond - starttime;

            displayTip("RGB活体分数：" + rgbScore, rgbLivenessScoreTv);
            displayTip("RGB活体耗时：" + duration, rgbLivenssDurationTv);

            return rgbScore;
        }

        private void toast(string text)
        {
            handler.Post(new Runnable(() => { Toast.MakeText(this, text, ToastLength.Long).Show(); }));
        }

        private void displayTip(string tip)
        {
            RunOnUiThread(new Runnable(() => { tipTv.Text = tip; }));
        }

        private void displayTip(string tip, TextView textView)
        {
            RunOnUiThread(new Runnable(() => { textView.Text = (tip); }));
        }


        private Paint paint = new Paint();

        RectF rectF = new RectF();

        /**
         * 绘制人脸框。
         */
        private void showFrame(ImageFrame imageFrame, FaceInfo[] faceInfos)
        {
            Canvas canvas = textureView.LockCanvas();
            if (canvas == null)
            {
                textureView.UnlockCanvasAndPost(canvas);
                return;
            }
            if (faceInfos == null || faceInfos.Length == 0)
            {
                // 清空canvas
                canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                textureView.UnlockCanvasAndPost(canvas);
                return;
            }
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            FaceInfo faceInfo = faceInfos[0];

            rectF.Set(getFaceRect(faceInfo, imageFrame));

            // 检测图片的坐标和显示的坐标不一样，需要转换。
            previewView.mapFromOriginalRect(rectF);

            float yaw = Java.Lang.Math.Abs(faceInfo.HeadPose[0]);
            float patch = Java.Lang.Math.Abs(faceInfo.HeadPose[1]);
            float roll = Java.Lang.Math.Abs(faceInfo.HeadPose[2]);
            if (yaw > 20 || patch > 20 || roll > 20)
            {
                // 不符合要求，绘制黄框
                paint.Color = (Color.Yellow);

                string text = "请正视屏幕";
                float width = paint.MeasureText(text) + 50;
                float x = rectF.CenterX() - width / 2;
                paint.Color = (Color.Red);
                paint.SetStyle(Paint.Style.Fill);
                canvas.DrawText(text, x + 25, rectF.Top - 20, paint);
                paint.Color = (Color.Yellow);

            }
            else
            {
                // 符合检测要求，绘制绿框
                paint.Color = (Color.Green);
            }
            paint.SetStyle(Paint.Style.Stroke);
            // 绘制框
            canvas.DrawRect(rectF, paint);
            textureView.UnlockCanvasAndPost(canvas);
        }

        /**
         * 绘制人脸框。
         *
         * @param model 追踪到的人脸
         */
        private void showFrame(FaceFilter.TrackedModel model)
        {
            Canvas canvas = textureView.LockCanvas();
            if (canvas == null)
            {
                return;
            }
            // 清空canvas
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            if (model != null)
            {
                model.getImageFrame().retain();
                rectF.Set(model.getFaceRect());

                // 检测图片的坐标和显示的坐标不一样，需要转换。
                previewView.mapFromOriginalRect(rectF);
                if (model.meetCriteria())
                {
                    // 符合检测要求，绘制绿框
                    paint.Color = (Color.Green);
                }
                else
                {
                    // 不符合要求，绘制黄框
                    paint.Color = (Color.Yellow);

                    string text = "请正视屏幕";
                    float width = paint.MeasureText(text) + 50;
                    float x = rectF.CenterX() - width / 2;
                    paint.Color = (Color.Red);
                    paint.SetStyle(Paint.Style.Fill);
                    canvas.DrawText(text, x + 25, rectF.Top - 20, paint);
                    paint.Color = (Color.Yellow);
                }
                paint.SetStyle(Paint.Style.Stroke);
                // 绘制框
                canvas.DrawRect(rectF, paint);
            }
            textureView.UnlockCanvasAndPost(canvas);
        }

        /**
         * 获取人脸框区域。
         *
         * @return 人脸框区域
         */
        // TODO padding?
        public Rect getFaceRect(FaceInfo faceInfo, ImageFrame frame)
        {
            Rect rect = new Rect();
            int[] points = new int[8];
            faceInfo.GetRectPoints(points);

            int left = points[2];
            int top = points[3];
            int right = points[6];
            int bottom = points[7];

            //            int width = (right - left) * 4 / 3;
            //            int height = (bottom - top) * 4 / 3;
            //
            //            left = getInfo().mCenter_x - width / 2;
            //            top = getInfo().mCenter_y - height / 2;
            //
            //            rect.top = top;
            //            rect.left = left;
            //            rect.right = left + width;
            //            rect.bottom = top + height;

            //            int width = (right - left) * 4 / 3;
            //            int height = (bottom - top) * 5 / 3;
            int width = (right - left);
            int height = (bottom - top);

            //            left = getInfo().mCenter_x - width / 2;
            //            top = getInfo().mCenter_y - height * 2 / 3;
            left = (int)(faceInfo.MCenterX - width / 2);
            top = (int)(faceInfo.MCenterY - height / 2);


            rect.Top = top < 0 ? 0 : top;
            rect.Left = left < 0 ? 0 : left;
            rect.Right = (left + width) > frame.getWidth() ? frame.getWidth() : (left + width);
            rect.Bottom = (top + height) > frame.getHeight() ? frame.getHeight() : (top + height);

            return rect;
        }

        /**
         * 进行属性检测
         */
        private void attrCheck(FaceInfo faceInfo, ImageFrame imageFrame)
        {
            // todo 人脸属性数据获取
            mFaceAttribute = FaceSDK.FaceAttribute(imageFrame.getArgb(),
                    imageFrame.getWidth(), imageFrame.getHeight(), FaceSDK.ImgType.Argb,
                    faceInfo.Landmarks.ToArray());
            RunOnUiThread(new Runnable(() => { textAttr.Text = ("人脸属性：" + getMsg(mFaceAttribute)); }));
        }


        public string getMsg(FaceAttribute attribute)
        {
            Java.Lang.StringBuilder msg = new Java.Lang.StringBuilder();
            if (attribute != null)
            {
                msg.Append((int)attribute.Age).Append(",")
                        .Append(attribute.Race == 0 ? "黄种人" : attribute.Race == 1 ? "白种人" :
                                attribute.Race == 2 ? "黑人" : attribute.Race == 3 ? "印度人" : "地球人").Append(",")
                        .Append(attribute.Expression == 0 ? "正常" : attribute.Expression == 1 ? "微笑" : "大笑").Append(",")
                        .Append(attribute.Gender == 0 ? "女" : attribute.Gender == 1 ? "男" : "婴儿").Append(",")
                        .Append(attribute.Glasses == 0 ? "不戴眼镜" : attribute.Glasses == 1 ? "普通透明眼镜" : "太阳镜");
            }
            return msg.ToString();
        }

    }
}