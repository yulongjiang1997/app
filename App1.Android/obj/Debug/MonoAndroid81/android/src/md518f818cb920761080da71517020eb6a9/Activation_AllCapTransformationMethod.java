package md518f818cb920761080da71517020eb6a9;


public class Activation_AllCapTransformationMethod
	extends android.text.method.ReplacementTransformationMethod
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getOriginal:()[C:GetGetOriginalHandler\n" +
			"n_getReplacement:()[C:GetGetReplacementHandler\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.BaiduSDKManager.Ui.Activation+AllCapTransformationMethod, App1.Android", Activation_AllCapTransformationMethod.class, __md_methods);
	}


	public Activation_AllCapTransformationMethod ()
	{
		super ();
		if (getClass () == Activation_AllCapTransformationMethod.class)
			mono.android.TypeManager.Activate ("App1.Droid.BaiduSDKManager.Ui.Activation+AllCapTransformationMethod, App1.Android", "", this, new java.lang.Object[] {  });
	}

	public Activation_AllCapTransformationMethod (boolean p0)
	{
		super ();
		if (getClass () == Activation_AllCapTransformationMethod.class)
			mono.android.TypeManager.Activate ("App1.Droid.BaiduSDKManager.Ui.Activation+AllCapTransformationMethod, App1.Android", "System.Boolean, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public char[] getOriginal ()
	{
		return n_getOriginal ();
	}

	private native char[] n_getOriginal ();


	public char[] getReplacement ()
	{
		return n_getReplacement ();
	}

	private native char[] n_getReplacement ();

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
