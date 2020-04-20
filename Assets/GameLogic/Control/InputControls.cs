using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

namespace InputControls
{
    [System.Serializable]
    public struct KeyInfo
    {
        public enum OnKey: byte
        {
            Down,
            Up,
            Hold
        }

        public KeyCode keyCode;
        public OnKey onKey;
        public bool isDoubleKey;

        public KeyInfo(KeyCode keyCode, OnKey onKey, bool isDoubleKey)
        {
            this.keyCode = keyCode;
            this.onKey = onKey;
            this.isDoubleKey = isDoubleKey;
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
        public static bool isActive(KeyCode keyCode, KeyInfo.OnKey onKey = KeyInfo.OnKey.Down)
        {
            bool active;
            if (onKey == KeyInfo.OnKey.Down)
                active = Input.GetKeyDown(keyCode);
            else if (onKey == KeyInfo.OnKey.Up)
                active = Input.GetKeyUp(keyCode);
            else
                active = Input.GetKey(keyCode);

            return active;
        }

        public static bool isActive(KeyInfo keyInfo)
        {
            return isActive(keyInfo.keyCode, keyInfo.onKey);
        }
    }

    [System.Serializable]
    public class KeyActiveEventGenerator : UpdatableEventGenerator
    {
        public List<KeyInfo> keys = new List<KeyInfo>();

        public KeyActiveEventGenerator(KeyInfo[] keys) : base()
        {
            foreach (var key in keys)
                this.keys.Add(key);
        }

        public KeyActiveEventGenerator(KeyInfo keyInfo) : this(new KeyInfo[] { keyInfo }) { }

        public bool isActive(){
            bool active = true;
            foreach (var key in keys)
                active &= KeyActiveChecker.isActive(key);
            return active;
        }

        public override void Update()
        {
            if (isActive())
            {
                Notify(null);
            }
        }
    }

    [System.Serializable]
    public abstract class KeyActiveEventListener : IEventListener<GameEvent>
    {
        public List<KeyInfo> keys = new List<KeyInfo>();

        public KeyActiveEventListener(KeyInfo[] keys)
        {
            foreach (var key in keys)
                this.keys.Add(key);
        }

        public KeyActiveEventListener(KeyInfo keyInfo) : this(new KeyInfo[] { keyInfo }) { }

        // extend from this class and override this method to perform some action
        public abstract bool OnEvent(GameEvent gameEvent);
    }

    [System.Serializable]
    public class DoubleKeyPressEventGenerator : KeyActiveEventGenerator
    {
        private float doubleKeyTime;
        private float timeSinceLastPress = 0f;

        public DoubleKeyPressEventGenerator(KeyInfo keyInfo, float doubleKeyTime = 0.2f) : base(keyInfo)
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
                    Notify(null);
                }
            }
        }
    }
}