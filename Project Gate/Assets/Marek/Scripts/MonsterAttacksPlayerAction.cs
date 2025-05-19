using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Monster attacks Player", story: "[Monster] attacks [Player]", category: "Action", id: "03fd148b053db62d4fe14e9f97ef8c55")]
public partial class MonsterAttacksPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<OpponentBase> Monster;
    [SerializeReference] public BlackboardVariable<Transform> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Monster != null)
        {
            Monster.Value.Attack();
            return Status.Success;
        }

        return Status.Failure;
        //return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

