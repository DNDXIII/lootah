using System;
using Gameplay.Shared;
using Unity.Behavior;
using UnityEngine;

namespace Behaviours.HelperScripts
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Line of Sight Detector", story: "Has LOS to [Target] with [Detector]", category: "Conditions", id: "84470b52d53d08b212ea3767020e5424")]
    public partial class LineOfSightDetectorCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<LineOfSightDetector> Detector;

        public override bool IsTrue()
        {
            return Detector.Value.PerformDetection(Target.Value.transform.position);
        }
    }
}
