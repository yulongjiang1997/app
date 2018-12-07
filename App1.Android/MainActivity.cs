using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Baidu.Idl.Facesdk;
using Android.Content;
using App1.Droid.BaiduSDKManager;
using App1.Droid.BaiduSDKManager.Manager;
using App1.Droid.Activitys;
using Java.Util.Logging;
using System.Collections.Generic;
using App1.Droid.BaiduSDKManager.Utils;
using App1.Droid.BaiduSDKManager.DB;
using Android.Support.V4.App;
using Android;
using Com.Baidu.Idl.License;
using Android.Text;
using Java.IO;
using Android.Util;
using App1.Droid.BaiduSDKManager.Entity;
using App1.Droid.Activitys.Utils;

namespace App1.Droid
{
    //[Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    //public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    //{

    //    public MainActivity()
    //    {
    //    }
    //    public MainActivity(Context _context)
    //    {

    //    }
    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        FaceSDKManager.init(this);
    //        FaceSDKManager.getInstance().setSdkInitListener(new SdkInitListener(this));
    //        var a = FaceSDKManager.initStatus;
    //        base.OnCreate(savedInstanceState);
    //        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
    //        LoadApplication(new App());
    //        RgbDetectActivity rgbDetectActivity = new RgbDetectActivity();
    //        rgbDetectActivity.ini();
    //    }
    //}
    public class MainActivity : Activity, View.IOnClickListener
    {

        private Button imageMatchBtn;
        private Button videoMatchImageBtn;
        private Button videoIdentifyBtn;
        private Button userGroupManagerBtn;
        private Button livenessSettingBtn;
        private Button deviceActivateBtn;
        private Button rgbIrBtn;
        private Button btAttrTrack;
        private int count;
        private Button btMultiThread;
        private Button featureSettingBtn;

        List<string> list = new List<string>();
        private bool success = false;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            imageMatchBtn = (Button)FindViewById(Resource.Id.image_match_image_btn);
            videoMatchImageBtn = (Button)FindViewById(Resource.Id.video_match_image_btn);
            videoIdentifyBtn = (Button)FindViewById(Resource.Id.video_identify_faces_btn);
            userGroupManagerBtn = (Button)FindViewById(Resource.Id.user_groud_manager_btn);
            livenessSettingBtn = (Button)FindViewById(Resource.Id.liveness_setting_btn);
            deviceActivateBtn = (Button)FindViewById(Resource.Id.device_activate_btn);
            rgbIrBtn = (Button)FindViewById(Resource.Id.rgb_ir_btn);
            btAttrTrack = (Button)FindViewById(Resource.Id.bt_attr_track);
            btMultiThread = (Button)FindViewById(Resource.Id.bt_multiThread);
            featureSettingBtn = (Button)FindViewById(Resource.Id.feature_setting_btn);

            imageMatchBtn.SetOnClickListener(this);
            videoMatchImageBtn.SetOnClickListener(this);
            videoIdentifyBtn.SetOnClickListener(this);
            userGroupManagerBtn.SetOnClickListener(this);
            livenessSettingBtn.SetOnClickListener(this);
            deviceActivateBtn.SetOnClickListener(this);
            deviceActivateBtn.SetOnClickListener(this);
            rgbIrBtn.SetOnClickListener(this);
            btAttrTrack.SetOnClickListener(this);
            btMultiThread.SetOnClickListener(this);
            featureSettingBtn.SetOnClickListener(this);


            PreferencesUtil.initPrefs(this);
            // 使用人脸1：n时使用
            DBManager.getInstance().init(this);
            livnessTypeTip();
            //        FaceEnvironment faceEnvironment = new FaceEnvironment();
            //        // 模糊度范围 (0-1) 推荐小于0.7
            //        faceEnvironment.setBlurrinessThreshold(FaceEnvironment.VALUE_BLURNESS);
            //        // 光照范围 (0-1) 推荐大于40
            //        faceEnvironment.setIlluminationThreshold(FaceEnvironment.VALUE_BLURNESS);
            //        // 人脸yaw,pitch,row 角度，范围（-45，45），推荐-15-15
            //        faceEnvironment.setPitch(FaceEnvironment.VALUE_HEAD_PITCH);
            //        faceEnvironment.setRoll(FaceEnvironment.VALUE_HEAD_ROLL);
            //        faceEnvironment.setYaw(FaceEnvironment.VALUE_HEAD_YAW);
            //        // 最小检测人脸（在图片人脸能够被检测到最小值）80-200， 越小越耗性能，推荐120-200
            //        faceEnvironment.setMinFaceSize(FaceEnvironment.VALUE_MIN_FACE_SIZE);
            //        // 人脸置信度（0-1）推荐大于0.6
            //        faceEnvironment.setNotFaceThreshold(FaceEnvironment.VALUE_NOT_FACE_THRESHOLD);
            //        // 人脸遮挡范围 （0-1） 推荐小于0.5
            //        faceEnvironment.setOcclulationThreshold(FaceEnvironment.VALUE_OCCLUSION);
            //        // 是否进行质量检测,开启会降低性能
            //        faceEnvironment.setCheckQuality(false);
            //        FaceSDKManager.getInstance().getFaceDetector().setFaceEnvironment(faceEnvironment);
            FaceSDKManager.init(this);
            FaceSDKManager.getInstance().setSdkInitListener(new SdkInitListener());

        }

