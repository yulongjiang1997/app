using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Manager;
using Java.IO;

namespace App1.Droid.BaiduSDKManager.Face
{
    public class FileImageSource : ImageSource
    {

    private string filePath;

    /**
     * 设置检测图片的位置。
     * @param filePath 图片路径
     */
    public void setFilePath(string filePath)
    {
        this.filePath = filePath;
    }

    
    public void start()
    {
        base.start();
        Bitmap bitmap = getImageThumbnail(filePath, 960, 960);
        int degree = getExifOrientation(filePath);

        if (degree == 90 || degree == 180 || degree == 270)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(degree);
            bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        // 文件转化为int[] argb
        int[] argb = new int[bitmap.Width * bitmap.Height];
        bitmap.GetPixels(argb, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);
        FaceSDKManager.getInstance().getFaceDetector().clearTrackedFaces();

        ImageFrame frame = new ImageFrame();
        frame.setArgb(argb);
        frame.setWidth(bitmap.Width);
        frame.setHeight(bitmap.Height);

            foreach (OnFrameAvailableListener listener in getListeners())
            {
                listener.onFrameAvailable(frame);
            }

            FaceSDKManager.getInstance().getFaceDetector().clearTrackedFaces();
    }

    private Bitmap getImageThumbnail(String imagePath, int width, int height)
    {
        Bitmap bitmap;
        BitmapFactory.Options options = new BitmapFactory.Options();
        options.InJustDecodeBounds = true;
        // 获取这个图片的宽和高，注意此处的bitmap为null
        BitmapFactory.DecodeFile(imagePath, options);
        options.InJustDecodeBounds = false; // 设为 false
        // 计算缩放比
        int h = options.OutHeight;
        int w = options.OutWidth;
        int beWidth = w / width;
        int beHeight = h / height;
        int be = 1;
        if (beWidth < beHeight)
        {
            be = beWidth;
        }
        else
        {
            be = beHeight;
        }
        if (be <= 0)
        {
            be = 1;
        }
        options.InSampleSize = be;
        options.InPreferredConfig = Bitmap.Config.Argb8888;

        // 重新读入图片，读取缩放后的bitmap，注意这次要把options.inJustDecodeBounds 设为 false
        bitmap = BitmapFactory.DecodeFile(imagePath, options);
        return bitmap;
    }

    private static int getExifOrientation(string filepath)
    {
        int degree = 0;
        ExifInterface exif = null;

        try
        {
            exif = new ExifInterface(filepath);
        }
        catch (IOException ex)
        {
            // MmsLog.e(ISMS_TAG, "getExifOrientation():", ex);
        }

        if (exif != null)
        {
            int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, -1);
            if (orientation != -1)
            {
                // We only recognize a subset of orientation tag values.
                switch (orientation)
                {
                    case 6:
                        degree = 90;
                        break;

                    case 3:
                        degree = 180;
                        break;

                    case 8:
                        degree = 270;
                        break;
                    default:
                        break;
                }
            }
        }
        return degree;
    }
}
}