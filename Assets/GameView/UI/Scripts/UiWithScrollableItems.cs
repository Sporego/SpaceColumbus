using System;
using System.Collections.Generic;

using UnityEngine;

namespace UI.Menus
{
    [System.Serializable]
    public class UiWithScrollableItems : MonoBehaviour
    {
        public UiTextField TitleTextLeft = new UiTextField();
        public UiTextField TitleTextRight = new UiTextField();

        public GameObject ContentRoot;

        public virtual void Awake()
        {
            TitleTextLeft.Initialize();
            TitleTextRight.Initialize();
        }

        void OnValidate()
        {
            Awake();
        }
    }
}
