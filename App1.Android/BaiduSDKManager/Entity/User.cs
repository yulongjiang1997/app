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

namespace App1.Droid.BaiduSDKManager.Entity
{
    public class User
    {

        private String userId = "";

        private String userInfo = "";

        private String groupId = "";

        private long ctime;

        private long updateTime;

        private List<Feature> featureList = new List<Feature>();

        public String getUserId()
        {
            return userId;
        }

        public void setUserId(String userId)
        {
            this.userId = userId;
        }

        public String getUserInfo()
        {
            return userInfo;
        }

        public void setUserInfo(String userInfo)
        {
            this.userInfo = userInfo;
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

        public List<Feature> getFeatureList()
        {
            return featureList;
        }

        public void setFeatureList(List<Feature> featureList)
        {
            this.featureList = featureList;
        }
    }

}