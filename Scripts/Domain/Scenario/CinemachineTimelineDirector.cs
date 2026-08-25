using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinemachineTimelineDirector : MonoBehaviour
{
	[SerializeField] List<CinemachineCamera> vcams;
	public static List<CinemachineCamera> Vcams;
	static PlayableDirector instance;
	public static PlayableDirector Instance { get => instance; }
	private void Awake()
	{
		if(instance != null)
		{
			Destroy(instance.gameObject);
			return;
		}
		instance = GetComponent<PlayableDirector>();

		CinemachineTimelineDirector.Vcams = vcams;
	}
}
