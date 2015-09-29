using UnityEngine;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.command.impl;

namespace StrangeTest
{
    public class StartCommand : Command
    {
        public override void Execute()
        {
            // perform all game start setup here
            Debug.Log("Hello World");
        }
    }
}