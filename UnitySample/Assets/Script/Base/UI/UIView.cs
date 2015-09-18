using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIView : UIWidget 
{
	public string Name;
	public UIPage Owner { get; set; }
	
	public void Init()
	{
		OnInit();
	}

	protected virtual void OnInit()
	{		
	}
	
	public void Close()
	{
		OnClose();
		GameObject.Destroy(gameObject);
	}
	
	protected virtual void OnClose()
	{
		if (Owner)
			Owner._RemoveView (Name);
	}
}
