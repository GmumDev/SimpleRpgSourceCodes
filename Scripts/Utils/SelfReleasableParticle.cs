using System;
using UnityEngine;

public class SelfReleasableParticle : MonoBehaviour
{
	Action<ParticleSystem> ReleaseAction;
	void Start()
	{
		// fail saif
		var main = GetComponent<ParticleSystem>().main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}
	public void Init(Action<ParticleSystem> releaseAction)
	{
		this.ReleaseAction = releaseAction;
	}
	void OnParticleSystemStopped()
	{
		ReleaseAction(GetComponent<ParticleSystem>());
	}
}
