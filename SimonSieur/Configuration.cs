using Dalamud.Configuration;

namespace SimonSieur;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool ForcedYes { get; set; } = false;
}
