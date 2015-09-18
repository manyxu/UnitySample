using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LuaInterface;
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

	List<string> mCachePaths;
	Dictionary<string, AssetBundle> mCacheAssets;
	List<LoadingWWW> mLoadingList;
	
	private AssetsManager()
	{
		mCachePaths = new List<string> ();
		mCacheAssets = new Dictionary<string, AssetBundle> ();
		mLoadingList = new List<LoadingWWW> ();
	}

	// cache
	public void AddCachePath(string path)
	{
		mCachePaths.Add (path);
	}

	public void ClearCachePath()
	{
		mCachePaths.Clear ();
	}

	public AssetBundle GetFromCache(string relPath) 
	{
		string absPath = "";
		AssetBundle bundle = null;
		
		foreach (var p in mCachePaths)
		{
			absPath = p + "/" + relPath;
			if (mCacheAssets.TryGetValue (absPath, out bundle)) 
			{
				return bundle;
			}
		}
		
		absPath = Application.streamingAssetsPath + "/" + relPath;
		
		if (mCacheAssets.TryGetValue(absPath, out bundle))
		{
			return bundle;
		}
		
		return null;
	}

	// Asset
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

	public void LoadAsset(string relPath, EventComplete complete, EventStatus status)
	{
		string absPath = "";
		LoadingWWW loading;
		
		foreach(var p in mCachePaths)
		{
			absPath = p + "/" + relPath;
			if(mCacheAssets.ContainsKey(absPath) && complete != null)
			{
				complete(mCacheAssets[absPath]);
			}
			
			if(System.IO.File.Exists(absPath))
			{
				loading.www = new WWW("file://" + absPath);
				loading.status = status;
				loading.complete = complete;
				loading.relPath = relPath;
				loading.absPath = absPath;
				if(loading.status != null)
					loading.status(new LoadingProgressArg(loading.www.progress, relPath));
				mLoadingList.Add(loading);
			}
		}
		
		absPath = Application.streamingAssetsPath + "/" + relPath;
		AssetBundle bundle = null;
		if (mCacheAssets.TryGetValue(absPath, out bundle) && complete != null)
		{
			complete(bundle);
		}
		
		if(Application.platform == RuntimePlatform.Android)
			loading.www = new WWW(absPath);
		else
			loading.www = new WWW("file://" + absPath);
		loading.status = status;
		loading.complete = complete;
		loading.relPath = relPath;
		loading.absPath = absPath;
		if(loading.status != null)
			loading.status(new LoadingProgressArg(loading.www.progress, relPath));
		mLoadingList.Add(loading);		
	}
	
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
	
	public void Destroy()
	{
		ClearAssetAll (false);
		sInstance = null;
	}
}
