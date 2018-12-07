
using Android.Content;
using Android.OS;
using Java.Lang;
using System.Collections.Generic;

namespace App1.Droid.BaiduSDKManager.Utils
{
    public class PreferencesUtil
    {

        /** SharedPreferences variant */
        private static ISharedPreferences mPrefs;
        /** constant #LENGTH */
        private static readonly string LENGTH_SUFFIX = "#LENGTH";
        /** constant [ */
        private static readonly string LEFT_MOUNT = "[";
        /** constant ] */
        private static readonly string RIGHT_MOUNT = "]";

        /**
         * Initialize the Prefs helper class to keep a reference to the SharedPreference for this application the
         * SharedPreference will use the package name of the application as the Key.
         *
         * @param context the Application context.
         */
        public static void initPrefs(Context context)
        {

            if (mPrefs == null)
            {
                string key = context.PackageName;
                if (key == null)
                {
                    throw new NullPointerException("Prefs key may not be null");
                }
                mPrefs = context.GetSharedPreferences(key, FileCreationMode.MultiProcess);
            }
        }

        /**
         * 重新创建Preference对象，在跨进程掉用的时候需要重新初始化
         * 在小米上有问题，废弃
         *
         * @param context the Application context.
         */
        public static void reInit(Context context)
        {
            if (context != null)
            {
                string key = context.PackageName;
                if (key == null)
                {
                    throw new NullPointerException("Prefs key may not be null");
                }
                mPrefs = context.GetSharedPreferences(key, FileCreationMode.MultiProcess);
            }
        }

        /**
         * Returns an instance of the shared preference for this app.
         *
         * @return an Instance of the SharedPreference
         */
        public static ISharedPreferences getPreferences()
        {
            if (mPrefs != null)
            {
                return mPrefs;
            }
            throw new RuntimeException(
                    "please call iniPrefs(context) in the Application class onCreate.");
        }

        ///**
        // * @return Returns a map containing a list of pairs key/value representing the preferences.
        // * @see SharedPreferences#getAll()
        // */
        //public static Dictionary<String, ?> getAll()
        //{
        //    return getPreferences().getAll();
        //}

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference value if it exists, or defValue. Throws ClassCastException if there is a
         *         preference with this name that is not an int.
         * @see SharedPreferences#getInt(String, int)
         */
        public static int getInt(string key,  int defValue)
        {
            return getPreferences().GetInt(key, defValue);
        }

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference value if it exists, or defValue. Throws ClassCastException if there is a
         *         preference with this name that is not a boolean.
         * @see SharedPreferences#getBoolean(String, boolean)
         */
        public static bool getBoolean( string key,  bool defValue)
        {
            return getPreferences().GetBoolean(key, defValue);
        }

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference value if it exists, or defValue. Throws ClassCastException if there is a
         *         preference with this name that is not a long.
         * @see SharedPreferences#getLong(String, long)
         */
        public static long getLong( string key,  long defValue)
        {
            return getPreferences().GetLong(key, defValue);
        }

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference value if it exists, or defValue. Throws ClassCastException if there is a
         *         preference with this name that is not a float.
         * @see SharedPreferences#getFloat(String, float)
         */
        public static float getFloat(  string key,   float defValue)
        {
            return getPreferences().GetFloat(key, defValue);
        }

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference value if it exists, or defValue. Throws ClassCastException if there is a
         *         preference with this name that is not a String.
         * @see SharedPreferences#getString(String, String)
         */
        public static string getString(  string key,   string defValue)
        {
            return getPreferences().GetString(key, defValue);
        }

        /**
         * @param key The name of the preference to retrieve.
         * @param defValue Value to return if this preference does not exist.
         * @return Returns the preference values if they exist, or defValues. Throws ClassCastException if there is a
         *         preference with this name that is not a Set.
         * @see SharedPreferences#getStringSet(String, Set)
         */
       
