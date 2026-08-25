using UnityEngine;
using UnityEngine.UI;

public class FadeInoutPanel : MonoBehaviour
{
	[SerializeField]
	Image _panel;
    bool isActive;
    [SerializeField]
    bool ToggleActivation;
    public bool IsActive { get => isActive; }
    public async Awaitable Fadein()
	{
        if (ToggleActivation) gameObject.SetActive(true);
        isActive = true;
        try
        {
            _panel.color = new Color(0, 0, 0, 0);
            this.gameObject.SetActive(true);
            while (_panel.color.a < 0.99f)
            {
                float newAlpha = Mathf.Min(_panel.color.a + Time.deltaTime, 1f);
                _panel.color = new Color(0, 0, 0, newAlpha);
                await Awaitable.NextFrameAsync();
            }
            _panel.color = new Color(0, 0, 0, 1);
            Debug.Log("[Fadein] 완료");
        }
        catch (System.Exception e)
        {
            _panel.color = new Color(0, 0, 0, 1);
            Debug.LogError($"[Fadein] 예외: {e.Message}");
        }
    }
    public async Awaitable Fadeout()
    {
        try
        {
            while (_panel.color.a > 0.01f)
            {
                float newAlpha = Mathf.Max(_panel.color.a - Time.deltaTime, 0f);
                _panel.color = new Color(0, 0, 0, newAlpha);
                await Awaitable.NextFrameAsync();
            }
            _panel.color = new Color(0, 0, 0, 0);
            Debug.Log("[FadeOut] 완료");
        }
        catch (System.Exception e)
        {
            _panel.color = new Color(0, 0, 0, 0);
            Debug.LogError($"[FadeOut] 예외: {e.Message}");
        }
        isActive = false;
		if (ToggleActivation) gameObject.SetActive(false);
	}
}
