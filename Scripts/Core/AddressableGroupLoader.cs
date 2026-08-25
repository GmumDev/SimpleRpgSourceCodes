using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// SaveData 및 SaveData_Origin Addressable 그룹의 에셋을 런타임에 로드하는 매니저
/// </summary>
public class AddressableGroupLoader : MonoBehaviour
{
    static AddressableGroupLoader instance;
    public static AddressableGroupLoader Instance { get => instance; }
    // ──────────────────────────────────────────────
    // 그룹 레이블 (Addressables Groups 창에서 설정한 Label과 일치해야 합니다)
    // ──────────────────────────────────────────────
    private const string LABEL_SAVE_DATA = "SaveData";
    private const string LABEL_SAVE_DATA_ORIGIN = "SaveData_Origin";

    // ──────────────────────────────────────────────
    // 로드된 에셋 캐시
    // ──────────────────────────────────────────────
    private readonly Dictionary<string, ScriptableObject> _saveDataAssets = new();
    private readonly Dictionary<string, ScriptableObject> _saveDataOriginAssets = new();

    // 핸들 추적 (Unload 시 사용)
    private readonly List<AsyncOperationHandle> _handles = new();

    // ──────────────────────────────────────────────
    // 공개 프로퍼티
    // ──────────────────────────────────────────────
    public IReadOnlyDictionary<string, ScriptableObject> SaveDataAssets => _saveDataAssets;
    public IReadOnlyDictionary<string, ScriptableObject> SaveDataOriginAssets => _saveDataOriginAssets;

    public bool IsLoaded { get; private set; }

    // ──────────────────────────────────────────────
    // 생명주기
    // ──────────────────────────────────────────────
    private void Awake()
    {
        if(instance != null)
        {
            return;
        }
        instance = this;
    }
    private async void Start()
    {
        await LoadAllGroupsAsync();
    }

    // ──────────────────────────────────────────────
    // 메인 로드 메서드
    // ──────────────────────────────────────────────

    /// <summary>두 그룹을 병렬로 로드합니다.</summary>
    public async Task LoadAllGroupsAsync()
    {
        IsLoaded = false;
        Debug.Log("[AddressableGroupLoader] 두 그룹 로드 시작...");

        // 두 그룹을 동시에 로드
        await Task.WhenAll(
            LoadGroupAsync(LABEL_SAVE_DATA, _saveDataAssets, "SaveData"),
            LoadGroupAsync(LABEL_SAVE_DATA_ORIGIN, _saveDataOriginAssets, "SaveData_Origin")
        );

        IsLoaded = true;
        Debug.Log($"[AddressableGroupLoader] 로드 완료 — " +
                  $"SaveData: {_saveDataAssets.Count}개, " +
                  $"SaveData_Origin: {_saveDataOriginAssets.Count}개");

        OnAllGroupsLoaded();
    }

    /// <summary>단일 레이블의 모든 ScriptableObject 에셋을 로드합니다.</summary>
    private async Task LoadGroupAsync(
        string label,
        Dictionary<string, ScriptableObject> cache,
        string logName)
    {
        // 1) 해당 레이블의 리소스 위치 조회
        var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(ScriptableObject));
        _handles.Add(locationHandle);
        IList<IResourceLocation> locations = await locationHandle.Task;

        if (locations == null || locations.Count == 0)
        {
            Debug.LogWarning($"[AddressableGroupLoader] '{logName}' 그룹에서 에셋을 찾지 못했습니다. " +
                             $"레이블 '{label}'이 올바르게 설정되었는지 확인하세요.");
            return;
        }

        Debug.Log($"[AddressableGroupLoader] '{logName}' 그룹: {locations.Count}개 위치 발견");

        // 2) 각 위치에서 에셋 로드 (병렬)
        var loadTasks = new List<Task>();
        foreach (var location in locations)
        {
            loadTasks.Add(LoadSingleAssetAsync(location, cache, logName));
        }
        await Task.WhenAll(loadTasks);
    }

    private async Task LoadSingleAssetAsync(
        IResourceLocation location,
        Dictionary<string, ScriptableObject> cache,
        string logName)
    {
        var handle = Addressables.LoadAssetAsync<ScriptableObject>(location);
        _handles.Add(handle);

        ScriptableObject asset = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded && asset != null)
        {
            // primaryKey = 에셋 경로 (예: "datas/NPCs/NPC_Archer")
            string key = location.PrimaryKey;
            cache[key] = asset;
            Debug.Log($"[AddressableGroupLoader] [{logName}] 로드 성공: {key}");
        }
        else
        {
            Debug.LogError($"[AddressableGroupLoader] [{logName}] 로드 실패: {location.PrimaryKey}");
        }
    }

    // ──────────────────────────────────────────────
    // 에셋 접근 헬퍼
    // ──────────────────────────────────────────────

    /// <summary>
    /// 키(에셋 경로)로 SaveData 그룹의 에셋을 가져옵니다.
    /// </summary>
    public T GetSaveData<T>(string key) where T : ScriptableObject
    {
        return GetFromCache<T>(_saveDataAssets, key, "SaveData");
    }

    /// <summary>
    /// 키(에셋 경로)로 SaveData_Origin 그룹의 에셋을 가져옵니다.
    /// </summary>
    public T GetSaveDataOrigin<T>(string key) where T : ScriptableObject
    {
        return GetFromCache<T>(_saveDataOriginAssets, key, "SaveData_Origin");
    }

    private T GetFromCache<T>(
        Dictionary<string, ScriptableObject> cache,
        string key,
        string logName) where T : ScriptableObject
    {
        if (!IsLoaded)
        {
            Debug.LogWarning($"[AddressableGroupLoader] 아직 로드가 완료되지 않았습니다.");
            return null;
        }

        if (cache.TryGetValue(key, out var asset) && asset is T typed)
            return typed;

        Debug.LogWarning($"[AddressableGroupLoader] [{logName}] 키를 찾을 수 없습니다: {key}");
        return null;
    }

    // ──────────────────────────────────────────────
    // 언로드
    // ──────────────────────────────────────────────

    public void UnloadAll()
    {
        foreach (var handle in _handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        _handles.Clear();
        _saveDataAssets.Clear();
        _saveDataOriginAssets.Clear();
        IsLoaded = false;
        Debug.Log("[AddressableGroupLoader] 모든 에셋 언로드 완료");
    }

    // ──────────────────────────────────────────────
    // 콜백 (필요에 따라 오버라이드/이벤트로 교체)
    // ──────────────────────────────────────────────

    /// <summary>두 그룹 로드가 모두 완료되면 호출됩니다.</summary>
    protected virtual void OnAllGroupsLoaded()
    {
        GetComponent<SaveDataOverwriter>().LoadFromFile();
    }
}