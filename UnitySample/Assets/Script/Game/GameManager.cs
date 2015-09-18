using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SLua;

// delegate event
public delegate void EventUI();

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

		GameObject gameObjectUIRoot = GameObject.Find("UIRoot");
		GameObject.Destroy (gameObjectUIRoot);
		
		mAssetsMgr = AssetsManager.GetInstance ();
		string updatePath = Application.persistentDataPath + "Update";
		if (!Directory.Exists(updatePath)) 
			Directory.CreateDirectory(updatePath);
		mAssetsMgr.AddCachePath (updatePath);

		mUIMgr = UIManager.GetInstance ();

		_Load(0, () => {
			mUIMgr.Initlize();
		});

		mAssetsMgr.GetFromCache ("modelprefab.unity3d");
	}
	
	// assets
	static string[] UI_ASSETS = new string[] { "ui.unity3d", "modelprefab.unity3d"};
	void _Load(int startIndex, EventUI callback)
	{
		// load complete
		if (startIndex >= UI_ASSETS.Length)
		{
			if(callback != null)
				callback();
			return;
		}
		
		var assetMgr = AssetsManager.GetInstance();
		assetMgr.LoadAsset(UI_ASSETS[startIndex], (AssetBundle asset) => {
			_Load(startIndex + 1, callback); }, null);
	}

	public LuaSvr luaSvr_GM;
	public LuaTable luaSelf_GM;
	public LuaFunction luaUpdate_GM;

	// Use this for initialization
	void Start () {
		luaSvr_GM = new LuaSvr();
		luaSelf_GM =(LuaTable)luaSvr_GM.start("Lua/GameManager");
		luaUpdate_GM = (LuaFunction)luaSelf_GM["update"];
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

		luaUpdate_GM.call(luaSelf_GM);
	}
}
