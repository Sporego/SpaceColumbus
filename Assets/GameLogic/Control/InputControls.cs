using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.EventListeners;

namespace InputControls
{
    [System.Serializable]
    public struct KeyInfo
    {
        public KeyCode keyCode;
        public bool isKeyDownOnly;
        public bool isDoubleKey;

        public KeyInfo(KeyCode keyCode, bool isKeyDownOnly, bool isDoubleKey)
        {
            this.keyCode = keyCode;
            this.isKeyDownOnly = isKeyDownOnly;
            this.isDoubleKey = isKeyDownOnly;
        }
    }

    [System.Serializable]
    // Handles left modifiers keys (Alt, Ctrl, Shift)
    public class KeyModifier
    {
        public bool leftAlt;
        public bool leftControl;
        public bool leftShift;

        public bool isActive()
        {
            return (!leftAlt ^ Input.GetKey(KeyCode.LeftAlt)) &&
                (!leftControl ^ Input.GetKey(KeyCode.LeftControl)) &&
                (!leftShift ^ Input.GetKey(KeyCode.LeftShift));
        }
    }

    [System.Serializable]
    public static class KeyActiveChecker
    {
        public static bool isActive(KeyCode keyCode, bool onDown = false)
        {
            bool active;
            if (onDown)
                active = Input.GetKeyDown(keyCode);
            else
                active = Input.GetKey(keyCode);

            return active;
        }

        public static bool isActive(KeyInfo keyInfo)
        {
            return isActive(keyInfo.keyCode, keyInfo.isKeyDownOnly);
        }
    }

    [System.Serializable]
    public class KeyActiveEventGenerator : EventGenerator
    {
        private List<KeyInfo> keys = new List<KeyInfo>();

        public KeyActiveEventGenerator(KeyInfo[] keys)
        {
            foreach (var key in keys)
                this.keys.Add(key);
        }

        public KeyActiveEventGenerator(KeyInfo keyInfo) : this(new KeyInfo[] { keyInfo }) { }

        public bool isActive(){
            bool active = false;
            foreach (var key in keys)
                active &= KeyActiveChecker.isActive(key);
            return active;
        }

        public virtual void Update()
        {
            if (isActive())
            {
                string s = "[";
                foreach (var key in keys)
                    s += key + " ";
                s += "]";
                Debug.Log(s + " KEYS EVENT");
                OnEvent();
            }
        }
    }

    [System.Serializable]
    public abstract class KeyActiveEventListener : EventListener
    {
        public KeyInfo keyInfo { get; }

        public KeyActiveEventListener(KeyInfo keyInfo)
        {
            this.keyInfo = keyInfo;
        }

        public abstract void Notify();
    }

    public class DoubleKeyPressEventListener : KeyActiveEventListener
    {
        public float doubleKeyTime { get; }

        public DoubleKeyPressEventListener(KeyInfo keyInfo, float doubleKeyTime) : base(keyInfo)
        {
            this.doubleKeyTime = doubleKeyTime;
        }

        override public void Notify()
        {
            Debug.Log(this.keyInfo + " DOUBLE KEYDOWN EVENT");
        }
    }

    [System.Serializable]
    public class DoubleKeyPressEventGenerator : KeyActiveEventGenerator
    {
        private float doubleKeyTime;
        private float timeSinceLastPress = 0f;

        public DoubleKeyPressEventGenerator(KeyInfo keyInfo, float doubleKeyTime) : base(keyInfo)
        {
            this.doubleKeyTime = doubleKeyTime;
        }

        override public void Update()
        {
            timeSinceLastPress += Time.deltaTime;

            bool timeout = timeSinceLastPress > doubleKeyTime;

            if (isActive())
            {
                timeSinceLastPress = 0f;

                if (!timeout)
                {
                    OnEvent();
                }
            }
        }
    }
}