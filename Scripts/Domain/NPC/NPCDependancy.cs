using System;


[Serializable]
public class NPCDependancy
{
	public string otherNpcId;
	public string otherNpcFromScenarioId;
	public string otherNpcToScenarioId;
	public string myFromScenarioId;
	public string myToScenarioId;
}