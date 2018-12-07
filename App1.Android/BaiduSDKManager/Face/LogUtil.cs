using Android.Util;
using Java.IO;
using Java.Lang;
using Java.Nio.Channels;
using Java.Util;
using System.IO;

namespace App1.Droid.BaiduSDKManager.Face
{
    public class LogUtil
    {

        /**
         * The Constant TAG.
         */
        internal static readonly string TAG = typeof(LogUtil).Name;
        // public final static boolean LOG_ENABLED = true;

        // final static int LOGCAT_LEVEL = 16;// logcat level
        /**
         * The logcat level.
         */
        static int LOGCAT_LEVEL = 16;

        /**
         * log file level, must >= LOGCAT_LEVEL
         */
        static int FILE_LOG_LEVEL = 16;

        /**
         * The Constant LOG_LEVEL_ERROR.
         */
        static int LOG_LEVEL_ERROR = 16;

        /**
         * The Constant LOG_LEVEL_WARN.
         */
        static int LOG_LEVEL_WARN = 8;

        /**
         * The Constant LOG_LEVEL_INFO.
         */
        static int LOG_LEVEL_INFO = 4;

        /**
         * The Constant LOG_LEVEL_DEBUG.
         */
        static int LOG_LEVEL_DEBUG = 18;

        /**
         * The debug.
         */
        public static bool DEBUG = (LOGCAT_LEVEL <= LOG_LEVEL_DEBUG);

        /**
         * The info.
         */
        public static bool INFO = (LOGCAT_LEVEL <= LOG_LEVEL_INFO);

        /**
         * The warn.
         */
        public static bool WARN = (LOGCAT_LEVEL <= LOG_LEVEL_WARN);

        /**
         * The error.
         */
        public static bool ERROR = (LOGCAT_LEVEL <= LOG_LEVEL_ERROR);

        public static string E = "E";
        public static string D = "D";
        public static string V = "V";
        public static string W = "W";
        public static string I = "I";

        /**
         * The Constant LOG_FILE_NAME.
         */

        static string LOG_FILE_NAME = "turnstile.log";
        /**
         * The Constant LOG_TAG_STRING.
         */
        private static string LOG_TAG_STRING = "TurnStile";

        /**
         * The Constant LOG_ENTRY_FORMAT.
         */
        static string LOG_ENTRY_FORMAT = "[%tF %tT][%s][%s]%s"; // [2010-01-22
                                                                // 13:39:1][D][com.a.c]error
                                                                // occured
                                                                /**
                                                                 * The log stream.
                                                                 */
        static PrintStream logStream;

        /**
         * The initialized.
         */
        static bool initialized = true;

        private static string TAG_LEFT_BRICK = " [";

        private static string TAG_RIGHT_BRICK = "]: ";

        private static string TAG_COLOMN = ":";

        /**
         * Enable file log.
         */
        private void enableFileLog()
        {
            FILE_LOG_LEVEL = 18;
        }

        /**
         * D.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void d(string tag, string msg)
        {
            if (DEBUG)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.d(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_DEBUG)
                {
                    write(D, fulltag, msg, null);
                }
            }
        }

        /**
         * D.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void d(string tag, string msg, Throwable error)
        {
            if (DEBUG)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.d(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_DEBUG)
                {
                    write(D, fulltag, msg, error);
                }
            }
        }

        /**
         * V.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void v(string tag, string msg)
        {
            if (DEBUG)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.v(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_DEBUG)
                {
                    write(V, fulltag, msg, null);
                }
            }
        }

        /**
         * V.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void v(string tag, string msg, Throwable error)
        {
            if (DEBUG)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.v(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_DEBUG)
                {
                    write(V, fulltag, msg, error);
                }
            }
        }

        /**
         * I.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void i(string tag, string msg)
        {
            if (INFO)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.i(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_INFO)
                {
                    write(I, fulltag, msg, null);
                }
            }
        }

        /**
         * I.
         *
         * @param rz  the rz
         * @param tag the tag
         * @param msg the msg
         */
        public static void i(string rz, string tag, string msg)
        {
            if (INFO)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.i(rz, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_INFO)
                {
                    write(I, fulltag, msg, null);
                }
            }
        }

