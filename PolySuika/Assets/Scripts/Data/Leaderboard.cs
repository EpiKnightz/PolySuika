using System;
using System.Collections.Generic;

[Serializable]
public struct Entry : IComparable<Entry>
{
    public string NickName;
    public int Score;

    public Entry(string name, int score)
    {
        NickName = name; Score = score;
    }

    public static bool operator >(Entry left, Entry right)
    {
        return left.Score > right.Score;
    }

    public static bool operator <(Entry left, Entry right)
    {
        return left.Score < right.Score;
    }

    public static bool operator >=(Entry left, Entry right)
    {
        return left.Score >= right.Score;
    }

    public static bool operator <=(Entry left, Entry right)
    {
        return left.Score <= right.Score;
    }

    public int CompareTo(Entry other)
    {
        return Score.CompareTo(other.Score);
    }
}

[Serializable]
public class Leaderboard
{
    private const int MAX_ENTRIES_COUNT = 10;
    public List<Entry> entries = new();

    public void Add(Entry entry)
    {
        entries.Add(entry);
        Sort();
        if (entries.Count > MAX_ENTRIES_COUNT)
        {
            entries.RemoveAt(MAX_ENTRIES_COUNT - 1);
        }
    }

    public void Add(string name, int score)
    {
        Entry entry = new(name, score);
        Add(entry);
    }

    public void Sort()
    {
        // Reverse sort with b compare to a
        entries.Sort((a, b) => b.CompareTo(a));
    }

    public bool CompareLast(int score)
    {
        return entries.Count < MAX_ENTRIES_COUNT || score > entries[entries.Count - 1].Score;
    }

    public void Clear()
    {
        entries.Clear();
    }
}
