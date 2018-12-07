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
using Java.Lang;
using Java.Util.Zip;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class ZipUtil
    {


        public static bool isSuccess;

        private ZipUtil()
        {
            // empty
        }

        /**
         * 压缩文件
         *
         * @param filePath 待压缩的文件路径
         * @return 压缩后的文件
         */
        public static File zip(string filePath)
        {
            File target = null;
            File source = new File(filePath);
            if (source.Exists())
            {
                // 压缩文件名=源文件名.zip
                string zipName = source.Name + ".zip";
                target = new File(source.Parent, zipName);
                if (target.Exists())
                {
                    target.Delete(); // 删除旧的文件
                }
                System.IO.FileStream fos = null;
                ZipOutputStream zos = null;
                try
                {
                    fos = new System.IO.FileStream(target.Path, System.IO.FileMode.Open);
                    zos = new ZipOutputStream(fos);
                    // 添加对应的文件Entry
                    addEntry("/", source, zos);
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e);
                }
                finally
                {
                    zos.Close();
                    fos.Close();
                }
            }
            return target;
        }

        /**
         * 扫描添加文件Entry
         *
         * @param base   基路径
         * @param source 源文件
         * @param zos    Zip文件输出流
         * @throws IOException
         */
        private static void addEntry(string bbase, File source, ZipOutputStream zos)
        {
            // 按目录分级，形如：/aaa/bbb.txt
            string entry = bbase + source.Name;
            if (source.IsDirectory)
            {
                foreach (File file in source.ListFiles())
                {
                    // 递归列出目录下的所有文件，添加文件Entry
                    addEntry(entry + "/", file, zos);
                }

            }
            else
            {
                System.IO.FileStream fis = null;
                BufferedInputStream bis = null;
                try
                {
                    byte[] buffer = new byte[1024 * 10];
                    fis = new System.IO.FileStream(source.Path, System.IO.FileMode.Append);
                    bis = new BufferedInputStream(fis, buffer.Length);
                    int read = 0;
                    zos.PutNextEntry(new ZipEntry(entry));
                    while ((read = bis.Read(buffer, 0, buffer.Length)) != -1)
                    {
                        zos.Write(buffer, 0, read);
                    }
                    zos.CloseEntry();
                }
                finally
                {
                    bis.Close();
                    fis.Close();
                }
            }
        }

        /**
         * 解压文件
         *
         * @param filePath 压缩文件路径
         */
        public static void unzip(string filePath)
        {
            //isSuccess = false;
            File source = new File(filePath);
            if (source.Exists())
            {
                ZipInputStream zis = null;
                BufferedOutputStream bos = null;
                try
                {
                    zis = new ZipInputStream(new System.IO.FileStream(source.Path, System.IO.FileMode.Append));
                    ZipEntry entry = null;
                    while ((entry = zis.NextEntry) != null
                            && !entry.IsDirectory)
                    {
                        File target = new File(source.Parent, entry.Name);
                        if (!target.ParentFile.Exists())
                        {
                            // 创建文件父目录
                            target.ParentFile.Mkdirs();
                        }
                        // 写入文件
                        bos = new BufferedOutputStream(new System.IO.FileStream(target.Path, System.IO.FileMode.Append));
                        int read = 0;
                        byte[] buffer = new byte[1024 * 10];
                        while ((read = zis.Read(buffer, 0, buffer.Length)) != -1)
                        {
                            bos.Write(buffer, 0, read);
                        }
                        bos.Flush();
                    }
                    zis.CloseEntry();
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e);
                }
                finally
                {
                    zis.Close();
                    bos.Close();
                    // isSuccess = true;
                }
            }
        }
    }
}