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
using Java.IO;

namespace App1.Droid.Activitys.Utils
{
    public class IOUtil
    {
        /**
         * 关闭一个或多个流对象
         *
         * @param closeables
         *            可关闭的流对象列表
         * @throws IOException
         */
        public static void close(ICloseable[] closeables)
        {
            if (closeables != null)
            {
                foreach (ICloseable closeable in closeables)
                {
                    if (closeable != null)
                    {
                        closeable.Close();
                    }
                }
            }
        }

        /**
         * 关闭一个或多个流对象
         *
         * @param closeables
         *            可关闭的流对象列表
         */
        public static void closeQuietly(ICloseable[] closeables)
        {
            try
            {
                close(closeables);
            }
            catch (IOException e)
            {
                // do nothing
            }
        }
    }
}