using System;
using System.Collections.Generic;

using UnityEngine;

using Animation;
using Animation.Systems;

namespace Entities
{
    public class StructureExtractor : Structure
    {
        public GameObject AnimatedObject;
        public ExtractorAnimationParams AnimationParams;
        private ExtractorAnimationBase AnimationBase;

        override public string Name { get { return "Extractor"; } }

        override public void Start()
        {
            base.Start();

            SetupAnimations();

            AnimationManager.RegisterAnimation<ExtractorAnimationSystem>(this.AnimationBase);
        }

        void SetupAnimations()
        {
            this.AnimationBase = new ExtractorAnimationBase(AnimatedObject, AnimationParams);
        }

        private void OnDestroy()
        {
            AnimationManager.UnregisterAnimation<ExtractorAnimationSystem>(this.AnimationBase);
        }
    }
}
