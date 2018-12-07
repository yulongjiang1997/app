using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.BaiduSDKManager.Entity;
using App1.Droid.BaiduSDKManager.Face;
using App1.Droid.BaiduSDKManager.Utils;
using Java.Lang;
using Java.Util.Concurrent.Atomic;
using Java.Util.Concurrent.Locks;

namespace App1.Droid.BaiduSDKManager.DB
{
    public class DBManager
    {
        /** The constant TAG */
        private static string TAG = "DBManager";

        private AtomicInteger mOpenCounter = new AtomicInteger();
        private static DBManager instance;
        private static SQLiteOpenHelper mDBHelper;
        private SQLiteDatabase mDatabase;
        private bool allowTransaction = true;
        private ILock writeLock = new ReentrantLock();
        private volatile bool writeLocked = false;
        /**
         * 获取DBManager 实例
         *
         * @return DBManager
         */
        public static DBManager getInstance()
        {
            if (instance == null)
            {
                instance = new DBManager();
            }
            return instance;
        }
        public void release()
        {
            if (mDBHelper != null)
            {
                mDBHelper.Close();
                mDBHelper = null;
            }
            instance = null;
        }
        public void init(Context context)
        {
            if (context == null)
            {
                return;
            }

            if (mDBHelper == null)
            {
                mDBHelper = new DBHelper(context.ApplicationContext);
            }
        }
        /**
         * 打开数据库
         *
         * @return SQLiteDatabase
         */
        public SQLiteDatabase openDatabase()
        {
            if (mOpenCounter.IncrementAndGet() == 1)
            {
                // Opening new database
                try
                {
                    mDatabase = mDBHelper.WritableDatabase;
                }
                catch (Java.Lang.Exception)
                {
                    mDatabase = mDBHelper.ReadableDatabase;
                }
            }
            return mDatabase;
        }
        /**
         * 关闭数据库
         */
        public void closeDatabase()
        {
            if (mOpenCounter.DecrementAndGet() == 0)
            {
                // Closing database
                mDatabase.Close();
            }
        }

