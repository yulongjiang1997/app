package md57cfec95a7c25db6ee8d55d7e381346cc;


public class NetRequest_TrustManager
	extends md57cfec95a7c25db6ee8d55d7e381346cc.NetRequest_X509TrustManager
	implements
		mono.android.IGCUserPeer,
		javax.net.ssl.TrustManager
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("App1.Droid.BaiduSDKManager.Utils.NetRequest+TrustManager, App1.Android", NetRequest_TrustManager.class, __md_methods);
	}


	public NetRequest_TrustManager ()
	{
		super ();
		if (getClass () == NetRequest_TrustManager.class)
			mono.android.TypeManager.Activate ("App1.Droid.BaiduSDKManager.Utils.NetRequest+TrustManager, App1.Android", "", this, new java.lang.Object[] {  });
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
