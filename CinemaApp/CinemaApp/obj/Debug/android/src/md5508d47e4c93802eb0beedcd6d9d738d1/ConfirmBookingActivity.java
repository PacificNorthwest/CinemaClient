package md5508d47e4c93802eb0beedcd6d9d738d1;


public class ConfirmBookingActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("CinemaApp.Activities.ConfirmBookingActivity, CinemaApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ConfirmBookingActivity.class, __md_methods);
	}


	public ConfirmBookingActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ConfirmBookingActivity.class)
			mono.android.TypeManager.Activate ("CinemaApp.Activities.ConfirmBookingActivity, CinemaApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
