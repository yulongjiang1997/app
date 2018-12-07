using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1.Droid.Activitys.Utils
{
    public class GlobalFaceTypeModel
    {

        // 模型的选择
        public static string TYPE_MODEL = "TYPE_MODEL";
        public static int RECOGNIZE_LIVE = 1;
        public static int RECOGNIZE_ID_PHOTO = 2;

        // 摄像头的选择
        public static string TYPE_CAMERA = "TYPE_CAMERA";
        public static int ORBBEC = 1;
        public static int IMIMECT = 2;
        public static int ORBBECPRO = 3;

        // 摄像头的选择
        public static string TYPE_THREAD = "TYPE_THREAD";
        public static int SINGLETHREAD = 1;
        public static int MULTITHREAD = 2;


    }
}