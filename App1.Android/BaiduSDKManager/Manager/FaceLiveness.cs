using Android.Util;
using Com.Baidu.Idl.Facesdk;
using Java.Util.Concurrent;
using System;
using Java.IO;
using Environment = Android.OS.Environment;
using Android.Graphics;
using File = Java.IO.File;
using static Android.Renderscripts.ScriptGroup;
using App1.Droid.BaiduSDKManager.CallBack;
using App1.Droid.BaiduSDKManager.Entity;

namespace App1.Droid.BaiduSDKManager.Manager
{
    public class FaceLiveness
    {

        private static readonly string TAG = "FaceLiveness";
        public static readonly int MASK_RGB = 0X0001;
        public static readonly int MASK_IR = 0X0010;
        public static readonly int MASK_DEPTH = 0X0100;

        public static FaceLiveness getInstance()
        {
            return FaceLiveness.HolderClass.instance;
        }

        private static class HolderClass
        {
            public static readonly FaceLiveness instance = new FaceLiveness();
        }

        private FaceLiveness()
        {
            es = Executors.NewSingleThreadExecutor();
        }

        private Bitmap bitmap;
        private ILivenessCallBack livenessCallBack;

        private int[] nirRgbArray;

        private int[] mRgbArray;
        private volatile bool isVisHavePixls = false;

        private byte[] mIrByte;
        private volatile bool isIRHavePixls = false;

        private byte[] mDepthArray;
        private volatile bool isDepthHavePixls;
        private IExecutorService es;
        private IFuture future;

        public void setLivenessCallBack(ILivenessCallBack callBack)
        {
            this.livenessCallBack = callBack;
        }

