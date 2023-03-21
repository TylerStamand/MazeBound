using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System;

public class SaveManager {

    static SaveManager instance;

    /// <summary>
    /// Called before file write
    /// </summary>
    public event Action OnSave;

    /// <summary>
    /// Called after file read
    /// </summary>
    public event Action OnLoad;

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
        OnSave?.Invoke();
        File.WriteAllText(UnityEngine.Application.dataPath + "/save.json", JsonConvert.SerializeObject(saveData, Formatting.Indented));
        Debug.Log("Saved to " + UnityEngine.Application.dataPath + "/save.json");
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
        if (File.Exists(UnityEngine.Application.dataPath + "/save.json")) {
            saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(UnityEngine.Application.dataPath + "/save.json"));
            return true;
        }
        return false;
    }

    public void Clear() {
        saveData.Clear();
    }

    public void Delete() {
        if (File.Exists(UnityEngine.Application.dataPath + "/save.json")) {
            File.Delete(UnityEngine.Application.dataPath + "/save.json");
        }
    }


}
