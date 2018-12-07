using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using App1.Droid.BaiduSDKManager.Face;
using App1.Droid.BaiduSDKManager.Manager;
using Com.Baidu.Idl.Facesdk;
using static Android.Graphics.SurfaceTexture;

namespace App1.Droid.BaiduSDKManager.Face
{
    /**
 * 封装了人脸检测的整体逻辑。
 */
    public class FaceDetectManager
    {
        /**
         * 该回调用于回调，人脸检测结果。当没有人脸时，infos 为null,status为 FaceDetector.DETECT_CODE_NO_FACE_DETECTED
         */
        public static ImageFrame lastFrame { get; set; }
        public interface OnFaceDetectListener
        {
            void onDetectFace(int status, FaceInfo[] infos, ImageFrame imageFrame);
        }

        public FaceDetectManager(Context context)
        {
            face = this;
        }

        /**
         * 图片源，获取检测图片。
         */
        private static ImageSource ImageSource { get; set; }
        /**
         * 人脸检测事件监听器
         */
        private static OnFaceDetectListener Listener { get; set; }
        private static FaceFilter faceFilter = new FaceFilter();
        private HandlerThread processThread;
        private Handler processHandler;
        private Handler uiHandler;
        private static bool UseDetect { get; set; } = false;
        public static FaceDetectManager face;

        private static List<FaceProcessor> preProcessors { get; set; } = new List<FaceProcessor>();

        public void setUseDetect(bool useDetect)
        {
            UseDetect = useDetect;
            // 传给facesdk的图片高宽不同，将不能正确检出人脸，需要clear前面的trackedFaces
            FaceSDKManager.getInstance().getFaceDetector().clearTrackedFaces();
        }

        /**
         * 设置人脸检测监听器，检测后的结果会回调。
         *
         * @param listener 监听器
         */
        public void setOnFaceDetectListener(OnFaceDetectListener listener)
        {
            Listener = listener;
        }

        /**
         * 设置图片帧来源
         *
         * @param imageSource 图片来源
         */
        public void setImageSource(ImageSource imageSource)
        {
            ImageSource = imageSource;
        }

        /**
         * @return 返回图片来源
         */
        public ImageSource getImageSource()
        {
            return ImageSource;
        }


        /**
         * 增加处理回调，在人脸检测前会被回调。
         *
         * @param processor 图片帧处理回调
         */
        public void addPreProcessor(FaceProcessor processor)
        {
            preProcessors.Add(processor);
        }

        /**
         * 设置人检跟踪回调。
         *
         * @param onTrackListener 人脸回调
         */
        public void setOnTrackListener(FaceFilter.OnTrackListener onTrackListener)
        {
            faceFilter.setOnTrackListener(onTrackListener);
        }

        /**
         * @return 返回过虑器
         */
        public FaceFilter getFaceFilter()
        {
            return faceFilter;
        }

        public void start()
        {
            LogUtil.init();
            ImageSource.addOnFrameAvailableListener(onFrameAvailableListener);
            processThread = new HandlerThread("process");
            processThread.Priority = 9;
            processThread.Start();
            processHandler = new Handler(processThread.Looper);
            uiHandler = new Handler();
            ImageSource.start();
        }

        private static void Run()
        {
            Task.Run(() =>
            {
                if (lastFrame == null)
                {
                    return;
                }
                int[] argb;
                int width;
                int height;
                ArgbPool pool;
                argb = lastFrame.getArgb();
                width = lastFrame.getWidth();
                height = lastFrame.getHeight();
                pool = lastFrame.getPool();
                lastFrame = null;
                process(argb, width, height, pool);
            });
        }
       
        public void stop()
        {
            if (ImageSource != null)
            {
                ImageSource.stop();
                ImageSource.removeOnFrameAvailableListener(onFrameAvailableListener);
            }

            if (processThread != null)
            {
                processThread.Quit();
                processThread = null;
            }
        }

        bool skip = false;
       
        private static void process(int[] argb, int width, int height, ArgbPool pool)
        {
            //        if (skip) {
            //            skip = !skip;
            //            return;
            //        } else {
            //            skip = !skip;
            //        }
            int value = 0;

            ImageFrame frame = ImageSource.borrowImageFrame();
            frame.setArgb(argb);
            frame.setWidth(width);
            frame.setHeight(height);
            frame.setPool(pool);
            //        frame.retain();

            foreach (FaceProcessor processor in preProcessors)
            {
                if (processor.process(face, frame))
                {
                    break;
                }
            }
            if (UseDetect)
            {
                long starttime = DateTime.Now.Millisecond;
                value = FaceSDKManager.getInstance().getFaceDetector().detect(frame);
                // FaceSDKManager.getInstance().getFaceDetector().detectMultiFace(frame,5);
                FaceInfo[] faces = FaceSDKManager.getInstance().getFaceDetector().TrackedFaces;
                if (faces != null)
                {
                    Log.Error("faceMulti", faces.Length + "");
                }

                Log.Error("wtf", value + " process->" + (DateTime.Now.Millisecond - starttime));

                if (value == 0)
                {
                    faceFilter.filter(faces, frame);
                }
                if (Listener != null)
                {
                    Listener.onDetectFace(value, faces, frame);
                }
            }

            frame.release();
        }

        public OnFrameAvailableListener onFrameAvailableListener = new OnFrameAvailableListenerAnonymousInnerClass();
        public class OnFrameAvailableListenerAnonymousInnerClass : OnFrameAvailableListener
        {
            public OnFrameAvailableListenerAnonymousInnerClass()
            {

            }
            public void onFrameAvailable(ImageFrame imageFrame)
            {
                lastFrame = imageFrame;
                //            processHandler.removeCallbacks(processRunnable);
                //            processHandler.post(processRunnable);
                //            uiHandler.removeCallbacks(processRunnable);
                //            uiHandler.post(processRunnable);
                Run();
            }
        }

    }
    
}