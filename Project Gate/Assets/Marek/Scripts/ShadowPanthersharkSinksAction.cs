using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShadowPanthersharkSinks", story: "[ShadowPanthershark] go into ground", category: "Action", id: "c62722f6712ea3b6811e408146960459")]
public partial class ShadowPanthersharkSinksAction : Action
{
    [SerializeReference] public BlackboardVariable<ShadowPanthershark> ShadowPanthershark;

    protected override Status OnStart()
    {
        if (ShadowPanthershark != null)
        {
            ShadowPanthershark.Value.HideObjectUnderground();
            return Status.Success; 
        }
        return Status.Failure;
    }
}

