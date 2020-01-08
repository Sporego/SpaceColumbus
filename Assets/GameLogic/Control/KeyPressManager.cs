using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.EventListeners;

namespace InputControls
{
    [System.Serializable]
    public class KeyPressManager : MonoBehaviour
    {
        private Dictionary<KeyInfo, KeyActiveEventGenerator> keyActiveEventGenerators = new Dictionary<KeyInfo, KeyActiveEventGenerator>();

        public void Update()
        {
            foreach (var keyActiveEventGenerator in keyActiveEventGenerators.Values)
            {
                keyActiveEventGenerator.Update();
            }
        }

        public KeyActiveEventGenerator GetKeyGenerator(KeyActiveEventListener keyActiveEventListener)
        {
            KeyInfo keyInfo = keyActiveEventListener.keyInfo;
            if (keyActiveEventGenerators.ContainsKey(keyInfo))
            {
                return keyActiveEventGenerators[keyInfo];
            }
            else
            {
                KeyActiveEventGenerator keyActiveEventGenerator;
                if (keyInfo.isDoubleKey)
                    keyActiveEventGenerator = new DoubleKeyPressEventGenerator(keyInfo,
                        ((DoubleKeyPressEventListener)keyActiveEventListener).doubleKeyTime);
                else
                    keyActiveEventGenerator = new KeyActiveEventGenerator(keyInfo);

                keyActiveEventGenerators.Add(keyInfo, keyActiveEventGenerator);

                return keyActiveEventGenerator;
            }
        }

        public void AddKeyPressListener(KeyActiveEventListener keyActiveEventListener)
        {
            KeyActiveEventGenerator keyActiveEventGenerator = GetKeyGenerator(keyActiveEventListener);
            keyActiveEventGenerator.AddListener(keyActiveEventListener);
        }
    }
}

