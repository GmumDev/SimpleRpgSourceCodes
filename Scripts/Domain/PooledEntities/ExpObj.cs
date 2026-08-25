using System;
using UnityEngine;

public class ExpObj : MonoBehaviour
{
    int amount;
    Action<ExpObj> OnRelease;
    bool chasing;
    float TriggeredT;
    [SerializeField]
    ParticleSystem particle;
	public void Init(Action<ExpObj> OnRelease, int amount)
    {
        this.amount = amount;
        this.OnRelease = OnRelease;
        chasing = false;
        particle.Play();
	}
	private void Update()
	{
        if(chasing)
        {
            transform.position = Vector3.Lerp(transform.position, Player.Position, Time.deltaTime * 3f);

            if(Time.time - TriggeredT > 1f)
			{
				Player.GainExp(amount);
                chasing = false;
                particle.Stop();
				OnRelease(this);
			}
        }
	}
	private void OnTriggerEnter(Collider other)
    {
        if(chasing == false && other.gameObject.CompareTag("Player"))
        {
            chasing = true;
			TriggeredT = Time.time;
		}
    }
}
