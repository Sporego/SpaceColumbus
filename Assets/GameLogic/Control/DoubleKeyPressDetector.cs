using System;
using System.Collections.Generic;
using UnityEngine;

using Utilities.EventListeners;

namespace InputControls
{
    public class DoubleKeyPressDetector : MonoBehaviour
    {
        public float doubleKeyTime = 0.2f;

        public KeyInfo keyInfo;

        public void Start()
        {
            this.GetComponent<KeyPressManager>().AddKeyPressListener(new DoubleKeyPressEventListener(keyInfo, doubleKeyTime));
        }
    }
}

