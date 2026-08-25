using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractUISystem : MonoBehaviour
{
    static InteractUISystem instance;
    public static InteractUISystem Instance { get => instance; }

    
    [Header("Player NPC Interaction Panel")]
    [SerializeField]
    GameObject interactHoverPanel;
    [SerializeField]
    TextMeshProUGUI interactMassageTextField;

    private Dictionary<string, string> interactionMassage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        interactHoverPanel.SetActive(false);
        interactionMassage = new Dictionary<string, string>()
        {
            {"NPC", "대화하기"},
            {"Gatherable", "채집하기"},
			{"Gathering", "취소"},
		};
    }

    public void InteractTargetedOn(string targetType)
    {
        if(interactionMassage.ContainsKey(targetType))
        {
            interactMassageTextField.text = interactionMassage[targetType];
		}
        interactHoverPanel.SetActive(true);
    }
    public void InteractTargetedOff()
	{
        if(interactHoverPanel != null)
            interactHoverPanel.SetActive(false);
    }


}
