using Reflex.Core;
using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour, ISaveManager, IInstaller, IPref
{
    [SerializeField] private bool BeautifyJson = true;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ISaveManager));
        builder.AddSingleton(this, typeof(IPref));
    }

    public bool Save<T>(T data, string suffix = "")
    {
        try
        {
            string json = JsonUtility.ToJson(data, BeautifyJson);
            string path = Path.Join(Application.persistentDataPath, $"{typeof(T).Name}" + suffix + ".ini");
            var streamWriter = new StreamWriter(path);
            streamWriter.Write(json);
            streamWriter.Close();
            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);
#endif
            return false;
        }
    }

    public bool TryLoad<T>(out T result, string suffix = "")
    {
        string path = Path.Join(Application.persistentDataPath, $"{typeof(T).Name}" + suffix + ".ini");
        try
        {
            var streamReader = new StreamReader(path);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            result = JsonUtility.FromJson<T>(json);
            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);
#endif
            result = default;
            return false;
        }
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

    }
}
