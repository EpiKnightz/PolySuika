using TMPro;
using UnityEngine;
using Utilities;

public class UIEntry : MonoBehaviour
{
    public TextMeshProUGUI RankField;
    public TextMeshProUGUI NameField;
    public TextMeshProUGUI ScoreField;

    public void UpdateEntry(Entry entry, int Rank)
    {
        if (NameField != null
            && ScoreField != null)
        {
            if (Rank == 0)
            {
                NameField.SetText(TextUtilities.RainbowString(entry.NickName));
            }
            else if ( Rank == 1)
            {
                NameField.SetText(TextUtilities.RankToColoredText(5, entry.NickName));
            }
            else if (Rank == 2)
            {
                NameField.SetText(TextUtilities.RankToColoredText(4, entry.NickName));
            }else
            {
                NameField.SetText(entry.NickName);
            }
            ScoreField.SetText(entry.Score.ToString()); // To formatted string later
            RankField.SetText((Rank + 1).ToString());
        }
    }
}
