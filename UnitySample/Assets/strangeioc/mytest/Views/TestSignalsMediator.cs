using System;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace StrangeTest
{

    public class TestSignalsMediator : Mediator
    {

        [Inject]
        public TestSignalsView view { get; set; }
        
        [Inject]
        public DoManagementSignal doManagement { get; set; }

        public override void OnRegister()
        {
            view.buttonClicked.AddListener(doManagement.Dispatch);
        }

    }

}