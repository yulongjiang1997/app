using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace App1.Droid.BaiduSDKManager.Face.Camera
{
    public class CameraView : FrameLayout
    {

        /**
         * 照相回调
         */
        interface OnTakePictureCallback
        {
            void onPictureTaken(Bitmap bitmap);
        }

        /**
         * 垂直方向 {@link #setOrientation(int)}
         */
        public static int ORIENTATION_PORTRAIT = 0;
        /**
         * 水平方向 {@link #setOrientation(int)}
         */
        public static int ORIENTATION_HORIZONTAL = 1;
        /**
         * 水平翻转方向 {@link #setOrientation(int)}
         */
        public static int ORIENTATION_INVERT = 2;

        public enum CameraViewEnum { ORIENTATION_PORTRAIT, ORIENTATION_HORIZONTAL, ORIENTATION_INVERT }
        public interface Orientation
        {

        }

        private CameraViewTakePictureCallback cameraViewTakePictureCallback = new CameraViewTakePictureCallback();

        private ICameraControl<CameraView> cameraControl;

        /**
         * 相机预览View
         */
        private View displayView;
        /**
         * 身份证，银行卡，等裁剪用的遮罩
         */
        //    private MaskView maskView;

        /**
         * 用于显示提示证 "请对齐身份证正面" 之类的
         */
        private ImageView hintView;

        public ICameraControl<CameraView> getCameraControl()
        {
            return cameraControl;
        }

        public void setOrientation(int orientation)
        {
            cameraControl.setDisplayOrientation(orientation);
        }

        public CameraView(Context context) : base(context)
        {
            init();
        }

        public CameraView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public CameraView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init();
        }

        public void start()
        {
            cameraControl.start();
            KeepScreenOn = true;
        }

        public void stop()
        {
            KeepScreenOn = false;
        }


        private void init()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                cameraControl = new Camera2Control(Context);
            }
            else
            {
                cameraControl = new Camera1Control(Context);
            }
            displayView = cameraControl.getDisplayView();
            AddView(displayView);


            hintView = new ImageView(Context);
            AddView(hintView);
        }


        protected void onLayout(bool changed, int left, int top, int right, int bottom)
        {
            displayView.Layout(left, 0, right, bottom - top);
        }
    }
}
