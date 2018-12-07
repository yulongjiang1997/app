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
using App1.Droid.Activitys.Utils;
using App1.Droid.BaiduSDKManager;
using App1.Droid.BaiduSDKManager.Utils;

namespace App1.Droid.Activitys
{
    public class LivenessSettingActivity : Activity, View.IOnClickListener
    {

        public static int TYPE_NO_LIVENSS = 1;
        public static int TYPE_RGB_LIVENSS = 2;
        public static int TYPE_RGB_IR_LIVENSS = 3;
        public static int TYPE_RGB_DEPTH_LIVENSS = 4;
        public static int TYPE_RGB_IR_DEPTH_LIVENSS = 5;
        public static String TYPE_LIVENSS = "TYPE_LIVENSS";
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private RadioButton radioButton5;
        private RadioGroup livenessRg;
        private Button confirmBtn;
        private int livenessType = TYPE_NO_LIVENSS;

        private RadioGroup rgCamera;
        private RadioButton rbOrbbec;
        private RadioButton rbIminect;
        private RadioButton rbOrbbecPro;

        private LinearLayout linearCamera;


        protected void onCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.liveness_setting_layout);
            findView();

            int livenessType = PreferencesUtil.getInt(TYPE_LIVENSS, TYPE_NO_LIVENSS);
            int cameraType = PreferencesUtil.getInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
            defaultLiveness(livenessType);
            defaultCamera(cameraType);
        }

        private void findView()
        {
            linearCamera =  (LinearLayout)FindViewById(Resource.Id.linear_camera);
            radioButton1 = (RadioButton)FindViewById(Resource.Id.no_liveness_rb);
            radioButton2 = (RadioButton)FindViewById(Resource.Id.rgb_liveness_rb);
            radioButton3 = (RadioButton)FindViewById(Resource.Id.rgb_depth_liveness_rb);
            radioButton4 = (RadioButton)FindViewById(Resource.Id.rgb_ir_liveness_rb);
            radioButton5 = (RadioButton)FindViewById(Resource.Id.rgb_ir_depth_liveness_rb);
            livenessRg = (RadioGroup)FindViewById(Resource.Id.liveness_rg);
            confirmBtn = (Button)FindViewById(Resource.Id.confirm_btn);
            confirmBtn.SetOnClickListener(this);
            rgCamera = (RadioGroup)FindViewById(Resource.Id.rg_camera);
            rbOrbbec = (RadioButton)FindViewById(Resource.Id.rb_orbbec);
            rbIminect = (RadioButton)FindViewById(Resource.Id.rb_iminect);
            rbOrbbecPro = (RadioButton)FindViewById(Resource.Id.rb_orbbec_pro);

            //    livenessRg.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {

            //        public void onCheckedChanged(RadioGroup rg, int checkedId)
            //    {
            //        switch (checkedId)
            //        {
            //            case R.id.no_liveness_rb:
            //                linearCamera.setVisibility(View.GONE);
            //                PreferencesUtil.putInt(TYPE_LIVENSS, TYPE_NO_LIVENSS);
            //                break;
            //            case R.id.rgb_liveness_rb:
            //                linearCamera.setVisibility(View.GONE);
            //                PreferencesUtil.putInt(TYPE_LIVENSS, TYPE_RGB_LIVENSS);
            //                break;
            //            case R.id.rgb_ir_liveness_rb:
            //                linearCamera.setVisibility(View.GONE);
            //                PreferencesUtil.putInt(TYPE_LIVENSS, TYPE_RGB_IR_LIVENSS);
            //                break;
            //            case R.id.rgb_depth_liveness_rb:
            //                linearCamera.setVisibility(View.VISIBLE);
            //                PreferencesUtil.putInt(TYPE_LIVENSS, TYPE_RGB_DEPTH_LIVENSS);
            //                break;
            //            case R.id.rgb_ir_depth_liveness_rb:
            //                linearCamera.setVisibility(View.GONE);
            //                PreferencesUtil.putInt(TYPE_LIVENSS, TYPE_RGB_IR_DEPTH_LIVENSS);
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //});
            //        rgCamera.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {

            //            public void onCheckedChanged(RadioGroup group, int i)
            //    {
            //        switch (i)
            //        {
            //            case R.id.rb_orbbec:
            //                PreferencesUtil.putInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBEC);
            //                break;
            //            case R.id.rb_iminect:
            //                PreferencesUtil.putInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.IMIMECT);
            //                break;
            //            case R.id.rb_orbbec_pro:
            //                PreferencesUtil.putInt(GlobalFaceTypeModel.TYPE_CAMERA, GlobalFaceTypeModel.ORBBECPRO);
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //});
        }

        private void defaultLiveness(int livenessType)
        {
            if (livenessType == TYPE_NO_LIVENSS)
            {
                radioButton1.Checked=true;
            }
            else if (livenessType == TYPE_RGB_LIVENSS)
            {
                radioButton2.Checked=true;
            }
            else if (livenessType == TYPE_RGB_DEPTH_LIVENSS)
            {
                radioButton3.Checked=true;
            }
            else if (livenessType == TYPE_RGB_IR_LIVENSS)
            {
                radioButton4.Checked=true;
            }
            else if (livenessType == TYPE_RGB_IR_DEPTH_LIVENSS)
            {
                radioButton5.Checked=true;
            }
        }

        private void defaultCamera(int cameraType)
        {
            if (cameraType == GlobalFaceTypeModel.ORBBEC)
            {
                rbOrbbec.Checked=true;
            }
            else if (cameraType == GlobalFaceTypeModel.IMIMECT)
            {
                rbIminect.Checked=true;
            }
            else if (cameraType == GlobalFaceTypeModel.ORBBECPRO)
            {
                rbOrbbecPro.Checked=true;
            }
        }

        
    public void OnClick(View v)
        {
            if (v == confirmBtn)
            {
                Finish();
            }
        }
    }
}