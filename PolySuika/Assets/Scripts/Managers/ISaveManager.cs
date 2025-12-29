public interface ISaveManager
{
    T Load<T>();
    void Save<T>(T data);
}