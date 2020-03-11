using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Events
{
    public abstract class GameEvent { }

    public interface IEventListener<T>
    {
        void Notify(T gameEvent);
    }

    public abstract class EventGenerator<T>
    {
        protected List<IEventListener<T>> eventListeners { get; }

        public EventGenerator()
        {
            eventListeners = new List<IEventListener<T>>();
        }

        public void AddListener(IEventListener<T> eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public virtual void OnEvent(T gameEvent)
        {
            foreach (var eventListener in eventListeners)
                eventListener.Notify(gameEvent);
        }
    }

    public abstract class UpdatableEventGenerator : EventGenerator<GameEvent>
    {
        public UpdatableEventGenerator() : base() { }

        public abstract void Update();
    }
}
