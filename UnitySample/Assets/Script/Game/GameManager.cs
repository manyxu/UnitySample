using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour 
{
	// Instances
	[HideInInspector]
	public static GameManager sInstance = null;

	// manages
	[HideInInspector]
	public AssetsManager mAssetsMgr = null;
	
	[HideInInspector]
	public UIManager mUIMgr = null;
	
	void Awake()
	{
		if (null != sInstance)
			return;

		sInstance = this;

		// keep alive on change scene
		DontDestroyOnLoad(this.gameObject);

		mAssetsMgr = AssetsManager.GetInstance ();
		string updatePath = Application.persistentDataPath + "Update";
		if (!Directory.Exists(updatePath)) 
			Directory.CreateDirectory(updatePath);
		mAssetsMgr.AddCachePath (updatePath);

		mUIMgr = UIManager.GetInstance ();
		mUIMgr.OnInitlize += () => 
		{
			Debug.Log("GameManagerLoaded");

			UIPage page = mUIMgr.PagePush(PageName.UIPageMain);
			page.OpenView<UIViewLogin>(ViewName.Login);

			Application.LoadLevel("Login");
		};
		mUIMgr.Initlize ();
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnDestroy()
	{
		if (null != mAssetsMgr)
		{
			mAssetsMgr.Destroy();
			mAssetsMgr = null;
		}

		if (null != mUIMgr) 
		{
			mUIMgr.Destroy();
			mUIMgr = null;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (null != mAssetsMgr)
			mAssetsMgr.Tick ();
	}
}
