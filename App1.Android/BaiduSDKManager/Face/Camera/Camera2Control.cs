using System;
using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;

namespace App1.Droid.BaiduSDKManager.Face.Camera
{
    public class Camera2Control<T> : ICameraControl<T>
    {

        /**
         * Conversion from screen rotation to JPEG orientation.
         */
        private static SparseIntArray ORIENTATIONS = new SparseIntArray();
        private static int MAX_PREVIEW_SIZE = 2048;

        //    static {
        //    ORIENTATIONS.append(Surface.ROTATION_0, 90);
        //    ORIENTATIONS.append(Surface.ROTATION_90, 0);
        //    ORIENTATIONS.append(Surface.ROTATION_180, 270);
        //    ORIENTATIONS.append(Surface.ROTATION_270, 180);
        //}

        private static int STATE_PREVIEW = 0;
        private static int STATE_WAITING_FOR_LOCK = 1;
        private static int STATE_WAITING_FOR_CAPTURE = 2;
        private static int STATE_CAPTURING = 3;
        private static int STATE_PICTURE_TAKEN = 4;

        private static int MAX_PREVIEW_WIDTH = 1920;
        private static int MAX_PREVIEW_HEIGHT = 1080;

        private int flashMode;
        private int orientation = 0;
        private int state = STATE_PREVIEW;

        private Context context;
        private OnTakePictureCallback onTakePictureCallback;
        private PermissionCallback permissionCallback;
        private SurfaceTexture surfaceTexture;

        private string cameraId;
        private TextureView textureView;
        private Size previewSize;

        private HandlerThread backgroundThread;
        private Handler backgroundHandler;
        private ImageReader imageReader;
        private CameraCaptureSession captureSession;
        private CameraDevice cameraDevice;

        private CaptureRequest.Builder previewRequestBuilder;
        private CaptureRequest previewRequest;

        private Semaphore cameraLock = new Semaphore(1);
        private int sensorOrientation;

        private int camFacing = (int)LensFacing.Back;

        private Handler handler = new Handler(Looper.MainLooper);

        private int preferredWidth = 1280;
        private int preferredHeight = 720;

        private bool usbCamera = false;

        public void switchCamera()
        {
            if (camFacing == (int)LensFacing.Back)
            {
                camFacing = (int)LensFacing.Front;
            }
            else
            {
                camFacing = (int)LensFacing.Back;
            }
            //        openCamera(textureView.Width, textureView.Height);
            stop();
            //    handler.postDelayed(new Runnable()
            //    {


            //    public void run()
            //    {
            //        start();
            //    }
            //}, 800);
        }


        public void start()
        {
            startBackgroundThread();
            //        TextureView textureView = previewView.getTextureView();
            //        if (!textureView.isAvailable()) {
            //            textureView.setSurfaceTextureListener(mS);
            //        } else {
            //            startPreview();
            //        }
            openCamera(preferredWidth, preferredHeight);
        }


        public void stop()
        {
            if (imageReader != null)
            {
                imageReader.Close();
                closeCamera();
                stopBackgroundThread();
                imageReader = null;
            }
        }


        public void pause()
        {
            //        setFlashMode(FLASH_MODE_OFF);
        }


        public void resume()
        {
            state = STATE_PREVIEW;
        }


        public void setOnFrameListener(OnFrameListener<T> listener)
        {
            this.onFrameListener = listener;
        }


        public void setPreferredPreviewSize(int width, int height)
        {
            this.preferredWidth = Java.Lang.Math.Max(width, height);
            this.preferredHeight = Java.Lang.Math.Min(width, height);
        }

        private OnFrameListener<T> onFrameListener;


        public View getDisplayView()
        {
            return textureView;
        }

        private PreviewView previewView;


        public void setPreviewView(PreviewView previewView)
        {
            this.previewView = previewView;
            textureView = previewView.getTextureView();
            if (surfaceTexture != null)
            {
                surfaceTexture.DetachFromGLContext();
                textureView.SurfaceTexture = surfaceTexture;
            }
            //textureView.SurfaceTextureListener(surfaceTextureListener);
        }


        public PreviewView getPreviewView()
        {
            return previewView;
        }


        public Rect getPreviewFrame()
        {
            return null;
        }



        public void setPermissionCallback(PermissionCallback callback)
        {
            this.permissionCallback = callback;
        }