        public bool addGroup(Group group)
        {
            if (mDBHelper == null)
            {
                return false;
            }

            mDatabase = mDBHelper.WritableDatabase;
            ContentValues cv = new ContentValues();
            cv.Put("group_id", group.getGroupId());
            cv.Put("desc", group.getDesc() == null ? "" : group.getDesc());
            cv.Put("update_time", DateTime.Now.Millisecond);
            cv.Put("ctime", DateTime.Now.Millisecond);

            long rowId = -1;
            try
            {
                rowId = mDatabase.Insert(DBHelper.TABLE_USER_GROUP, null, cv);
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            if (rowId < 0)
            {
                return false;
            }
            LogUtils.i("insert group success:" + rowId);
            return true;
        }

        public bool addUser(User user)
        {
            if (mDBHelper == null)
            {
                return false;
            }
            try
            {
                mDatabase = mDBHelper.WritableDatabase;
                beginTransaction(mDatabase);
                ContentValues cv = new ContentValues();
                cv.Put("user_id", user.getUserId());
                cv.Put("user_info", user.getUserInfo());
                cv.Put("group_id", user.getGroupId());
                cv.Put("update_time", DateTime.Now.Millisecond);
                cv.Put("ctime", DateTime.Now.Millisecond);

                long rowId = mDatabase.Insert(DBHelper.TABLE_USER, null, cv);
                if (rowId < 0)
                {
                    return false;
                }

                foreach (Feature feature in user.getFeatureList())
                {
                    if (!addFeature(feature, mDatabase))
                    {
                        return false;
                    }
                }

                setTransactionSuccessful(mDatabase);
                LogUtil.i("info", "insert user success:" + rowId);
            }
            catch (Java.Lang.Exception)
            {
                endTransaction(mDatabase);
            }

            return true;
        }


        public bool addFeature(Feature feature)
        {
            if (mDBHelper == null)
            {
                return false;
            }
            mDatabase = mDBHelper.WritableDatabase;
            ContentValues cv = new ContentValues();
            cv.Put("face_token", feature.getFaceToken());
            cv.Put("feature", feature.getFeature());
            cv.Put("user_id", feature.getUserId());
            cv.Put("group_id", feature.getGroupId());
            cv.Put("update_time", DateTime.Now.Millisecond);
            cv.Put("ctime", DateTime.Now.Millisecond);


            if (mDatabase.Insert(DBHelper.TABLE_FEATURE, null, cv) < 0)
            {
                return false;
            }

            return true;
        }

        public bool addFeature(Feature feature, SQLiteDatabase mDatabase)
        {

            ContentValues cv = new ContentValues();
            cv.Put("face_token", feature.getFaceToken());
            cv.Put("feature", feature.getFeature());
            cv.Put("user_id", feature.getUserId());
            cv.Put("group_id", feature.getGroupId());
            cv.Put("image_name", feature.getImageName());
            cv.Put("update_time", DateTime.Now.Millisecond);
            cv.Put("ctime", DateTime.Now.Millisecond);

            if (mDatabase.Insert(DBHelper.TABLE_FEATURE, null, cv) < 0)
            {
                return false;
            }

            return true;
        }

        public List<Feature> queryFeatureByUeserId(string userId)
        {
            List<Feature> featureList = new List<Feature>();
            ICursor cursor = null;

            try
            {
                if (mDBHelper == null)
                {
                    return featureList;
                }
                SQLiteDatabase db = mDBHelper.ReadableDatabase;
                string where = "user_id = ? ";
                string[] whereValue = { userId };
                cursor = db.Query(DBHelper.TABLE_FEATURE, null, where, whereValue, null, null, null);
                while (cursor != null && cursor.Count > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string faceToken = cursor.GetString(cursor.GetColumnIndex("face_token"));
                    byte[] featureContent = cursor.GetBlob(cursor.GetColumnIndex("feature"));
                    string groupId = cursor.GetString(cursor.GetColumnIndex("group_id"));
                    string imageName = cursor.GetString(cursor.GetColumnIndex("image_name"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));

                    Feature feature = new Feature();
                    feature.setFaceToken(faceToken);
                    feature.setFeature(featureContent);
                    feature.setCtime(ctime);
                    feature.setUpdateTime(updateTime);
                    feature.setGroupId(groupId);
                    feature.setUserId(userId);
                    feature.setImageName(imageName);
                    featureList.Add(feature);
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return featureList;
        }

        public List<Feature> queryFeatureByGroupId(string groupId)
        {
            List<Feature> featureList = new List<Feature>();
            ICursor cursor = null;

            try
            {
                if (mDBHelper == null)
                {
                    return featureList;
                }
                SQLiteDatabase db = mDBHelper.ReadableDatabase;
                string where = "group_id = ? ";
                string[] whereValue = { groupId };
                cursor = db.Query(DBHelper.TABLE_FEATURE, null, where, whereValue, null, null, null);
                while (cursor != null && cursor.Count > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string faceToken = cursor.GetString(cursor.GetColumnIndex("face_token"));
                    byte[] featureContent = cursor.GetBlob(cursor.GetColumnIndex("feature"));
                    string userId = cursor.GetString(cursor.GetColumnIndex("user_id"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));
                    string imageName = cursor.GetString(cursor.GetColumnIndex("image_name"));

                    Feature feature = new Feature();
                    feature.setFaceToken(faceToken);
                    feature.setFeature(featureContent);
                    feature.setCtime(ctime);
                    feature.setUpdateTime(updateTime);
                    feature.setGroupId(groupId);
                    feature.setUserId(userId);
                    feature.setImageName(imageName);
                    featureList.Add(feature);
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return featureList;
        }

        public byte[] queryFeature(string faceToken)
        {
            byte[] feature = null;
            ICursor cursor = null;

            try
            {
                if (mDBHelper == null)
                {
                    return feature;
                }
                SQLiteDatabase db = mDBHelper. ReadableDatabase ;
                string where = "face_token = ? ";
                string[] whereValue = { faceToken };
                cursor = db.Query(DBHelper.TABLE_FEATURE, null, where, whereValue, null, null, null);
                if (cursor != null && cursor. Count  > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    feature = cursor.GetBlob(cursor.GetColumnIndex("feature"));

                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return feature;
        }

        public List<Feature> queryFeature(string groupId, string userId)
        {
             List<Feature> featureList = new  List<Feature>();
            ICursor cursor = null;

            try
            {
                if (mDBHelper == null)
                {
                    return featureList;
                }
                SQLiteDatabase db = mDBHelper. ReadableDatabase ;
                string where = "group_id = ? and user_id = ?";
                string[] whereValue = { groupId, userId };
                cursor = db.Query(DBHelper.TABLE_FEATURE, null, where, whereValue, null, null, null);
                while (cursor != null && cursor. Count  > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string faceToken = cursor.GetString(cursor.GetColumnIndex("face_token"));
                    byte[] featureContent = cursor.GetBlob(cursor.GetColumnIndex("feature"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));
                    string imageName = cursor.GetString(cursor.GetColumnIndex("image_name"));

                    Feature feature = new Feature();
                    feature.setFaceToken(faceToken);
                    feature.setFeature(featureContent);
                    feature.setCtime(ctime);
                    feature.setUpdateTime(updateTime);
                    feature.setGroupId(groupId);
                    feature.setUserId(userId);
                    feature.setImageName(imageName);
                    featureList.Add(feature);
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return featureList;
        }

        public User queryUser(string groupId, string userId)
        {
            ICursor cursor = null;

            try
            {
                if (mDBHelper == null)
                {
                    return null;
                }
                SQLiteDatabase db = mDBHelper.ReadableDatabase;
                string where = "user_id = ? and group_id = ? ";
                string[] whereValue = { userId, groupId };
                cursor = db.Query(DBHelper.TABLE_USER, null, where, whereValue, null, null, null);
                if (cursor != null && cursor.Count > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string userInfo = cursor.GetString(cursor.GetColumnIndex("user_info"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));

                    User user = new User();
                    user.setUserId(userId);
                    user.setGroupId(groupId);
                    user.setUserInfo(userInfo);
                    user.setCtime(ctime);
                    user.setUpdateTime(updateTime);
                    return user;
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return null;
        }

        public List<User> queryUserByGroupId(string groupId)
        {
            ICursor cursor = null;
            List<User> users = new List<User>();
            try
            {
                if (mDBHelper == null)
                {
                    return null;
                }
                SQLiteDatabase db = mDBHelper.ReadableDatabase;
                string where = "group_id = ? ";
                string[] whereValue = { groupId };
                cursor = db.Query(DBHelper.TABLE_USER, null, where, whereValue, null, null, null);
                while (cursor != null && cursor.Count > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string userInfo = cursor.GetString(cursor.GetColumnIndex("user_info"));
                    string userId = cursor.GetString(cursor.GetColumnIndex("user_id"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));

                    User user = new User();
                    user.setUserId(userId);
                    user.setGroupId(groupId);
                    user.setUserInfo(userInfo);
                    user.setCtime(ctime);
                    user.setUpdateTime(updateTime);
                    users.Add(user);
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return users;
        }

        public List<Group> queryGroups(int start, int offset)
        {
            ICursor cursor = null;
            List<Group> groupList = new List<Group>();
            try
            {
                if (mDBHelper == null)
                {
                    return null;
                }
                SQLiteDatabase db = mDBHelper.ReadableDatabase;
                //            String limit = "_id asc limit " +  start + " offset " + offset;
                string limit = start + " , " + offset;
                cursor = db.Query(DBHelper.TABLE_USER_GROUP, null, null, null, null, null, null, limit);
                while (cursor != null && cursor.Count > 0 && cursor.MoveToNext())
                {
                    int dbId = cursor.GetInt(cursor.GetColumnIndex("_id"));
                    string groupId = cursor.GetString(cursor.GetColumnIndex("group_id"));
                    string desc = cursor.GetString(cursor.GetColumnIndex("desc"));
                    long updateTime = cursor.GetLong(cursor.GetColumnIndex("update_time"));
                    long ctime = cursor.GetLong(cursor.GetColumnIndex("ctime"));

                    Group group = new Group();
                    group.setGroupId(groupId);
                    group.setDesc(desc);

                    groupList.Add(group);
                }
            }
            catch (Java.Lang.Exception)
            {
                closeCursor(cursor);
            }
            return groupList;
        }

        public bool updateUser(User user, int mode)
        {

            bool success = false;
            if (mDBHelper == null)
            {
                return success;
            }
            try
            {
                mDatabase = mDBHelper.WritableDatabase;
                beginTransaction(mDatabase);

                if (user != null)
                {
                    mDatabase.BeginTransaction();
                    string where = "user_id = ? and group_id = ?";
                    string[] whereValue = { user.getUserId(), user.getGroupId() };
                    ContentValues cv = new ContentValues();

                    cv.Put("user_id", user.getUserInfo());
                    cv.Put("user_info", user.getUserInfo());
                    cv.Put("group_id", user.getGroupId());
                    cv.Put("update_time", DateTime.Now.Millisecond);
                    if (mDatabase.Update(DBHelper.TABLE_USER, cv, where, whereValue) < 0)
                    {
                        return false;
                    }

                    // 1更新，直接删掉已有feature  2追加
                    if (mode == 1)
                    {
                        if (mDatabase.Delete(DBHelper.TABLE_FEATURE, where, whereValue) < 0)
                        {
                            return false;
                        }
                        foreach (Feature feature in user.getFeatureList())
                        {
                            if (!addFeature(feature, mDatabase))
                            {
                                return false;
                            }
                        }

                    }
                    else if (mode == 2)
                    {
                        foreach (Feature feature in user.getFeatureList())
                        {
                            if (!addFeature(feature, mDatabase))
                            {
                                return false;
                            }
                        }

                    }
                }
                setTransactionSuccessful(mDatabase);
                success = true;
            }
            catch (Java.Lang.Exception)
            {
                endTransaction(mDatabase);
            }
            return success;
        }

        public bool deleteFeature(string userId, string groupId, string faceToken)
        {
            bool success = false;
            try
            {
                mDatabase = mDBHelper.WritableDatabase;
                beginTransaction(mDatabase);

                if (!TextUtils.IsEmpty(userId))
                {
                    string where = "user_id = ? and groupId = ? and face_token=?";
                    string[] whereValue = { userId, groupId, faceToken };

                    if (mDatabase.Delete(DBHelper.TABLE_FEATURE, where, whereValue) < 0)
                    {
                        return false;
                    }
                    setTransactionSuccessful(mDatabase);
                    success = true;
                }

            }
            catch (Java.Lang.Exception)
            {
                endTransaction(mDatabase);
            }
            return success;
        }

        public bool deleteUser(string userId, string groupId)
        {
            bool success = false;
            try
            {
                mDatabase = mDBHelper.WritableDatabase;
                beginTransaction(mDatabase);

                if (!TextUtils.IsEmpty(userId))
                {
                    string where = "user_id = ? and group_id = ?";
                    string[] whereValue = { userId, groupId };

                    if (mDatabase.Delete(DBHelper.TABLE_FEATURE, where, whereValue) < 0)
                    {
                        return false;
                    }
                    if (mDatabase.Delete(DBHelper.TABLE_USER, where, whereValue) < 0)
                    {
                        return false;
                    }

                    setTransactionSuccessful(mDatabase);
                    success = true;
                }

            }
            catch (Java.Lang.Exception)
            {
                endTransaction(mDatabase);
            }
            return success;
        }

        public bool deleteGroup(string groupId)
        {
            bool success = false;
            try
            {
                mDatabase = mDBHelper.WritableDatabase;
                beginTransaction(mDatabase);

                if (!TextUtils.IsEmpty(groupId))
                {
                    string where = "group_id = ?";
                    string[] whereValue = { groupId };
                    if (mDatabase.Delete(DBHelper.TABLE_FEATURE, where, whereValue) < 0)
                    {
                        return false;
                    }
                    if (mDatabase.Delete(DBHelper.TABLE_USER, where, whereValue) < 0)
                    {
                        return false;
                    }
                    if (mDatabase.Delete(DBHelper.TABLE_USER_GROUP, where, whereValue) < 0)
                    {
                        return false;
                    }

                    setTransactionSuccessful(mDatabase);
                    success = true;
                }

            }
            catch (Java.Lang.Exception)
            {
                endTransaction(mDatabase);
            }
            return success;
        }


        private void beginTransaction(SQLiteDatabase mDatabase)
        {
            if (allowTransaction)
            {
                mDatabase.BeginTransaction();
            }
            else
            {
                writeLock.Lock();
                writeLocked = true;
            }
        }

        private void setTransactionSuccessful(SQLiteDatabase mDatabase)
        {
            if (allowTransaction)
            {
                mDatabase.SetTransactionSuccessful();
            }
        }

        private void endTransaction(SQLiteDatabase mDatabase)
        {
            if (allowTransaction)
            {
                mDatabase.EndTransaction();
            }
            if (writeLocked)
            {
                writeLock.Unlock();
                writeLocked = false;
            }
        }

        private void closeCursor(ICursor cursor)
        {
            if (cursor != null)
            {
                try
                {
                    cursor.Close();
                }
                catch (Throwable e)
                {
                }
            }
        }
    }
}