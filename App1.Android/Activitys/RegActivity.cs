using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Activitys;
using App1.Droid.Activitys.Utils;
using App1.Droid.BaiduSDKManager.DB;
using App1.Droid.BaiduSDKManager.Entity;
using App1.Droid.BaiduSDKManager.Face;
using App1.Droid.BaiduSDKManager.Utils;
using Java.IO;
using Java.Util.Regex;

namespace App1.Droid.Activitys
{
    public class RegActivity : Activity, View.IOnClickListener
    {

        public static int SOURCE_REG = 1;
        private static int REQUEST_CODE_PICK_IMAGE = 1000;
        private static int REQUEST_CODE_AUTO_DETECT = 100;
        private EditText usernameEt;
        // private EditText groupIdEt;
        private Spinner groupIdSpinner;
        private ImageView avatarIv;
        private Button autoDetectBtn;
        private Button fromAlbumButton;
        private Button submitButton;

        // 注册时使用人脸图片路径。
        private String faceImagePath;

        // 从相机识别时使用。
        private FaceDetectManager detectManager;
        private List<string> groupIds = new List<string>();
        private string groupId = "";


        protected void onCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            detectManager = new FaceDetectManager(ApplicationContext);
            SetContentView(Resource.Layout.activity_reg);

            usernameEt = (EditText)FindViewById(Resource.Id.username_et);
            // groupIdEt = (EditText) FindViewById(R.id.group_id_et);
            groupIdSpinner = (Spinner)FindViewById(Resource.Id.spinner);
            avatarIv = (ImageView)FindViewById(Resource.Id.avatar_iv);
            autoDetectBtn = (Button)FindViewById(Resource.Id.auto_detect_btn);
            fromAlbumButton = (Button)FindViewById(Resource.Id.pick_from_album_btn);
            submitButton = (Button)FindViewById(Resource.Id.submit_btn);
            submitButton.Visibility = ViewStates.Gone;

            autoDetectBtn.SetOnClickListener(this);
            fromAlbumButton.SetOnClickListener(this);
            submitButton.SetOnClickListener(this);

            //    groupIdSpinner.setOnItemSelectedListener(new Spinner.OnItemSelectedListener()
            //    {//选择item的选择点击监听事件
            //    public void onItemSelected(AdapterView<?> arg0, View arg1,
            //                               int arg2, long arg3)
            //    {
            //        if (arg2 < groupIds.size())
            //        {
            //            groupId = groupIds.get(arg2);
            //        }
            //    }

            //    public void onNothingSelected(AdapterView<?> arg0)
            //    {

            //    }
            //});

            init();
        }