        public void setDisplayOrientation(int displayOrientation)
        {
            this.orientation = displayOrientation / 90;
        }


        public void refreshPermission()
        {
            openCamera(preferredWidth, preferredHeight);
        }



        public int getFlashMode()
        {
            return flashMode;
        }


        public void setCameraFacing(int cameraFacing)
        {
            camFacing = cameraFacing == cameraFacing ? (int)LensFacing.Front :
                    (int)LensFacing.Back;
        }

        public Camera2Control(Context activity)
        {
            this.context = activity;
        }

        //    private  TextureView.SurfaceTextureListener surfaceTextureListener =
        //            new TextureView.SurfaceTextureListener()
        //            {


        //                    public void onSurfaceTextureAvailable(SurfaceTexture texture, int width, int height)
        //    {
        //        openCamera(width, height);
        //    }


        //    public void onSurfaceTextureSizeChanged(SurfaceTexture texture, int width, int height)
        //    {
        //        configureTransform(width, height);
        //    }


        //    public boolean onSurfaceTextureDestroyed(SurfaceTexture texture)
        //    {
        //        stop();
        //        return false;
        //    }


        //    public void onSurfaceTextureUpdated(SurfaceTexture texture)
        //    {
        //        final Bitmap bitmap = previewView.getTextureView().getBitmap();
        //        if (onFrameListener != null && bitmap != null)
        //        {
        //            onFrameListener.onPreviewFrame(bitmap, 0, bitmap.Width, bitmap.Height);
        //        }
        //    }
        //};

