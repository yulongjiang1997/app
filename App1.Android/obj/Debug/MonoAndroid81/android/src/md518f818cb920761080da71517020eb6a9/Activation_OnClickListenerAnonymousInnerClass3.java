package md518f818cb920761080da71517020eb6a9;


public class Activation_OnClickListenerAnonymousInnerClass3
	extends java.lang.Object
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
		mono.android.Runtime.register ("App1.Droid.BaiduSDKManager.Ui.Activation+OnClickListenerAnonymousInnerClass3, App1.Android", Activation_OnClickListenerAnonymousInnerClass3.class, __md_methods);
	}


	public Activation_OnClickListenerAnonymousInnerClass3 ()
	{
		super ();
		if (getClass () == Activation_OnClickListenerAnonymousInnerClass3.class)
			mono.android.TypeManager.Activate ("App1.Droid.BaiduSDKManager.Ui.Activation+OnClickListenerAnonymousInnerClass3, App1.Android", "", this, new java.lang.Object[] {  });
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