        /**
         * I.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void i(string tag, string msg, Throwable error)
        {
            if (INFO)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.i(LOG_TAG_STRING, tag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_INFO)
                {
                    write(I, fulltag, msg, error);
                }
            }
        }

        /**
         * W.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void w(string tag, string msg)
        {
            if (WARN)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.w(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_WARN)
                {
                    write(W, fulltag, msg, null);
                }
            }
        }

        /**
         * W.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void w(string tag, string msg, Throwable error)
        {
            if (WARN)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.w(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);
                if (FILE_LOG_LEVEL <= LOG_LEVEL_WARN)
                {
                    write(W, fulltag, msg, error);
                }
            }
        }

        /**
         * E.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void e(string tag, string msg)
        {
            if (ERROR)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.e(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);

                if (FILE_LOG_LEVEL <= LOG_LEVEL_ERROR)
                {
                    write(E, fulltag, msg, null);
                }
            }
        }

        /**
         * E.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void e(string tag, string msg, Throwable error)
        {
            if (ERROR)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                //            Log.e(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);

                if (FILE_LOG_LEVEL <= LOG_LEVEL_ERROR)
                {
                    write(E, fulltag, msg, error);
                }
            }
        }

        /**
         * Wtf.
         *
         * @param tag the tag
         * @param msg the msg
         */
        public static void wtf(string tag, string msg)
        {
            if (ERROR)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;
                Log.Wtf(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg);

                if (FILE_LOG_LEVEL <= LOG_LEVEL_ERROR)
                {
                    write(E, fulltag, msg, null);
                }
            }
        }

        /**
         * Wtf.
         *
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        public static void wtf(string tag, string msg, Throwable error)
        {
            if (ERROR)
            {
                string fulltag = getFullTag() + TAG_COLOMN + tag;

                Log.Wtf(LOG_TAG_STRING, fulltag + TAG_LEFT_BRICK + TAG_RIGHT_BRICK + msg, error);

                if (FILE_LOG_LEVEL <= LOG_LEVEL_ERROR)
                {
                    write(E, fulltag, msg, error);
                }
            }
        }

        /**
         * Write.
         *
         * @param level the level
         * @param tag   the tag
         * @param msg   the msg
         * @param error the error
         */
        private static void write(string level, string tag, string msg, Throwable error)
        {
            if (!initialized)
            {
                init();
            }
            if (logStream == null || logStream.CheckError())
            {
                initialized = false;
                return;
            }

            Date now = new Date();

            logStream.Printf(LOG_ENTRY_FORMAT, now, now, level, tag, TAG_LEFT_BRICK
                    + TAG_RIGHT_BRICK + msg);
            logStream.Println();

            if (error != null)
            {
                error.PrintStackTrace(logStream);
                logStream.Println();
            }
        }

        //static {
        //init();
        //    }

        /**
         * Inits the.
         */
        public static  void init()
        {

            //        DEBUG = false;
            //        INFO = false;
            //        WARN = false;
            //        ERROR = false;

            if (initialized)
            {
                return;
            }

            DEBUG = (LOGCAT_LEVEL <= LOG_LEVEL_DEBUG);
            INFO = (LOGCAT_LEVEL <= LOG_LEVEL_INFO);
            WARN = (LOGCAT_LEVEL <= LOG_LEVEL_WARN);
            ERROR = (LOGCAT_LEVEL <= LOG_LEVEL_ERROR);

            try
            {
                Java.IO.File sdRoot = getSDRootFile();
                if (sdRoot != null)
                {
                    Java.IO.File logFile = new Java.IO.File(sdRoot, LOG_FILE_NAME);
                    if (!logFile.Exists())
                    {
                        logFile.CreateNewFile();
                    }

                    Log.Debug(LOG_TAG_STRING, TAG + " : Log to file : " + logFile);
                    if (logStream != null)
                    {
                        logStream.Close();
                    }
                    FileStream file = new FileStream(logFile.Path, FileMode.Open);
                    logStream = new PrintStream(file, true);
                    initialized = true;
                }
                logFileChannel = getFileLock();
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(LOG_TAG_STRING, "init log stream failed", e);
            }
        }


        /**
         * Checks if is sd card available.
         *
         * @return true, if is sd card available
         */
        public static bool isSdCardAvailable()
        {
            return Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted);
        }

        /**
         * Gets the SD root file.
         *
         * @return the SD root file
         */
        public static Java.IO.File getSDRootFile()
        {
            if (isSdCardAvailable())
            {
                return Android.OS.Environment.ExternalStorageDirectory;
            }
            else
            {
                return null;
            }
        }

        //    /*
        //     * (non-Javadoc)
        //     *
        //     * @see java.lang.Object#finalize()
        //     */
        //    @Override
        //    protected void finalize() throws Throwable {
        //        super.finalize();
        //        if (logStream != null) {
        //            logStream.close();
        //        }
        //    }

        private static string getFullTag()
        {
            return Thread.CurrentThread().Name;
        }

        private static FileChannel logFileChannel;

        private static FileChannel getFileLock()
        {

            if (logFileChannel == null)
            {
                Java.IO.File sdRoot = getSDRootFile();
                if (sdRoot != null)
                {
                    Java.IO.File logFile = new Java.IO.File(sdRoot, LOG_FILE_NAME);

                    try
                    {
                        logFileChannel = new FileOutputStream(logFile).Channel;
                    }
                    catch (Java.IO.FileNotFoundException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            return logFileChannel;
        }

    }
}