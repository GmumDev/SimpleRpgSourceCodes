using System;
using System.Text;

[Serializable]
public class QuestRewardContext
{
    public QuestRewardType type;
	public int amount;

	// item reward
	public string rewardId;

	public string UIText
	{
		get
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(ItemDataContainer.GetNameWithId(rewardId)).Append(' ').Append(amount).Append('°³');
			return builder.ToString();
		}
	}
}
