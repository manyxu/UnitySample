using System;
using UnityEngine;

namespace StrangeTest
{

    public class ManagerAsNormalClass : ISomeManager
    {

        public ManagerAsNormalClass()
        {
        }

        public void DoManagement()
        {
            Debug.Log("Manager implemented as a normal class");
        }

    }
}