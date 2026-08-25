using UnityEngine;
using UnityEngine.Pool;

public class ExpObjPoolManager : MonoBehaviour
{
    static ExpObjPoolManager instance;
    public static ExpObjPoolManager Instance { get => instance; }
    [SerializeField]
    GameObject expPrefab;

    IObjectPool<ExpObj> expPool;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        expPool = new ObjectPool<ExpObj>(
            createFunc: CreateItem,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: true,   // helps catch double-release mistakes
                defaultCapacity: 10,
                maxSize: 100
        );
    }
    public void Generate(Vector3 position, int amount)
    {
        var obj = expPool.Get();
		obj.gameObject.transform.position = position;
        obj.Init(OnRelease, amount);
    }

    #region ObjectPool
    private ExpObj CreateItem()
    {
        GameObject gameObject = Instantiate(expPrefab, transform);
        var expobj = gameObject.GetComponent<ExpObj>();
        return expobj;
    }

    // Called when an item is taken from the pool.
    private void OnGet(ExpObj expball)
    {
        expball.gameObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(ExpObj expball)
    {
        expball.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(ExpObj expball)
    {
        Destroy(expball.gameObject);
    }
    #endregion
}
