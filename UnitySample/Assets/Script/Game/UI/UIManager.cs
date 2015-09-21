using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SLua;

// defines
public sealed class PageName
{
	public const string UIPageMain = "UIPageMain";
}

public sealed class ViewName
{
	public const string Login = "PanelLogin";
}

public sealed class WidgetName
{
}

// factory
public abstract class UIAbsFactory
{
	public UIAbsFactory(string name)
	{
		Name = name;
	}
	
	public abstract Object Create();
	public string Name { get; private set; }
}

public class UIFactory<T> : UIAbsFactory where T : Component
{
	private Object mPrefabe;

	public UIFactory(string asset, string name)
		: base(name)
	{
		_InitPrefab(asset);
	}
	
	void _InitPrefab(string bundleName)
	{
		var assetMgr = AssetsManager.GetInstance();

		var asset = assetMgr.GetFromCache(bundleName);
		if (asset == null) return;

		mPrefabe = asset.Load(Name, typeof(GameObject)) as GameObject;
	}
	
	public override Object Create()
	{
		if (mPrefabe == null)
			return null;

		var gameO = GameObject.Instantiate(mPrefabe) as GameObject;
		if (gameO == null)
			return null;

		var ret = gameO.gameObject.GetComponent<T>();
		if (ret != null)
			return ret;

		return gameO.gameObject.AddComponent<T>();
	}
}

// manager
[CustomLuaClassAttribute]
public class UIManager
{	
	// construct
	static private UIManager sInstance = null;
	public static UIManager GetInstance()
	{
		if (sInstance == null)
			sInstance = new UIManager();

		return sInstance;
	}
	
	private UIManager()
	{
	}

	// factory
	private Dictionary<string, UIAbsFactory> mFacDic = new Dictionary<string, UIAbsFactory>();
	public Dictionary<string, UIAbsFactory> FactoryDic { get { return mFacDic; } }

	// uis
	[HideInInspector]
	public GameObject mRoot;

	public void Initlize()
	{
		mRoot = new GameObject("UIRoot");
		GameObject.DontDestroyOnLoad(mRoot);

		// pages
		mFacDic.Add(PageName.UIPageMain, new UIFactory<UIPageMain>("ui.unity3d", PageName.UIPageMain));
		
		// views
		mFacDic.Add(ViewName.Login, new UIFactory<UIViewLogin>("ui.unity3d", ViewName.Login));

		// open page
		UIPage page = PagePush(PageName.UIPageMain);
		page.OpenView<UIViewLogin>(ViewName.Login);

		// load level;
		//Application.LoadLevel("Login");
	}

	// pages
	private Stack<UIPage> mPageStack = new Stack<UIPage>();

	public UIPage PagePush(string name)
	{
		UIAbsFactory fac;
		if( mFacDic.TryGetValue(name, out fac) )
		{
			UIPage p = fac.Create() as UIPage;
			if (mPageStack.Count > 0)
			{
				var top = mPageStack.Peek();
				top.gameObject.SetActive(false);
			}
			mPageStack.Push(p);
			p.transform.SetParent(mRoot.transform);
		
			return p;
		}
		
		return null;
	}
	
	public int PagePop()
	{
		if (mPageStack.Count > 0)
		{
			var page = mPageStack.Pop();
			GameObject.Destroy(page.gameObject);

			if (mPageStack.Count != 0)
			{
				page = mPageStack.Peek();
				page.gameObject.SetActive(true);
			}
		}
		
		return -1;
	}
	
	public T TopPage<T>() where T : UIPage
	{
		if (mPageStack.Count > 0)
		{
			return mPageStack.Peek() as T;
		}
		
		return null;
	}
	
	// helps
	private GameObject mBanner;
	private GameObject mAlert;
	
	public void ShowBanner(string msg)
	{
		if (mBanner == null)
		{
			var assetMgr = AssetsManager.GetInstance();
			var asset = assetMgr.GetFromCache("ui.unity3d");
			if (asset == null) return;
			var prefab = asset.Load("Banner", typeof(GameObject)) as GameObject;
			if (prefab == null) return;
			mBanner = GameObject.Instantiate(prefab) as GameObject;
			var page = mPageStack.Peek();
			mBanner.transform.parent = page.transform;
			mBanner.transform.localPosition = Vector3.zero;
			mBanner.transform.localScale = Vector3.one;
		}
		
		var val = mBanner.transform.FindChild("Text");
		var txt = val.GetComponent<Text>();
		txt.text = msg;
	}
	
	public void CloseBanner()
	{
		if (mBanner != null)
		{
			GameObject.Destroy(mBanner);
			mBanner = null;
		}
	}
	public IEnumerator CloseBannerInSecond(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		UIManager.GetInstance().CloseBanner();
	} 
	
	public Sprite IconCreate(string name)
	{
		var assetMgr = AssetsManager.GetInstance ();
		var ab = assetMgr.GetFromCache ("icons.unity3d");
		if (ab == null)
			return null;
		return ab.Load (name, typeof(Sprite)) as Sprite;
	}
	
	public void Alert(string msg, EventUI func = null)
	{
		if (mAlert == null)
		{
			var assetMgr = AssetsManager.GetInstance();
			var asset = assetMgr.GetFromCache("ui.unity3d");
			if (asset == null) return;
			var prefab = asset.Load("Alert", typeof(GameObject)) as GameObject;
			if (prefab == null) return;
			mAlert = GameObject.Instantiate(prefab) as GameObject;
			var transBtn = mAlert.transform.FindChild("OK");
			var btn = transBtn.GetComponent<Button>();
			var page = mPageStack.Peek();
			mAlert.transform.parent = page.transform;
			mAlert.transform.localPosition = Vector3.zero;
			mAlert.transform.localScale = Vector3.one;
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() =>
			                        {
				if (mAlert != null)
				{
					if (func != null)
						func();
					GameObject.Destroy(mAlert);
					mAlert = null;
				}
			});
		}
		
		var transTxt = mAlert.transform.FindChild("Text");
		var txt = transTxt.GetComponent<Text>();
		txt.text = msg;
	}
	
	public bool IsPointerOverUIObject(Vector2 screenPosition)
	{
		var page = TopPage<UIPage>();
		if(page != null)
		{
			var canvas = page.GetComponent<Canvas>();
			
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			
			eventDataCurrentPosition.position = screenPosition;
			
			GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
			
			List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
			
			uiRaycaster.Raycast(eventDataCurrentPosition, results);
			
			return results.Count > 0;
		}
		
		return false;
	}
	
	public void Destroy()
	{
	}
}
