using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelinePlayer : MonoBehaviour
{
	static TimelinePlayer instance;
	public static TimelinePlayer Instance { get => instance; }
	PlayableDirector director;
	TimelineFinishedEvent finishedEv;
	List<SubscriptionToken> tokens;
	[SerializeField]
	CinemachineBrain brain;

	public bool IsPlaying { get => director != null && director.state == PlayState.Playing; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
		finishedEv = new TimelineFinishedEvent();
		tokens = new List<SubscriptionToken>();
		tokens.Add(EventBus.Subscribe<ScenarioNodeFinishedEvent>(OnScenarioNodeFinished));
	}
	void OnScenarioNodeFinished(ScenarioNodeFinishedEvent ev)
	{
		if((ev.eventType & ScenarioNodeFinishedEventType.DoTimeline) == ScenarioNodeFinishedEventType.DoTimeline)
		{
			StopTimeline();
			PlayTimeline(ev.timelineId);
		}
	}
	void PlayTimeline(string id)
	{
		director = CinemachineTimelineDirector.Instance;
		director.playableAsset = SOLoader<TimelineSO>.Instance.GetSO(id).playable;

		BindVCam();
		director.Play();
		director.stopped += OnDirectorStopped;
	}

	private void OnDirectorStopped(PlayableDirector obj)
	{
		CameraManager.Instance.PrioritizeFocusCam();
		director.stopped -= OnDirectorStopped;
		EventBus.Publish(finishedEv);
	}

	void StopTimeline()
	{
		if(director != null)
			director.Stop();
	}
	void BindVCam()
	{
		if (director == null) return;

		TimelineAsset timeline = (TimelineAsset)director.playableAsset;
		int animtrackidx = 0;
		int acttrackidx = 0;
		foreach (var track in timeline.GetOutputTracks())
		{
			if (track is CinemachineTrack)
			{
				director.SetGenericBinding(track, brain);
				int shotIdx = 0;
				foreach (var clip in track.GetClips())
				{
					var vcam = CinemachineTimelineDirector.Vcams[shotIdx];
					shotIdx = (shotIdx + 1) % CinemachineTimelineDirector.Vcams.Count;
					var shot = clip.asset as CinemachineShot;

					director.SetReferenceValue(shot.VirtualCamera.exposedName, vcam);
				}
			}
			else if (track is AnimationTrack)
			{
				director.SetGenericBinding(track, CinemachineTimelineDirector.Vcams[animtrackidx].gameObject);
				animtrackidx = (animtrackidx + 1) % CinemachineTimelineDirector.Vcams.Count;
			}
			else if (track is ActivationTrack)
			{
				director.SetGenericBinding(track, CinemachineTimelineDirector.Vcams[acttrackidx].gameObject);
				acttrackidx = (acttrackidx + 1) % CinemachineTimelineDirector.Vcams.Count;

			}
		}
	}
	private void OnDestroy()
	{
		if(tokens != null)
			foreach (var token in tokens)
				EventBus.Unsubscribe(token);
	}

}
