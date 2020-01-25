using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Events
{
    public abstract class GameEvent { }

    public interface IEventListener
    {
        void Notify(GameEvent gameEvent);
    }

    public abstract class EventGenerator
    {
        protected List<IEventListener> eventListeners { get; }

        public EventGenerator()
        {
            eventListeners = new List<IEventListener>();
        }

        public void AddListener(IEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public virtual void OnEvent(GameEvent gameEvent)
        {
            foreach (var eventListener in eventListeners)
                eventListener.Notify(gameEvent);
        }

        public abstract void Update();
    }
}