        private void openCamera(int width, int height)
        {
            // 6.0+的系统需要检查系统权限 。
            if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera)
                    != Permission.Granted)
            {
                requestCameraPermission();
                return;
            }
            setUpCameraOutputs(width, height);
            configureTransform(width, height);
            CameraManager manager = (CameraManager)context.GetSystemService(Context.CameraService);
            try
            {
                if (!cameraLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                //manager.OpenCamera("0", deviceStateCallback, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
        }

        //    private  CameraDevice.StateCallback deviceStateCallback = new CameraDevice.StateCallback()
        //    {


        //    public void onOpened(@NonNull CameraDevice cameraDevice)
        //    {
        //        cameraLock.release();
        //        Camera2Control.this.cameraDevice = cameraDevice;
        //        createCameraPreviewSession();
        //    }


        //    public void onDisconnected(@NonNull CameraDevice cameraDevice)
        //    {
        //        cameraLock.release();
        //        cameraDevice.close();
        //        Camera2Control.this.cameraDevice = null;
        //    }


        //    public void onError(@NonNull CameraDevice cameraDevice, int error)
        //    {
        //        cameraLock.release();
        //        cameraDevice.close();
        //        Camera2Control.this.cameraDevice = null;
        //    }
        //};

        private void createCameraPreviewSession()
        {
            try
            {
                if (surfaceTexture == null)
                {
                    surfaceTexture = new SurfaceTexture(11); // TODO
                }

                if (textureView != null)
                {
                    //    handler.post(new Runnable()
                    //    {


                    //    public void run()
                    //    {
                    //        try
                    //        {
                    //            surfaceTexture.detachFromGLContext();
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            e.printStackTrace();
                    //        }
                    //        if (textureView.getSurfaceTexture() != surfaceTexture)
                    //        {
                    //            textureView.setSurfaceTexture(surfaceTexture);
                    //        }
                    //    }
                    //});
                }

                Surface surface = new Surface(surfaceTexture);
                int rotation = ORIENTATIONS.Get(orientation);
                if (rotation % 180 == 90)
                {
                    surfaceTexture.SetDefaultBufferSize(preferredWidth, preferredHeight);
                }
                else
                {
                    surfaceTexture.SetDefaultBufferSize(preferredHeight, preferredWidth);
                }
                previewRequestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                previewRequestBuilder.AddTarget(surface);

                imageReader = ImageReader.NewInstance(preferredWidth, preferredHeight, ImageFormat.Yuv420888, 2);
                //    imageReader.SetOnImageAvailableListener(new OnImageAvailableListener()
                //    {


                //    public void onImageAvailable(ImageReader reader)
                //    {
                //    }
                //}, backgroundHandler);

                previewRequestBuilder.AddTarget(imageReader.Surface);

                updateFlashMode(this.flashMode, previewRequestBuilder);

                //    cameraDevice.createCaptureSession(Arrays.asList(surface, imageReader.getSurface()),
                //                new CameraCaptureSession.StateCallback()
                //                {




                //            public void onConfigured(@NonNull CameraCaptureSession cameraCaptureSession)
                //    {
                //        // The camera is already closed
                //        if (null == cameraDevice)
                //        {
                //            return;
                //        }
                //        captureSession = cameraCaptureSession;
                //        try
                //        {
                //            previewRequestBuilder.set(CaptureRequest.CONTROL_AF_MODE,
                //                    CaptureRequest.CONTROL_AF_MODE_CONTINUOUS_PICTURE);

                //            previewRequest = previewRequestBuilder.build();
                //            captureSession.setRepeatingRequest(previewRequest,
                //                    captureCallback, backgroundHandler);
                //        }
                //        catch (CameraAccessException e)
                //        {
                //            e.printStackTrace();
                //        }
                //    }


                //    public void onConfigureFailed(@NonNull CameraCaptureSession session)
                //    {
                //        Log.e("xx", "onConfigureFailed" + session);
                //    }
                //}, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }




        public int getCameraDisplayOrientation(Context context, int degrees, int cameraId, Android.Hardware.Camera camera)
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

        private byte[] YUV_420_888_toRGBIntrinsics(Image image)
        {

            if (image == null) return null;

            long starttime = DateTime.Now.Millisecond;
            int W = image.Width;
            int H = image.Height;

            Image.Plane Y = image.GetPlanes()[0];
            Image.Plane U = image.GetPlanes()[1];
            Image.Plane V = image.GetPlanes()[2];

            int Yb = Y.Buffer.Remaining();
            int Ub = U.Buffer.Remaining();
            int Vb = V.Buffer.Remaining();

            byte[] data = new byte[Yb + Ub + Vb];


            Y.Buffer.Get(data, 0, Yb);
            V.Buffer.Get(data, Yb, Vb);
            U.Buffer.Get(data, Yb + Vb, Ub);



            //        RenderScript rs = RenderScript.create(context);
            //        ScriptIntrinsicYuvToRGB yuvToRgbIntrinsic = ScriptIntrinsicYuvToRGB.create(rs, Element.U8_4(rs));
            //
            //        Type.Builder yuvType = new Type.Builder(rs, Element.U8(rs)).setX(data.length);
            //        Allocation in = Allocation.createTyped(rs, yuvType.create(), Allocation.USAGE_SCRIPT);
            //
            //        Type.Builder rgbaType = new Type.Builder(rs, Element.RGBA_8888(rs)).setX(W).setY(H);
            //        Allocation out = Allocation.createTyped(rs, rgbaType.create(), Allocation.USAGE_SCRIPT);
            //
            //        in.copyFromUnchecked(data);
            //
            //        yuvToRgbIntrinsic.setInput(in);
            //        yuvToRgbIntrinsic.forEach(out);
            //        int[] argb = new int[data.length];
            //        byte[] argb = new byte[image.Width *image.Height * 4 ];
            //        out.copyTo(argb);

            Log.Error("wtf", "YUV_420_888_toRGBIntrinsics->" + (DateTime.Now.Millisecond - starttime));
            return data;
        }


        public static byte[] yuvImageToByteArray(Image image)
        {

            //assert(image.Format == ImageFormat.Yuv420888);

            int width = image.Width;
            int height = image.Height;

            Image.Plane[] planes = image.GetPlanes();
            byte[] result = new byte[width * height * 3 / 2];

            int stride = planes[0].RowStride;
            if (stride == width)
            {
                planes[0].Buffer.Get(result, 0, width);
            }
            else
            {
                for (int row = 0; row < height; row++)
                {
                    planes[0].Buffer.Position(row * stride);
                    planes[0].Buffer.Get(result, row * width, width);
                }
            }

            stride = planes[1].RowStride;
            //assert(stride == planes[2].RowStride);
            byte[] rowBytesCb = new byte[stride];
            byte[] rowBytesCr = new byte[stride];

            for (int row = 0; row < height / 2; row++)
            {
                int rowOffset = width * height + width / 2 * row;
                planes[1].Buffer.Position(row * stride);
                planes[1].Buffer.Get(rowBytesCb, 0, width / 2);
                planes[2].Buffer.Position(row * stride);
                planes[2].Buffer.Get(rowBytesCr, 0, width / 2);

                for (int col = 0; col < width / 2; col++)
                {
                    result[rowOffset + col * 2] = rowBytesCr[col];
                    result[rowOffset + col * 2 + 1] = rowBytesCb[col];
                }
            }
            return result;
        }

        //    private CameraCaptureSession.CaptureCallback captureCallback =
        //            new CameraCaptureSession.CaptureCallback()
        //            {
        //                    private void process(CaptureResult result)
        //    {
        //        switch (state)
        //        {
        //            case STATE_PREVIEW:
        //                {
        //                    break;
        //                }
        //            case STATE_WAITING_FOR_LOCK:
        //                {
        //                    Integer afState = result.get(CaptureResult.CONTROL_AF_STATE);
        //                    if (afState == null || afState == CaptureResult.CONTROL_AF_STATE_PASSIVE_SCAN)
        //                    {
        //                        captureStillPicture();
        //                    }
        //                    else if (CaptureResult.CONTROL_AF_STATE_FOCUSED_LOCKED == afState
        //                          || CaptureRequest.CONTROL_AF_STATE_INACTIVE == afState
        //                          || CaptureResult.CONTROL_AF_STATE_NOT_FOCUSED_LOCKED == afState
        //                          || CaptureResult.CONTROL_AF_STATE_PASSIVE_FOCUSED == afState)
        //                    {
        //                        Integer aeState = result.get(CaptureResult.CONTROL_AE_STATE);
        //                        if (aeState == null
        //                                || aeState == CaptureResult.CONTROL_AE_STATE_CONVERGED)
        //                        {
        //                            captureStillPicture();
        //                        }
        //                        else
        //                        {
        //                            runPreCaptureSequence();
        //                        }
        //                    }
        //                    break;
        //                }
        //            case STATE_WAITING_FOR_CAPTURE:
        //                {
        //                    Integer aeState = result.get(CaptureResult.CONTROL_AE_STATE);
        //                    if (aeState == null
        //                            || aeState == CaptureResult.CONTROL_AE_STATE_PRECAPTURE
        //                            || aeState == CaptureRequest.CONTROL_AE_STATE_FLASH_REQUIRED)
        //                    {
        //                        state = STATE_CAPTURING;
        //                    }
        //                    else
        //                    {
        //                        if (aeState == CaptureResult.CONTROL_AE_STATE_CONVERGED)
        //                        {
        //                            captureStillPicture();
        //                        }
        //                    }
        //                    break;
        //                }
        //            case STATE_CAPTURING:
        //                {
        //                    Integer aeState = result.get(CaptureResult.CONTROL_AE_STATE);
        //                    if (aeState == null || aeState != CaptureResult.CONTROL_AE_STATE_PRECAPTURE)
        //                    {
        //                        captureStillPicture();
        //                    }
        //                    break;
        //                }
        //            default:
        //                break;
        //        }
        //    }


        //    public void onCaptureProgressed(@NonNull CameraCaptureSession session,
        //                                    @NonNull CaptureRequest request,
        //                                    @NonNull CaptureResult partialResult)
        //    {
        //        process(partialResult);
        //    }


        //    public void onCaptureCompleted(@NonNull CameraCaptureSession session,
        //                                   @NonNull CaptureRequest request,
        //                                   @NonNull TotalCaptureResult result)
        //    {
        //        process(result);
        //    }

        //};

        private Size getOptimalSize(Size[] choices, int textureViewWidth,
                                    int textureViewHeight, int maxWidth, int maxHeight, Size aspectRatio)
        {
            List<Size> bigEnough = new List<Size>();
            List<Size> notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;
            foreach (Size option in choices)
            {
                if (option.Width <= maxWidth && option.Height <= maxHeight && option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth && option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }


            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            //if (bigEnough.Count > 0)
            //{
            //    return Collections.Min(bigEnough, sizeComparator);
            //}

            foreach (Size option in choices)
            {
                if (option.Width > maxWidth && option.Height > maxHeight)
                {
                    return option;
                }
            }


            //if (notBigEnough.Count > 0)
            //{
            //    return Collections.Max(notBigEnough, sizeComparator);
            //}

            return choices[0];
        }

        //    private Comparator<Size> sizeComparator = new Comparator<Size>()
        //    {


        //    public int compare(Size lhs, Size rhs)
        //    {
        //        return Long.signum((long)lhs.Width * lhs.Height - (long)rhs.Width * rhs.Height);
        //    }
        //};

        private void requestCameraPermission()
        {
            if (permissionCallback != null)
            {
                permissionCallback.onRequestPermission();
            }
        }

        private void setUpCameraOutputs(int width, int height)
        {
            CameraManager manager = (CameraManager)context.GetSystemService(Context.CameraService);
            try
            {
                //for (string cameraId : manager.getCameraIdList())
                //{
                //    CameraCharacteristics characteristics =
                //            manager.getCameraCharacteristics(cameraId);

                //    Integer facing = characteristics.get(CameraCharacteristics.LENS_FACING);
                //    if (facing != null && facing == camFacing)
                //    {
                //        continue;
                //    }

                //    //StreamConfigurationMap map = characteristics.get(
                //    //        CameraCharacteristics.SCALER_STREAM_CONFIGURATION_MAP);
                //    //if (map == null)
                //    //{
                //    //    continue;
                //    //}

                //    //WindowManager windowManager = (WindowManager)context.getSystemService(Context.WINDOW_SERVICE);
                //    //Point screenSize = new Point();
                //    //windowManager.getDefaultDisplay().getSize(screenSize);
                //    //int maxImageSize = Math.max(MAX_PREVIEW_SIZE, screenSize.y * 2 / 3);

                //    //Size size = getOptimalSize(map.getOutputSizes(ImageFormat.JPEG), textureView.Width,
                //    //        textureView.Height, maxImageSize, maxImageSize, new Size(4, 3));

                //    //int displayRotation = orientation;
                //    //// noinspection ConstantConditions
                //    //sensorOrientation = characteristics.get(CameraCharacteristics.SENSOR_ORIENTATION);
                //    //boolean swappedDimensions = false;
                //    //switch (displayRotation)
                //    //{
                //    //    case Surface.ROTATION_0:
                //    //    case Surface.ROTATION_180:
                //    //        if (sensorOrientation == 90 || sensorOrientation == 270)
                //    //        {
                //    //            swappedDimensions = true;
                //    //        }
                //    //        break;
                //    //    case Surface.ROTATION_90:
                //    //    case Surface.ROTATION_270:
                //    //        if (sensorOrientation == 0 || sensorOrientation == 180)
                //    //        {
                //    //            swappedDimensions = true;
                //    //        }
                //    //        break;
                //    //    default:
                //    //}
                //    ////                orientation = sensorOrientation;

                //    //int rotatedPreviewWidth = width;
                //    //int rotatedPreviewHeight = height;
                //    //int maxPreviewWidth = screenSize.x;
                //    //int maxPreviewHeight = screenSize.y;

                //    //if (swappedDimensions)
                //    //{
                //    //    rotatedPreviewWidth = height;
                //    //    rotatedPreviewHeight = width;
                //    //    maxPreviewWidth = screenSize.y;
                //    //    maxPreviewHeight = screenSize.x;
                //    //}

                //    //maxPreviewWidth = Math.min(maxPreviewWidth, MAX_PREVIEW_WIDTH);
                //    //maxPreviewHeight = Math.min(maxPreviewHeight, MAX_PREVIEW_HEIGHT);

                //    //previewSize = getOptimalSize(map.getOutputSizes(SurfaceTexture.class),
                //    //    rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                //    //    maxPreviewHeight, size);
                //    this.cameraId = cameraId;

                //    return;
                //}
            }
            catch (Java.Lang.Exception e) when (e is CameraAccessException || e is System.NullReferenceException)
            {
                e.PrintStackTrace();
            }
        }

        private void closeCamera()
        {
            try
            {
                cameraLock.Acquire();
                if (null != captureSession)
                {
                    captureSession.Close();
                    captureSession = null;
                }
                if (null != cameraDevice)
                {
                    cameraDevice.Close();
                    cameraDevice = null;
                }
                if (null != imageReader)
                {
                    imageReader.Close();
                    imageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                cameraLock.Release();
            }
        }

        private void startBackgroundThread()
        {
            backgroundThread = new HandlerThread("ocr_camera");
            backgroundThread.Start();
            backgroundHandler = new Handler(backgroundThread.Looper);//这个是属性。。。  有些地方就是这么坑 
        }

        private void stopBackgroundThread()
        {
            if (backgroundThread != null)
            {
                backgroundThread.QuitSafely();
                backgroundThread = null;
                backgroundHandler = null;
            }
        }

        private Matrix matrix = new Matrix();
        private CameraCaptureSession.CaptureCallback captureCallback;

        private void configureTransform(int viewWidth, int viewHeight)
        {
            if (null == textureView || null == previewSize || null == context)
            {
                return;
            }
            int rotation = orientation;

            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, previewSize.Height, previewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            //if ( Surface.ROTATION_90 == rotation || Surface.ROTATION_270 == rotation)
            //{
            //    bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
            //    matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
            //    float scale = Java.Lang.Math.Max(
            //            (float)viewHeight / previewSize.Height,
            //            (float)viewWidth / previewSize.Width);
            //    matrix.PostScale(scale, scale, centerX, centerY);
            //    matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            //}
            //else if (Surface.ROTATION_180 == rotation)
            //{
            //    matrix.PostRotate(180, centerX, centerY);
            //}
            textureView.SetTransform(matrix);
        }

        // 拍照前，先对焦
        private void lockFocus()
        {
            if (captureSession != null && state == STATE_PREVIEW)
            {
                try
                {
                    //previewRequestBuilder.Set(CaptureRequest.ControlAfTrigger,
                    //       ControlAFTrigger.Start;
                    state = STATE_WAITING_FOR_LOCK;
                    captureSession.Capture(previewRequestBuilder.Build(), captureCallback,
                            backgroundHandler);
                }
                catch (CameraAccessException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        private void runPreCaptureSequence()
        {
            try
            {
                //previewRequestBuilder.Set(ControlAEPrecaptureTrigger.Cancel,
                //        ControlAEPrecaptureTrigger.Cancel);
                state = STATE_WAITING_FOR_CAPTURE;
                captureSession.Capture(previewRequestBuilder.Build(), captureCallback,
                        backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // 拍照session
        private void captureStillPicture()
        {
            try
            {
                if (null == context || null == cameraDevice)
                {
                    return;
                }
                CaptureRequest.Builder captureBuilder =
                       cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                //captureBuilder.Set(CaptureRequest.ControlAfMode,
                //        CaptureRequest.CONTROL_AF_MODE_CONTINUOUS_PICTURE);

                captureBuilder.Set(CaptureRequest.JpegOrientation, getOrientation(orientation));
                updateFlashMode(this.flashMode, captureBuilder);
                //    CameraCaptureSession.CaptureCallback captureCallback =
                //            new CameraCaptureSession.CaptureCallback()
                //            {



                //            public void onCaptureCompleted(@NonNull CameraCaptureSession session,
                //                                           @NonNull CaptureRequest request,
                //                                           @NonNull TotalCaptureResult result)
                //    {
                //        unlockFocus();
                //    }
                //};

                //// 停止预览
                //captureSession.stopRepeating();
                //captureSession.capture(captureBuilder.build(), captureCallback, backgroundHandler);
                //state = STATE_PICTURE_TAKEN;
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private int getOrientation(int rotation)
        {
            return (ORIENTATIONS.Get(rotation) + sensorOrientation + 270) % 360;
        }

        // 停止对焦
        private void unlockFocus()
        {
            try
            {
               // previewRequestBuilder.Set(CaptureRequest.ControlAfTrigger,ControlAFTrigger.Cancel);
                captureSession.Capture(previewRequestBuilder.Build(), captureCallback,
                        backgroundHandler);
                state = STATE_PREVIEW;
                // 预览
                captureSession.SetRepeatingRequest(previewRequest, captureCallback,
                        backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private void updateFlashMode(int flashMode, CaptureRequest.Builder builder)
        {
            //switch (flashMode)
            //{
            //    case 1:
            //        builder.Set(flashMode,FlashMode.Torch);
            //        break;
            //    case 0:
            //        builder.Set(CaptureRequest.FLASH_MODE, FlashMode.Off);
            //        break;
            //    case 2:
            //    default:
            //        builder.Set(CaptureRequest.FLASH_MODE, FlashMode.Single);
            //        break;
            //}
        }
    }
}