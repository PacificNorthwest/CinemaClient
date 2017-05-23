package md53d4c9ea3ff56243656cf080fd6953304;


public class DescriptionLeadingMarginSpan2
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.text.style.LeadingMarginSpan.LeadingMarginSpan2,
		android.text.style.LeadingMarginSpan,
		android.text.style.ParagraphStyle,
		android.text.style.WrapTogetherSpan
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getLeadingMarginLineCount:()I:GetGetLeadingMarginLineCountHandler:Android.Text.Style.ILeadingMarginSpanLeadingMarginSpan2Invoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_drawLeadingMargin:(Landroid/graphics/Canvas;Landroid/graphics/Paint;IIIIILjava/lang/CharSequence;IIZLandroid/text/Layout;)V:GetDrawLeadingMargin_Landroid_graphics_Canvas_Landroid_graphics_Paint_IIIIILjava_lang_CharSequence_IIZLandroid_text_Layout_Handler:Android.Text.Style.ILeadingMarginSpanInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_getLeadingMargin:(Z)I:GetGetLeadingMargin_ZHandler:Android.Text.Style.ILeadingMarginSpanInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("CinemaApp.DescriptionLeadingMarginSpan2, CinemaApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DescriptionLeadingMarginSpan2.class, __md_methods);
	}


	public DescriptionLeadingMarginSpan2 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DescriptionLeadingMarginSpan2.class)
			mono.android.TypeManager.Activate ("CinemaApp.DescriptionLeadingMarginSpan2, CinemaApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public DescriptionLeadingMarginSpan2 (int p0, int p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == DescriptionLeadingMarginSpan2.class)
			mono.android.TypeManager.Activate ("CinemaApp.DescriptionLeadingMarginSpan2, CinemaApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public int getLeadingMarginLineCount ()
	{
		return n_getLeadingMarginLineCount ();
	}

	private native int n_getLeadingMarginLineCount ();


	public void drawLeadingMargin (android.graphics.Canvas p0, android.graphics.Paint p1, int p2, int p3, int p4, int p5, int p6, java.lang.CharSequence p7, int p8, int p9, boolean p10, android.text.Layout p11)
	{
		n_drawLeadingMargin (p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
	}

	private native void n_drawLeadingMargin (android.graphics.Canvas p0, android.graphics.Paint p1, int p2, int p3, int p4, int p5, int p6, java.lang.CharSequence p7, int p8, int p9, boolean p10, android.text.Layout p11);


	public int getLeadingMargin (boolean p0)
	{
		return n_getLeadingMargin (p0);
	}

	private native int n_getLeadingMargin (boolean p0);

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
