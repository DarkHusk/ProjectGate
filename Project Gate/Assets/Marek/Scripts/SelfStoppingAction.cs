using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Self stopping", story: "[Self] stops moving", category: "Action", id: "f0684303bb615a9ea4e53e288f463046")]
public partial class SelfStoppingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        NavMeshAgent agent = Self.Value.GetComponent<NavMeshAgent>();
           if (agent != null)
           {
               agent.isStopped = true;
               agent.velocity = Vector3.zero;
               return Status.Success;
           }
        return Status.Failure;
    }
}