        private void init()
        {
            List<Group> groupList = DBManager.getInstance().queryGroups(0, 1000);
            foreach (Group group in groupList)
            {
                groupIds.Add(group.getGroupId());
            }

            if (groupIds.Count() > 0)
            {
                groupId = groupIds[0];
            }
            ArrayAdapter arrayAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, groupIds);
            arrayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            groupIdSpinner.Adapter = arrayAdapter;
        }


        public void OnClick(View v)
        {
            if (v == autoDetectBtn)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage)
                        != Permission.Granted || ActivityCompat.CheckSelfPermission(this, Manifest
                        .Permission.Camera) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[]{Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.Camera}, 100);
                    return;
                }
                avatarIv.SetImageResource(Resource.Drawable.avatar);
                faceImagePath = null;
                int type = PreferencesUtil.getInt(LivenessSettingActivity.TYPE_LIVENSS, LivenessSettingActivity
                        .TYPE_NO_LIVENSS);
                if (type == LivenessSettingActivity.TYPE_NO_LIVENSS || type == LivenessSettingActivity.TYPE_RGB_LIVENSS)
                {
                    Intent intent = new Intent(this, typeof(RgbDetectActivity));
                    intent.PutExtra("source", SOURCE_REG);
                    StartActivityForResult(intent, REQUEST_CODE_AUTO_DETECT);
                }
                else if (type == LivenessSettingActivity.TYPE_RGB_IR_LIVENSS)
                {
                    Intent intent = new Intent(this, typeof(RgbIrLivenessActivity));
                    intent.PutExtra("source", SOURCE_REG);
                    StartActivityForResult(intent, REQUEST_CODE_AUTO_DETECT);
                }
                //else if (type == LivenessSettingActivity.TYPE_RGB_DEPTH_LIVENSS)
                //{
                //    int cameraType = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
                //    Intent intent3 = null;
                //    if (cameraType == GlobalFaceTypeModel.ORBBEC)
                //    {
                //        intent3 = new Intent(this, typeof(OrbbecLivenessDetectActivity));
                //    }
                //    else if (cameraType == GlobalFaceTypeModel.IMIMECT)
                //    {
                //        intent3 = new Intent(this, typeof(IminectLivenessDetectActivity));
                //    }
                //    else if (cameraType == GlobalFaceTypeModel.ORBBECPRO)
                //    {
                //        intent3 = new Intent(this, typeof(OrbbecProLivenessDetectActivity));
                //    }
                //    if (intent3 != null)
                //    {
                //        intent3.PutExtra("source", SOURCE_REG);
                //        StartActivityForResult(intent3, REQUEST_CODE_AUTO_DETECT);
                //    }
                //}
            }
            else if (v == fromAlbumButton)
            {

                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage)
                        != Permission.Granted)
                {
                    //ActivityCompat.requestPermissions(this, new String[] { Manifest.permission.READ_EXTERNAL_STORAGE }, 100);
                    return;
                }
                avatarIv.SetImageResource(Resource.Drawable.avatar);
                faceImagePath = null;
                Intent intent = new Intent(Intent.ActionPick);
                intent.SetType("image/*");
                StartActivityForResult(intent, REQUEST_CODE_PICK_IMAGE);

            }
            else if (v == submitButton)
            {
                register(faceImagePath);
            }
        }


        protected void onActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == REQUEST_CODE_AUTO_DETECT && data != null)
            {
                faceImagePath = data.GetStringExtra("file_path");

                Bitmap bitmap = BitmapFactory.DecodeFile(faceImagePath);
                avatarIv.SetImageBitmap(bitmap);
                submitButton.Visibility = ViewStates.Visible;
            }
            else if (requestCode == REQUEST_CODE_PICK_IMAGE && resultCode == Result.Ok)
            {
                if (data != null)
                {
                    var uri = data.Data;
                    //            String filePath = getRealPathFromURI(uri);
                    String filePath = imageUriToFile(uri);
                    detect(filePath);
                }
            }
        }

        // 从相册检测。
        private void detect(string filePath)
        {
            FileImageSource fileImageSource = new FileImageSource();
            fileImageSource.setFilePath(filePath);
            detectManager.setImageSource(fileImageSource);
            detectManager.setUseDetect(true);
            //            detectManager.setOnFaceDetectListener(new FaceDetectManager.OnFaceDetectListener()
            //            {


            //            public void onDetectFace(int status, FaceInfo[] faces, ImageFrame frame)
            //            {
            //                if (faces != null && status != FaceTracker.ErrCode.NO_FACE_DETECTED.ordinal()
            //                        && status != FaceTracker.ErrCode.UNKNOW_TYPE.ordinal())
            //                {
            //                    final Bitmap cropBitmap = FaceCropper.getFace(frame.getArgb(), faces[0], frame.getWidth());
            //                    handler.post(new Runnable()
            //                    {


            //                        public void run()
            //                    {
            //                        avatarIv.setImageBitmap(cropBitmap);
            //                    }
            //                });

            //            // File file = File.createTempFile(UUID.randomUUID().toString() + "", ".jpg");
            //            File faceDir = FileUitls.getFaceDirectory();
            //            if (faceDir != null)
            //            {
            //                String imageName = UUID.randomUUID().toString();
            //                File file = new File(faceDir, imageName);
            //                // 压缩人脸图片至300 * 300，减少网络传输时间
            //                ImageUtils.resize(cropBitmap, file, 300, 300);
            //                RegActivity.this.faceImagePath = file.getAbsolutePath();
            //                submitButton.setVisibility(View.VISIBLE);
            //            }
            //            else
            //            {
            //                toast("注册人脸目录未找到");
            //            }
            //        } else {
            //                    toast("未检测到人脸，可能原因：人脸太小（必须大于最小检测人脸minFaceSize），或者人脸角度太大，人脸不是朝上");
            //    }
            //}
            //        });
            detectManager.start();
        }

        private string getRealPathFromURI(Android.Net.Uri contentURI)
        {
            string result = null;
            ICursor cursor = null;
            try
            {
                cursor = ContentResolver.Query(contentURI, null, null, null, null);
                if (cursor == null)
                { // Source is Dropbox or other similar local file path
                    result = contentURI.Path;
                }
                else
                {
                    cursor.MoveToFirst();
                    int idx = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);
                    result = cursor.GetString(idx);
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }

            return result;
        }


        public String imageUriToFile(Android.Net.Uri uri)
        {
            if (null == uri)
            {
                return null;
            }
            string scheme = uri.Scheme;
            String data = null;
            if (scheme == null)
            {
                data = uri.Path;
            }
            else if (ContentResolver.SchemeFile.Equals(scheme))
            {
                data = uri.Path;
            }
            else if (ContentResolver.SchemeContent.Equals(scheme))
            {
                ICursor cursor = ContentResolver.Query(uri,
                        new string[] { MediaStore.Images.ImageColumns.Data },
                        null, null, null);
                if (null != cursor)
                {
                    if (cursor.MoveToFirst())
                    {
                        int index = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);
                        if (index > -1)
                        {
                            data = cursor.GetString(index);
                        }
                    }
                    cursor.Close();
                }
            }
            return data;
        }


        private File target;

        private void register(string filePath)
        {

            string username = usernameEt.Text.ToString().Trim();
            if (TextUtils.IsEmpty(username))
            {
                Toast.MakeText(this, "userid不能为空", ToastLength.Short).Show();
                return;
            }
            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile("^[0-9a-zA-Z_-]{1,}$");
            Matcher matcher = pattern.Matcher(username);
            if (!matcher.Matches())
            {
                Toast.MakeText(this, "userid由数字、字母、下划线中的一个或者多个组合", ToastLength.Short).Show();
                return;
            }

            // final String groupId = groupIdEt.getText().toString().trim();
            if (TextUtils.IsEmpty(groupId))
            {
                Toast.MakeText(this, "分组groupId为空", ToastLength.Short).Show();
                return;
            }
            matcher = pattern.Matcher(username);
            if (!matcher.Matches())
            {
                Toast.MakeText(this, "groupId由数字、字母、下划线中的一个或者多个组合", ToastLength.Short).Show();
                return;
            }
            /*
             * 用户id（由数字、字母、下划线组成），长度限制128B
             * uid为用户的id,百度对uid不做限制和处理，应该与您的帐号系统中的用户id对应。
             *
             */
            string uid = Guid.NewGuid().ToString();
            // String uid = 修改为自己用户系统中用户的id;

            if (TextUtils.IsEmpty(faceImagePath))
            {
                Toast.MakeText(this, "人脸文件不存在", ToastLength.Long).Show();
                return;
            }
            File file = new File(filePath);
            if (!file.Exists())
            {
                Toast.MakeText(this, "人脸文件不存在", ToastLength.Long).Show();
                return;
            }


            User user = new User();
            user.setUserId(uid);
            user.setUserInfo(username);
            user.setGroupId(groupId);

            //    Executors.newSingleThreadExecutor().submit(new Runnable()
            //    {



            //    public void run()
            //    {
            //        ARGBImg argbImg = FeatureUtils.getARGBImgFromPath(filePath);
            //        byte[] bytes = new byte[2048];
            //        int ret = 0;
            //        int type = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_MODEL, GlobalFaceTypeModel.RECOGNIZE_LIVE);
            //        if (type == GlobalFaceTypeModel.RECOGNIZE_LIVE)
            //        {
            //            ret = FaceSDKManager.getInstance().getFaceFeature().faceFeature(argbImg, bytes, 50);
            //        }
            //        else if (type == GlobalFaceTypeModel.RECOGNIZE_ID_PHOTO)
            //        {
            //            ret = FaceSDKManager.getInstance().getFaceFeature().faceFeatureForIDPhoto(argbImg, bytes, 50);
            //        }
            //        if (ret == FaceDetector.NO_FACE_DETECTED)
            //        {
            //            toast("人脸太小（必须打于最小检测人脸minFaceSize），或者人脸角度太大，人脸不是朝上");
            //        }
            //        else if (ret != -1)
            //        {
            //            Feature feature = new Feature();
            //            feature.setGroupId(groupId);
            //            feature.setUserId(uid);
            //            feature.setFeature(bytes);
            //            feature.setImageName(file.getName());

            //            user.getFeatureList().add(feature);

            //            //                   target = new File(Environment.getExternalStorageDirectory().getAbsolutePath() + "/chaixiaogangFeature2");
            //            //                   Utils.saveToFile(target,"feature2.txt",bytes);

            //            if (FaceApi.getInstance().userAdd(user))
            //            {
            //                toast("注册成功");
            //                finish();
            //            }
            //            else
            //            {
            //                toast("注册失败");
            //            }

            //        }
            //        else
            //        {
            //            toast("抽取特征失败");
            //        }
            //    }
            //});
        }


        private void toast(string text)
        {
            //    handler.post(new Runnable()
            //    {


            //    public void run()
            //    {
            //        Toast.MakeText(RegActivity.this, text, Toast.LENGTH_LONG).show();
            //    }
            //});
        }

        private Handler handler = new Handler(Looper.MainLooper);
    }
}