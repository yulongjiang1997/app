using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace App1.Droid.BaiduSDKManager.DB
{
    public class DBHelper : SQLiteOpenHelper
    {
        private static string CREATE_TABLE_START_SQL = "CREATE TABLE IF NOT EXISTS ";
        private static string CREATE_TABLE_PRIMIRY_SQL = " integer primary key autoincrement,";
        private static string CREATE_TABLE = " CREATE TABLE ";
        private static string INTEGER_PRIMARY_KEY = " INTEGER PRIMARY KEY  NOT NULL  ";
        private static string LEFT_BRACKET = " ( ";
        private static string RIGHT_BRACKET = " ) ";
        private static string VARCHAR = " VARCHAR ";
        private static string INTEGER = " INTEGER ";
        private static string DEFAULT = " DEFAULT ";
        private static string DOT = " , ";
        private static string DEFAULT_ENPTY_STRING = " DEFAULT \"\" ";
        private static string DEFAULT_ZERO = " DEFAULT 0 ";
        private static string DEFAULT_ONE = " DEFAULT 1 ";
        /** 数据库名称 */
        private static string DB_NAME = "face.db";
        /** 数据库版本 */
        private static int VERSION = 1;
        /** 人脸特征表 */
        public static string TABLE_FEATURE = "feature";
        /** 用户组表 */
        public static string TABLE_USER_GROUP = "user_group";
        public static string TABLE_USER = "user";

        public DBHelper(Context context) : base(context, DB_NAME, null, VERSION)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            createTables(db);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            if (newVersion > oldVersion)
            {
                db.ExecSQL("DROP TABLE IF EXISTS " + TABLE_FEATURE);
                db.ExecSQL("DROP TABLE IF EXISTS " + TABLE_USER_GROUP);
                db.ExecSQL("DROP TABLE IF EXISTS " + TABLE_USER);

                OnCreate(db);
            }
        }
        public  void createTables(SQLiteDatabase db)
        {
            if (db == null || db.IsReadOnly)
            {
                db = WritableDatabase;
            }
            // 创建人脸特征表的SQL语句
            StringBuffer featureSql = new StringBuffer();
            featureSql.Append(CREATE_TABLE_START_SQL).Append(TABLE_FEATURE).Append(" ( ");
            featureSql.Append(" _id").Append(CREATE_TABLE_PRIMIRY_SQL);
            featureSql.Append(" face_token").Append(" varchar(128) default \"\" ,");
            featureSql.Append(" group_id").Append(" varchar(32) default \"\" ,");
            featureSql.Append(" user_id").Append(" varchar(32) default \"\" ,");
            featureSql.Append(" feature").Append(" blob   ,");
            featureSql.Append(" image_name").Append(" varchar(64) default \"\"  ,");
            featureSql.Append(" ctime").Append(" long ,");
            featureSql.Append(" update_time").Append(" long )");

            // 创建用户组表的SQL语句
            StringBuffer groupSql = new StringBuffer();
            groupSql.Append(CREATE_TABLE_START_SQL).Append(TABLE_USER_GROUP).Append(" ( ");
            groupSql.Append(" _id").Append(CREATE_TABLE_PRIMIRY_SQL);
            groupSql.Append(" group_id").Append(" varchar(32) default \"\" ,");
            groupSql.Append(" desc").Append(" varchar(32) default \"\"  ,");
            groupSql.Append(" ctime").Append(" long ,");
            groupSql.Append(" update_time").Append(" long )");

            // 创建用户表的SQL语句
            StringBuffer userSql = new StringBuffer();
            userSql.Append(CREATE_TABLE_START_SQL).Append(TABLE_USER).Append(" ( ");
            userSql.Append(" _id").Append(CREATE_TABLE_PRIMIRY_SQL);
            userSql.Append(" user_id").Append(" varchar(32) default \"\"   ,");
            userSql.Append(" user_info").Append(" varchar(32) default \"\"   ,");
            userSql.Append(" group_id").Append(" varchar(32) default \"\"   ,");

            userSql.Append(" ctime").Append(" long ,");
            userSql.Append(" update_time").Append(" long )");


            try
            {
                db.ExecSQL(groupSql.ToString());
                db.ExecSQL(userSql.ToString());
                db.ExecSQL(featureSql.ToString());
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }
    }

}