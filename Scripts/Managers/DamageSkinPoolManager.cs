using UnityEngine;
using UnityEngine.Pool;

public class DamageSkinPoolManager : MonoBehaviour
{
    static DamageSkinPoolManager instance;
    public static DamageSkinPoolManager Instance { get => instance; }
	[SerializeField]
	Transform worldSpaceCanvas;
	[SerializeField]
    GameObject damageSkinPrefab;

    IObjectPool<DamageSkin> pool;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

		pool = new ObjectPool<DamageSkin>(
            createFunc: CreateItem,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: true,   // helps catch double-release mistakes
                defaultCapacity: 10,
                maxSize: 100
        );
    }
    public void Generate(Vector3 position, int damage)
    {
        var obj = pool.Get();
		obj.gameObject.transform.position = position;
        obj.Init(OnRelease, damage);
    }

    #region ObjectPool
    private DamageSkin CreateItem()
    {
        GameObject gameObject = Instantiate(damageSkinPrefab, worldSpaceCanvas);
        var obj = gameObject.GetComponent<DamageSkin>();
        return obj;
    }

    // Called when an item is taken from the pool.
    private void OnGet(DamageSkin obj)
    {
		obj.gameObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(DamageSkin obj)
    {
		obj.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(DamageSkin obj)
    {
        Destroy(obj.gameObject);
    }
    #endregion
}
