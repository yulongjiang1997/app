using System;
using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Java.IO;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent.Atomic;

namespace App1.Droid.BaiduSDKManager.Face.Camera
{
    public class Camera1Control : ICameraControl<Camera1Control>
    {

        private static SparseIntArray ORIENTATIONS = new SparseIntArray();
        private static int MAX_PREVIEW_SIZE = 2048;

        //public class ImpliedClass
        //{
        //    //        ORIENTATIONS.append(Surface.ROTATION_0, 90);
        //    //        ORIENTATIONS.append(Surface.ROTATION_90, 0);
        //    //        ORIENTATIONS.append(Surface.ROTATION_180, 270);
        //    //        ORIENTATIONS.append(Surface.ROTATION_270, 180);

        //    ORIENTATIONS.append(Surface.ROTATION_0, 0);
        //    ORIENTATIONS.append(Surface.ROTATION_90, 90);
        //    ORIENTATIONS.append(Surface.ROTATION_180, 180);
        //    ORIENTATIONS.append(Surface.ROTATION_270, 270);
        //}


        private int displayOrientation = 0;
        private int cameraId = 0;
        private int flashMode;
        private AtomicBoolean takingPicture = new AtomicBoolean(false);

        private Context context;
        private Android.Hardware.Camera camera;
        private HandlerThread cameraHandlerThread = null;
        private Handler cameraHandler = null;
        private Handler uiHandler = null;

        private Android.Hardware.Camera.Parameters parameters;
        private PermissionCallback permissionCallback;
        private Rect previewFrame = new Rect();

        private int preferredWidth = 1280;
        private int preferredHeight = 720;


        private int cameraFacing = 1;


        public void setDisplayOrientation(int displayOrientation)
        {
            this.displayOrientation = displayOrientation;
        }

        /**
         * {@inheritDoc}
         */

        public void refreshPermission()
        {
            startPreview(true);
        }

        /**
         * {@inheritDoc}
         */
        //    @Override
        //    public void setFlashMode(@FlashMode int flashMode) {
        //        if (this.flashMode == flashMode) {
        //            return;
        //        }
        //        this.flashMode = flashMode;
        //        updateFlashMode(flashMode);
        //    }

        public int getFlashMode()
        {
            return flashMode;
        }


        public void setCameraFacing(int cameraFacing)
        {
            this.cameraFacing = cameraFacing;
        }



        public void start()
        {
            postStartCamera();
        }

        private SurfaceTexture surfaceTexture;

        private void postStartCamera()
        {
            if (cameraHandlerThread == null || !cameraHandlerThread.IsAlive)
            {
                cameraHandlerThread = new HandlerThread("camera");
                cameraHandlerThread.Start();
                cameraHandler = new Handler(cameraHandlerThread.Looper);
                uiHandler = new Handler(Looper.MainLooper);
            }

            if (cameraHandler == null)
            {
                return;
            }

            //    cameraHandler.post(new Runnable()
            //    {


            //    public void run()
            //    {
            //        try
            //        {
            //            startCamera();
            //        }
            //        catch (RuntimeException e)
            //        {
            //            e.printStackTrace();
            //        }
            //        catch (Exception e)
            //        {
            //            e.printStackTrace();
            //        }

            //    }
            //});
        }

        private void startCamera()
        {
            if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera)
                    != Android.Content.PM.Permission.Granted)
            {
                if (permissionCallback != null)
                {
                    permissionCallback.onRequestPermission();
                }
                return;
            }
            if (camera == null)
            {
                Android.Hardware.Camera.CameraInfo cameraInfo = new Android.Hardware.Camera.CameraInfo();
                for (int i = 0; i < Android.Hardware.Camera.NumberOfCameras; i++)
                {
                    Android.Hardware.Camera.GetCameraInfo(i, cameraInfo);
                    if ((int)cameraInfo.Facing == cameraFacing)
                    {
                        cameraId = i;
                    }
                }
                camera = Android.Hardware.Camera.Open(cameraId);
            }
            //        if (parameters == null) {
            //            parameters = camera.getParameters();
            //            parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            //        }


            int detectRotation = 0;
            if (cameraFacing == 1)
            {
                int rotation = ORIENTATIONS.Get(displayOrientation);
                rotation = getCameraDisplayOrientation(rotation, cameraId, camera);
                camera.SetDisplayOrientation(rotation);
                detectRotation = rotation;
                if (displayOrientation == 0)
                {
                    if (detectRotation == 90 || detectRotation == 270)
                    {
                        detectRotation = (detectRotation + 180) % 360;
                    }
                }
            }
            else if (cameraFacing == 0)
            {
                int rotation = ORIENTATIONS.Get(displayOrientation);
                rotation = getCameraDisplayOrientation(rotation, cameraId, camera);
                camera.SetDisplayOrientation(rotation);
                detectRotation = rotation;
            }
            else if (cameraFacing == 2)
            {
                camera.SetDisplayOrientation(0);
                detectRotation = 0;
            }

