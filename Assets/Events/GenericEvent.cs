using System.Collections.Generic;

public class GenericEvent<T> where T : class, new()
{
    private Dictionary<string, T> map = new();

    public T Get(string channel = "")
    {
        map.TryAdd(channel, new T());
        return map[channel];
    }
}