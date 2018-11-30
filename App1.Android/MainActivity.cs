using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Baidu.Idl.Facesdk;
using Android.Content;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Context context = null;
        public MainActivity()
        {
        }
        public MainActivity(Context _context)
        {
            context = _context;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FaceSDK.InitModel(this);
            FaceSDK.GetARGBFromYUVimg();
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
    }
}