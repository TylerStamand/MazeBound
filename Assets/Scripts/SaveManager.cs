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
    /// Called after file write
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

    public T GetData<T>(string key) {
        T value = default(T);
        if (saveData.ContainsKey(key)) {
            if (saveData[key] is T) {
                value = (T)saveData[key];
            }
        }
        return value;
    }

    public void Load() {
        if (File.Exists(UnityEngine.Application.dataPath + "/save.json")) {
            saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(UnityEngine.Application.dataPath + "/save.json"));
        }
        OnLoad?.Invoke();
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
