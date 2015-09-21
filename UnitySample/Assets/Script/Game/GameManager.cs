using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SLua;

// delegate event
public delegate void EventUI();

// 游戏管理器 
// 这是一个单件实例，控制着整个游戏的生命
[CustomLuaClassAttribute]
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

	[HideInInspector]
	public LuaSvr luaSvr_GM;
	[HideInInspector]
	public LuaTable luaSelf_GM;
	[HideInInspector]
	public LuaFunction luaUpdate_GM;
	
	void Awake()
	{
		if (null != sInstance)
			return;

		sInstance = this;

		// keep alive on change scene
		DontDestroyOnLoad(this.gameObject);

		// if exist gameobject called "UIRoot". Destory it.
		// this maybe we add to desing ui temply
		GameObject gameObjectUIRoot = GameObject.Find("UIRoot");
		GameObject.Destroy (gameObjectUIRoot);

		mAssetsMgr = AssetsManager.GetInstance ();

		mUIMgr = UIManager.GetInstance ();
		
		// we has put these code into lua script
		/*
		// asset manager update path
		string updatePath = Application.persistentDataPath + "Update";
		if (!Directory.Exists(updatePath)) Directory.CreateDirectory(updatePath);
		mAssetsMgr.AddUpdatePath (updatePath);

		// ui manager
		_Load(0, () => {
			mUIMgr.Initlize();
		});
		*/

		// lua
		luaSvr_GM = new LuaSvr();
		luaSelf_GM =(LuaTable)luaSvr_GM.start("Lua/GameManager");
		luaUpdate_GM = (LuaFunction)luaSelf_GM["update"];
	}
	
	// assets
	static string[] UI_ASSETS = new string[] { "ui.unity3d", "modelprefab.unity3d"};
	void _Load(int startIndex, EventUI allLoadedCallback)
	{
		// load complete
		if (startIndex >= UI_ASSETS.Length)
		{
			if(allLoadedCallback != null)
				allLoadedCallback();
			return;
		}
		
		var assetMgr = AssetsManager.GetInstance();
		assetMgr.LoadAsset(UI_ASSETS[startIndex], (AssetBundle asset) => {
			_Load(startIndex + 1, allLoadedCallback); }, null);
	}

	// Use this for initialization
	void Start () {
	}

	void OnDestroy()
	{
		if (null != mUIMgr) 
		{
			mUIMgr.Destroy();
			mUIMgr = null;
		}

		if (null != mAssetsMgr)
		{
			mAssetsMgr.Destroy();
			mAssetsMgr = null;
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
