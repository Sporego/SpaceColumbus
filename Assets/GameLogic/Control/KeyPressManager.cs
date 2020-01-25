using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

namespace InputControls
{
    [System.Serializable]
    public class KeyPressManager : MonoBehaviour
    {
        private Dictionary<List<KeyInfo>, KeyActiveEventGenerator> keyActiveEventGenerators = new Dictionary<List<KeyInfo>, KeyActiveEventGenerator>();

        public void Update()
        {
            foreach (var keyActiveEventGenerator in keyActiveEventGenerators.Values)
            {
                keyActiveEventGenerator.Update();
            }
        }

        public KeyActiveEventGenerator GetKeyGenerator(KeyActiveEventListener keyActiveEventListener)
        {
            var keys = keyActiveEventListener.keys;
            keys.Sort();  // arbitrary built-in sort function; this might not be doing what we expect it does

            if (keyActiveEventGenerators.ContainsKey(keys))
            {
                return keyActiveEventGenerators[keys];
            }
            else
            {
                Debug.Log("Making a new listener for key " + keyActiveEventListener.keys[0].keyCode);
                KeyActiveEventGenerator keyActiveEventGenerator;
                if (keys.Count == 1)  // only 1 key
                {
                    var key = keys[0];
                    if (key.isDoubleKey)
                        keyActiveEventGenerator = new DoubleKeyPressEventGenerator(key);
                    else
                        keyActiveEventGenerator = new KeyActiveEventGenerator(key);
                }
                else // more than 1 key; can't do double press for multikey inputs
                    keyActiveEventGenerator = new KeyActiveEventGenerator(keys.ToArray());

                keyActiveEventGenerators.Add(keys, keyActiveEventGenerator);

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

