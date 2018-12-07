using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using Java.Util;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class FileUitls
    {

        /**
         * Checks if is sd card available.
         *
         * @return true, if is sd card available
         */
        public static bool isSdCardAvailable()
        {
            return Environment.ExternalStorageState.Equals(Environment.MediaMounted);
        }

        /**
         * Gets the SD root file.
         *
         * @return the SD root file
         */
        public static File getSDRootFile()
        {
            if (isSdCardAvailable())
            {
                return Environment.ExternalStorageDirectory;
            }
            else
            {
                return null;
            }
        }

        public static File getFaceDirectory()
        {
            File sdRootFile = getSDRootFile();
            File file = null;
            if (sdRootFile != null && sdRootFile.Exists())
            {
                file = new File(sdRootFile, "faces");
                if (!file.Exists())
                {
                    bool success = file.Mkdirs();
                }
            }

            return file;
        }

        public static File getBatchFaceDirectory(string batchDir)
        {
            File sdRootFile = getSDRootFile();
            File file = null;
            if (sdRootFile != null && sdRootFile.Exists())
            {
                file = new File(sdRootFile, batchDir);
                if (!file.Exists())
                {
                    bool success = file.Mkdirs();
                }
            }

            return file;
        }

        public static bool saveFile(File file, Bitmap bitmap)
        {
            System.IO.FileStream oout = null;
            try
            {
                oout = new System.IO.FileStream(file.Path, System.IO.FileMode.Open);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, oout);
                return true;
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                try
                {
                    if (oout != null)
                    {
                        oout.Close();
                    }
                }
                catch (Exception e)
                {
                    e.PrintStackTrace();
                }
            }
            return false;
        }

        public static bool checklicense(Context context, string licenseName)
        {
            string filePath = context.FilesDir.Parent + "/" + licenseName;
            File file = new File(filePath);
            if (file.Exists())
            {
                return true;
            }

            String[] content = d(context, licenseName);
            if (content == null || content.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static String[] d(Context var1, string licenseName)
        {
            ArrayList var2 = null;
            System.IO.Stream var3 = null;

            String var7;
        label163:
            {
                try
                {
                    BufferedReader var5 = null;
                    InputStreamReader var6 = null;
                    var3 = a(var1, licenseName);
                    Log.Info("Face-SDK", "open license file path " + licenseName);
                    if (null != var3)
                    {
                        var6 = new InputStreamReader(var3);
                        var5 = new BufferedReader(var6);
                        var2 = new ArrayList();

                        while (true)
                        {
                            string var4;
                            if ((var4 = var5.ReadLine()) == null)
                            {
                                goto label163;
                            }

                            Log.Info("Face-SDK", "readLine " + var4);
                            var2.Add(var4);
                        }
                    }

                    Log.Info("Face", "open license file error.");
                    var7 = null;
                }
                catch (FileNotFoundException var22)
                {
                    var22.PrintStackTrace();
                    goto label163;
                }
                catch (IOException var23)
                {
                    var23.PrintStackTrace();
                    goto label163;
                }
                catch (Exception var24)
                {
                    var24.PrintStackTrace();
                    goto label163;
                }
                finally
                {
                    if (var3 != null)
                    {
                        try
                        {
                            var3.Close();
                        }
                        catch (IOException var21)
                        {
                            var21.PrintStackTrace();
                        }
                    }

                }
            }

            String[] var26 = null;
            if (var2 != null && var2.Size() > 0)
            {
                c(var1, licenseName, var2);
                var26 = new String[var2.Size()];
                int var27 = 0;

                for (IIterator var28 = var2.Iterator(); var28.HasNext; ++var27)
                {
                    var7 = (String)var28.Next();
                    var26[var27] = var7;
                    Log.Info ("License-SDK", "license file info =" + var7);
                }
            }

            return var26;
        }


        public static System.IO.Stream a(Context var0, string var1)
        {
            if (var0 == null)
            {
                return null;
            }
            else
            {
                object var2 = b(var0, var1);
                Log.Info("Face", "read_license_from_data");
                if (var2 == null)
                {
                    Log.Info("Face", "read_license_from_asset");
                    // var2 = c(var0, var1);
                }

                return (System.IO.Stream)var2;
            }
        }

        private static FileInputStream b(Context var0, string var1)
        {
            if (var0 == null)
            {
                return null;
            }
            else
            {
                FileInputStream var2 = null;

                try
                {
                    File var3 = var0.GetDir(var1, 0);
                    if (var3 == null || !var3.Exists() || !var3.IsFile)
                    {
                        Log.Info("Face", "read_license_from_data file not found");
                        return null;
                    }

                    var2 = new FileInputStream(var3);
                }
                catch (FileNotFoundException var4)
                {
                    Log.Info("Face", "read_license_from_data FileNotFoundException");
                    var4.PrintStackTrace();
                }
                catch (Exception var5)
                {
                    Log.Info("Face", "read_license_from_data Exception " + var5.Message);
                    var5.PrintStackTrace();
                }

                return var2;
            }
        }

        public static bool c(Context var0, string var1,ArrayList var2)
        {
            Log.Info("Face", "write_license_content");
            if (var2 != null && var2.ToArray().Length != 0 && var0 != null)
            {
                bool var3 = true;
                File var4 = var0.GetDir(var1, 0);
                if (var4 != null && var4.Exists())
                {
                    var4.Delete();
                }

                if (var4 != null && !var4.Exists())
                {
                    try
                    {
                        var4.CreateNewFile();
                    }
                    catch (IOException var19)
                    {
                        Log.Info("Face", "write_license_content IOException");
                        var19.PrintStackTrace();
                    }
                }

                FileOutputStream var5 = null;

                try
                {
                    var5 = new FileOutputStream(var4);
                    IIterator var6 = var2.Iterator();

                    while (var6.HasNext)
                    {
                        String var7 = (String)var6.Next();
                        var5.Write(var7.GetBytes());
                        var5.Write(10);
                    }
                }
                catch (FileNotFoundException var20)
                {
                    var3 = false;
                    Log.Info("Face", "write_license_content FileNotFoundException");
                    var20.PrintStackTrace();
                }
                catch (IOException var21)
                {
                    var3 = false;
                    Log.Info("Face", "write_license_content IOException");
                    var21.PrintStackTrace();
                }
                finally
                {
                    if (var5 != null)
                    {
                        try
                        {
                            var5.Close();
                        }
                        catch (IOException var18)
                        {
                            var3 = false;
                            var18.PrintStackTrace();
                        }
                    }
                }
                return var3;
            }
            else
            {
                return false;
            }
        }

        public static void deleteLicense(Context context, string licenseName)
        {
            string filePath = context.FilesDir.Parent + "/" + licenseName;
            File file = new File(filePath);
            if (file.Exists())
            {
                file.Delete();
            }

            File var4 = context.GetDir(licenseName, 0);
            if (var4 != null && var4.Exists())
            {
                var4.Delete();
            }
        }
    }
    }
