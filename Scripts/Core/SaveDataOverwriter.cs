using System.IO;
using UnityEngine;

/// <summary>
/// SaveData 그룹의 모든 에셋 필드값을
/// SaveData_Origin 그룹의 동일 키 에셋 필드값으로 덮어쓰거나,
/// JSON 파일로 저장/복원합니다.
///
/// 같은 GameObject에 AddressableGroupLoader가 있어야 합니다.
///
/// 저장 경로: Application.persistentDataPath/SaveData/
/// 각 에셋 키의 '/' → '_' 치환 후 .json 확장자로 저장됩니다.
///   예) datas/NPCs/NPC_Archer  →  SaveData/datas_NPCs_NPC_Archer.json
/// </summary>
[RequireComponent(typeof(AddressableGroupLoader))]
public class SaveDataOverwriter : MonoBehaviour
{
    static SaveDataOverwriter instance;
    public static SaveDataOverwriter Instance { get => instance; }
    // 저장 폴더명 (persistentDataPath 하위)
    private const string SAVE_FOLDER = "SaveData";

    private AddressableGroupLoader _loader;
    private string _saveDirectory;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        _loader = GetComponent<AddressableGroupLoader>();
        _saveDirectory = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
    }

    // ──────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// SaveData의 모든 에셋을 SaveData_Origin의 동일 키 에셋 값으로 덮어씁니다.
    /// (메모리에만 반영 — 영구 저장하려면 SaveToFile()을 추가로 호출하세요)
    /// </summary>
    public void OverwriteSaveDataWithOrigin()
    {
        if (!_loader.IsLoaded)
        {
            Debug.LogWarning("[SaveDataOverwriter] 아직 로드가 완료되지 않았습니다. " +
                             "AddressableGroupLoader.LoadAllGroupsAsync() 완료 후 호출하세요.");
            return;
        }

        var saveDataAssets = _loader.SaveDataAssets;
        var originAssets = _loader.SaveDataOriginAssets;

        int successCount = 0, skipCount = 0, failCount = 0;

        foreach (var kvp in originAssets)
        {
            string key = kvp.Key;
            ScriptableObject origin = kvp.Value;

            if (!saveDataAssets.TryGetValue(key, out ScriptableObject target))
            {
                Debug.LogWarning($"[SaveDataOverwriter] SaveData에 키 없음, 스킵: {key}");
                skipCount++;
                continue;
            }

            if (origin.GetType() != target.GetType())
            {
                Debug.LogWarning($"[SaveDataOverwriter] 타입 불일치 ({origin.GetType().Name} ≠ " +
                                 $"{target.GetType().Name}), 스킵: {key}");
                skipCount++;
                continue;
            }

            if (CopyValues(origin, target)) successCount++;
            else failCount++;
        }

        Debug.Log($"[SaveDataOverwriter] OverwriteSaveDataWithOrigin 완료 — " +
                  $"성공: {successCount}, 스킵: {skipCount}, 실패: {failCount}");
    }

    /// <summary>
    /// Serialize() 등 전처리가 끝난 특정 SO 하나만 JSON 파일로 저장합니다.
    /// 빌드 런타임에서도 동작합니다.
    ///
    /// 사용 예:
    ///   protected virtual void OnDestroy()
    ///   {
    ///       npcSo.Serialize();
    ///       saveDataOverwriter.SaveToFile(npcSo);
    ///   }
    /// </summary>
    public void SaveToFile(ScriptableObject so)
    {
        if (!_loader.IsLoaded)
        {
            Debug.LogWarning("[SaveDataOverwriter] 아직 로드가 완료되지 않았습니다.");
            return;
        }

        // SO 이름으로 키 역조회 (인스펙터 참조와 Addressables 로드 참조가 다른 인스턴스일 수 있으므로)
        string key = null;
        foreach (var kvp in _loader.SaveDataAssets)
        {
            if (kvp.Value.name == so.name) { key = kvp.Key; break; }
        }

        if (key == null)
        {
            Debug.LogError($"[SaveDataOverwriter] SaveData 그룹에 없는 SO입니다: {so.name}");
            return;
        }

        EnsureDirectoryExists(_saveDirectory);

        try
        {
            File.WriteAllText(KeyToFilePath(key), JsonUtility.ToJson(so, prettyPrint: true));
            Debug.Log($"[SaveDataOverwriter] 개별 저장 완료: {key}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveDataOverwriter] 개별 저장 실패 ({key}): {ex.Message}");
        }
    }

    /// <summary>
    /// 현재 메모리의 SaveData 에셋 전체를 JSON 파일로 저장합니다.
    /// 빌드 런타임에서도 동작합니다 (Application.persistentDataPath 사용).
    /// </summary>
    public void SaveToFile()
    {
        if (!_loader.IsLoaded)
        {
            Debug.LogWarning("[SaveDataOverwriter] 아직 로드가 완료되지 않았습니다.");
            return;
        }

        EnsureDirectoryExists(_saveDirectory);

        int successCount = 0, failCount = 0;

        foreach (var kvp in _loader.SaveDataAssets)
        {
            string key = kvp.Key;
            string path = KeyToFilePath(key);

            try
            {
                string json = JsonUtility.ToJson(kvp.Value, prettyPrint: true);
                File.WriteAllText(path, json);
                successCount++;
                Debug.Log($"[SaveDataOverwriter] 저장 완료: {path}");
            }
            catch (System.Exception ex)
            {
                failCount++;
                Debug.LogError($"[SaveDataOverwriter] 저장 실패 ({key}): {ex.Message}");
            }
        }

        Debug.Log($"[SaveDataOverwriter] SaveToFile 완료 — " +
                  $"성공: {successCount}, 실패: {failCount}  경로: {_saveDirectory}");
    }

    /// <summary>
    /// JSON 파일에서 SaveData 에셋으로 값을 복원합니다.
    /// 파일이 없는 키는 그대로 유지됩니다 (Origin 값 = 기본값).
    /// </summary>
    public void LoadFromFile()
    {
        if (!_loader.IsLoaded)
        {
            Debug.LogWarning("[SaveDataOverwriter] 아직 로드가 완료되지 않았습니다.");
            return;
        }

        if (!Directory.Exists(_saveDirectory))
        {
            Debug.Log($"[SaveDataOverwriter] 저장 폴더 없음 — 기본값(Origin) 유지: {_saveDirectory}");
            return;
        }

        int successCount = 0, missingCount = 0, failCount = 0;

        foreach (var kvp in _loader.SaveDataAssets)
        {
            string key = kvp.Key;
            string path = KeyToFilePath(key);

            if (!File.Exists(path))
            {
                missingCount++;
                Debug.Log($"[SaveDataOverwriter] 파일 없음, 기본값 유지: {key}");
                continue;
            }

            try
            {
                string json = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, kvp.Value);
                successCount++;
                Debug.Log($"[SaveDataOverwriter] 복원 완료: {key}");
            }
            catch (System.Exception ex)
            {
                failCount++;
                Debug.LogError($"[SaveDataOverwriter] 복원 실패 ({key}): {ex.Message}");
            }
        }

        Debug.Log($"[SaveDataOverwriter] LoadFromFile 완료 — " +
                  $"성공: {successCount}, 파일없음: {missingCount}, 실패: {failCount}");
    }

    /// <summary>
    /// Origin으로 초기화한 뒤 파일까지 저장합니다 (편의 메서드).
    /// 씬 버튼이 참조하는 중...
    /// </summary>
    public void ResetAndSave()
    {
        OverwriteSaveDataWithOrigin();
        SaveToFile();
    }

    // ──────────────────────────────────────────────────────────
    // 내부 유틸
    // ──────────────────────────────────────────────────────────

    private static bool CopyValues(ScriptableObject source, ScriptableObject target)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), target);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveDataOverwriter] CopyValues 예외: {ex.Message}");
            return false;
        }
    }

    /// <summary>에셋 키를 파일 경로로 변환합니다. '/' → '_', .json 추가</summary>
    private string KeyToFilePath(string key)
    {
        string fileName = key.Replace("/", "_").Replace("\\", "_") + ".json";
        return Path.Combine(_saveDirectory, fileName);
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}