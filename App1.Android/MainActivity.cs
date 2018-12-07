using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Baidu.Idl.Facesdk;
using Android.Content;
using App1.Droid.BaiduSDKManager;
using App1.Droid.BaiduSDKManager.Manager;
using App1.Droid.Activitys;
using Java.Util.Logging;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        public MainActivity()
        {
        }
        public MainActivity(Context _context)
        {
           
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FaceSDKManager.init(this);
            FaceSDKManager.getInstance().setSdkInitListener(new SdkInitListener(this));
            var a = FaceSDKManager.initStatus;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            RgbDetectActivity rgbDetectActivity = new RgbDetectActivity();
            rgbDetectActivity.ini();
        }
    }
}