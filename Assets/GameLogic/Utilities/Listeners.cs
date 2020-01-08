using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.EventListeners
{
    public interface EventListener
    {
        void Notify();
    }

    public class EventGenerator
    {
        private List<EventListener> eventListeners = new List<EventListener>();

        public void AddListener(EventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void OnEvent()
        {
            foreach (var eventListener in eventListeners)
                eventListener.Notify();
        }
    }
}
