using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Spawns Monsters", story: "[Enemy] performs Spawn() action and clears spawnList null references", category: "Action", id: "ad0ca8bc9d5abfd0ce893fae11fe0c1c")]
public partial class EnemySpawnsMonstersAction : Action
{
    [SerializeReference] public BlackboardVariable<Lich> Enemy;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Enemy != null)
        {
            Enemy.Value.CleanUpDestroyedMonsters();
            Enemy.Value.Spawn();
            return Status.Success;
        }
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

