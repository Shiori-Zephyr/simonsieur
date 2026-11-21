using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace SimonSieur;

public class ConfigWindow : Window
{
    private readonly SimonSieurPlugin _plugin;

    public ConfigWindow(SimonSieurPlugin plugin)
        : base("Simon Sieur", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse)
    {
        _plugin = plugin;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(250, 0),
            MaximumSize = new Vector2(400, 200),
        };
    }

    public override void Draw()
    {
        var forcedYes = _plugin.Configuration.ForcedYes;
        if (ImGui.Checkbox("Forced Yes", ref forcedYes))
        {
            _plugin.Configuration.ForcedYes = forcedYes;
            _plugin.SaveConfiguration();
        }

        ImGui.TextDisabled("When enabled, every Yes/No dialog is auto-confirmed.");
    }
}
