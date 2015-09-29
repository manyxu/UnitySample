using System;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace StrangeTest
{

    public class TestSignalsView : View
    {
        public Signal buttonClicked = new Signal();

        private Rect buttonRect = new Rect(0, 0, 200, 50);
        public void OnGUI()
        {
            if (GUI.Button(buttonRect, "Manage"))
            {
                buttonClicked.Dispatch();
            }
        }
    }

}