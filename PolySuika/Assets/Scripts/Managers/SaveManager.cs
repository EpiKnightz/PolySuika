using System.IO;
using UnityEngine;
using Reflex.Core;

public class SaveManager : MonoBehaviour, ISaveManager, IInstaller
{
    [SerializeField] private bool BeautifyJson = true;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ISaveManager));
    }

    public void Save<T>(T data)
    {
        string json = JsonUtility.ToJson(data, BeautifyJson);
        string path = Path.Join(Application.persistentDataPath, $"{typeof(T).Name}.ini");
        var streamWriter = new StreamWriter(path);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    public T Load<T>()
    {
        string path = Path.Join(Application.persistentDataPath, $"{typeof(T).Name}.ini");
        try
        {
            var streamReader = new StreamReader(path);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonUtility.FromJson<T>(json);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e);
            return default;
        }
    }
}
