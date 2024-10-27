using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveSystem
{
    public static void Save<T>(T objectToSave, string fileName)
    {
        string json = JsonConvert.SerializeObject(objectToSave, Formatting.Indented);
        string filePath = GetFilePath(fileName);
        File.WriteAllText(filePath, json);
    }

    public static T Load<T>(string fileName)
    {
        string filePath = GetFilePath(fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            T loadedObject = JsonConvert.DeserializeObject<T>(json);
            return loadedObject;
        }
        return default;
    }

    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

}
