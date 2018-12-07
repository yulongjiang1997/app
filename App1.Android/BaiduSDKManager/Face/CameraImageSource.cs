using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Face.Camera;
using Java.Nio;
using App1.Droid.BaiduSDKManager.Manager;
using FaceDetector = App1.Droid.BaiduSDKManager.Manager.FaceDetector;

namespace App1.Droid.BaiduSDKManager.Face
{
    /**
 * 封装了系统做机做为输入源。
 */
    public class CameraImageSource : ImageSource
    {

        /**
         * 相机控制类
         */
        private ICameraControl<Camera1Control> cameraControl;
        private Context context;

        public ICameraControl<Camera1Control> getCameraControl()
        {
            return cameraControl;
        }

        private static  ArgbPool argbPool = new ArgbPool();

        private int cameraFaceType = 1;

        public void setCameraFacing(int type)
        {
            this.cameraFaceType = type;
        }


        private byte[] imageToByteArray(Image image)
        {
            ByteBuffer yBuffer = image.GetPlanes()[0].Buffer;
            ByteBuffer uBuffer = image.GetPlanes()[2].Buffer;
            ByteBuffer vBuffer = image.GetPlanes()[1].Buffer;

            int ySize = yBuffer.Remaining();
            int uSize = uBuffer.Remaining();
            int vSize = vBuffer.Remaining();

            byte[] nv21 = new byte[ySize + uSize + vSize];
            // U and V are swapped
            yBuffer.Get(nv21, 0, ySize);
            uBuffer.Get(nv21, ySize, vSize);
            vBuffer.Get(nv21, ySize + vSize, uSize);

            return nv21;
        }



        private class OnFrameListenerAnonymousInnerClass : OnFrameListener<byte[]>
        {
            public void onPreviewFrame(byte[] data, int rotation, int width, int height)
            {
                int[] argb = argbPool.acquire(width, height);

                if (argb == null || argb.Length != width * height)
                {
                    argb = new int[width * height];
                }

                rotation = rotation < 0 ? 360 + rotation : rotation;
                long starttime = DateTime.Now.Millisecond;
                FaceDetector.yuvToARGB(data, width, height, argb, rotation, 0);

                // 旋转了90或270度。高宽需要替换
                if (rotation % 180 == 90)
                {
                    int temp = width;
                    width = height;
                    height = temp;
                }

                ImageFrame frame = new ImageFrame();
                frame.setArgb(argb);
                frame.setWidth(width);
                frame.setHeight(height);
                frame.setPool(argbPool);
                List<OnFrameAvailableListener> listeners = getListeners();
                foreach (OnFrameAvailableListener listener in listeners)
                {
                    listener.onFrameAvailable(frame);
                }
                argbPool.release(argb);
            }
            
        }


        public CameraImageSource(Context context)
        {
            this.context = context;
            cameraControl = new Camera1Control(getContext());
            cameraControl.setCameraFacing(cameraFaceType);
            cameraControl.setOnFrameListener(new OnFrameListenerAnonymousInnerClass());
        }

        private void addCamera2Control()
        {
            // cameraControl = new Camera2Control(getContext());
            //cameraControl.setCameraFacing(cameraFaceType);
            //    cameraControl.setOnFrameListener(new ICameraControl.OnFrameListener<Bitmap>() {

            //            public void onPreviewFrame(Bitmap data, int rotation, int width, int height)
            //    {
            //        int[] argb = argbPool.acquire(width, height);

            //        if (argb == null || argb.length != width * height)
            //        {
            //            argb = new int[width * height];
            //        }

            //        rotation = rotation < 0 ? 360 + rotation : rotation;
            //        long starttime = System.currentTimeMillis();
            //        data.getPixels(argb, 0, width, 0, 0, width, height);

            //        // 旋转了90或270度。高宽需要替换
            //        if (rotation % 180 == 90)
            //        {
            //            int temp = width;
            //            width = height;
            //            height = temp;
            //        }

            //        ImageFrame frame = new ImageFrame();
            //        frame.setArgb(argb);
            //        frame.setWidth(width);
            //        frame.setHeight(height);
            //        frame.setPool(argbPool);
            //        ArrayList<OnFrameAvailableListener> listeners = getListeners();
            //        for (OnFrameAvailableListener listener : listeners) {
            //        listener.onFrameAvailable(frame);
            //    }
            //}
            //        });
        }



        public void start()
        {
            base.start();
            cameraControl.start();
        }


        public void stop()
        {
            base.stop();
            cameraControl.stop();
        }

        private Context getContext()
        {
            return context;
        }


        public void setPreviewView(PreviewView previewView)
        {
            cameraControl.setPreviewView(previewView);
        }
    }
}