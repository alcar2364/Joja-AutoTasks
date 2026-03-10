namespace JojaAutoTasks.Configuration;

/// <summary>Reads typed configuration from the mod's data source.</summary>
internal interface IConfigReader
{
    /// <summary>Reads the persisted configuration, returning a new instance of <typeparamref name="T" />.</summary>
    T ReadConfig<T>() where T : class, new();
}
