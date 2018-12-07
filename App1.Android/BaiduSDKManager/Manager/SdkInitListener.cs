using System;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;

namespace App1.Droid.BaiduSDKManager.Manager
{
    public class SdkInitListener : FaceSDKManager.SdkInitListener
    {
        private readonly Context context;
        public SdkInitListener() { }
        public SdkInitListener(Context _context)
        {
            context = _context;
        }

        private Handler handler = new Handler(Looper.MainLooper);

        public  void initFail(int errorCode, string msg)
        {
            Toast.MakeText(context, "sdk init fail"+msg, ToastLength.Long).Show();
        }

        public void initStart()
        {
            Toast.MakeText(context, "sdk init Start", ToastLength.Long).Show();
        }

        public void initSuccess()
        {
            Toast.MakeText(context, "sdk init Success", ToastLength.Long).Show();
        }
    }
}
