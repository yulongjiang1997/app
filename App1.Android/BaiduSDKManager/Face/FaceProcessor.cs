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

namespace App1.Droid.BaiduSDKManager.Face
{
    public interface FaceProcessor
    {
        /**
         * FaceDetectManager 回调该方法，对图片帧进行处理。
         * @param detectManager 回调的 FaceDetectManager
         * @param frame 需要处理的图片帧。
         * @return 返回true剩下的FaceProcessor将不会被回调。
         */
        bool process(FaceDetectManager detectManager, ImageFrame frame);
    }
}