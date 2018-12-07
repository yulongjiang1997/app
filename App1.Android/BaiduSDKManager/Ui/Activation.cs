using System;
using System.IO;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Manager;
using App1.Droid.BaiduSDKManager.Utils;
using Com.Baidu.Idl.Facesdk;
using Com.Baidu.Idl.License;
using Java.IO;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using Org.Json;

namespace App1.Droid.BaiduSDKManager.Ui
{
    public class Activation
    {

        private static Context Contexts { get; set; }
        private Button activateBtn;
        private Button backBtn;
        private TextView deviceIdTv;
        private static EditText KeyEt { get; set; }
        private static string device = "";
        private static Dialog activationDialog;
        private static Handler handler = new Handler(Looper.MainLooper);
        private static ActivationCallback activationCallback { get; set; }
        private static int lastKeyLen = 0;

        private TextView tvOnLineText;
        private TextView tvOffLineText;
        private Button btOffLineActive;
        static ArrayList list = new ArrayList();

        private static bool success = false;

        public Activation(Context context)
        {
            Contexts = context;
        }

        public void setActivationCallback(ActivationCallback callback)
        {
            activationCallback = callback;
        }

        public void show()
        {
            PreferencesUtil.initPrefs(Contexts.ApplicationContext);
            activationDialog = new Dialog(Contexts);
            activationDialog.SetTitle("设备激活");
            activationDialog.SetContentView(initView());
            activationDialog.SetCancelable(false);
            activationDialog.Show();
            addLisenter();
        }

        private LinearLayout initView()
        {
            device = AndroidLicenser.Get_device_id(Contexts.ApplicationContext);
            LinearLayout root = new LinearLayout(Contexts);
            root.Orientation = Android.Widget.Orientation.Vertical;
            LinearLayout.LayoutParams rootParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            rootParams.Gravity = GravityFlags.Center;
            root.SetBackgroundColor(Android.Graphics.Color.White);
            root.Focusable = true;
            root.FocusableInTouchMode = true;

            TextView titleTv = new TextView(Contexts);
            titleTv.Text = "设备激活";
            titleTv.TextSize = dip2px(15);
            titleTv.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.Black));

