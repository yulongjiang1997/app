using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class LogUtils
    {

        private static string TAG = "FaceRecognize";
        public static string customTagPrefix = "";

        private LogUtils()
        {
        }

        public static bool allowD = true;
        public static bool allowE = true;
        public static bool allowI = true;
        public static bool allowV = true;
        public static bool allowW = true;
        public static bool allowWtf = true;

        private static string generateTag(StackTraceElement caller)
        {
            string TAG = "%s.%s(L:%d)";
            string callerClazzName = caller.ClassName;
            callerClazzName = callerClazzName.Substring(callerClazzName.LastIndexOf(".") + 1);
            TAG = string.Format(TAG, callerClazzName, caller.MethodName, caller.LineNumber);
            TAG = TextUtils.IsEmpty(customTagPrefix) ? TAG : customTagPrefix + ":" + TAG;
            return TAG;
        }

        public static CustomLogger customLogger;

        public interface CustomLogger
        {
            void d(string TAG, string content);

            void d(string TAG, string content, Throwable tr);

            void e(string TAG, string content);

            void e(string TAG, string content, Throwable tr);

            void i(string TAG, string content);

            void i(string TAG, string content, Throwable tr);

            void v(string TAG, string content);

            void v(string TAG, string content, Throwable tr);

            void w(string TAG, string content);

            void w(string TAG, string content, Throwable tr);

            void w(string TAG, Throwable tr);

            void wtf(string TAG, string content);

            void wtf(string TAG, string content, Throwable tr);

            void wtf(string TAG, Throwable tr);
        }

        public static void d(string content)
        {
            if (!allowD) return;

            if (customLogger != null)
            {
                customLogger.d(TAG, content);
            }
            else
            {
                Log.Debug(TAG, content);
            }
        }

        public static void d(string content, Throwable tr)
        {
            if (!allowD) return;

            if (customLogger != null)
            {
                customLogger.d(TAG, content, tr);
            }
            else
            {
                Log.Debug(TAG, content, tr);
            }
        }

        public static void e(string content)
        {
            if (!allowE) return;

            if (customLogger != null)
            {
                customLogger.e(TAG, content);
            }
            else
            {
                Log.Error(TAG, content);
            }
        }

        public static void e(string content, Throwable tr)
        {
            if (!allowE) return;

            if (customLogger != null)
            {
                customLogger.e(TAG, content, tr);
            }
            else
            {
                Log.Error(TAG, content, tr);
            }
        }

        public static void i(string content)
        {
            if (!allowI) return;

            if (customLogger != null)
            {
                customLogger.i(TAG, content);
            }
            else
            {
                Log.Info (TAG, content);
            }
        }

        public static void i(string content, Throwable tr)
        {
            if (!allowI) return;

            if (customLogger != null)
            {
                customLogger.i(TAG, content, tr);
            }
            else
            {
                Log.Info (TAG, content, tr);
            }
        }

        public static void v(string content)
        {
            if (!allowV) return;

            if (customLogger != null)
            {
                customLogger.v(TAG, content);
            }
            else
            {
                Log.Verbose(TAG, content);
            }
        }

        public static void v(string content, Throwable tr)
        {
            if (!allowV) return;

            if (customLogger != null)
            {
                customLogger.v(TAG, content, tr);
            }
            else
            {
                Log.Verbose(TAG, content, tr);
            }
        }

        public static void w(string content)
        {
            if (!allowW) return;

            if (customLogger != null)
            {
                customLogger.w(TAG, content);
            }
            else
            {
                Log.Wtf(TAG, content);
            }
        }

        public static void w(string content, Throwable tr)
        {
            if (!allowW) return;

            if (customLogger != null)
            {
                customLogger.w(TAG, content, tr);
            }
            else
            {
                Log.Wtf(TAG, content, tr);
            }
        }

        public static void w(Throwable tr)
        {
            if (!allowW) return;

            if (customLogger != null)
            {
                customLogger.w(TAG, tr);
            }
            else
            {
                Log.Wtf(TAG, tr);
            }
        }


        public static void wtf(string content)
        {
            if (!allowWtf) return;

            if (customLogger != null)
            {
                customLogger.wtf(TAG, content);
            }
            else
            {
                Log.Wtf(TAG, content);
            }
        }

        public static void wtf(string content, Throwable tr)
        {
            if (!allowWtf) return;

            if (customLogger != null)
            {
                customLogger.wtf(TAG, content, tr);
            }
            else
            {
                Log.Wtf(TAG, content, tr);
            }
        }

        public static void wtf(Throwable tr)
        {
            if (!allowWtf) return;

            if (customLogger != null)
            {
                customLogger.wtf(TAG, tr);
            }
            else
            {
                Log.Wtf(TAG, tr);
            }
        }

    }
}