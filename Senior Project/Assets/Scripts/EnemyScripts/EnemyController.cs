using UnityEngine;




public class EnemyController : MonoBehaviour
{
    public Stats stats;
    [HideInInspector] public bool inInvicibilityFrames = false;
    [HideInInspector] public bool dead = false;

    public virtual void Damage(float damageAmount)
    {
        stats.AddToStat(StatType.health, -damageAmount);
        if (stats.stat[StatType.health] <= 0)
        {
            CommitDie();
        }
    }

    public virtual void CommitDie()
    {
        dead = true;
        Debug.Log(gameObject.name + " is dead");
    }

    public virtual void Explode()
    {
        Debug.Log("Boom");
    }
}
