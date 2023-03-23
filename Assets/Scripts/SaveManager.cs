using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System;

public class SaveManager {

    static readonly string savePath = UnityEngine.Application.dataPath + "/save.json";
    static SaveManager instance;


    Dictionary<string, object> saveData = new Dictionary<string, object>();

    public static SaveManager Instance {
        get {
            if (instance == null) {
                instance = new SaveManager();
            }
            return instance;
        }
    }

    SaveManager() { }


    public void Save() {
        File.WriteAllText(savePath, JsonConvert.SerializeObject(saveData, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
        Debug.Log("Saved to " + savePath);
    }

    public void SetData(string key, object value) {
        saveData[key] = value;
    }


    /// <summary>
    /// Retrieves save data using key identifier
    /// Returns default value if not found
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetData<T>(string key) {
        T value = default(T);
        if (saveData.ContainsKey(key)) {
            if (saveData[key] is T) {
                value = (T)saveData[key];
            }
        }
        return value;
    }

    public bool Load() {
        if (File.Exists(savePath)) {
            saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(savePath), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            foreach (var item in saveData) {
                Debug.Log(item.Key + " : " + item.Value);
            }
            return true;
        }
        return false;
    }

    public void Clear() {
        saveData.Clear();
    }

    public void Delete() {
        if (File.Exists(savePath)) {
            File.Delete(savePath);
        }
    }


}
