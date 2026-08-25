using UnityEditor;
using UnityEngine;

public class DrawCircleEditorMode : MonoBehaviour
{
	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		// 원의 중심을 현재 오브젝트 위치로 설정
		Handles.color = Color.black;
		// Handles.DrawWireDisc (중심점, 노멀 벡터(방향), 반지름)
		Handles.DrawWireDisc(transform.position, Vector3.up, 1f);
#endif
	}
}
