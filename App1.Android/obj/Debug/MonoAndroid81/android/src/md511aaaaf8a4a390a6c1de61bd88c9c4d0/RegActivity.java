package md511aaaaf8a4a390a6c1de61bd88c9c4d0;


public class RegActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/view/View;)V:GetOnClick_Landroid_view_View_Handler:Android.Views.View/IOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.BaiduSDKManager.Face.RegActivity, App1.Android", RegActivity.class, __md_methods);
	}


	public RegActivity ()
	{
		super ();
		if (getClass () == RegActivity.class)
			mono.android.TypeManager.Activate ("App1.Droid.BaiduSDKManager.Face.RegActivity, App1.Android", "", this, new java.lang.Object[] {  });
	}


	public void onClick (android.view.View p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (android.view.View p0);

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
