using App1.Droid.BaiduSDKManager.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Droid.BaiduSDKManager.CallBack
{
    public interface ILivenessCallBack
    {
         void onCallback(LivenessModel livenessModel);

         void onTip(int code, string msg);

         void onCanvasRectCallback(LivenessModel livenessModel);
    }
}
