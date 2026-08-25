
public class EnemyKilledEvent
{
    public string enemyId;
    public int enemyKilledCnt;
    public int dropExp;
    public EnemyKilledEvent(
        string enemyId, 
        int enemyKilledCnt,
        int dropExp)
    {
        this.enemyId = enemyId;
        this.enemyKilledCnt = enemyKilledCnt;
        this.dropExp = dropExp;
    }
}
