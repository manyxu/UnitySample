using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitySampleEditor
{
	static bool sIsShowDialog = true;

	static List<UnityEngine.Object> FindUI()
	{
		List<UnityEngine.Object> ui = new List<UnityEngine.Object> ();
		var guids = AssetDatabase.FindAssets ("t:GameObject", new string[] {"Assets/Media/UI"});

		foreach (var g in guids) 
		{
			string path = AssetDatabase.GUIDToAssetPath(g);
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
			if (null != obj)
			{
				ui.Add(obj);
			}
		}

		return ui;
	}

	[MenuItem ("UnitySample/Build_UI")]
	static void Build_UI ()
	{  
		BuildAssetBundleOptions opt = BuildAssetBundleOptions.CollectDependencies |
			BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;

		var objs = FindUI ();
		if (objs.Count > 0) 
		{
			BuildPipeline.BuildAssetBundle(null, objs.ToArray(),
			                               Application.streamingAssetsPath + "/ui.unity3d", opt, 
			                               EditorUserBuildSettings.activeBuildTarget);
		}

		if (sIsShowDialog)
			EditorUtility.DisplayDialog ("Build Assets", "Build Assets Complete!", "OK");

		AssetDatabase.Refresh ();
	}

	static List<UnityEngine.Object> FindModelPrefab()
	{
		List<UnityEngine.Object> ui = new List<UnityEngine.Object> ();
		var guids = AssetDatabase.FindAssets ("t:GameObject", new string[] {"Assets/Media/ModelPrefab"});
		
		foreach (var g in guids) 
		{
			string path = AssetDatabase.GUIDToAssetPath(g);
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
			if (null != obj)
			{
				ui.Add(obj);
			}
		}
		
		return ui;
	}

	[MenuItem ("UnitySample/Build_ModelPrefab")]
	static void Build_ModelPrefab ()
	{
		BuildAssetBundleOptions opt = BuildAssetBundleOptions.CollectDependencies |
			BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;
		
		var objs = FindModelPrefab ();
		if (objs.Count > 0) 
		{
			BuildPipeline.BuildAssetBundle(null, objs.ToArray(),
			                               Application.streamingAssetsPath + "/modelprefab.unity3d", opt, 
			                               EditorUserBuildSettings.activeBuildTarget);
		}
		
		if (sIsShowDialog)
			EditorUtility.DisplayDialog ("Build Assets", "Build Assets Complete!", "OK");
		
		AssetDatabase.Refresh ();
	}

	[MenuItem ("UnitySample/Build_All")]
	static void Build_All ()
	{
		Build_UI ();
	}
}
