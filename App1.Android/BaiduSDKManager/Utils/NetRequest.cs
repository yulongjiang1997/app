using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Net;
using Java.Security;
using Java.Security.Cert;
using Javax.Net.Ssl;
using Org.Json;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class NetRequest
    {

        private static string TAG = "NetRequest";
        private static IHostnameVerifier DO_NOT_VERIFY = new HostnameVerifier();
        public class HostnameVerifier : Java.Lang.Object, IHostnameVerifier
        {
            //public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool Verify(string hostname, ISSLSession session)
            {
                return true;
            }
        }


        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        public static bool request(NetRequest.RequestAdapter adapter)
        {
            bool requireRetry = false;
            int retryCount = adapter.getRetryCount();
            bool code = false;
            object exception = null;
            object result = null;
            HttpsURLConnection conn = null;
            // HttpURLConnection conn = null;
            OutputStream oout = null;
            InputStream iin = null;
            bool var36 = false;
            do
            {
                try
                {

                    try
                    {
                        requireRetry = false;
                        var36 = false;
                        exception = null;
                        result = null;
                        URL var34 = new URL(adapter.getURL());
                        trustAllHosts();
                        conn = (HttpsURLConnection)var34.OpenConnection();
                        // conn = (HttpURLConnection)var34.openConnection();
                        conn.ConnectTimeout = adapter.getConnectTimeout();
                        conn.DoInput = (true);
                        conn.DoOutput = (true);
                        conn.SetRequestProperty("Content-Type", "application/json");
                        conn.ReadTimeout = (adapter.getReadTimeout());
                        conn.RequestMethod = (adapter.getRequestMethod());
                        conn.UseCaches = (false);
                        conn.HostnameVerifier = (DO_NOT_VERIFY);
                        //oout = parse(new ByteArrayInputStream(StreamToBytes(conn.OutputStream)));
                        oout.Write(Encoding.Default.GetBytes(adapter.getRequeststring()));
                        oout.Flush();
                        if ((int)conn.ResponseCode != 200)
                        {
                            var36 = true;
                            new IllegalStateException("ResponseCode: " + conn.ResponseCode);
                        }
                        else
                        {
                            iin = new ByteArrayInputStream(StreamToBytes(conn.InputStream));
                            adapter.parseResponse(iin);
                        }
                    }
                    catch (SocketTimeoutException var32)
                    {
                        var32.PrintStackTrace();
                        requireRetry = true;
                        var36 = true;
                    }
                    catch (SSLHandshakeException e)
                    {
                        e.PrintStackTrace();
                        // throw new SSLHandshakeException();
                    }
                    catch (Java.IO.IOException var33)
                    {
                        var33.PrintStackTrace();
                        var36 = true;
                    }
                    catch (JSONException var341)
                    {
                        var341.PrintStackTrace();
                        var36 = true;
                    }
                    catch (Java.Lang.Exception var35)
                    {
                        var35.PrintStackTrace();
                        var36 = true;
                    }
                }
                catch (Java.Lang.Exception)
                {
                    if (oout != null)
                    {
                        try
                        {
                            oout.Close();
                        }
                        catch (Java.IO.IOException var31)
                        {

                        }
                    }

                    if (iin != null)
                    {
                        try
                        {
                            iin.Close();
                        }
                        catch (Java.IO.IOException var30)
                        {

                        }
                    }

                    if (conn != null)
                    {
                        conn.Disconnect();
                    }

                }
            } while (requireRetry && retryCount-- > 0);

            return var36;
        }

        public static bool isConnected(Context context)
        {
            if (context == null)
            {
                return false;
            }
            else
            {
                try
                {
                    ConnectivityManager connectivityManager = (ConnectivityManager)context.
                       GetSystemService(Context.ConnectivityService);
                    NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
                    return activeNetworkInfo == null ? false : activeNetworkInfo.IsConnected;
                }
                catch (System.Exception e)
                {

                    throw;
                }

            }
        }
        //inputStream转outputStream
        public static ByteArrayOutputStream parse(InputStream iin)
        {
            ByteArrayOutputStream swapStream = new ByteArrayOutputStream();
            int ch;
            while ((ch = iin.Read()) != -1) {
                swapStream.Write(ch);
            }
            return swapStream;
        }
        private NetRequest()
        {
            throw new RuntimeException("This class instance can not be created.");
        }

        public class X509TrustManager : Java.Lang.Object, IX509TrustManager
        {
            public IntPtr Handle => throw new NotImplementedException();

            public void CheckClientTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
            {

            }

            public void CheckServerTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
            {

            }

            Java.Security.Cert.X509Certificate[] IX509TrustManager.GetAcceptedIssuers()
            {
                return new Java.Security.Cert.X509Certificate[0];
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public class TrustManager : X509TrustManager, ITrustManager
        {
            public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {

            }
        }

        private static void trustAllHosts()
        {
            ITrustManager[] trustAllCerts = new TrustManager[] { };


            try
            {
                SSLContext e = SSLContext.GetInstance("TLS");
                e.Init((IKeyManager[])null, trustAllCerts, new SecureRandom());
                HttpsURLConnection.DefaultSSLSocketFactory = e.SocketFactory;
            }
            catch (Java.Lang.Exception var2)
            {
                var2.PrintStackTrace();
            }

        }

        public abstract class RequestAdapter
        {
            public static int RESPONSE_STATUS_NORMAL = 0;
            public static int RESPONSE_STATUS_ERROR_TIMEOUT = 1;
            public static int RESPONSE_STATUS_ERROR_IO = 2;
            public static int RESPONSE_STATUS_ERROR_PARSE_JSON = 3;
            public static int RESPONSE_STATUS_ERROR_RESPONSE_CODE = 4;
            public static int RESPONSE_STATUS_ERROR_UNKNOWN = 5;
            private static int RETRY_COUNT = 2;
            private static int CONNECT_TIMEOUT = 5000;
            private static int READ_TIMEOUT = 5000;
            private static string REQUEST_METHOD = "POST";

            public RequestAdapter()
            {
            }

            public abstract string getURL();

            public abstract string getRequeststring();

            public abstract void parseResponse(InputStream var1);

            public int getRetryCount()
            {
                return 0;
            }

            public int getConnectTimeout()
            {
                return 5000;
            }

            public int getReadTimeout()
            {
                return 5000;
            }

            public string getRequestMethod()
            {
                return "POST";
            }
        }
    }
}