            LinearLayout.LayoutParams titleParams = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.WrapContent,
                    LinearLayout.LayoutParams.WrapContent);
            titleParams.Gravity = GravityFlags.Center;
            titleParams.TopMargin = dip2px(10);
            titleParams.RightMargin = dip2px(30);
            titleParams.LeftMargin = dip2px(30);

            deviceIdTv = new TextView(Contexts);
            deviceIdTv.SetTextIsSelectable(true);
            deviceIdTv.Text = "设备指纹：" + device;
            deviceIdTv.TextSize = dip2px(12);
            deviceIdTv.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.Black));

            LinearLayout.LayoutParams deviceIdParams = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.WrapContent,
                    LinearLayout.LayoutParams.WrapContent);
            deviceIdParams.Gravity = GravityFlags.Center;
            deviceIdParams.TopMargin = dip2px(40);
            deviceIdParams.RightMargin = dip2px(30);
            deviceIdParams.LeftMargin = dip2px(30);

            KeyEt = new EditText(Contexts);
            KeyEt.Hint = "输入序列号";
            KeyEt.Text = PreferencesUtil.getString("activate_key", "");
            // keyEt.setText("VMVY-PLkd-OsJN-veIc");

            LinearLayout.LayoutParams keyParams = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            keyParams.Gravity = GravityFlags.Center;
            keyParams.TopMargin = dip2px(40);
            keyParams.RightMargin = dip2px(30);
            keyParams.LeftMargin = dip2px(30);
            KeyEt.TransformationMethod = new AllCapTransformationMethod(true);
            KeyEt.SetWidth(dip2px(260));

            LinearLayout.LayoutParams activateParams = new LinearLayout.LayoutParams(dip2px(180), dip2px(40));
            activateParams.Gravity = GravityFlags.Center;
            activateParams.TopMargin = dip2px(40);
            activateParams.RightMargin = dip2px(40);
            activateParams.LeftMargin = dip2px(40);
            activateBtn = new Button(Contexts);
            // activateBtn.setId(100);
            activateBtn.Text = "在线激活";
            activateBtn.TextSize = dip2px(12);
            activateBtn.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.White));
            activateBtn.SetBackgroundColor(Contexts.Resources.GetColor(Resource.Color.material_blue_grey_800));

            LinearLayout.LayoutParams activateParamsone = new LinearLayout.LayoutParams(dip2px(360), dip2px(48));
            activateParamsone.Gravity = GravityFlags.Center;
            activateParamsone.TopMargin = dip2px(8);
            activateParamsone.RightMargin = dip2px(20);
            activateParamsone.LeftMargin = dip2px(60);
            tvOnLineText = new TextView(Contexts);
            tvOnLineText.Text = "在线激活:输入序列号，保持设备联网，SDK会自动进行激活";
            tvOnLineText.TextSize = dip2px(8);
            tvOnLineText.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.White));

            LinearLayout.LayoutParams activateParamsoffLine = new LinearLayout.LayoutParams(dip2px(180), dip2px(40));
            activateParamsoffLine.Gravity = GravityFlags.Center;
            activateParamsoffLine.TopMargin = dip2px(5);
            activateParamsoffLine.RightMargin = dip2px(40);
            activateParamsoffLine.LeftMargin = dip2px(40);
            btOffLineActive = new Button(Contexts);
            // activateBtn.setId(100);
            activateBtn.Text = "离线激活";
            activateBtn.TextSize = dip2px(12);
            activateBtn.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.White));
            activateBtn.SetBackgroundColor(Contexts.Resources.GetColor(Resource.Color.material_blue_grey_800));


            LinearLayout.LayoutParams tvOffLineParams = new LinearLayout.LayoutParams(dip2px(380), dip2px(48));
            tvOffLineParams.Gravity = GravityFlags.Center;
            tvOffLineParams.TopMargin = dip2px(8);
            tvOffLineParams.RightMargin = dip2px(20);
            tvOffLineParams.LeftMargin = dip2px(20);
            tvOffLineText = new TextView(Contexts);
            tvOffLineText.Text = "离线激活:将激活文件置于SD卡根目录（/storage/emulated/0）中，SDK会自动进行激活";
            tvOffLineText.TextSize = dip2px(8);
            tvOffLineText.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.White));

            LinearLayout.LayoutParams backParams = new LinearLayout.LayoutParams(dip2px(180), dip2px(40));
            backParams.Gravity = GravityFlags.Center;
            backParams.TopMargin = dip2px(5);
            backParams.BottomMargin = dip2px(20);
            backParams.RightMargin = dip2px(40);
            backParams.LeftMargin = dip2px(40);
            backBtn = new Button(Contexts);
            // activateBtn.setId(100);
            backBtn.Text = "返      回";
            backBtn.SetTextColor(Contexts.Resources.GetColor(Android.Resource.Color.White));
            backBtn.SetBackgroundColor(Contexts.Resources.GetColor(Resource.Color.material_blue_grey_800));
            backBtn.TextSize = dip2px(12);

            root.AddView(titleTv, titleParams);
            root.AddView(deviceIdTv, deviceIdParams);
            root.AddView(KeyEt, keyParams);
            root.AddView(activateBtn, activateParams);
            root.AddView(tvOnLineText, activateParamsone);
            root.AddView(btOffLineActive, activateParamsoffLine);
            root.AddView(tvOffLineText, tvOffLineParams);
            root.AddView(backBtn, backParams);
            return root;
        }


        public void beforeTextChanged(object s, int start, int count, int after)
        {

        }

        public void onTextChanged(object s, int start, int before, int count)
        {

        }

        public void afterTextChanged(IEditable s)
        {
            if (s.ToString().Length > 19)
            {
                KeyEt.Text = s.ToString().Substring(0, 19);
                KeyEt.SetSelection(KeyEt.Text.Length);
                lastKeyLen = s.Length();
                return;
            }
            if (s.ToString().Length < lastKeyLen)
            {
                lastKeyLen = s.Length();
                return;
            }
            string text = s.ToString().Trim();
            if (KeyEt.SelectionStart < text.Length)
            {
                return;
            }
            if (text.Length == 4 || text.Length == 9 || text.Length == 14)
            {
                KeyEt.Text = text + "-";
                KeyEt.SetSelection(KeyEt.Text.Length);
            }

            lastKeyLen = s.Length();
        }


        private class TextWatcherAnonymousInnerClass : Java.Lang.Object, ITextWatcher
        {
            //public IntPtr Handle => throw new NotImplementedException();

            public void AfterTextChanged(IEditable s)
            {
                if (s.ToString().Length > 19)
                {
                    KeyEt.Text = s.ToString().Substring(0, 19);
                    KeyEt.SetSelection(KeyEt.Text.Length);
                    lastKeyLen = s.Length();
                    return;
                }
                if (s.ToString().Length < lastKeyLen)
                {
                    lastKeyLen = s.Length();
                    return;
                }
                string text = s.ToString().Trim();
                if (KeyEt.SelectionStart < text.Length)
                {
                    return;
                }
                if (text.Length == 4 || text.Length == 9 || text.Length == 14)
                {
                    KeyEt.Text = text + "-";
                    KeyEt.SetSelection(KeyEt.Text.Length);
                }

                lastKeyLen = s.Length();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
                //throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                //throw new NotImplementedException();
            }
        }

        private class OnClickListenerAnonymousInnerClass : Java.Lang.Object, View.IOnClickListener
        {
            // public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void OnClick(View v)
            {
                string key = KeyEt.Text.ToString().Trim().ToUpper();
                if (TextUtils.IsEmpty(key))
                {
                    Toast.MakeText(Contexts, "序列号不能为空", ToastLength.Short).Show();
                    return;
                }
                request(key);
            }

        }

        private class OnClickListenerAnonymousInnerClass2 : Java.Lang.Object, View.IOnClickListener
        {
            //public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void OnClick(View view)
            {
                if (ActivityCompat.CheckSelfPermission(Contexts, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions((Activity)Contexts, new string[] { Manifest.Permission.WriteExternalStorage }, 100);
                    return;
                }
                string path = getSDPath();
                offLineActive(path);
            }

        }

        private class OnClickListenerAnonymousInnerClass3 : Java.Lang.Object, View.IOnClickListener
        {
            //public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void OnClick(View v)
            {
                if (activationDialog != null)
                {
                    activationDialog.Dismiss();
                }
            }

        }

        private void addLisenter()
        {
            KeyEt.AddTextChangedListener(new TextWatcherAnonymousInnerClass());
            activateBtn.SetOnClickListener(new OnClickListenerAnonymousInnerClass());

            btOffLineActive.SetOnClickListener(new OnClickListenerAnonymousInnerClass2());

            backBtn.SetOnClickListener(new OnClickListenerAnonymousInnerClass3());
        }


        private static void offLineActive(string path)
        {

            if (FaceSDK.AuthorityStatus == AndroidLicenser.ErrorCode.Success.Ordinal())
            {
                Toast.MakeText(Contexts, "已经激活成功", ToastLength.Long).Show();
                return;
            }

            string firstPath = path + "/" + "License.zip";
            if (fileIsExists(firstPath))
            {
                if (!TextUtils.IsEmpty(firstPath))
                {
                    ZipUtil.unzip(firstPath);
                }
                //            if (ZipUtil.isSuccess) {
                //                String secondPath = path + "/" + "Win.zip";
                //                if (!TextUtils.isEmpty(secondPath)) {
                //                    ZipUtil.unzip(secondPath);
                //                }
                //            }
                string keyPath = path + "/" + "license.key";
                string key = readFile(keyPath, "key");
                PreferencesUtil.putString("activate_key", key);
                string liscensePaht = path + "/" + "license.ini";
                string liscense = readFile(liscensePaht, "liscense");
                success = FileUitls.c(Contexts, FaceSDKManager.LICENSE_NAME, list);
                if (success)
                {
                    toast("激活成功");
                    FaceSDKManager.initStatus = FaceSDKManager.SDK_UNINIT;
                    FaceSDKManager.init(Contexts);
                }
                else
                {
                    toast("激活失败");
                }
            }
            else
            {
                toast("授权文件不存在!");
            }
        }

        // 读取文本文件中的内容
        public static string readFile(string strFilePath, string mark)
        {
            string path = strFilePath;
            string content = ""; // 文件内容字符串
                                 // 打开文件
            Java.IO.File file = new Java.IO.File(path);
            // 如果path是传递过来的参数，可以做一个非目录的判断
            if (file.IsDirectory)
            {
                Log.Debug("TestFile", "The File doesn't not exist.");
            }
            else
            {
                try
                {
                    var instream = new System.IO.FileStream(file.Path, System.IO.FileMode.Append);
                    if (instream != null)
                    {
                        InputStreamReader inputreader = new InputStreamReader(instream);
                        BufferedReader buffreader = new BufferedReader(inputreader);
                        string line;
                        // 分行读取
                        while ((line = buffreader.ReadLine()) != null)
                        {
                            content = line;
                            if (mark.Equals("liscense"))
                            {
                                list.Add(line);
                            }
                        }
                        instream.Close();
                    }
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    Log.Debug("TestFile", "The File doesn't not exist.");
                }
                catch (Java.IO.IOException e)
                {
                    Log.Debug("TestFile", e.Message);
                }
            }
            return content;
        }

        // 判断文件是否存在
        public static bool fileIsExists(string strFile)
        {
            try
            {
                Java.IO.File f = new Java.IO.File(strFile);
                if (!f.Exists())
                {
                    return false;
                }
            }
            catch (Java.Lang.Exception e)
            {
                return false;
            }

            return true;
        }

        public static string getSDPath()
        {
            Java.IO.File sdDir = null;
            bool sdCardExist = Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted); // 判断sd卡是否存在
            if (sdCardExist)
            {
                sdDir = Android.OS.Environment.ExternalStorageDirectory; // 获取跟目录
            }
            if (sdDir != null)
            {
                return sdDir.ToString();
            }
            return null;
        }

        private static void toast(string text)
        {
            handler.Post(new Runnable(() => { Toast.MakeText(Contexts, text, ToastLength.Long).Show(); }));
        }


        private int dip2px(int dip)
        {
            Resources resources = Contexts.Resources;
            int px = (int)TypedValue.ApplyDimension(
                    ComplexUnitType.Dip, dip, resources.DisplayMetrics);

            return px;
        }

        private static void request(string key)
        {

            System.Threading.Tasks.Task.Run(() => { netRequest(key); });
            Executors.NewSingleThreadExecutor().Submit(new Runnable(() => { }));

        }

        public class NetRequesta:NetRequest.RequestAdapter

        {

            string key;
            public NetRequesta(string _key)
            {
                key = _key;
            }

            public override string getURL()
            {
                return "https://ai.baidu.com/activation/key/activate";
            }

            public override string getRequeststring()
            {
                try
                {
                    JSONObject jsonObject = new JSONObject();
                    jsonObject.Put("deviceId", device);
                    jsonObject.Put("key", key);
                    jsonObject.Put("platformType", 2);
                    jsonObject.Put("version", "3.4.2");

                    return jsonObject.ToString();
                }
                catch (JSONException var10)
                {
                    var10.PrintStackTrace();
                    return null;
                }
            }

            public override void parseResponse(InputStream var1)
            {
                ByteArrayOutputStream oout = new ByteArrayOutputStream();
                byte[] buffer = new byte[1024];

                try
                {
                    int e;
                    while ((e = var1.Read(buffer)) > 0)
                    {
                        oout.Write(buffer, 0, e);
                    }
                    oout.Flush();
                    JSONObject json = new JSONObject(System.Text.Encoding.Default.GetString(oout.ToByteArray()));
                    Log.Info("wtf", "netRequest->" + json.ToString());
                    int errorCode = json.OptInt("error_code");
                    if (errorCode != 0)
                    {
                        string errorMsg = json.OptString("error_msg");
                        toast(errorMsg);
                    }
                    else
                    {
                        parse(json, key);
                    }
                }
                catch (Java.Lang.Exception e)
                {
                    toast("激活失败");
                }
                finally
                {
                    if (oout != null)
                    {
                        try
                        {
                            oout.Close();
                        }
                        catch (Java.IO.IOException var12)
                        {
                            var12.PrintStackTrace();
                        }
                    }
                }
            }
        }

        private static void netRequest(string key)
        {
            if (NetRequest.isConnected(Contexts))
            {
                bool success = NetRequest.request(new NetRequesta(key));

                if (!success)
                {
                    toast("激活失败");
                }
            }
            else
            {
                toast("没有连接网络");
            }
        }

        private static void parse(JSONObject json, string key)
        {
            bool success = false;
            JSONObject result = json.OptJSONObject("result");
            if (result != null)
            {
                string license = result.OptString("license");
                if (!TextUtils.IsEmpty(license))
                {
                    string[] licenses = license.Split(',');
                    if (licenses != null && licenses.Length == 2)
                    {
                        PreferencesUtil.putString("activate_key", key);
                        Java.Util.ArrayList list = new Java.Util.ArrayList();
                        list.Add(licenses[0]);
                        list.Add(licenses[1]);
                        success = FileUitls.c(Contexts, FaceSDKManager.LICENSE_NAME, list);
                    }
                }
            }

            if (success)
            {
                toast("激活成功");
                if (activationCallback != null)
                {
                    activationCallback.callback(true);
                    activationDialog.Dismiss();
                }
            }
            else
            {
                toast("激活失败");
            }
        }

        public interface ActivationCallback
        {
            void callback(bool success);
        }

        public class AllCapTransformationMethod : ReplacementTransformationMethod
        {

            private char[] lower = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
                'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            private char[] upper = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q',
                'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            private bool allUpper = false;

            public AllCapTransformationMethod(bool needUpper)
            {
                this.allUpper = needUpper;
            }


            protected override char[] GetOriginal()
            {
                if (allUpper)
                {
                    return lower;
                }
                else
                {
                    return upper;
                }
            }

            protected override char[] GetReplacement()
            {
                if (allUpper)
                {
                    return upper;
                }
                else
                {
                    return lower;
                }
            }
        }

    }
}