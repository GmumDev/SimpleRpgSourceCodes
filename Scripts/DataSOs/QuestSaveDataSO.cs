using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

[CreateAssetMenu(fileName = "QuestSaveDataSO", menuName = "Scriptable Objects/QuestSaveDataSO")]
public class QuestSaveDataSO : ScriptableObject
{
	public Dictionary<string, QuestState> datas;
	[SerializeField] List<string> datas_key;
	[SerializeField] List<QuestState> datas_value;

	public void Deserialize()
	{
		datas = new Dictionary<string, QuestState>();
		if(datas_key != null)
			for(int i = 0; i < datas_key.Count; i++)
			{
				datas.Add(datas_key[i], datas_value[i]);
			}
	}
	public void Serialize()
	{
		datas_key = new List<string>();
		datas_value = new List<QuestState>();
		foreach (var data in datas)
		{
			datas_key.Add(data.Key);
			datas_value.Add(data.Value);
		}
	}
}
