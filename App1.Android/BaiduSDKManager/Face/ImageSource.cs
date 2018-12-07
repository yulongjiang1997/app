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
using static Android.Graphics.SurfaceTexture;

namespace App1.Droid.BaiduSDKManager.Face
{
    public class ImageSource
    {

        public ImageFrame borrowImageFrame()
        {
            return new ImageFrame();
        }

        private static List<OnFrameAvailableListener> listeners { get; set; } = new List<OnFrameAvailableListener>();

        /** 注册监听器，当有图片帧时会回调。*/
        public void addOnFrameAvailableListener(OnFrameAvailableListener listener)
        {
            this.listeners.Add(listener);
        }

        /** 删除监听器*/
        public void removeOnFrameAvailableListener(OnFrameAvailableListener listener)
        {
            if (listener != null)
            {
                this.listeners.Remove(listener);
            }
        }

        /** 获取监听器列表 */
        protected static List<OnFrameAvailableListener> getListeners()
        {
            return listeners;
        }

        /** 打开图片源。*/
        public void start()
        {

        }

        /** 停止图片源。*/
        public void stop()
        {

        }

        /** 设置预览View用于显示预览图。*/
        public void setPreviewView(PreviewView previewView)
        {
        }
    }

}