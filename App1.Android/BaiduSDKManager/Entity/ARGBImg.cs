using System;

namespace App1.Droid.BaiduSDKManager.Entity
{
    public class ARGBImg
    {
        public int[] data;
        public int width;
        public int height;
        public int angle = 0;
        public int flip = 0;

        public ARGBImg()
        {
        }

        public ARGBImg(int[] data, int width, int height, int angle, int flip)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.angle = angle;
            this.flip = flip;
        }

        public String toString()
        {
            return "width:" + this.width + ", "
                    + "height:" + this.height + ", " + "angle:"
                    + this.angle + ", " + "flip:" + this.flip;
        }
    }
}