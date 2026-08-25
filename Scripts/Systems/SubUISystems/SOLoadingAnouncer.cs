using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SOLoadingAnouncer : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI tmpro;
    Dictionary<int, string> anouncements;
    void Start()
    {
        panel.SetActive(true);
		anouncements = new Dictionary<int, string>()
        {
            {0, "시나리오 정보 로딩 중..." },
			{5, "타임라인 정보 로딩 중..." },
			{8, "퀘스트 정보 로딩 중..." },
			{10, "로딩 완료! 게임이 잠시 후 시작해요" },
		};

	}

    // Update is called once per frame
    void Update()
    {
        int v = GameManager.Instance.LoadingProgress;

		if (anouncements.ContainsKey(v))
        {
            tmpro.text = anouncements[v];
        }
	}
}
