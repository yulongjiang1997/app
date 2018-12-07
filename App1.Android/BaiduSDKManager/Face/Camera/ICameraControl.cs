using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1.Droid.BaiduSDKManager.Face.Camera
{
    public interface OnFrameListener<T>
    {
        void onPreviewFrame(T data, int rotation, int width, int height);
    }

    /**
         * 照相回调。
         */
    public interface OnTakePictureCallback
    {
        void onPictureTaken(byte[] data);
    }

    public static class IcameraControlModel
    {
        /**
        * 闪光灯关
        */
        public static int FLASH_MODE_OFF = 0;
        /**
         * 闪光灯开
         */
        public static int FLASH_MODE_TORCH = 1;
        /**
         * 闪光灯自动
         */
        public static int FLASH_MODE_AUTO = 2;



        public static int CAMERA_FACING_BACK = 0;

        public static int CAMERA_FACING_FRONT = 1;

        public static int CAMERA_USB = 2;
    }

    public interface ICameraControl<T>
    {

        /**
         * 打开相机。
         */
        void start();

        /**
         * 关闭相机
         */
        void stop();

        void pause();

        void resume();

        void setOnFrameListener(OnFrameListener<T> listener);

        void setPreferredPreviewSize(int width, int height);

        /**
         * 相机对应的预览视图。
         *
         * @return 预览视图
         */
        View getDisplayView();

        void setPreviewView(PreviewView previewView);

        PreviewView getPreviewView();

        /**
         * 看到的预览可能不是照片的全部。返回预览视图的全貌。
         *
         * @return 预览视图frame;
         */
        Rect getPreviewFrame();

        /**
         * 拍照。结果在回调中获取。
         *
         * @param callback 拍照结果回调
         */
        //    void takePicture(OnTakePictureCallback callback);

        /**
         * 设置权限回调，当手机没有拍照权限时，可在回调中获取。
         *
         * @param callback 权限回调
         */
        void setPermissionCallback(PermissionCallback callback);

        /**
         * 设置水平方向
         *
         * @param displayOrientation 参数值见 {@link CameraView.Orientation}
         */
        void setDisplayOrientation(int displayOrientation);

        /**
         * 获取到拍照权限时，调用些函数以继续。
         */
        void refreshPermission();

        /**
         * 设置闪光灯状态。
         *
         * @param flashMode {@link #FLASH_MODE_TORCH,#FLASH_MODE_OFF,#FLASH_MODE_AUTO}
         */
        //    void setFlashMode(@FlashMode int flashMode);

        /**
         * 获取当前闪光灯状态
         *
         * @return 当前闪光灯状态
         */
        int getFlashMode();

        void setCameraFacing(int cameraFacing);

    }
}