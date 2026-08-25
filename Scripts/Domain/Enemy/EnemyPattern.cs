using UnityEngine;

public abstract class EnemyPattern : MonoBehaviour, IEnemyPattern
{
    public abstract void DoPattern();
    public abstract void CancelPattern();
}
