using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Threading.Tasks;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyAttack", story: "[Agent] attacks [Target]", category: "Action", id: "33d70525c848d86454fc81169813674e")]
public partial class EnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        /*if (Agent != null)
        {
            Agent.Attack();  // Call the Attack method
            return Status.Success;
        }

        return Status.Failure;*/
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

