using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace SimonSieur;

public sealed class SimonSieurPlugin : IDalamudPlugin
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly ICommandManager _commandManager;
    private readonly IAddonLifecycle _addonLifecycle;
    private readonly IPluginLog _pluginLog;
    private readonly WindowSystem _windowSystem = new(nameof(SimonSieurPlugin));
    private readonly ConfigWindow _configWindow;
    internal readonly Configuration Configuration;

    public SimonSieurPlugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IAddonLifecycle addonLifecycle,
        IPluginLog pluginLog)
    {
        _pluginInterface = pluginInterface;
        _commandManager = commandManager;
        _addonLifecycle = addonLifecycle;
        _pluginLog = pluginLog;

        Configuration = (Configuration?)_pluginInterface.GetPluginConfig() ?? new Configuration();

        _configWindow = new ConfigWindow(this);
        _windowSystem.AddWindow(_configWindow);

        _pluginInterface.UiBuilder.Draw += _windowSystem.Draw;
        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;
        _pluginInterface.UiBuilder.OpenMainUi += ToggleConfig;

        _commandManager.AddHandler("/simonsieur", new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle Simon Sieur config window",
        });

        _addonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", OnSelectYesnoSetup);
    }

    internal void SaveConfiguration()
    {
        _pluginInterface.SavePluginConfig(Configuration);
    }

    private void OnCommand(string command, string arguments)
    {
        switch (arguments)
        {
            case "on":
                Configuration.ForcedYes = true;
                SaveConfiguration();
                break;
            case "off":
                Configuration.ForcedYes = false;
                SaveConfiguration();
                break;
            case "toggle":
                Configuration.ForcedYes = !Configuration.ForcedYes;
                SaveConfiguration();
                break;
            default:
                ToggleConfig();
                break;
        }
    }

    private void ToggleConfig()
    {
        _configWindow.IsOpen = !_configWindow.IsOpen;
    }

    private unsafe void OnSelectYesnoSetup(AddonEvent eventType, AddonArgs addonInfo)
    {
        if (!Configuration.ForcedYes)
            return;

        var atk = (AtkUnitBase*)addonInfo.Addon;
        if (atk == null)
            return;

        var addon = (AddonSelectYesno*)atk;
        _pluginLog.Information("Forced yes active: auto-clicking Yes");
        addon->YesButton->ClickAddonButton(atk);
    }

    public void Dispose()
    {
        _addonLifecycle.UnregisterListener(OnSelectYesnoSetup);
        _pluginInterface.UiBuilder.Draw -= _windowSystem.Draw;
        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfig;
        _pluginInterface.UiBuilder.OpenMainUi -= ToggleConfig;
        _commandManager.RemoveHandler("/simonsieur");
    }
}
