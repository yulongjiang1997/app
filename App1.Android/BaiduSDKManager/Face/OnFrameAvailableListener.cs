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
    public interface OnFrameAvailableListener
    {
        /**
         * 每当图片源有新一帧时图片源会回调该方法。
         * @param frame 新的一帧
         */
        void onFrameAvailable(ImageFrame frame);
    }
}