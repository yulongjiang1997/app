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
    public class Group
    {

        private string groupId = "";
        private string desc = "";

        public string getGroupId()
        {
            return groupId;
        }

        public void setGroupId(string groupId)
        {
            this.groupId = groupId;
        }

        public string getDesc()
        {
            return desc;
        }

        public void setDesc(string desc)
        {
            this.desc = desc;
        }
    }
}