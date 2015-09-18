using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIViewLogin : UIView 
{
	Button mButRegist;
	Button mButLogin;
	Button mButFastEnter;

	protected override void OnInit()
	{
		mButRegist = TagChild_Component<Button> ("ButRegist");
		mButLogin = TagChild_Component<Button> ("ButLogin");
		mButFastEnter = TagChild_Component<Button> ("FastEnter");

		mButRegist.onClick.AddListener (OnRegistChlicked);
		mButLogin.onClick.AddListener (OnLoginClicked);
		mButFastEnter.onClick.AddListener (OnFastEnter);
	}

	void OnRegistChlicked()
	{
	}

	void OnLoginClicked()
	{
	}

	void OnFastEnter()
	{
		Application.LoadLevel ("Scene0");
		Close ();
	}
}
