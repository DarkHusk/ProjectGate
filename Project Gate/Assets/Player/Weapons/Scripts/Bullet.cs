using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [SerializeField] BulletData bulletData;

    List<GameObject> opponentsHit = new List<GameObject>();

    void OnCollisionEnter(Collision collision)
    {
        OpponentBase enemy = collision.gameObject.GetComponent<OpponentBase>();
        if (enemy != null && !opponentsHit.Contains(enemy.gameObject))
        {
            enemy.TakeDamage(bulletData.GetDamage());
            opponentsHit.Add(enemy.gameObject);
        }
    }
}
