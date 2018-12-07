﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager;
using App1.Droid.BaiduSDKManager.CallBack;
using App1.Droid.BaiduSDKManager.Entity;
using App1.Droid.BaiduSDKManager.Manager;
using Com.Baidu.Idl.Facesdk;
using Java.Lang;
using Java.Util.Concurrent;
using static Android.Renderscripts.ScriptGroup;

namespace App1.Droid.Activitys
{
    public class RgbIrVideoAttributeActivity : Activity, ILivenessCallBack, View.IOnClickListener
    {
        public RgbIrVideoAttributeActivity()
        {
            paint.Color = (Color.Yellow);
            paint.SetStyle(Paint.Style.Stroke);
            paint.TextSize = (30);
        }

        private static string TAG = "RgbIrVideoAttribute";
        // 图片越大，性能消耗越大，也可以选择640*480， 1280*720
        private static int PREFER_WIDTH = 640;
        private static int PERFER_HEIGH = 480;
        Preview[] mPreview;
        Android.Hardware.Camera[] mCamera;
        private int mCameraNum;
        private ImageView testIv;

        private TextView tipTv;

        private TextView detectDurationTv;
        private TextView rgbLivenssDurationTv;
        private TextView rgbLivenessScoreTv;
        private TextView irLivenssDurationTv;
        private TextView irLivenessScoreTv;

        private FaceAttribute mFaceAttribute;

        private TextView tvAttr;
        private IExecutorService es = Executors.NewSingleThreadExecutor();
        private Future future;
        private bool success = false;
        private byte[] photoFeature = new byte[2048];
        private volatile int[] niRargb;
        private volatile int[] rgbData;
        private volatile byte[] irData;
        private int camemra1DataMean;
        private int camemra2DataMean;
        private volatile bool camemra1IsRgb = false;
        private volatile bool rgbOrIrConfirm = false;

