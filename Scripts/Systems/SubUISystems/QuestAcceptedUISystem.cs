using System.Collections.Generic;
using UnityEngine;

public class QuestAcceptedUISystem : MonoBehaviour
{
	static QuestAcceptedUISystem instance;
	[Header("Quest Completed Panel")]
	[SerializeField]
	GameObject questAcceptedPanel;
	List<SubscriptionToken> tokens = new List<SubscriptionToken>();


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	void Start()
	{
		questAcceptedPanel.SetActive(false);

	}
	private void OnEnable()
	{
		tokens.Add(EventBus.Subscribe<QuestAcceptedEvent>(OnQuestAccepted));
	}
	private void OnDisable()
	{
		foreach (var token in tokens)
			EventBus.Unsubscribe(token);
	}

	void OnQuestAccepted(QuestAcceptedEvent ev)
	{
		questAcceptedPanel.SetActive(true);
	}
}