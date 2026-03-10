using StardewModdingAPI;

namespace JojaAutoTasks.Configuration;

/// <summary>Reads configuration through the SMAPI <see cref="IModHelper" />.</summary>
internal sealed class SmapiConfigReader : IConfigReader
{
    // -- Dependencies -- //
    private readonly IModHelper _helper;

    // -- Constructor -- //
    internal SmapiConfigReader(IModHelper helper)
    {
        _helper = helper;
    }

    // -- Public API -- //
    public T ReadConfig<T>() where T : class, new()
    {
        return _helper.ReadConfig<T>();
    }
}
