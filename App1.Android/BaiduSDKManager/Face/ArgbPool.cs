using Android.Support.V4.Util;
using Java.Lang;
namespace App1.Droid.BaiduSDKManager.Face
{
    public class ArgbPool
    {
        
        Pools.SynchronizedPool pool = new Pools.SynchronizedPool(5);

        public ArgbPool()
        {

        }

        public int[] acquire(int width, int height)
        {
            int[] argb = (int[])pool.Acquire();
            if (argb == null || argb.Length != width * height)
            {
                argb = new int[width * height];
            }
            return argb;
        }

        public void release(int[] data)
        {
            try
            {
                pool.Release(data);
            }
            catch (IllegalStateException ignored)
            {
                // ignored
            }
        }
    }
}
