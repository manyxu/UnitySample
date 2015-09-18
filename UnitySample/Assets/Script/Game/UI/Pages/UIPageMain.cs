using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIPageMain : UIPage
{
	static UIPageMain mInstance = null;
	public static UIPageMain GetInstance()
	{
		return mInstance;
	}

	public override void OnLoad()
	{
		base.OnLoad ();

		mInstance = this;
	}
	
	public override void OnLoaded ()
	{		
	}
}