        // textureView用于绘制人脸框等。
        private TextureView textureView;
        private TextureView textureViewOne;
        private SurfaceView surface_one, surface_two;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_rgb_ir_video_attribute);
            FaceSDK.InitModel(this);
            findView();
            FaceSDK.FaceAttributeModelInit(this);
            FaceLiveness.getInstance().setLivenessCallBack(this);

        }


        private void findView()
        {
            textureViewOne = (TextureView)FindViewById(Resource.Id.texture_view_one);
            textureViewOne.SetOpaque(false);
            textureView = (TextureView)FindViewById(Resource.Id.texture_view);
            textureView.SetOpaque(false);
            surface_one = (SurfaceView)FindViewById(Resource.Id.surface_one);
            surface_two = (SurfaceView)FindViewById(Resource.Id.surface_two);
            tipTv = (TextView)FindViewById(Resource.Id.tip_tv);
            detectDurationTv = (TextView)FindViewById(Resource.Id.detect_duration_tv);
            rgbLivenssDurationTv = (TextView)FindViewById(Resource.Id.rgb_liveness_duration_tv);
            rgbLivenessScoreTv = (TextView)FindViewById(Resource.Id.rgb_liveness_score_tv);
            irLivenssDurationTv = (TextView)FindViewById(Resource.Id.ir_liveness_duration_tv);
            irLivenessScoreTv = (TextView)FindViewById(Resource.Id.ir_liveness_score_tv);

            detectDurationTv = (TextView)FindViewById(Resource.Id.detect_duration_tv);
            rgbLivenssDurationTv = (TextView)FindViewById(Resource.Id.rgb_liveness_duration_tv);
            rgbLivenessScoreTv = (TextView)FindViewById(Resource.Id.rgb_liveness_score_tv);
            irLivenssDurationTv = (TextView)FindViewById(Resource.Id.ir_liveness_duration_tv);
            irLivenessScoreTv = (TextView)FindViewById(Resource.Id.ir_liveness_score_tv);
            tvAttr = (TextView)FindViewById(Resource.Id.action_bar_root);//待修改

            mCameraNum = Android.Hardware.Camera.NumberOfCameras;

            testIv = (ImageView)FindViewById(Resource.Id.test_iv);

            if (mCameraNum != 2)
            {
                Toast.MakeText(this, "未检测到2个摄像头", ToastLength.Long).Show();
                return;
            }
            else
            {
                SurfaceView[] surfaViews = new SurfaceView[mCameraNum];
                mPreview = new Preview[mCameraNum];
                mCamera = new Android.Hardware.Camera[mCameraNum];
                mPreview[0] = new Preview(this, surface_one);
                mPreview[1] = new Preview(this, surface_two);
            }


            //        for (int i = 0; i < mCameraNum; i++) {
            //            surfaViews[i] = new SurfaceView(this);
            //            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(
            //                    LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT, 1.0f);
            //            // lp.setMargins(10, 10, 10, 10);
            //            surfaViews[i].setLayoutParams(lp);
            //            ((LinearLayout) FindViewById(Resource.Id.camera_layout)).addView(surfaViews[i]);
            //
            //            mPreview[i] = new Preview(this, surfaViews[i]);
            //            mPreview[i].setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
            //            ((RelativeLayout) FindViewById(Resource.Id.layout)).addView(mPreview[i]);
            //        }
        }



        protected override void OnResume()
        {
            base.OnResume();
            if (mCameraNum != 2)
            {
                Toast.MakeText(this, "未检测到2个摄像头", ToastLength.Long).Show();
                return;
            }
            else
            {
                try
                {
                    mCamera[0] = Android.Hardware.Camera.Open(0);
                    mCamera[1] = Android.Hardware.Camera.Open(1);
                    mPreview[0].setCamera(mCamera[0], PREFER_WIDTH, PERFER_HEIGH);
                    mPreview[1].setCamera(mCamera[1], PREFER_WIDTH, PERFER_HEIGH);
                    //    mCamera[0].setPreviewCallback(new Camera.PreviewCallback() {
                    //    @Override
                    //    public void onPreviewFrame(byte[] data, Camera camera)
                    //    {
                    //        if (rgbOrIrConfirm)
                    //        {
                    //            choiceRgbOrIrType(0, data);
                    //        }
                    //        else if (camemra1DataMean == 0)
                    //        {
                    //            rgbOrIr(0, data);
                    //        }
                    //    }
                    //});
                    //    mCamera[1].setPreviewCallback(new Camera.PreviewCallback() {
                    //        @Override
                    //        public void onPreviewFrame(byte[] data, Camera camera)
                    //    {
                    //        if (rgbOrIrConfirm)
                    //        {
                    //            choiceRgbOrIrType(1, data);
                    //        }
                    //        else if (camemra2DataMean == 0)
                    //        {
                    //            rgbOrIr(1, data);
                    //        }

                    //    }
                    //});
                }
                catch (RuntimeException e)
                {
                    Log.Error(TAG, e.Message);
                }

            }

            if (textureView != null)
            {
                textureView.SetOpaque(false);
            }
            if (textureViewOne != null)
            {
                textureViewOne.SetOpaque(false);
            }
        }

        private void rgbOrIr(int index, byte[] data)
        {
            byte[] tmp = new byte[PREFER_WIDTH * PERFER_HEIGH];
            System.Array.Copy(data, 0, tmp, 0, PREFER_WIDTH * PERFER_HEIGH);
            int count = 0;
            int total = 0;
            for (int i = 0; i < PREFER_WIDTH * PERFER_HEIGH; i = i + 100)
            {
                total += byteToInt(tmp[i]);
                count++;
            }

            if (index == 0)
            {
                camemra1DataMean = total / count;
            }
            else
            {
                camemra2DataMean = total / count;
            }
            if (camemra1DataMean != 0 && camemra2DataMean != 0)
            {
                if (camemra1DataMean > camemra2DataMean)
                {
                    camemra1IsRgb = true;
                }
                else
                {
                    camemra1IsRgb = false;
                }
                rgbOrIrConfirm = true;
            }
        }

        public int byteToInt(byte b)
        {
            //Java 总是把 byte 当做有符处理；我们可以通过将其和 0xFF 进行二进制与得到它的无符值
            return b & 0xFF;
        }

        // 判断camera数据类型，rgb活体和ir活体调用接口不同，需要区分，通过选取一点数量点的取平均值比较，大的为rgb，小的为ir
        private void choiceRgbOrIrType(int index, byte[] data)
        {
            // camera1如果为rgb数据，调用dealRgb，否则为Ir数据，调用Ir
            if (index == 0)
            {
                if (camemra1IsRgb)
                {
                    dealRgb(data);
                }
                else
                {
                    dealIr(data);
                }
            }
            else
            {
                if (camemra1IsRgb)
                {
                    dealIr(data);
                }
                else
                {
                    dealRgb(data);
                }
            }
        }

        private void dealRgb(byte[] data)
        {
            if (rgbData == null)
            {
                int[] argb = new int[PREFER_WIDTH * PERFER_HEIGH];
                FaceSDK.GetARGBFromYUVimg(data, argb, PREFER_WIDTH, PERFER_HEIGH, 0, 0);

                rgbData = argb;
                checkData();
                Bitmap bitmap = Bitmap.CreateBitmap(argb, PREFER_WIDTH, PERFER_HEIGH, Bitmap.Config.Argb8888);
                RunOnUiThread(new Runnable(() => { testIv.SetImageBitmap(bitmap); }));
            }
        }

        private void dealIr(byte[] data)
        {
            if (irData == null)
            {
                // int[] argb = new int[640 * 480];
                // FaceSDK.getARGBFromYUVimg(data, argb, 480, 640, 0, 0);

                niRargb = new int[PREFER_WIDTH * PERFER_HEIGH];
                FaceSDK.GetARGBFromYUVimg(data, niRargb, PERFER_HEIGH, PREFER_WIDTH, 0, 0);

                byte[] ir = new byte[PREFER_WIDTH * PERFER_HEIGH];
                System.Array.Copy(data, 0, ir, 0, PREFER_WIDTH * PERFER_HEIGH);
                irData = ir;
                checkData();
            }
        }

        private void checkData()
        {
            if (rgbData != null && irData != null)
            {
                FaceLiveness.getInstance().setNirRgbInt(niRargb);
                FaceLiveness.getInstance().setRgbInt(rgbData);
                FaceLiveness.getInstance().setIrData(irData);
                FaceLiveness.getInstance().livenessCheck(PREFER_WIDTH, PERFER_HEIGH, 0x0011);
                rgbData = null;
                irData = null;
            }
        }


        protected override void OnPause()
        {
            for (int i = 0; i < mCameraNum; i++)
            {
                if (mCameraNum >= 2)
                {
                    if (mCamera[i] != null)
                    {
                        mCamera[i].SetPreviewCallback(null);
                        mCamera[i].StopPreview();
                        // mPreview[i].setCamera(null);
                        mCamera[i].Release();
                        mCamera[i] = null;
                    }
                }
            }
            base.OnPause();
        }

        LivenessModel mModel;


        public  void onCallback(LivenessModel livenessModel)
        {
            checkResult(livenessModel);
            mModel = livenessModel;
        }


        public  void onTip(int code, string msg)
        {
            RunOnUiThread(new Runnable(() => { tipTv.Text = (msg); }));
            showFrame2(mModel);
        }


        public  void onCanvasRectCallback(LivenessModel livenessModel)
        {
            //        if ((livenessModel.getLiveType() & MASK_RGB) == MASK_RGB) {
            //            showFrame(livenessModel.getImageFrame(), livenessModel.getTrackFaceInfo());
            //        }
        }


        public  void OnClick(View v)
        {

        }

        private void checkResult(LivenessModel model)
        {

            if (model == null)
            {
                ClearTip();
                return;

            }
            displayResult(model);
            int type = model.getLiveType();
            bool livenessSuccess = false;
            // 同一时刻都通过才认为活体通过，开发者也可以根据自己的需求修改策略
            if ((type & FaceLiveness.MASK_RGB) == FaceLiveness.MASK_RGB)
            {
                livenessSuccess = (model.getRgbLivenessScore() > FaceEnvironment.LIVENESS_RGB_THRESHOLD) ? true : false;
            }
            if ((type & FaceLiveness.MASK_IR) == FaceLiveness.MASK_IR)
            {
                bool irScore = (model.getIrLivenessScore() > FaceEnvironment.LIVENESS_IR_THRESHOLD) ? true : false;
                if (!irScore)
                {
                    livenessSuccess = false;
                }
                else
                {
                    livenessSuccess &= irScore;
                }
            }
            if ((type & FaceLiveness.MASK_DEPTH) == FaceLiveness.MASK_DEPTH)
            {
                bool depthScore = (model.getDepthLivenessScore() > FaceEnvironment.LIVENESS_DEPTH_THRESHOLD) ? true :
                        false;
                if (!depthScore)
                {
                    livenessSuccess = false;
                }
                else
                {
                    livenessSuccess &= depthScore;
                }
            }

            if (livenessSuccess)
            {
                attrCheck(model.getFaceInfo(), model.getImageFrame());
            }
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
            RunOnUiThread(new Runnable(() => { tvAttr.Text = ("人脸属性：" + getMsg(mFaceAttribute)); }));
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

        private void displayResult(LivenessModel livenessModel)
        {
            RunOnUiThread(new Runnable(() =>
            {
                int type = livenessModel.getLiveType();
                detectDurationTv.Text = ("人脸检测耗时：" + livenessModel.getRgbDetectDuration());
                if ((type & FaceLiveness.MASK_RGB) == FaceLiveness.MASK_RGB)
                {
                    rgbLivenessScoreTv.Text = ("RGB活体得分：" + livenessModel.getRgbLivenessScore());
                    rgbLivenssDurationTv.Text = ("RGB活体耗时：" + livenessModel.getRgbLivenessDuration());
                }

                if ((type & FaceLiveness.MASK_IR) == FaceLiveness.MASK_IR)
                {

                }

                if ((type & FaceLiveness.MASK_DEPTH) == FaceLiveness.MASK_DEPTH)
                {
                    irLivenessScoreTv.Text = ("IR活体得分：" + livenessModel.getIrLivenessScore());
                    irLivenssDurationTv.Text = ("IR活体耗时：" + livenessModel.getIrLivenessDuration());
                }
            }));
        }


        private void ClearTip()
        {
            RunOnUiThread(new Runnable(() =>
            {
                detectDurationTv.Text = ("");
                rgbLivenessScoreTv.Text = ("");
                rgbLivenssDurationTv.Text = ("");
                irLivenessScoreTv.Text = ("");
                irLivenssDurationTv.Text = ("");
                tvAttr.Text = ("");
            }));
        }

        private void toast(string tip)
        {
            RunOnUiThread(new Runnable(() => { Toast.MakeText(this, tip, ToastLength.Short).Show(); }));
        }

        private Paint paint = new Paint();


        RectF rectF = new RectF();


        /**
         * 绘制人脸框。
         */
        private void showFrame2(LivenessModel model)
        {

            if (camemra1IsRgb)
            {
                Canvas canvas2 = textureViewOne.LockCanvas();
                if (canvas2 == null)
                {
                    textureViewOne.UnlockCanvasAndPost(canvas2);
                    return;
                }

                if (model == null)
                {
                    canvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                    textureViewOne.UnlockCanvasAndPost(canvas2);
                    return;
                }

                FaceInfo[] faceInfos = model.getTrackFaceInfo();
                ImageFrame imageFrame = model.getImageFrame();
                if (faceInfos == null || faceInfos.Length == 0)
                {
                    // 清空canvas
                    canvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                    textureViewOne.UnlockCanvasAndPost(canvas2);
                    return;
                }
                canvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                FaceInfo faceInfo2 = faceInfos[0];
                rectF.Set(getFaceRectTwo(faceInfo2, imageFrame));
                // 检测图片的坐标和显示的坐标不一样，需要转换。
                // mPreview[typeIndex].mapFromOriginalRect(rectF);
                float yaw2 = Java.Lang.Math.Abs(faceInfo2.HeadPose[0]);
                float patch2 = Java.Lang.Math.Abs(faceInfo2.HeadPose[1]);
                float roll2 = Java.Lang.Math.Abs(faceInfo2.HeadPose[2]);
                if (yaw2 > 20 || patch2 > 20 || roll2 > 20)
                {
                    // 不符合要求，绘制黄框
                    paint.Color=(Color.Yellow);

                    string text = "请正视屏幕";
                    float width = paint.MeasureText(text) + 50;
                    float x = rectF.CenterX() - width / 2;
                    paint.Color=(Color.Red);
                    paint.SetStyle(Paint.Style.Fill);
                    canvas2.DrawText(text, x + 25, rectF.Top - 20, paint);
                    paint.Color=(Color.Yellow);
                }
                else
                {
                    // 符合检测要求，绘制绿框
                    paint.Color=(Color.Green);
                }
                paint.SetStyle(Paint.Style.Stroke);
                // 绘制框
                canvas2.DrawRect(rectF, paint);
                textureViewOne.UnlockCanvasAndPost(canvas2);


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
                textureView.UnlockCanvasAndPost(canvas);

            }
            else
            {
                Canvas canvas = textureView.LockCanvas();
                if (canvas == null)
                {
                    textureView.UnlockCanvasAndPost(canvas);
                    return;
                }

                if (model == null)
                {
                    // 清空canvas
                    canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                    textureView.UnlockCanvasAndPost(canvas);
                    return;
                }
                FaceInfo[] faceInfos = model.getTrackFaceInfo();
                ImageFrame imageFrame = model.getImageFrame();
                if (faceInfos == null || faceInfos.Length == 0)
                {
                    // 清空canvas
                    canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                    textureView.UnlockCanvasAndPost(canvas);
                    return;
                }
                canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

                FaceInfo faceInfo = faceInfos[0];

                rectF.Set(getFaceRectTwo(faceInfo, imageFrame));

                // 检测图片的坐标和显示的坐标不一样，需要转换。
                // mPreview[typeIndex].mapFromOriginalRect(rectF);

                float yaw = Java.Lang.Math.Abs(faceInfo.HeadPose[0]);
                float patch = Java.Lang.Math.Abs(faceInfo.HeadPose[1]);
                float roll = Java.Lang.Math.Abs(faceInfo.HeadPose[2]);
                if (yaw > 20 || patch > 20 || roll > 20)
                {
                    // 不符合要求，绘制黄框
                    paint.Color=(Color.Yellow);

                    string text = "请正视屏幕";
                    float width = paint.MeasureText(text) + 50;
                    float x = rectF.CenterX() - width / 2;
                    paint.Color=(Color.Red);
                    paint.SetStyle(Paint.Style.Fill);
                    canvas.DrawText(text, x + 25, rectF.Top - 20, paint);
                    paint.Color=(Color.Yellow);

                }
                else
                {
                    // 符合检测要求，绘制绿框
                    paint.Color=(Color.Green);
                }
                paint.SetStyle(Paint.Style.Stroke);
                // 绘制框
                canvas.DrawRect(rectF, paint);
                textureView.UnlockCanvasAndPost(canvas);


                Canvas canvas2 = textureViewOne.LockCanvas();
                if (canvas2 == null)
                {
                    textureViewOne.UnlockCanvasAndPost(canvas2);
                    return;
                }
                if (faceInfos == null || faceInfos.Length == 0)
                {
                    // 清空canvas
                    canvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                    textureViewOne.UnlockCanvasAndPost(canvas2);
                    return;
                }
                canvas2.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                textureViewOne.UnlockCanvasAndPost(canvas2);
            }
        }

        public Rect getFaceRectTwo(FaceInfo faceInfo, ImageFrame frame)
        {
            Rect rect = new Rect();
            int[] points = new int[8];
            faceInfo.GetRectPoints(points);
            int left = points[2];
            int top = points[3];
            int right = points[6];
            int bottom = points[7];
            //        int previewWidth=surfaViews[typeIndex].getWidth();
            //        int previewHeight=surfaViews[typeIndex].getHeight();
            int previewWidth;
            int previewHeight;

            //        previewWidth = surface_one.getWidth();
            //        previewHeight = surface_one.getHeight();
            if (camemra1IsRgb)
            {
                previewWidth = surface_one.MeasuredWidth;
                previewHeight = surface_one.MeasuredHeight;
            }
            else
            {
                previewWidth = surface_two.MeasuredWidth;
                previewHeight = surface_two.MeasuredHeight;
            }

            float scaleW = 1.0f * previewWidth / frame.getWidth();
            float scaleH = 1.0f * previewHeight / frame.getHeight();
            int width = (right - left);
            int height = (bottom - top);
            left = (int)((faceInfo.MCenterX - width / 2) * scaleW);
            top = (int)((faceInfo.MCenterY - height / 2) * scaleW);
            //        left = (int) ((faceInfo.mCenter_x)* scaleW);
            //        top =  (int) ((faceInfo.mCenter_y) * scaleW);
            rect.Top = top < 0 ? 0 : top;
            rect.Left = left < 0 ? 0 : left;
            rect.Right = (left + width) > frame.getWidth() ? frame.getWidth() : (left + width);
            rect.Bottom = (top + height) > frame.getHeight() ? frame.getHeight() : (top + height);
            return rect;
        }

    }

}