        protected override void OnResume()
        {
            base.OnResume();
            //        int type=PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_THREAD,GlobalFaceTypeModel.SINGLETHREAD);
            //        if (type==GlobalFaceTypeModel.SINGLETHREAD){
            //            FaceSDKManager.getInstance().init(this);
            //        }
        }


        protected override void OnStop()
        {
            base.OnStop();
        }


        public override void OnClick(View v)
        {
            if (v == deviceActivateBtn)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, 100);
                    return;
                }
                FaceSDKManager.showActivation();
                return;
            }

            if (FaceSDKManager.getInstance().InitStatus() == FaceSDKManager.SDK_UNACTIVATION)
            {
                toast("SDK还未激活，请先激活");
                return;
            }
            else if (FaceSDKManager.getInstance().InitStatus() == FaceSDKManager.SDK_UNINIT)
            {
                toast("SDK还未初始化完成，请先初始化");
                return;
            }
            else if (FaceSDKManager.getInstance().InitStatus() == FaceSDKManager.SDK_INITING)
            {
                toast("SDK正在初始化，请稍后再试");
                return;
            }
            if (v == imageMatchBtn)
            {
                Intent intent = new Intent(this, typeof(ImageMacthImageActivity));
                StartActivity(intent);
            }
            else if (v == videoMatchImageBtn)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 100);
                    return;
                }
                choiceMatchType();
            }
            else if (v == videoIdentifyBtn)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 100);
                    return;
                }
                showSingleAlertDialog();
            }
            else if (v == userGroupManagerBtn)
            {
                Intent intent = new Intent(this, typeof(UserGroupManagerActivity));
                StartActivity(intent);
            }
            else if (v == livenessSettingBtn)
            {
                Intent intent = new Intent(this, typeof(LivenessSettingActivity));
                StartActivity(intent);
            }
            else if (v == rgbIrBtn)
            {
                Intent intent = new Intent(this, typeof(QuestiongActivity));
                StartActivity(intent);
            }
            else if (v == btAttrTrack)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 100);
                    return;
                }
                choiceAttrTrackType();
            }
            else if (v == btMultiThread)
            {
                Intent intent = new Intent(this, typeof(MultiThreadActivity));
                StartActivity(intent);
                //            // 测试
                //            MultiThreadManager.getInstance().start(this);
            }
            else if (v == featureSettingBtn)
            {
                Intent intent = new Intent(this, typeof(FeatureSettingActivity));
                StartActivity(intent);
            }
        }

        private void offLineActive(String path)
        {

            if (FaceSDK.AuthorityStatus == AndroidLicenser.ErrorCode.Success.Ordinal())
            {
                Toast.MakeText(this, "已经激活成功", ToastLength.Long).Show();
                return;
            }

            String firstPath = path + "/" + "License.zip";
            if (fileIsExists(firstPath))
            {
                if (!TextUtils.IsEmpty(firstPath))
                {
                    ZipUtil.unzip(firstPath);
                }
                if (ZipUtil.isSuccess)
                {
                    String secondPath = path + "/" + "Win.zip";
                    if (!TextUtils.IsEmpty(secondPath))
                    {
                        ZipUtil.unzip(secondPath);
                    }
                }
                String keyPath = path + "/" + "license.key";
                String key = readFile(keyPath, "key");
                PreferencesUtil.putString("activate_key", key);
                String liscensePaht = path + "/" + "license.ini";
                String liscense = readFile(liscensePaht, "liscense");
                success = FileUitls.c(this, FaceSDKManager.LICENSE_NAME, list);
                if (success)
                {
                    toast("激活成功");
                    FaceSDKManager.initStatus = FaceSDKManager.SDK_UNINIT;
                    FaceSDKManager.init(this);
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
        public String readFile(String strFilePath, String mark)
        {
            String path = strFilePath;
            String content = ""; // 文件内容字符串
                                 // 打开文件
            File file = new File(path);
            // 如果path是传递过来的参数，可以做一个非目录的判断
            if (file.IsDirectory)
            {
                Log.Debug("TestFile", "The File doesn't not exist.");
            }
            else
            {
                try
                {
                    InputStream instream = new FileInputStream(file);
                    if (instream != null)
                    {
                        InputStreamReader inputreader = new InputStreamReader(instream);
                        BufferedReader buffreader = new BufferedReader(inputreader);
                        String line;
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
                catch (IOException e)
                {
                    Log.Debug("TestFile", e.Message);
                }
            }
            return content;
        }

        // 判断文件是否存在
        public bool fileIsExists(String strFile)
        {
            try
            {
                File f = new File(strFile);
                if (!f.Exists())
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        private AlertDialog alertDialog;
        private String[] items;

        public void showSingleAlertDialog()
        {

            List<Group> groupList = FaceApi.getInstance().getGroupList(0, 1000);
            if (groupList.Count <= 0)
            {
                Toast.MakeText(this, "还没有分组，请创建分组并添加用户", ToastLength.Short).Show();
                return;
            }
            items = new String[groupList.Count];
            for (int i = 0; i < groupList.Count; i++)
            {
                Group group = groupList[i];
                items[i] = group.getGroupId();
            }

            AlertDialog.Builder alertBuilder = new AlertDialog.Builder(this);
            alertBuilder.SetTitle("请选择分组groupID");
            //    alertBuilder.SetSingleChoiceItems(items, 0, new DialogInterface.OnClickListener() {
            //    public override void OnClick(DialogInterface arg0, int index)
            //    {
            //        Toast.MakeText(this, items[index], ToastLength.Short.Show();

            //        choiceIdentityType(items[index]);
            //        alertDialog.dismiss();
            //    }
            //});

            alertDialog = alertBuilder.Create();
            alertDialog.Show();
        }

        private void choiceMatchType()
        {
            int type = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                    .TYPE_NO_LIVENSS);
            if (type == LivenessSettingActivity.TYPE_NO_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：无活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoMatchImageActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：单目RGB活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoMatchImageActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_IR_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+IR活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbIrVideoMathImageActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_DEPTH_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+Depth活体", ToastLength.Long).Show();
                int CameraType = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
                Intent intent = null;
                if (CameraType == GlobalFaceTypeModel.ORBBEC)
                {
                    intent = new Intent(this, typeof(OrbbecVideoMatchImageActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.IMIMECT)
                {
                    intent = new Intent(this, typeof(IminectVideoMatchImageActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.ORBBECPRO)
                {
                    intent = new Intent(this, typeof(OrbbecProVideoMatchImageActivity));
                }
                StartActivity(intent);
            }
        }

        private void choiceIdentityType(String groupId)
        {
            int type = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                    .TYPE_NO_LIVENSS);
            if (type == LivenessSettingActivity.TYPE_NO_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：无活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoIdentityActivity));
                intent.PutExtra("group_id", groupId);
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：单目RGB活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoIdentityActivity));
                intent.PutExtra("group_id", groupId);
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_IR_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+IR活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbIrVideoIdentifyActivity));
                intent.PutExtra("group_id", groupId);
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_DEPTH_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+Depth活体", ToastLength.Long).Show();
                int CameraType = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
                Intent intent = null;
                if (CameraType == GlobalFaceTypeModel.ORBBEC)
                {
                    intent = new Intent(this, typeof(OrbbecVideoIdentifyActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.IMIMECT)
                {
                    intent = new Intent(this, typeof(IminectVideoIdentifyActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.ORBBECPRO)
                {
                    intent = new Intent(this, typeof(OrbbecProVideoIdentifyActivity));
                }
                if (intent != null)
                {
                    intent.PutExtra("group_id", groupId);
                    StartActivity(intent);
                }

            }
        }


        private void choiceAttrTrackType()
        {
            int type = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                    .TYPE_NO_LIVENSS);
            if (type == LivenessSettingActivity.TYPE_NO_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：无活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoAttributeTrackActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：单目RGB活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbVideoAttributeTrackActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_IR_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+IR活体", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(RgbIrVideoAttributeActivity));
                StartActivity(intent);
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_DEPTH_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+Depth活体", ToastLength.Long).Show();
                int CameraType = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
                Intent intent = null;
                if (CameraType == GlobalFaceTypeModel.ORBBEC)
                {
                    intent = new Intent(this, typeof(OrbbecVideoAttributeActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.IMIMECT)
                {
                    intent = new Intent(this, typeof(IminectVideoAtrributeActivity));
                }
                else if (CameraType == GlobalFaceTypeModel.ORBBECPRO)
                {
                    intent = new Intent(this, typeof(OrbbecProVideoAttributeActivity));
                }
                StartActivity(intent);
            }
        }


        private void livnessTypeTip()
        {
            int type = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                    .TYPE_NO_LIVENSS);

            if (type == LivenessSettingActivity.TYPE_NO_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：无活体, 请选用普通USB摄像头", ToastLength.Long).Show();
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：单目RGB活体, 请选用普通USB摄像头", ToastLength.Long).Show();
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_IR_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+IR活体, 请选用RGB+IR摄像头",
                        ToastLength.Long).Show();
            }
            else if (type == LivenessSettingActivity.TYPE_RGB_DEPTH_LIVENSS)
            {
                Toast.MakeText(this, "当前活体策略：双目RGB+Depth活体，请选用RGB+Depth摄像头", ToastLength.Long).Show();
            }
        }

        private void toast(string text)
        {
            handler.Post(() => { Toast.MakeText(this, text, ToastLength.Long).Show(); });
            //    handler.post(new Runnable() {
            //    @Override
            //    public void run()
            //    {
            //       
            //    }
            //});
        }

        private Android.OS.Handler handler = new Android.OS.Handler(Looper.MainLooper);



        protected override void OnDestroy()
        {
            base.OnDestroy();
        }


        public String getSDPath()
        {
            File sdDir = null;
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
    }
}