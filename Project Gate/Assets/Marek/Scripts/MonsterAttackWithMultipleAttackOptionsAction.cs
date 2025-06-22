using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Threading;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MonsterAttackWithMultipleAttackOptions", story: "[Self] attacks [Target] with attack [whichAttack]", category: "Action", id: "739f392479622492c33adc5903666c90")]
public partial class MonsterAttackWithMultipleAttackOptionsAction : Action
{
    [SerializeReference] public BlackboardVariable<ShadowPanthershark> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> WhichAttack;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self != null)
        {
            Self.Value.Attack(WhichAttack);
            return Status.Success;
        }

        return Status.Failure;
    }
}

