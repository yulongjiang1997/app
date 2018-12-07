using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace App1.Droid.BaiduSDKManager.Entity
{
    public class Feature
    {

        private String faceToken = "";

        private byte[] feature;

        private String userId = "";

        private String groupId = "";

        private long ctime;

        private long updateTime;

        private String imageName = "";

        public String getFaceToken()
        {
            if (feature != null)
            {
                byte[] bbase = Base64.Encode(feature, Base64.NoWrap);
                faceToken = Encoding.Default.GetString(bbase);
            }
            return faceToken;
        }

        public void setFaceToken(String faceToken)
        {
            this.faceToken = faceToken;
        }

        public byte[] getFeature()
        {
            return feature;
        }

        public void setFeature(byte[] feature)
        {
            this.feature = feature;
        }

        public String getUserId()
        {
            return userId;
        }

        public void setUserId(String userId)
        {
            this.userId = userId;
        }

        public String getGroupId()
        {
            return groupId;
        }

        public void setGroupId(String groupId)
        {
            this.groupId = groupId;
        }

        public long getCtime()
        {
            return ctime;
        }

        public void setCtime(long ctime)
        {
            this.ctime = ctime;
        }

        public long getUpdateTime()
        {
            return updateTime;
        }

        public void setUpdateTime(long updateTime)
        {
            this.updateTime = updateTime;
        }

        public String getImageName()
        {
            return imageName;
        }

        public void setImageName(String imageName)
        {
            this.imageName = imageName;
        }
    }

}