    public static ICollection<string> getStringSet(  string key, HashSet<string> defValue)
        {
            ISharedPreferences prefs = getPreferences();
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb)
            {
                return prefs.GetStringSet(key, defValue);
            }
            else
            {
                if (prefs.Contains(key + LENGTH_SUFFIX))
                {
                    HashSet<string> set = new HashSet<string>();
                    // Workaround for pre-HC's lack of StringSets
                    int stringSetLength = prefs.GetInt(key + LENGTH_SUFFIX, -1);
                    if (stringSetLength >= 0)
                    {
                        for (int i = 0; i < stringSetLength; i++)
                        {
                            prefs.GetString(key + LEFT_MOUNT + i + RIGHT_MOUNT, null);
                        }
                    }
                    return set;
                }
            }
            return defValue;
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         * @see Editor#putLong(String, long)
         */
        public static void putLong(  string key,   long value)
        {
              var  editor = getPreferences().Edit();
            editor.PutLong(key, value);
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         * @see Editor#putInt(String, int)
         */
        public static void putInt(  string key,   int value)
        {
              var  editor = getPreferences().Edit();
            editor.PutInt(key, value);
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         *
         * @see Editor#putFloat(String, float)
         */
        public static void putFloat(  string key,   float value)
        {
              var editor = getPreferences().Edit();
            editor.PutFloat(key, value);
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         *
         * @see Editor#putBoolean(String, boolean)
         */
        public static void putBoolean(   string key,   bool value)
        {
var editor = getPreferences().Edit();
            editor.PutBoolean(key, value);
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         *
         * @see Editor#putString(String, String)
         */
        public static void putString(    string key,   string value)
        {
              var editor = getPreferences().Edit();
            editor.PutString(key, value);
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to modify.
         * @param value The new value for the preference.
         * @see Editor#putStringSet(String, Set)
         */
    public static void putStringSet(  string key,   HashSet<string> value)
        {
           var editor = getPreferences().Edit();
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb)
            {
                editor.PutStringSet(key, value);
            }
            else
            {
                // Workaround for pre-HC's lack of StringSets
                int stringSetLength = 0;
                if (mPrefs.Contains(key + LENGTH_SUFFIX))
                {
                    // First read what the value was
                    stringSetLength = mPrefs.GetInt(key + LENGTH_SUFFIX, -1);
                }
                editor.PutInt(key + LENGTH_SUFFIX, value.Count);
                int i = 0;
                foreach (string aValue in value)
                {
                    editor.PutString(key + LEFT_MOUNT + i + RIGHT_MOUNT, aValue);
                    i++;
                }
                for (; i < stringSetLength; i++)
                {
                    // Remove any remaining values
                    editor.Remove(key + LEFT_MOUNT + i + RIGHT_MOUNT);
                }
            }
            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }


        /**
         * @param key The name of the preference to remove.
         * @see Editor#remove(String)
         */
        public static void remove(  string key)
        {
            ISharedPreferences prefs = getPreferences();
             var  editor = prefs.Edit();
            if (prefs.Contains(key + LENGTH_SUFFIX))
            {
                // Workaround for pre-HC's lack of StringSets
                int stringSetLength = prefs.GetInt(key + LENGTH_SUFFIX, -1);
                if (stringSetLength >= 0)
                {
                    editor.Remove(key + LENGTH_SUFFIX);
                    for (int i = 0; i < stringSetLength; i++)
                    {
                        editor.Remove(key + LEFT_MOUNT + i + RIGHT_MOUNT);
                    }
                }
            }
            editor.Remove(key);

            if (Build.VERSION.SdkInt < Build.VERSION_CODES.Gingerbread)
            {
                editor.Commit();
            }
            else
            {
                editor.Apply();
            }
        }

        /**
         * @param key The name of the preference to check.
         * @see SharedPreferences#contains(String)
         * @return boolean true flase
         */
        public static bool contains(  string key)
        {
            return getPreferences().Contains(key);
        }
    }
}