        /**
         * 设置可见光图
         *
         * @param bitmap
         */
        public void setRgbBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                if (mRgbArray == null)
                {
                    mRgbArray = new int[bitmap.Width * bitmap.Height];
                }
                bitmap.GetPixels(mRgbArray, 0, bitmap.Width, 0, 0,
                        bitmap.Width, bitmap.Height);
                this.bitmap = bitmap;
                isVisHavePixls = true;
            }
        }

        public void setNirRgbInt(int[] argbData)
        {
            if (nirRgbArray == null)
            {
                nirRgbArray = new int[argbData.Length];
            }
            Array.Copy(argbData, 0, nirRgbArray, 0, argbData.Length);
            isVisHavePixls = true;
        }

        public void setRgbInt(int[] argbData)
        {
            if (mRgbArray == null)
            {
                mRgbArray = new int[argbData.Length];
            }
            Array.Copy(argbData, 0, mRgbArray, 0, argbData.Length);
            isVisHavePixls = true;
        }

        private int[] byte2int(byte[] b)
        {
            // 数组长度对4余数
            int r;
            byte[] copy;
            if ((r = b.Length % 4) != 0)
            {
                copy = new byte[b.Length - r + 4];
                Array.Copy(b, 0, copy, 0, b.Length);
            }
            else
            {
                copy = b;
            }

            int[] x = new int[copy.Length / 4 + 1];
            int pos = 0;

           
            for (int i = 0; i < x.Length - 1; i++)
            {
                x[i] = (int)(copy[pos] << 24 & 0xff000000) | (copy[pos + 1] << 16 & 0xff0000)
                        | (copy[pos + 2] << 8 & 0xff00) | (copy[pos + 3] & 0xff);
                pos += 4;
            
            }
            x[x.Length - 1] = r;
            return x;
        }

        /**
         * 设置深度图
         *
         * @param irData
         */
        public void setIrData(byte[] irData)
        {

            if (irData == null)
            {
                return;
            }
            if (mIrByte == null)
            {
                mIrByte = new byte[irData.Length];
            }

           Array.Copy(irData, 0, mIrByte, 0, irData.Length);
            isIRHavePixls = true;
        }


        /**
         * 设置深度图
         *
         * @param depthData
         */
        public void setDepthData(byte[] depthData)
        {

            if (mDepthArray == null)
            {
                mDepthArray = new byte[depthData.Length];
            }

            Array.Copy(depthData, 0, mDepthArray, 0, depthData.Length);
            isDepthHavePixls = true;
        }


        public void clearInfo()
        {
            try
            {
                isDepthHavePixls = false;
                isIRHavePixls = false;
                isVisHavePixls = false;

            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public float rgbLiveness(int[] data, int width, int height, int[] landmarks)
        {
            float rgbScore = FaceSDK.Run_livenessSilentPredict(FaceSDK.LivenessTypeId.LiveidVis, data,
                   height, width, 24, landmarks);
            return rgbScore;
        }

        public float irLiveness(byte[] data, int width, int height, int[] landmarks)
        {
            float irScore = FaceSDK.Run_livenessSilentPredictByte(FaceSDK.LivenessTypeId.LiveidIr, data,
                   height, width, 0, landmarks);
            return irScore;
        }

        public float depthLiveness(byte[] data, int width, int height, int[] landmarks)
        {
            float depthScore = FaceSDK.Run_livenessSilentPredictByte(FaceSDK.LivenessTypeId.LiveidDepth, data,
                   height, width, 2, landmarks);
            return depthScore;
        }

        public void livenessCheck(int width, int height, int type)
        {

            if (future != null && !future.IsDone)
            {
                return;
            }
            //future = es.submit(new Runnable() { 
            //    public override void run()
            //    {
            //        // Thread.currentThread().setPriority(Thread.MAX_PRIORITY);
            //        onLivenessCheck(width, height, type);
            //    }
            //    });
        }

        public void livenessCheck(int width, int height, int type, int[] rgb, byte[] ir, byte[] depth)
        {
            if (future != null && !future.IsDone)
            {
                return;
            }

            setDepthData(depth);
            setRgbInt(rgb);
            setIrData(ir);

            //    future = es.submit(new Runnable() {
            //        @Override
            //        public void run()
            //    {
            //        onLivenessCheck(width, height, type);
            //    }
            //});
        }


        // 活体检测
        private bool onLivenessCheck(int width, int height, int type)
        {
            bool isLiveness = false;
            // 判断当前是否有人脸
            long startTime = DateTime.Now.Millisecond;
            int errorCode = FaceSDKManager.getInstance().getFaceDetector().detect(mRgbArray, width, height);
            
            LivenessModel livenessModel = new LivenessModel();
            livenessModel.setRgbDetectDuration(DateTime.Now.Millisecond - startTime);
            livenessModel.getImageFrame().setArgb(mRgbArray);
            livenessModel.getImageFrame().setWidth(width);
            livenessModel.getImageFrame().setHeight(height);
            livenessModel.setLiveType(type);
            livenessModel.setFaceDetectCode(errorCode);
            Log.Debug(TAG, "max_face_verification: " + errorCode + " duration:" + (DateTime.Now.Millisecond - startTime));

            if (errorCode == FaceTracker.ErrCode.Ok.Ordinal() || errorCode == FaceTracker.ErrCode.DataHitLast.Ordinal())
            {
                FaceInfo[] trackedface = FaceSDKManager.getInstance().getFaceDetector().TrackedFaces;
                livenessModel.setTrackFaceInfo(trackedface);
                if (trackedface != null && trackedface.Length > 0)
                {
                    FaceInfo faceInfo = trackedface[0];
                    livenessModel.setFaceInfo(faceInfo);

                    // 塞选人脸，可以调节距离、角度
                    //                if (!filter(faceInfo, width, height)) {
                    //                    livenessCallBack.onCallback(null);
                    //                    return isLiveness;
                    //                }
                    if (livenessCallBack != null)
                    {
                        livenessCallBack.onTip(0, "活体判断中");
                    }
                    float rgbScore = 0;
                    if ((type & MASK_RGB) == MASK_RGB)
                    {
                        startTime = DateTime.Now.Millisecond;
                        rgbScore = rgbLiveness(mRgbArray, width, height, FaceFeature.PerformQuery<int>(trackedface[0].Landmarks));
                        livenessModel.setRgbLivenessScore(rgbScore);
                        livenessModel.setRgbLivenessDuration(DateTime.Now.Millisecond - startTime);
                    }
                    float irScore = 0;
                    if ((type & MASK_IR) == MASK_IR)
                    {
                        float maxWidth = 0;
                        int maxId = 0;
                        float detectScore = 0;
                        if (trackedface != null && trackedface.Length > 0)
                        {
                            for (int i = 0; i < trackedface.Length; i++)
                            {
                                if (trackedface[i].MWidth > maxWidth)
                                {
                                    maxId = i;
                                    maxWidth = trackedface[i].MWidth;
                                    detectScore = trackedface[i].MConf;
                                }
                            }
                        }
                        if (trackedface != null)
                        {
                            float[] faceT = new float[]{trackedface[maxId].MCenterX,
                                trackedface[maxId].MCenterY, trackedface[maxId].MWidth,
                                trackedface[maxId].MAngle};
                            int[] shape = new int[144];
                            int[] nPoint = new int[] { 0 };
                            float[] score = new float[] { 0.0F };
                            FaceSDK.Run_align(nirRgbArray, height, width, FaceSDK.ImgType.Argb,
                                    FaceSDK.AlignMethodType.Cdnn, faceT, shape, nPoint, score, detectScore);
                            livenessModel.setShape(shape);
                            startTime = DateTime.Now.Millisecond;
                            //                    irScore = irLiveness(mIrByte, width, height, trackedfaces[0].landmarks);
                            irScore = irLiveness(mIrByte, width, height, shape);
                            livenessModel.setIrLivenessDuration(DateTime.Now.Millisecond - startTime);
                            livenessModel.setIrLivenessScore(irScore);
                        }
                    }
                    float depthScore = 0;
                    if ((type & MASK_DEPTH) == MASK_DEPTH)
                    {
                        startTime = DateTime.Now.Millisecond;
                        if (trackedface != null)
                        {
                            depthScore = depthLiveness(mDepthArray, width, height, FaceFeature.PerformQuery<int>(trackedface[0].Landmarks));
                            livenessModel.setDetphtLivenessDuration(DateTime.Now.Millisecond - startTime);
                            livenessModel.setDepthLivenessScore(depthScore);
                        }
                    }
                    if (livenessCallBack != null)
                    {
                        livenessCallBack.onCallback(livenessModel);
                    }

                    //                long time = System.currentTimeMillis();
                    //                saveRgbImage(String.valueOf(time), rgbScore, mRgbArray, width, height);
                    //                saveFile(String.valueOf(time), "nir", irScore, mIrByte);
                    //                saveFile(String.valueOf(time), "depth", depthScore, mDepthArray);
                }
            }
            else
            {
                checkFaceCode(errorCode);
                if (livenessCallBack != null)
                {
                    livenessCallBack.onCallback(null);
                }
            }
            // clearInfo();
            FaceInfo[] trackedfaces = FaceSDKManager.getInstance().getFaceDetector().TrackedFaces;
            livenessModel.setTrackFaceInfo(trackedfaces);
            if (livenessCallBack != null)
            {
                livenessCallBack.onCanvasRectCallback(livenessModel);
            }
            return isLiveness;
        }

        private bool filter(FaceInfo faceInfo, int bitMapWidth, int bitMapHeight)
        {

            if (faceInfo.MConf < 0.6)
            {
                livenessCallBack.onTip(0, "人脸置信度太低");
                // clearInfo();
                return false;
            }
            float[] headPose = FaceFeature.PerformQuery<float>(faceInfo.HeadPose);
            // Log.i("wtf", "headpose->" + headPose[0] + " " + headPose[1] + " " + headPose[2]);
            if (Java.Lang.Math.Abs(headPose[0]) > 15 || Java.Lang.Math.Abs(headPose[1]) > 15 || Java.Lang.Math.Abs(headPose[2]) > 15)
            {
                livenessCallBack.onTip(0, "人脸置角度太大，请正对屏幕");
                return false;
            }

            // 判断人脸大小，若人脸超过屏幕二分一，则提示文案“人脸离手机太近，请调整与手机的距离”；
            // 若人脸小于屏幕三分一，则提示“人脸离手机太远，请调整与手机的距离”
            float ratio = (float)faceInfo.MWidth / (float)bitMapHeight;
            // Log.i("liveness_ratio", "ratio=" + ratio);
            if (ratio > 0.6)
            {
                livenessCallBack.onTip(0, "人脸离屏幕太近，请调整与屏幕的距离");
                // clearInfo();
                return false;
            }
            else if (ratio < 0.2)
            {
                livenessCallBack.onTip(0, "人脸离屏幕太远，请调整与屏幕的距离");
                // clearInfo();
                return false;
            }
            else if (faceInfo.MCenterX > bitMapWidth * 3 / 4)
            {
                livenessCallBack.onTip(0, "人脸在屏幕中太靠右");
                clearInfo();
                return false;
            }
            else if (faceInfo.MCenterX < bitMapWidth / 4)
            {
                livenessCallBack.onTip(0, "人脸在屏幕中太靠左");
                // clearInfo();
                return false;
            }
            else if (faceInfo.MCenterY > bitMapHeight * 3 / 4)
            {
                livenessCallBack.onTip(0, "人脸在屏幕中太靠下");
                // clearInfo();
                return false;
            }
            else if (faceInfo.MCenterX < bitMapHeight / 4)
            {
                livenessCallBack.onTip(0, "人脸在屏幕中太靠上");
                // clearInfo();
                return false;
            }

            return true;
        }


        private long lasttime;
        private int unDetectedFaceCount = 0;

        private void checkFaceCode(int errCode)
        {
            if (errCode == FaceTracker.ErrCode.NoFaceDetected.Ordinal())
            {
                if (DateTime.Now.Millisecond - lasttime > 1000 || unDetectedFaceCount > 5)
                {
                    livenessCallBack.onTip(errCode, "未检测到人脸");
                    unDetectedFaceCount = 0;
                }
                unDetectedFaceCount++;
                lasttime = DateTime.Now.Millisecond;
            }
            else if (errCode == FaceTracker.ErrCode.ImgBlured.Ordinal()
                  || errCode == FaceTracker.ErrCode.PitchOutOfDownMaxRange.Ordinal()
                  || errCode == FaceTracker.ErrCode.PitchOutOfUpMaxRange.Ordinal()
                  || errCode == FaceTracker.ErrCode.YawOutOfLeftMaxRange.Ordinal()
                  || errCode == FaceTracker.ErrCode.YawOutOfRightMaxRange.Ordinal())
            {
                livenessCallBack.onTip(errCode, "请静止平视屏幕");
                unDetectedFaceCount = 0;
            }
            else if (errCode == FaceTracker.ErrCode.PoorIllumination.Ordinal())
            {
                livenessCallBack.onTip(errCode, "光线太暗，请到更明亮的地方");
                unDetectedFaceCount = 0;
            }
            else
            {
                livenessCallBack.onTip(errCode, "未检测到人脸");
                unDetectedFaceCount = 0;
            }
        }


        public void release()
        {
            if (future != null)
            {
                future.Cancel(true);
            }

        }

        private bool saveRgbImage(string prefix, float score, int[] rgb, int width, int height)
        {
            bool success = false;
            if (rgb == null)
            {
                return success;
            }
            if (!Environment.IsExternalStorageEmulated)
            {
                return success;
            }
            Java.IO.File sdCard = Environment.ExternalStorageDirectory;
            string uuid = Guid.NewGuid().ToString();
            File dir = new File(sdCard.AbsolutePath + "/rgb_ir_depth/" + uuid);
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            string rgbFilename = string.Format("%s_%s_%s.jpg", prefix, "rgb", score);
            File rgbFile = new File(dir, rgbFilename);
            if (rgbFile.Exists())
            {
                rgbFile.Delete();
            }
            System.IO.FileStream fos = null;
            try
            {
                fos = new System.IO.FileStream(rgbFile.Path,System.IO.FileMode.Open);
                Bitmap bitmap = Bitmap.CreateBitmap(rgb, width, height, Bitmap.Config.Argb8888);
                Log.Info(TAG, "strFileName 1= " + rgbFile.Path);
                if (null != fos)
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100,fos);
                    fos.Flush();
                    fos.Close();
                    success = true;
                }
            }
            catch (Java.IO.FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
            catch (Java.IO.IOException e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                    }
                    catch (Java.IO.IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            return success;
        }

        public bool saveFile(string prefix, string type, float score, byte[] data)
        {
            bool success = false;
            if (data == null)
            {
                return success;
            }
            if (!Environment.IsExternalStorageEmulated)
            {
                return success;
            }
            File sdCard = Environment.ExternalStorageDirectory;
            string uuid = Guid.NewGuid().ToString();
            File dir = new File(sdCard.AbsolutePath + "/rgb_ir_depth/" + uuid);
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }
            if (data != null)
            {
                string nirFilename = string.Format("%s_%s_%s", prefix, type, score);
                File nirFile = new File(dir, nirFilename);
                if (nirFile.Exists())
                {
                    nirFile.Delete();
                }
                FileOutputStream fos = null;
                try
                {
                    fos = new FileOutputStream(nirFile);
                    fos.Write(data, 0, data.Length);
                    fos.Flush();
                    fos.Close();
                    success = true;
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    e.PrintStackTrace();
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }
                finally
                {
                    if (fos != null)
                    {
                        try
                        {
                            fos.Close();
                        }
                        catch (Java.IO.IOException e)
                        {
                            e.PrintStackTrace();
                        }
                    }
                }
            }
            return success;
        }

    }
}
