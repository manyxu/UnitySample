using System;
using UnityEngine;

namespace StrangeTest
{
    public class ManagerAsMonoBehaviour : MonoBehaviour, ISomeManager
    {

        public void DoManagement()
        {
            Debug.Log("Manager implemented as MonoBehaviour");
        }
    }

}