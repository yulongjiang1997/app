using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Ui;
using App1.Droid.BaiduSDKManager.Utils;
using Com.Baidu.Idl.Facesdk;
using Com.Baidu.Idl.License;
using Java.Util.Concurrent;
using System;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using static App1.Droid.BaiduSDKManager.Ui.Activation;

namespace App1.Droid.BaiduSDKManager.Manager
{
    public class FaceSDKManager
    {

        public static readonly int SDK_UNACTIVATION = 1;
        public static readonly int SDK_UNINIT = 2;
        public static readonly int SDK_INITING = 3;
        public static readonly int SDK_INITED = 4;
        public static readonly int SDK_FAIL = 5;

        public static readonly String LICENSE_NAME = "idl-license.face-android";
        private static FaceDetector faceDetector;
        private static FaceFeature faceFeature;
        public static  Context Context { get; set; }
        private static SdkInitListener SsdkInitListener { get; set; }
        public static volatile int initStatus = SDK_UNACTIVATION;
        private static readonly Handler handler = new Handler(Looper.MainLooper);
        

        private FaceSDKManager()
        {
            faceDetector = new FaceDetector();
            faceFeature = new FaceFeature();
        }

        private static class HolderClass
        {
            public static readonly FaceSDKManager instance = new FaceSDKManager();
        }

        public static FaceSDKManager getInstance()
        {
            return HolderClass.instance;
        }

        public int InitStatus()
        {
            return initStatus;
        }

        public void setSdkInitListener(SdkInitListener sdkInitListener)
        {
            SsdkInitListener = sdkInitListener;
        }

        public FaceDetector getFaceDetector()
        {
            return faceDetector;
        }

        public FaceFeature getFaceFeature()
        {
            return faceFeature;
        }


        /**
         * FaceSDK 初始化，用户可以根据自己的需求实例化FaceTracker 和 FaceRecognize
         *
         * @param context
         */
        public static  void init(Context context)
        {
            Context = context;
            if (!check())
            {
                initStatus = SDK_UNACTIVATION;
                return;
            }
            PreferencesUtil.initPrefs(context.ApplicationContext);
            // final String key = "faceexample-face-android";
            string key = PreferencesUtil.getString("activate_key", "");
            if (Android.Text.TextUtils.IsEmpty(key))
            {
                Toast.MakeText(context, "激活序列号为空, 请先激活", ToastLength.Short).Show();
                return;
            }

            initStatus = SDK_INITING;
            Task.Run(() => { run(key); });
        }
        public static void run(string key)
        {
            if (SsdkInitListener != null)
            {
                SsdkInitListener.initStart();
            }
            Log.Error("FaceSDK", "初始化授权");
            FaceSDK.InitLicense(Context, key, LICENSE_NAME, false);
            if (!sdkInitStatus())
            {
                return;
            }
            Log.Error("FaceSDK", "初始化sdk");
            faceDetector.init(Context);
            faceFeature.init(Context);
            initLiveness(Context);
        }
        /**
         * 初始化 活体检测
         *
         * @param context
         */
        private static void initLiveness(Context context)
        {
            FaceSDK.LivenessSilentInit(context, FaceSDK.LivenessTypeId.LiveidVis, 2);
            FaceSDK.LivenessSilentInit(context, FaceSDK.LivenessTypeId.LiveidIr);
            FaceSDK.LivenessSilentInit(context, FaceSDK.LivenessTypeId.LiveidDepth);
        }

        private static bool sdkInitStatus()
        {
            bool success = false;
            int status = FaceSDK.AuthorityStatus;
            if (status == AndroidLicenser.ErrorCode.Success.Ordinal())
            {
                initStatus = SDK_INITED;
                success = true;
                faceDetector.InitStatus = initStatus;
                Log.Info("FaceSDK", "授权成功");
                if (SsdkInitListener != null)
                {
                    SsdkInitListener.initSuccess();
                }

            }
            else if (status == AndroidLicenser.ErrorCode.LicenseExpired.Ordinal())
            {
                initStatus = SDK_FAIL;
                // FileUitls.deleteLicense(context, LICENSE_NAME);
                Log.Info("FaceSDK", "授权过期");
                if (SsdkInitListener != null)
                {
                    SsdkInitListener.initFail(status, "授权过期");
                }
                showActivation();
            }
            else
            {
                initStatus = SDK_FAIL;
                // FileUitls.deleteLicense(context, LICENSE_NAME);
                Log.Info("FaceSDK", "授权失败" + status);
                if (SsdkInitListener != null)
                {
                    SsdkInitListener.initFail(status, "授权失败");
                }
                showActivation();
            }
            return success;
        }


        public static bool check()
        {
            if (!FileUitls.checklicense(Context, LICENSE_NAME))
            {
                showActivation();
                return false;
            }
            else
            {
                return true;
            }
        }
        

        public class ActivationCallbacka : ActivationCallback
        {
            public void callback(bool success)
            {
                initStatus = SDK_UNINIT;
                Log.Info("wtf", "activation callback");
                init(Context);
            }
        }

        public static void run(Activation activation)
        {
            activation.show();
        }


        public static void showActivation()
        {
            if (FaceSDK.AuthorityStatus == AndroidLicenser.ErrorCode.Success.Ordinal())
            {
                Toast.MakeText(Context, "已经激活成功", ToastLength.Long).Show();
                // return;
            }
            Activation activation = new Activation(Context);
            activation.setActivationCallback(new ActivationCallbacka());
            handler.Post(()=> run(activation));

        }


        public interface SdkInitListener
        {

            void initStart();

            void initSuccess();

            void initFail(int errorCode, String msg);
        }


    }
}
