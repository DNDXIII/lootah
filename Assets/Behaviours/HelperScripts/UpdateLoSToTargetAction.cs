using System;
using Gameplay.Shared;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behaviours.HelperScripts
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Update LoS to Target", story: "Update [LoS] to [Target] with [Detector]",
        category: "Action",
        id: "84c715c849702e6cf5e04bd8f29e0a26")]
    public partial class UpdateLoSToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<bool> LoS;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<LineOfSightDetector> Detector;

        protected override Status OnUpdate()
        {
            LoS.Value = Detector.Value.PerformDetection(Target.Value.transform.position);

            return Status.Success;
        }
    }
}