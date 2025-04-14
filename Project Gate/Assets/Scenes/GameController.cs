using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public OpponentBase opponent;

    float damageInterval = 1f; // time in sec
    float damageTimer = 0f;

    void FixedUpdate()
    { if (opponent != null)
        {
            opponent.Attack();

            damageTimer += Time.fixedDeltaTime;

            if (damageTimer >= damageInterval)
            {
                opponent.TakeDamage(5);

                damageTimer = 0f;
            }
        }
    }


}
