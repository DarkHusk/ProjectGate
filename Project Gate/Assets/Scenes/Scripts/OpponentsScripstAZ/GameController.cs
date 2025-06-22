using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Golem opponent;
    public Vampire opponentVamp;
    public Werewolf opponentwere;
    public FlyingDemon flyingDemon;
    float damageInterval = 2f; // time in sec
    float damageTimer = 0f;

    void FixedUpdate()
    { 
        {
           // opponent.Attack();

            damageTimer += Time.fixedDeltaTime;

            if (damageTimer >= damageInterval)
            {
                 if(opponent!= null)
                opponent.TakeDamage(10);
                if (opponentVamp != null)
                    opponentVamp.TakeDamage(10);
                if (opponentwere != null)
                    opponentwere.TakeDamage(10);
                if (flyingDemon != null)
                    flyingDemon.TakeDamage(10);
                
                //Debug.Log("Opponent hit! Health: " + opponent.currentHealth);

                damageTimer = 0f;
            }
        }
    }


}
