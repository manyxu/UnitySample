using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using SLua;

[CustomLuaClassAttribute]
public class UIWidget : MonoBehaviour 
{
	[HideInInspector]

	private List<UIWidget> mWidgets = new List<UIWidget>();

	// Load UnLoad
	void Awake()
	{
		OnLoad();
		StartCoroutine (PastOneFrame ());
	}
	
	public virtual void OnLoad()
	{
	}
	
	IEnumerator PastOneFrame()
	{
		yield return new WaitForFixedUpdate ();
		OnLoaded ();
	}

	public virtual void OnLoaded()
	{
	}

	void OnDestroy()
	{
		OnUnload();
	}

	public virtual void OnUnload()
	{
		
	}

	// create add
	public static T Create<T>(string name) where T : UIWidget
	{
		UIWidget widget;
		UIAbsFactory factory;
		
		var factoryDictionary = UIManager.GetInstance().FactoryDic;
		if(factoryDictionary.TryGetValue(name, out factory))
		{
			widget = factory.Create() as UIWidget;
			return (T)widget;
		}
		
		return null;
	}
	
	public virtual void AddWidget(UIWidget widget)
	{
		mWidgets.Add(widget);
		widget.transform.SetParent (transform, false);
	}

	// Tag
	public virtual T TagChild_Component<T>(string name) where T : Component
	{
		var child = transform.FindChild(name);
		if (child == null)
			return null;

		return child.gameObject.GetComponent<T>();
	}

	public static T TagChild_Component<T>(GameObject widget, string name) where T : Component
	{
		var child = widget.transform.FindChild(name);
		if (child == null)
			return null;
		
		return child.gameObject.GetComponent<T>();
	}
	
	public virtual T TagChild_ChildrenComponent<T>(string name) where T : Component
	{
		var child = transform.FindChild(name);
		if (child == null)
			return null;

		return child.gameObject.GetComponentInChildren<T>();
	}

	public static T TagChild_ChildrenComponent<T>(GameObject widget, string name) where T : Component
	{
		var child = widget.transform.FindChild(name);
		if (child == null)
			return null;
		
		return child.gameObject.GetComponentInChildren<T>();
	}

	public static List<T> TagChildren_ComponentList<T>(GameObject obj) where T : Component
	{
		List<T> returnlist = new List<T>();

		for(int i = 0; i < obj.transform.childCount; i++)
		{
			var child = obj.transform.GetChild (i);

			if(child != null) 
				returnlist.Add(child.gameObject.GetComponent<T>());
		}

		return returnlist;
	}
}
