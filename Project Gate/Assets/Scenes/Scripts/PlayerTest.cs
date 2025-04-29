using UnityEngine;

public class PlayerTest : MonoBehaviour
{



    public Transform transform => base.transform; // albo po prostu u¿ywaj transform
    public float health = 100f;

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        //Debug.Log("Player hit! Health: " + health);
    }

}
