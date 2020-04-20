using System;
using System.Collections.Generic;

using UnityEngine;

using Entities;
using Animation.Systems;

namespace Animation
{
    public static class AnimationManager
    {
        private static Dictionary<Type, AnimationSystem> AnimationSystems = new Dictionary<Type, AnimationSystem>();

        public static T GetOrCreateAnimationSystem<T>() where T : AnimationSystem, new()
        {
            Type type = typeof(T);

            if (type is ExtractorAnimationSystem) { }
            else
            {
                throw new NotImplementedException("Tried to get a not-implemented Animation System.");
            }

            if (!AnimationSystems.ContainsKey(type))
                AnimationSystems[type] = new T();

            return AnimationSystems[type] as T;
        }

        public static void Update(float time, float deltaTime)
        {
            foreach (var animationSystem in AnimationSystems.Values)
            {
                animationSystem.Update(time, deltaTime);
            }
        }

        public static void RegisterAnimation<T>(AnimationBase ab) where T : AnimationSystem, new()
        {
            AnimationSystem animator = GetOrCreateAnimationSystem<T>();

            if (animator != null)
                animator.AddAnimated(ab);
        }

        public static void UnregisterAnimation<T>(AnimationBase ab) where T : AnimationSystem, new()
        {
            AnimationSystem animator = GetOrCreateAnimationSystem<T>();

            if (animator != null)
                animator.RemoveAnimated(ab);
        }
    }
}
