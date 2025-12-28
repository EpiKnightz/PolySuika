using System.IO;
using System.Linq;
using UnityEngine;
using Utilities;
//using JsonUtility;

public class SaveManager : MonoBehaviour
{
    public bool BeautifyJson = true;
    

    private void Start()
    {
        //UpdateLeaderboardFromDisk();
    }


    public void SaveLeaderboard(Leaderboard leaderboard)
    {
        string json = JsonUtility.ToJson(leaderboard, BeautifyJson);
        string path = Path.Join(Application.persistentDataPath, "Leaderboard.ini");
        StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    public Leaderboard GetLeaderboard()
    {
        string path = Path.Join(Application.persistentDataPath, "Leaderboard.ini");
        try
        {
            StreamReader streamReader = new StreamReader(path);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonUtility.FromJson<Leaderboard>(json);
        }
        catch (FileNotFoundException e)
        {
            print(e);
            return null;
        }
    }
}
