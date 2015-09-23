using UnityEngine;
using System.Collections;

public class NetSystem 
{
	private NetSystem()
	{
	}

	private static NetSystem sNetSystem = null;

	public static NetSystem GetInstance()
	{
		if (null == sNetSystem)
			sNetSystem = new NetSystem ();

		return sNetSystem;
	}

	public void Destroy()
	{
		sNetSystem = null;
	}

	/*public Session Session
	{
		get { return _session; }
	}*/

}
