using UnityEngine;
using System.Collections;

public class BatStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AssetBundle ab = AssetsManager.GetInstance().GetFromCache("modelprefab.unity3d");

		GameObject groundPrefab = ab.Load("Ground") as GameObject;
		GameObject ground = GameObject.Instantiate (groundPrefab) as GameObject;
		
		GameObject tankPrefab = ab.Load("Tank") as GameObject;
		GameObject tank = GameObject.Instantiate (tankPrefab) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
