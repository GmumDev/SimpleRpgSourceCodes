using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeoutOnEnabled : MonoBehaviour
{
	CanvasGroup canvasGroup;
	Coroutine fadeoutCoroutine;
	private void OnEnable()
	{
		canvasGroup = GetComponent<CanvasGroup>();

		if (fadeoutCoroutine != null)
			StopCoroutine(fadeoutCoroutine);
		canvasGroup.alpha = 1f;
		fadeoutCoroutine = StartCoroutine(FadeOut()); 
	}
	IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(1);
		while (canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime;
			yield return null;
		}

		gameObject.SetActive(false);
	}
}
