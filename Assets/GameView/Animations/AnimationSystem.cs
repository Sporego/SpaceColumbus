using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common;

namespace Animation.Systems
{
    // derive from this class to customize params
    public abstract class AnimationParams : ScriptableObject { }

    public abstract class AnimationBase : IIdentifiable
    {
        public GameObject go { get; set; }

        public AnimationParams Params { get; set; }

        public AnimationBase(GameObject go, AnimationParams Params)
        {
            this.go = go;
            this.Params = Params;
        }

        public int GetId()
        {
            return this.go.GetInstanceID();
        }
    }

    public abstract class AnimationSystem<T> : AnimationSystem where T : AnimationBase
    {
        public override void Update(float time, float deltaTime, AnimationBase animationBase)
        {
            Update(time, deltaTime, animationBase as T);
        }

        public abstract void Update(float time, float deltaTime, T animationBase);
    }

    public abstract class AnimationSystem
    {
        private static readonly int AnimationSystemCapacity = 997; // 997 is a prime number

        public Dictionary<int, AnimationBase> Animated;

        public AnimationSystem() : this(-1) { }

        public AnimationSystem(int capacity)
        {
            if (capacity == -1)
                capacity = AnimationSystemCapacity;

            this.Animated = new Dictionary<int, AnimationBase>(capacity: capacity);
        }

        public virtual void Update(float time, float deltaTime)
        {
            foreach (var animationBase in Animated.Values)
            {
                Update(time, deltaTime, animationBase);
            }
        }

        public abstract void Update(float time, float deltaTime, AnimationBase animationBase);

        public virtual void AddAnimated(AnimationBase ap)
        {
            int id = ap.GetId();

            if (!this.Animated.ContainsKey(id))
                this.Animated.Add(id, ap);
        }

        public void RemoveAnimated(AnimationBase ap)
        {
            RemoveAnimated(ap.GetId());
        }

        public void RemoveAnimated(int id)
        {
            if (this.Animated.ContainsKey(id))
                this.Animated.Remove(id);
        }
    }
}