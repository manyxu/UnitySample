using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace StrangeTest
{

    public class TestBootstrap : ContextView
    {
        void Awake()
        {
            this.context = new TestSignalsContext(this);
        }
    }

}