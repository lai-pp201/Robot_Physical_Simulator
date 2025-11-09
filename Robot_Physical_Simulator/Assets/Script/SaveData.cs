using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void Save(object save, string fileName)
    {
        // 將資料序列化為 JSON 格式
        string savingString = JsonUtility.ToJson(save);

        // 根據設備判斷儲存路徑
        string directoryPath;

#if UNITY_ANDROID && !UNITY_EDITOR
    // Android 設備：存放在 Download/Butterfly 資料夾
    directoryPath = System.IO.Path.Combine(Application.persistentDataPath, "Butterfly");
#else
        // 電腦設備：存放在 StreamingAssets/GameData 資料夾
        directoryPath = Application.dataPath + "/StreamingAssets/GameData/";
#endif

        // 檢查資料夾是否存在，不存在則創建
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // 設定完整檔案路徑
        string filePath = System.IO.Path.Combine(directoryPath, fileName);

        // 將資料寫入到指定路徑的 JSON 檔案中
        System.IO.File.WriteAllText(filePath, savingString);

        // Debug 確認
        Debug.Log("儲存完成，路徑：" + filePath);
    }
}