            opPreviewSize(preferredWidth, preferredHeight);
            Android.Hardware.Camera.Size size = camera.GetParameters().PreviewSize;
            if (detectRotation % 180 == 90)
            {
                previewView.setPreviewSize(size.Height, size.Width);
            }
            else
            {
                previewView.setPreviewSize(size.Width, size.Height);
            }
            int temp = detectRotation;
            try
            {
                // if (cameraFacing == ICameraControl.CAMERA_USB) {
                // camera.SetPreviewTexture(textureView.getSurfaceTexture());
                //            } else {
                //            surfaceTexture = new SurfaceTexture(11);
                //            camera.setPreviewTexture(surfaceTexture);
                //            uiHandler.post(new Runnable() {
                //                @Override
                //                public void run() {
                //
                //                    if (textureView != null) {
                //                        surfaceTexture.detachFromGLContext();
                //                        textureView.setSurfaceTexture(surfaceTexture);
                //                    }
                //                }
                //            });
                // }
                //            camera.addCallbackBuffer(new byte[size.width * size.height * 3 / 2]);
                //            camera.setPreviewCallbackWithBuffer(new Camera.PreviewCallback() {
                //
                //                @Override
                //                public void onPreviewFrame(byte[] data, Camera camera) {
                //                    Log.i("wtf", "onPreviewFrame-->");
                //                    onFrameListener.onPreviewFrame(data, temp, size.width, size.height);
                //                    camera.addCallbackBuffer(data);
                //           ad     }
                //            });
                //    camera.setPreviewCallback(new Camera.PreviewCallback() {

                //    public void onPreviewFrame(byte[] data, Camera camera)
                //    {
                //        LogUtil.i("wtf", "onPreviewFrame-->");
                //        onFrameListener.onPreviewFrame(data, temp, size.width, size.height);
                //    }
                //});

            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                LogUtil.i("wtf", e.ToString());
            }
            catch (RuntimeException e)
            {
                e.PrintStackTrace();
                LogUtil.i("wtf", e.ToString());
            }

        }

        private TextureView textureView;


        public void setTextureView(TextureView textureView)
        {
            this.textureView = textureView;
            if (surfaceTexture != null)
            {
                surfaceTexture.DetachFromGLContext();
                textureView.SurfaceTexture = surfaceTexture;
            }
        }

        private int getCameraDisplayOrientation(int degrees, int cameraId, Android.Hardware.Camera camera)
        {
            Android.Hardware.Camera.CameraInfo info = new Android.Hardware.Camera.CameraInfo();
            Android.Hardware.Camera.GetCameraInfo(cameraId, info);
            int rotation = 0;
            if (info.Facing == Android.Hardware.Camera.CameraInfo.CameraFacingFront)
            {
                rotation = (info.Orientation + degrees) % 360;
                rotation = (360 - rotation) % 360; // compensate the mirror
            }
            else
            { // back-facing
                rotation = (info.Orientation - degrees + 360) % 360;
            }
            return rotation;
        }


        public void stop()
        {
            if (camera != null)
            {
                camera.StopPreview();
                camera.SetPreviewCallback(null);
                camera.Release();
                camera = null;
            }
            if (cameraHandlerThread != null)
            {
                cameraHandlerThread.Quit();
                cameraHandlerThread = null;
            }
            LogUtil.i("wtf", "stop");
        }


        public void pause()
        {
            if (camera != null)
            {
                camera.StopPreview();
            }
            //        setFlashMode(FLASH_MODE_OFF);
            LogUtil.i("wtf", "pause");
        }


        public void resume()
        {
            takingPicture.Set(false);
            if (camera == null)
            {
                postStartCamera();
            }
        }

        private OnFrameListener<Camera1Control> onFrameListener;


        public void setOnFrameListener(OnFrameListener<Camera1Control> listener)
        {
            this.onFrameListener = listener;
        }


        public void setPreferredPreviewSize(int width, int height)
        {
            this.preferredWidth = Java.Lang.Math.Max(width, height);
            this.preferredHeight = Java.Lang.Math.Min(width, height);
        }


        public View getDisplayView()
        {
            return null;
        }


        private PreviewView previewView;


        public void setPreviewView(PreviewView previewView)
        {
            this.previewView = previewView;
            setTextureView(previewView.getTextureView());
        }


        public PreviewView getPreviewView()
        {
            return previewView;
        }

        //    @Override
        //    public void takePicture(final OnTakePictureCallback onTakePictureCallback) {
        //        if (takingPicture.get()) {
        //            return;
        //        }
        //
        //        switch (displayOrientation) {
        //            case CameraView.ORIENTATION_PORTRAIT:
        //                parameters.setRotation(90);
        //                break;
        //            case CameraView.ORIENTATION_HORIZONTAL:
        //                parameters.setRotation(0);
        //                break;
        //            case CameraView.ORIENTATION_INVERT:
        //                parameters.setRotation(180);
        //                break;
        //            default:
        //                break;
        //        }
        //        Camera.Size picSize =
        //                getOptimalSize(preferredWidth, preferredHeight, camera.getParameters().getSupportedPictureSizes());
        //        parameters.setPictureSize(picSize.width, picSize.height);
        //        camera.setParameters(parameters);
        //        takingPicture.set(true);
        //        camera.autoFocus(new Camera.AutoFocusCallback() {
        //            @Override
        //            public void onAutoFocus(boolean success, Camera camera) {
        //                camera.cancelAutoFocus();
        //                try {
        //                    camera.takePicture(null, null, new Camera.PictureCallback() {
        //                        @Override
        //                        public void onPictureTaken(byte[] data, Camera camera) {
        //                            camera.startPreview();
        //                            takingPicture.set(false);
        //                            if (onTakePictureCallback != null) {
        //                                onTakePictureCallback.onPictureTaken(data);
        //                            }
        //                        }
        //                    });
        //                } catch (RuntimeException e) {
        //                    e.printStackTrace();
        //                    camera.startPreview();
        //                    takingPicture.set(false);
        //                }
        //            }
        //        });
        //    }


        public void setPermissionCallback(PermissionCallback callback)
        {
            this.permissionCallback = callback;
        }

        public Camera1Control(Context context)
        {
            this.context = context;
        }

        // 开启预览
        private void startPreview(bool checkPermission)
        {
            if (ActivityCompat.CheckSelfPermission(context, Manifest.Permission.Camera)
                    != Permission.Granted)
            {
                if (checkPermission && permissionCallback != null)
                {
                    permissionCallback.onRequestPermission();
                }
                return;
            }
            camera.StartPreview();
        }

        private void opPreviewSize(int width, int height)
        {

            if (camera != null && width > 0)
            {
                try
                {
                    Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
                    Android.Hardware.Camera.Size optSize = getOptimalSize(width, height, new List<Android.Hardware.Camera.Size>(camera.GetParameters().SupportedPreviewSizes));
                    Log.Info("wtf", "opPreviewSize-> " + optSize.Width + " " + optSize.Height);
                    parameters.SetPreviewSize(optSize.Width, optSize.Height);
                    // parameters.setPreviewFpsRange(10, 15);
                    camera.SetParameters(parameters);
                    camera.StartPreview();
                }
                catch (RuntimeException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        private Android.Hardware.Camera.Size getOptimalSize(int width, int height, List<Android.Hardware.Camera.Size> sizes)
        {

            Android.Hardware.Camera.Size pictureSize = sizes[0];

            List<Android.Hardware.Camera.Size> candidates = new List<Android.Hardware.Camera.Size>();

            foreach (Android.Hardware.Camera.Size size in sizes)
            {
                if (size.Width >= width && size.Height >= height && size.Width * height == size.Height * width)
                {
                    // 比例相同
                    candidates.Add(size);
                }
                else if (size.Height >= width && size.Width >= height && size.Width * width == size.Height * height)
                {
                    // 反比例
                    candidates.Add(size);
                }
            }

            //if (candidates != null)
            //{
            //    return Collections.Min(candidates, sizeComparator);
            //}

            foreach (Android.Hardware.Camera.Size size in sizes)
            {
                if (size.Width >= width && size.Height >= height)
                {
                    return size;
                }
            }

            return pictureSize;
        }

        private class ComparatorAnonymousInnerClass : IComparer<Android.Hardware.Camera.Size>
        {
            public int Compare(Android.Hardware.Camera.Size lhs, Android.Hardware.Camera.Size rhs)
            {
                return Long.Signum((long)lhs.Width * lhs.Height - (long)rhs.Width * rhs.Height);
            }
        }

        private IComparer<Android.Hardware.Camera.Size> sizeComparator = new ComparatorAnonymousInnerClass();



        private void updateFlashMode(int flashMode)
        {
            switch (flashMode)
            {
                case 1:
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                    break;
                case 0:
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
                    break;
                case 2:
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeAuto;
                    break;
                default:
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeAuto;
                    break;
            }
            //        camera.setParameters(parameters);
        }

        private int getSurfaceOrientation()
        {
            int orientation = displayOrientation;
            switch (orientation)
            {
                case 0:
                    return 90;
                case 1:
                    return 0;
                case 2:
                    return 180;
                default:
                    return 90;
            }
        }
        public Rect getPreviewFrame()
        {
            return previewFrame;
        }
    }
}