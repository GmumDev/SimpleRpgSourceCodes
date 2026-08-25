using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SOLoader<TSO> where TSO : SORuntimeLoadable
{
	private static SOLoader<TSO> instance;
	public static SOLoader<TSO> Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SOLoader<TSO>();
			}
			return instance;
		}
	}
	private Dictionary<string, TSO> datas = new Dictionary<string, TSO>();

    public async Awaitable LoadData(string id)
    {
        string assetKey = id;
        AsyncOperationHandle<TSO> handle = Addressables.LoadAssetAsync<TSO>(assetKey);

        TSO so = await handle.Task;
        try
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 2. 중복 키 체크 (TryAdd 또는 인덱서 사용)
                if (datas.ContainsKey(so.id))
                {
                    Debug.LogWarning($"[LoadData] 이미 존재하는 키입니다: {so.id}");
                    // 이미 존재한다면 새로 로드한 것은 해제해줘야 할 수도 있음
                    Addressables.Release(handle);
                    return;
                }

                datas.Add(so.id, so);
                Debug.Log($"[LoadData] 로드 성공: {assetKey}");
            }
            else
            {
                Debug.LogError($"[LoadData] 로드 실패: {assetKey}");
                Addressables.Release(handle);
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (handle.IsValid()) Addressables.Release(handle);
        }
    }
    // 나중에 데이터를 지울 때
    public void UnloadData(string id)
    {
        if (datas.Remove(id, out TSO so))
        {
            Addressables.Release(so); // 에셋 참조를 넣어 해제 (카운트 -1)
        }
    }
    public TSO GetSO(string id)
	{
		if (datas.ContainsKey(id)) return datas[id];
		return default;
	}
}
