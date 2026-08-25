using System.Collections.Generic;
using UnityEngine;
using static NPC_QuestGiver;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NPCSO")]
public class NPCSO: ScriptableObject
{
    public string id;
    public string npcName;
    public string scenarioId;

	public Dictionary<string, OwnedQuestCompletion> ownedQuestStates;
	[SerializeField] List<string> ownedQuestStates_Keys;
	[SerializeField] List<OwnedQuestCompletion> ownedQuestStates_Values;

	public NPCDependancy[] NPCDependancies;  // connection class 

	public void Serialize()
	{
		ownedQuestStates_Keys = new List<string>();
		ownedQuestStates_Values = new List<OwnedQuestCompletion>();
		foreach(var obj in ownedQuestStates)
		{
			ownedQuestStates_Keys.Add(obj.Key);
			ownedQuestStates_Values.Add(obj.Value);
		}
	}
	public void Deserialize()
	{
		ownedQuestStates = new Dictionary<string, OwnedQuestCompletion>();
		if(ownedQuestStates_Keys != null)
			for(int i = 0; i < ownedQuestStates_Keys.Count; i++)
			{
				ownedQuestStates.Add(ownedQuestStates_Keys[i], ownedQuestStates_Values[i]);
			}
	}
}
