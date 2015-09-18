using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPage : UIWidget 
{
	public GameObject mCanvasObject;

	private Dictionary<string, UIView> mViews = new Dictionary<string, UIView>();

	private Dictionary<string, UIAbsFactory> mFacDic = null;
	public override void OnLoad()
	{
		mFacDic = UIManager.GetInstance().FactoryDic;
		base.OnLoad();

		mCanvasObject = transform.GetChild (0).gameObject;
	}
	
	public virtual T OpenView<T> (string name) where T : UIView
	{
		UIView view;
		if (mViews.TryGetValue(name, out view))
		{
			return (T)view;
		}
		
		UIAbsFactory fac;
		if (mFacDic.TryGetValue(name, out fac))
		{
			view = fac.Create() as UIView;

			view.Name = name;
			view.Owner = this;

			view.gameObject.transform.SetParent(mCanvasObject.transform, false);

			view.Init();
			mViews.Add(name, view);

			return view as T;
		}
		return null;
	}

	public virtual T FindView<T>(string name) where T : UIView
	{
		UIView view;
		if (mViews.TryGetValue(name, out view))
		{
			return (T)view;
		}
		
		return null;
	}

	public void _RemoveView(string name)
	{
		mViews.Remove(name);
	}
}
