using System;
using System.Collections.Generic;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance;
    public DevConsoleUI ui;

    Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        commands.Add("heal", Heal);
        commands.Add("give_gold", GiveGold);
    }

    public void Execute(string input)
    {
        var args = input.Split(' ');
        var command = args[0];
        var parameters = args.Length > 1 ? args[1..] : Array.Empty<string>();

        if (commands.TryGetValue(command, out var action))
        {
            action(parameters);
        }
        else
        {
            Log($"Unknown command: {command}");
        }
    }

    private void Log(string msg)
    {
        Debug.Log($"[CONSOLE] {msg}");
        ui.Log($"[CONSOLE] {msg}");
    }

    private void Heal(string[] args)
    {
        int amount = int.Parse(args[0]);
        // TODO: Actually heal the player

        Log($"Healed player for {amount}");
    }

    private void GiveGold(string[] args)
    {
        int amount = int.Parse(args[0]);
        InventoryManager.Instance.AddGold(amount);

        Log($"Added {amount} gold");
    }
}
