using UnityEngine;
using UnityEngine.Pool;

public class ParticlePoolManager: MonoBehaviour
{
    [SerializeField]
    GameObject particlePrefab;

	IObjectPool<ParticleSystem> particlePool;

	public void SetParticle(GameObject particle)
	{
		this.particlePrefab = particle;
	}
	private void Awake()
	{
		particlePool = new ObjectPool<ParticleSystem>(
			createFunc: CreateItem,
				actionOnGet: OnGet,
				actionOnRelease: OnRelease,
				actionOnDestroy: OnDestroyItem,
				collectionCheck: true   // helps catch double-release mistakes
			);
	}

	public void Generate(Vector3 position)
	{
		var obj = particlePool.Get();
		if (AsyncSceneManager.IsDontDestroyOnLoad(gameObject))
			DontDestroyOnLoad(obj.gameObject);
		obj.gameObject.transform.position = position;
		obj.GetComponent<SelfReleasableParticle>().Init(particlePool.Release);
	}

	#region ObjectPool
	private ParticleSystem CreateItem()
	{
		GameObject gameObject = Instantiate(particlePrefab);
		var particle = gameObject.GetComponent<ParticleSystem>();
		particle.Stop();
		return particle;
	}

	// Called when an item is taken from the pool.
	private void OnGet(ParticleSystem particle)
	{
		particle.Play();
	}

	// Called when an item is returned to the pool.
	private void OnRelease(ParticleSystem particle)
	{
		particle.Stop();
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(ParticleSystem particle)
	{
		Destroy(particle.gameObject);
	}
	#endregion
}
