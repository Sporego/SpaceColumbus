using UnityEngine;

namespace Animation.Systems
{
    [CreateAssetMenu(fileName = "New Extractor Animation Params", menuName = "ScriptableObjects/ExtractorAnimationParams")]
    public class ExtractorAnimationParams : AnimationParams
    {
        public float speed;
        public float moveAmount;
    }

    public class ExtractorAnimationBase : AnimationBase
    {
        public Transform Pivot { get; private set; }
        public float TimeAtSpawn;

        public ExtractorAnimationBase(GameObject go, ExtractorAnimationParams Params) : base(go, Params)
        {
            this.TimeAtSpawn = Time.time;
            this.Pivot = this.go.transform.parent.transform;
        }
    }

    public class ExtractorAnimationSystem : AnimationSystem<ExtractorAnimationBase>
    {
        override public void Update(float time, float deltaTime, ExtractorAnimationBase eab)
        {
            var Params = eab.Params as ExtractorAnimationParams;
            time -= eab.TimeAtSpawn;
            eab.go.transform.position = eab.Pivot.position + new Vector3(0, Params.moveAmount * Mathf.Sin(time * Params.speed));
        }
    }
}
