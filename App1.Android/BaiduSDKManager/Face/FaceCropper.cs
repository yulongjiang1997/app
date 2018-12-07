using Android.Graphics;
using Com.Baidu.Idl.Facesdk;
using Java.Lang;

namespace App1.Droid.BaiduSDKManager.Face
{
    /**
  *人脸裁剪工具类。
  */
    public class FaceCropper
    {

        /**
         * 高速裁剪狂，防止框超出图片范围。
         * @param argb 图片argb数据
         * @param width 图片宽度
         * @param rect 裁剪框
         */
        public static void adjustRect(int[] argb, int width, Rect rect)
        {
            rect.Left = Math.Max(rect.Left, 0);
            rect.Right = Math.Min(rect.Right, width);
            int height = argb.Length / width;
            rect.Bottom = Math.Min(rect.Bottom, height);
            rect.Sort();
        }

        /**
         * 裁剪argb中的一块儿，裁剪框如果超出图片范围会被调整，所以记得检查。
         * @param argb 图片argb数据
         * @param width 图片宽度
         * @param rect 裁剪框
         */
        public static int[] crop(int[] argb, int width, Rect rect)
        {
            // adjustRect(argb, width, rect);
            int[] image = new int[rect.Width() * rect.Height()];

            for (int i = rect.Top; i < rect.Bottom; i++)
            {
                int rowIndex = width * i;
                try
                {
                    System.Array.Copy(argb, rowIndex + rect.Left, image, rect.Width() * (i - rect.Top), rect.Width());
                }
                catch (Exception e)
                {
                    e.PrintStackTrace();
                    return argb;
                }
            }
            return image;
        }

        public static Rect getCropFaceRect(int[] argb, FaceInfo faceInfo, int imageWidth)
        {
            int[] points = new int[8];

            faceInfo.GetRectPoints(points);

            int left = points[2];
            int top = points[3];
            int right = points[6];
            int bottom = points[7];

            int width = right - left;
            int height = bottom - top;

            width = width * 3 / 2;
            height = height * 2;
            //
            left = (int)(faceInfo.MCenterX - width / 2);
            top = (int)(faceInfo.MCenterY - height / 2);

            height = height * 4 / 5;
            //
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);

            Rect region = new Rect(left, top, left + width, top + height);
            adjustRect(argb, width, region);
            return region;

        }

        /**
         * 裁剪图片中的人脸。
         * @param argb argb图片数据
         * @param faceInfo 人脸信息
         * @param imageWidth 图片宽度
         * @return 返回裁剪后的人脸图片
         */
        public static int[] getCropFace(int[] argb, FaceInfo faceInfo, int imageWidth)
        {
            //        int[] points = new int[8];
            //
            //        faceInfo.getRectPoints(points);
            //
            //        int left = points[2];
            //        int top = points[3];
            //        int right = points[6];
            //        int bottom = points[7];
            //
            //        int width = right - left;
            //        int height = bottom - top;
            //
            //        width = width * 3 / 2;
            //        height = height * 2;
            //        //
            //        left = faceInfo.mCenter_x - width / 2;
            //        top = faceInfo.mCenter_y - height / 2;
            //
            //        height = height * 4 / 5;
            //        //
            //        left = Math.max(left, 0);
            //        top = Math.max(top, 0);
            //
            //        Rect region = new Rect(left, top, left + width, top + height);
            Rect region = getCropFaceRect(argb, faceInfo, imageWidth);
            return crop(argb, imageWidth, region);

        }

        /**
         * 裁剪图片中的人脸。
         * @param argb argb图片数据
         * @param faceInfo 人脸信息
         * @param imageWidth 图片宽度
         * @return 返回裁剪后的人脸图片
         */
        public static Bitmap getFace(int[] argb, FaceInfo faceInfo, int imageWidth)
        {
            //        int[] points = new int[8];
            //
            //        faceInfo.getRectPoints(points);
            //
            //        int left = points[2];
            //        int top = points[3];
            //        int right = points[6];
            //        int bottom = points[7];
            //
            //        int width = right - left;
            //        int height = bottom - top;
            //
            //        width = width * 3 / 2;
            //        height = height * 2;
            //        //
            //        left = faceInfo.mCenter_x - width / 2;
            //        top = faceInfo.mCenter_y - height / 2;
            //
            //        height = height * 4 / 5;
            //        //
            //        left = Math.max(left, 0);
            //        top = Math.max(top, 0);

            Rect region = getFaceRect(argb, faceInfo, imageWidth);


            int offset = (region.Top * imageWidth) + region.Left;
            return Bitmap.CreateBitmap(argb, offset, imageWidth, region.Width(), region.Height(),
                    Bitmap.Config.Argb8888);
        }

        public static Rect getFaceRect(int[] argb, FaceInfo faceInfo, int imageWidth)
        {
            int[] points = new int[8];

            faceInfo.GetRectPoints(points);

            int left = points[2];
            int top = points[3];
            int right = points[6];
            int bottom = points[7];

            int width = right - left;
            int height = bottom - top;

            width = width * 3 / 2;
            height = height * 2;
            //
            left = (int)(faceInfo.MCenterX - width / 2);
            top = (int)(faceInfo.MCenterY - height / 2);

            height = height * 4 / 5;
            //
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);

            Rect region = new Rect(left, top, left + width, top + height);
            FaceCropper.adjustRect(argb, imageWidth, region);
            return region;
        }
    }

}