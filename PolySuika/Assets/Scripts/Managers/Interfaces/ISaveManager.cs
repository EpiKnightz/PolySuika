public interface ISaveManager
{
    bool TryLoad<T>(out T result, string suffix = "");
    bool Save<T>(T data, string suffix = "");
}