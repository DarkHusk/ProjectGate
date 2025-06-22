using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShadowPanthersharkEmergesFromGround", story: "[ShadowPanthershark] emerges from the ground", category: "Action", id: "f42a5db392dc264a9a17877cca097440")]
public partial class ShadowPanthersharkEmergesFromGroundAction : Action
{
    [SerializeReference] public BlackboardVariable<ShadowPanthershark> ShadowPanthershark;

    protected override Status OnStart()
    {
        if (ShadowPanthershark != null)
        {
            ShadowPanthershark.Value.RestoreObject();
            return Status.Success;
        }
        return Status.Failure;
    }
}

