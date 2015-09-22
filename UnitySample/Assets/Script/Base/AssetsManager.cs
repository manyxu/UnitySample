using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SLua;

[CustomLuaClassAttribute]
public class LoadingProgressArg : EventArgs
{
	public LoadingProgressArg(float process, string assetName)
	{
		Process = process;
		AssetName = assetName;
	}
	
	public float Process { get; private set; }
	public string AssetName { get; private set; }
}

public delegate void EventStatus(LoadingProgressArg arg0);
public delegate void EventComplete(AssetBundle bundle);

// 资源管理器是一个AssetBundle的管理器
// 首先从Update目录加载AssetBunlde，如果没有从StreamingPath获取
[CustomLuaClassAttribute]
public class AssetsManager
{
	public struct LoadingWWW
	{
		public WWW www;
		public EventStatus status;
		public EventComplete complete;
		public string relPath;
		public string absPath;
	}

	// instance
	static private AssetsManager sInstance = null;
	public static AssetsManager GetInstance()
	{
		if (null == sInstance)
			sInstance = new AssetsManager ();
		return sInstance;
	}

	List<string> mUpdatePaths;
	Dictionary<string, AssetBundle> mCacheAssets;
	List<LoadingWWW> mLoadingList;
	
	private AssetsManager()
	{
		mUpdatePaths = new List<string> ();
		mCacheAssets = new Dictionary<string, AssetBundle> ();
		mLoadingList = new List<LoadingWWW> ();
	}

	public void Destroy()
	{
		ClearAssetAll (false);
		sInstance = null;
	}
	
	// update
	public void AddUpdatePath(string path)
	{
		mUpdatePaths.Add (path);
	}

	public void ClearUpdatePath()
	{
		mUpdatePaths.Clear ();
	}

	// cache asset
	public AssetBundle GetFromCache(string relPath) 
	{
		string absPath = "";
		AssetBundle bundle = null;

		// first check updatepath to get
		foreach (var p in mUpdatePaths)
		{
			absPath = p + "/" + relPath;
			if (mCacheAssets.TryGetValue (absPath, out bundle)) 
			{
				return bundle;
			}
		}

		// then check streaming path
		absPath = Application.streamingAssetsPath + "/" + relPath;
		if (mCacheAssets.TryGetValue(absPath, out bundle))
		{
			return bundle;
		}
		
		return null;
	}
	
	public void ClearAssetAll(bool unloadAllLoadedObjects)
	{
		foreach (var pair in mCacheAssets) 
		{
			pair.Value.Unload(unloadAllLoadedObjects);
		}
		
		mCacheAssets.Clear ();
	}
	
	public void ClearAsset(string name)
	{
		var iter = mCacheAssets.GetEnumerator();
		string k = "";
		while(iter.MoveNext())
		{
			if(iter.Current.Key.EndsWith(name))
			{
				k = iter.Current.Key;
				break;
			}
		}

		AssetBundle ab;
		if(mCacheAssets.TryGetValue(k, out ab))
		{
			ab.Unload(false);
			mCacheAssets.Remove(k);
		}
	}

	public void LoadAsset(string relPath, EventComplete completeDoFun, EventStatus status)
	{
		string absPath = "";
		LoadingWWW loading;

		// first check updatepath to get
		foreach(var p in mUpdatePaths)
		{
			absPath = p + "/" + relPath;
			if(mCacheAssets.ContainsKey(absPath) && completeDoFun != null)
			{
				completeDoFun(mCacheAssets[absPath]);
				return;
			}
			
			if(System.IO.File.Exists(absPath))
			{
				loading.www = new WWW("file://" + absPath);
				loading.status = status;
				loading.complete = completeDoFun;
				loading.relPath = relPath;
				loading.absPath = absPath;
				if(loading.status != null)
					loading.status(new LoadingProgressArg(loading.www.progress, relPath));
				mLoadingList.Add(loading);
			}
		}

		// then check streaming path
		absPath = Application.streamingAssetsPath + "/" + relPath;
		AssetBundle bundle = null;
		if (mCacheAssets.TryGetValue(absPath, out bundle) && completeDoFun != null)
		{
			completeDoFun(bundle);
			return;
		}
		
		if(Application.platform == RuntimePlatform.Android)
			loading.www = new WWW(absPath);
		else
			loading.www = new WWW("file://" + absPath);
		loading.status = status;
		loading.complete = completeDoFun;
		loading.relPath = relPath;
		loading.absPath = absPath;
		if(loading.status != null)
			loading.status(new LoadingProgressArg(loading.www.progress, relPath));
		mLoadingList.Add(loading);		
	}

	[DoNotToLua]
	public void Tick()
	{
		for (int i = mLoadingList.Count - 1; i >= 0; i--) 
		{
			LoadingWWW loading = mLoadingList[i];
			if(loading.status != null)
				loading.status(new LoadingProgressArg(loading.www.progress, loading.relPath));

			if(loading.www.isDone)
			{
				mCacheAssets.Add(loading.absPath, loading.www.assetBundle);

				if(loading.complete != null)
					loading.complete(loading.www.assetBundle);

				mLoadingList.RemoveAt(i);
			}
		}
	}
}
