using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class ImageUtils
    {

        public static Android.Net.Uri geturi(Android.Content.Intent intent, Context context)
        {
            Android.Net.Uri uri = intent.Data;
            string type = intent.Type;
            if (uri.Scheme.Equals("file") && (type.Contains("image/*")))
            {
                string path = uri.EncodedPath;
                if (path != null)
                {
                    path = Android.Net.Uri.Decode(path);
                    ContentResolver cr = context.ContentResolver;
                    StringBuffer buff = new StringBuffer();
                    buff.Append("(").Append(MediaStore.Images.ImageColumns.Data).Append("=")
                            .Append("'" + path + "'").Append(")");
                    ICursor cur = cr.Query(MediaStore.Images.Media.ExternalContentUri,
                            new string[] { MediaStore.Images.ImageColumns.Id },
                            buff.ToString(), null, null);
                    int index = 0;
                    for (cur.MoveToFirst(); !cur.IsAfterLast; cur.MoveToNext())
                    {
                        index = cur.GetColumnIndex(MediaStore.Images.ImageColumns.Id);
                        // set _id value
                        index = cur.GetInt(index);
                    }
                    if (index == 0)
                    {
                        // do nothing
                    }
                    else
                    {
                        Android.Net.Uri uri_temp = Android.Net.Uri.Parse("content://media/external/images/media/" + index);
                        if (uri_temp != null)
                        {
                            uri = uri_temp;
                        }
                    }
                }
            }
            return uri;
        }

        public static void resize(Bitmap bitmap, File outputFile, int maxWidth, int maxHeight)
        {
            try
            {
                int bitmapWidth = bitmap.Width;
                int bitmapHeight = bitmap.Height;
                // 图片大于最大高宽，按大的值缩放
                if (bitmapWidth > maxHeight || bitmapHeight > maxWidth)
                {
                    float widthScale = maxWidth * 1.0f / bitmapWidth;
                    float heightScale = maxHeight * 1.0f / bitmapHeight;

                    float scale = Java.Lang.Math.Min(widthScale, heightScale);
                    Matrix matrix = new Matrix();
                    matrix.PostScale(scale, scale);
                    bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmapWidth, bitmapHeight, matrix, false);
                }

                // save image
                System.IO.FileStream oout = new System.IO.FileStream(outputFile.Path,System.IO.FileMode.Open);
                try
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, oout);
                }
                catch (Java.Lang.Exception e)
                {
                    e.PrintStackTrace();
                }
                finally
                {
                    try
                    {
                        oout.Close();
                    }
                    catch (Java.Lang.Exception e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        public static Bitmap RGB2Bitmap(byte[] bytes, int width, int height)
        {
            // use Bitmap.Config.ARGB_8888 instead of type is OK
            Bitmap stitchBmp = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            byte[] rgba = new byte[width * height * 4];
            for (int i = 0; i < width * height; i++)
            {
                byte b1 = bytes[i * 3 + 0];
                byte b2 = bytes[i * 3 + 1];
                byte b3 = bytes[i * 3 + 2];
                // set value
                rgba[i * 4 + 0] = b1;
                rgba[i * 4 + 1] = b2;
                rgba[i * 4 + 2] = b3;
                rgba[i * 4 + 3] = (byte)255;
            }
            stitchBmp.CopyPixelsFromBuffer(Java.Nio.ByteBuffer.Wrap(rgba));
            return stitchBmp;
        }
    }
}