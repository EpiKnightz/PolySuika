public interface IPref
{
    int GetInt(string key);
    bool HasKey(string key);
    void SaveInt(string key, int value);
}