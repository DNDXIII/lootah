using System;
using Gameplay.Shared;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behaviours.HelperScripts
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Range Detector", story: "Update Range [Detector] and assign [Target]", category: "Action",
        id: "bb10c6b258fc36c0be882acba30b0811")]
    public partial class RangeDetectorAction : Action
    {
        [SerializeReference] public BlackboardVariable<RangeDetector> Detector;
        [SerializeReference] public BlackboardVariable<GameObject> Target;


        protected override Status OnUpdate()
        {
            var detectedTarget = Detector.Value.UpdateDetection();
            if (detectedTarget != null)
            {
                Target.Value = detectedTarget;
                return Status.Success;
            }

            Target.Value = null;
            return Status.Failure;
        }
    }
}