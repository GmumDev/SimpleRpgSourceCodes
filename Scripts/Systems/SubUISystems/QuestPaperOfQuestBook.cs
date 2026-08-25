using System;
using TMPro;
using UnityEngine;

public class QuestPaperOfQuestBook : MonoBehaviour
{
	Action<string> OnClickedCallback;
	[SerializeField]
	TextMeshProUGUI title;

	string qid;

	public void Init(Action<string> OnClickedCallback, string qid, string title)
	{
		this.title.text = title;
		this.OnClickedCallback = OnClickedCallback;
		this.qid = qid;
	}
	// 이 스크립트 붙은 obj가 버튼이라 밖에서 참조함
	public void OnClicked()
	{
		OnClickedCallback(qid);
	}
}
