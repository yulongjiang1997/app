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
using Java.IO;

namespace App1.Droid.Activitys
{
    public class RgbDetectActivity : Activity
    {

        private static readonly int PICK_PHOTO = 100;
        private TextView nameTextView;
        // 预览View;
        private PreviewView previewView;
        // textureView用于绘制人脸框等。
        private TextureView textureView;
        private TextView detectDurationTv;
        private TextView rgbLivenssDurationTv;
        private TextView rgbLivenessScoreTv;
        private TextView featureDurationTv;
        // 用于检测人脸。
        private FaceDetectManager faceDetectManager;

        private TextView tipTv;
        // 为了方便调式。
        private ImageView testView;
        private Handler handler = new Handler();
        private int source;

        protected void onCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_rgb_detect);
            findView();
            init();
            addListener();

            Intent intent = Intent;
            if (intent != null)
            {
                source = intent.GetIntExtra("source", -1);
            }
        }

        public void ini()
        {
            //findView();
            init();
            addListener();

            Intent intent = Intent;
            if (intent != null)
            {
                source = intent.GetIntExtra("source", -1);
            }
        }
        private void findView()
        {
            testView = (ImageView)FindViewById(Resource.Id.test_view);
            previewView = (PreviewView)FindViewById(Resource.Id.preview_view);
            textureView = (TextureView)FindViewById(Resource.Id.texture_view);
            tipTv = (TextView)FindViewById(Resource.Id.tip_tv);
            detectDurationTv = (TextView)FindViewById(Resource.Id.detect_duration_tv);
            rgbLivenssDurationTv = (TextView)FindViewById(Resource.Id.rgb_liveness_duration_tv);
            rgbLivenessScoreTv = (TextView)FindViewById(Resource.Id.rgb_liveness_score_tv);
        }

        private void init()
        {
            faceDetectManager = new FaceDetectManager(this);
            // 从系统相机获取图片帧。
            CameraImageSource cameraImageSource = new CameraImageSource(this);
            // 图片越小检测速度越快，闸机场景640 * 480 可以满足需求。实际预览值可能和该值不同。和相机所支持的预览尺寸有关。
            // 可以通过 camera.getParameters().getSupportedPreviewSizes()查看支持列表。
            cameraImageSource.getCameraControl().setPreferredPreviewSize(1280, 720);

            // 设置最小人脸，该值越小，检测距离越远，该值越大，检测性能越好。范围为80-200

            previewView.setMirrored(false);
            // 设置预览
            cameraImageSource.setPreviewView(previewView);
            // 设置图片源
            faceDetectManager.setImageSource(cameraImageSource);
            faceDetectManager.setUseDetect(true);
            textureView.SetOpaque(false);

            // 不需要屏幕自动变黑。
            textureView.KeepScreenOn=true;
            bool isPortrait = Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait;
            if (isPortrait)
            {
                previewView.setScaleType(ScaleType.FIT_WIDTH);
                // 相机坚屏模式
                cameraImageSource.getCameraControl().setDisplayOrientation(0);
            }
            else
            {
                previewView.setScaleType(ScaleType.FIT_HEIGHT);
                // 相机横屏模式
                cameraImageSource.getCameraControl().setDisplayOrientation(1);
            }

            setCameraType(cameraImageSource);
        }

        private void setCameraType(CameraImageSource cameraImageSource)
        {
            // TODO 选择使用前置摄像头
            // cameraImageSource.getCameraControl().setCameraFacing(ICameraControl.CAMERA_FACING_FRONT);

            // TODO 选择使用usb摄像头
            cameraImageSource.getCameraControl().setCameraFacing(2);
            // 如果不设置，人脸框会镜像，显示不准
            previewView.getTextureView().ScaleX=-1;

            // TODO 选择使用后置摄像头
            //        cameraImageSource.getCameraControl().setCameraFacing(ICameraControl.CAMERA_FACING_BACK);
            //        previewView.getTextureView().setScaleX(-1);
        }

        private void addListener()
        {
            //        // 设置回调，回调人脸检测结果。
            //        faceDetectManager.setOnFaceDetectListener(new FaceDetectManager.OnFaceDetectListener()
            //        {
            //        public void onDetectFace(int retCode, FaceInfo[] infos, ImageFrame frame)
            //        {
            //            // TODO 显示检测的图片。用于调试，如果人脸sdk检测的人脸需要朝上，可以通过该图片判断
            //            final Bitmap bitmap =
            //              Bitmap.createBitmap(frame.getArgb(), frame.getWidth(), frame.getHeight(), Bitmap.Config.ARGB_8888);
            //            handler.post(new Runnable() {
            //                  @Override
            //                  public void run()
            //            {
            //                testView.setImageBitmap(bitmap);
            //            }
            //        });
            //        checkFace(retCode, infos, frame);
            //        showFrame(frame, infos);

            //    }
            //});
        }

        protected void onStart()
        {
            base.OnStart();
            // 开始检测
            faceDetectManager.start();
        }

        protected void onStop()
        {
            base.OnStop();
            // 结束检测。
            faceDetectManager.stop();
        }

        protected void onDestroy()
        {
            base.OnDestroy();
            faceDetectManager.stop();
        }

        private void checkFace(int retCode, FaceInfo[] faceInfos, ImageFrame frame)
        {
            if (retCode == FaceTracker.ErrCode.Ok.Ordinal() && faceInfos != null)
            {
                FaceInfo faceInfo = faceInfos[0];
                String tip = filter(faceInfo, frame);
                displayTip(tip);
            }
            else
            {
                String tip = checkFaceCode(retCode);
                displayTip(tip);
            }
        }


        private String filter(FaceInfo faceInfo, ImageFrame imageFrame)
        {

            String tip = "";
            if (faceInfo.MConf < 0.6)
            {
                tip = "人脸置信度太低";
                return tip;
            }

            float[] headPose = faceInfo.HeadPose.ToArray();
            if (Math.Abs(headPose[0]) > 20 || Math.Abs(headPose[1]) > 20 || Math.Abs(headPose[2]) > 20)
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
                saveFace(faceInfo, imageFrame);
            }
            else if (liveType == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {

                if (rgbLiveness(imageFrame, faceInfo) > 0.9)
                {
                    saveFace(faceInfo, imageFrame);
                }
                else
                {
                    toast("rgb活体分数过低");
                }
            }


            return tip;
        }

        private String checkFaceCode(int errCode)
        {
            String tip = "";
            if (errCode == FaceTracker.ErrCode.NoFaceDetected.Ordinal())
            {
                //            tip = "未检测到人脸";
            }
            else if (errCode == FaceTracker.ErrCode.ImgBlured.Ordinal() ||
                  errCode == FaceTracker.ErrCode.PitchOutOfDownMaxRange.Ordinal() ||
                  errCode == FaceTracker.ErrCode.PitchOutOfDownMaxRange.Ordinal() ||
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

            //    runOnUiThread(new Runnable() {
            //    @Override
            //    public void run()
            //    {
            //        rgbLivenssDurationTv.setVisibility(View.VISIBLE);
            //        rgbLivenessScoreTv.setVisibility(View.VISIBLE);
            //        rgbLivenssDurationTv.setText("RGB活体耗时：" + duration);
            //        rgbLivenessScoreTv.setText("RGB活体得分：" + rgbScore);
            //    }
            //});

            return rgbScore;
        }


        private void saveFace(FaceInfo faceInfo, ImageFrame imageFrame)
        {
            Bitmap bitmap = FaceCropper.getFace(imageFrame.getArgb(), faceInfo, imageFrame.getWidth());
            if (source == RegActivity.SOURCE_REG)
            {
                // 注册来源保存到注册人脸目录
                File faceDir = FileUitls.getFaceDirectory();
                if (faceDir != null)
                {
                    String imageName = Guid.NewGuid().ToString();
                    File file = new File(faceDir, imageName);
                    // 压缩人脸图片至300 * 300，减少网络传输时间
                    ImageUtils.resize(bitmap, file, 300, 300);
                    Intent intent = new Intent();
                    intent.PutExtra("file_path", file.AbsolutePath);
                    SetResult(Result.Ok, intent);
                    Finish();
                }
                else
                {
                    toast("注册人脸目录未找到");
                }
            }
            else
            {
                try
                {
                    // 其他来源保存到临时目录
                     File file = File.CreateTempFile(Guid.NewGuid().ToString()+ "", ".jpg");
                    // 人脸识别不需要整张图片。可以对人脸区别进行裁剪。减少流量消耗和，网络传输占用的时间消耗。
                    ImageUtils.resize(bitmap, file, 300, 300); Intent intent = new Intent();
                    intent.PutExtra("file_path", file.AbsolutePath);
                    SetResult(Result.Ok, intent);
                    Finish();

                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        private void toast(string text)
        {
            //    handler.post(new Runnable() {
            //        @Override
            //        public void run()
            //    {
            //        Toast.makeText(RgbDetectActivity.this, text, Toast.LENGTH_LONG).show();
            //    }
            //});
        }

        private void displayTip( string tip)
        {
            //    runOnUiThread(new Runnable() {
            //            @Override
            //            public void run()
            //    {
            //        tipTv.setText(tip);
            //    }
            //});
        }
        private Paint paint = new Paint();

        //{
        //    paint.setColor(Color.YELLOW);
        //    paint.setStyle(Paint.Style.STROKE);
        //    paint.setTextSize(30);
        //}

        RectF rectF = new RectF();

        /**
         * 绘制人脸框。
         *
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

            float yaw = Math.Abs(faceInfo.HeadPose[0]);
            float patch = Math.Abs(faceInfo.HeadPose[1]);
            float roll = Math.Abs(faceInfo.HeadPose[2]);
            if (yaw > 20 || patch > 20 || roll > 20)
            {
                // 不符合要求，绘制黄框
                paint.Color=Color.Yellow;

                String text = "请正视屏幕";
                float width = paint.MeasureText(text) + 50;
                float x = rectF.CenterX() - width / 2;
                paint.Color=Color.Red;
                paint.SetStyle(Paint.Style.Fill);
                canvas.DrawText(text, x + 25, rectF.Top - 20, paint);
                paint.Color=Color.Yellow;

            }
            else
            {
                // 符合检测要求，绘制绿框
                paint.Color=Color.Green;
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
                    paint.Color=Color.Green;
                }
                else
                {
                    // 不符合要求，绘制黄框
                    paint.Color=Color.Yellow;

                    String text = "请正视屏幕";
                    float width = paint.MeasureText(text) + 50;
                    float x = rectF.CenterX() - width / 2;
                    paint.Color=Color.Red;
                    paint.SetStyle(Paint.Style.Fill);
                    canvas.DrawText(text, x + 25, rectF.Top - 20, paint);
                    paint.Color=Color.Yellow;
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

    }
}