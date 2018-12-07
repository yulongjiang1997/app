package md59bff40fc14838581df2b3031aba4ef1e;


public class RgbIrLivenessActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("App1.Droid.Activitys.RgbIrLivenessActivity, App1.Android", RgbIrLivenessActivity.class, __md_methods);
	}


	public RgbIrLivenessActivity ()
	{
		super ();
		if (getClass () == RgbIrLivenessActivity.class)
			mono.android.TypeManager.Activate ("App1.Droid.Activitys.RgbIrLivenessActivity, App1.Android", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
