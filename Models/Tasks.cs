using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO; //Este también es importante
using Avalonia.Platform.Storage; //Este es el importante 
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using System;
using System.Linq;
public class Tareas
{
    [JsonPropertyName("Name")]
    public string? name { get; set; }
    [JsonPropertyName("Priority")]
    public string priority { get; set; } = "";
    [JsonPropertyName("Container")]
    public string container { get; set; } = ""; //esto si lo quisieramos hacer bien hariamos un id en vez de una string, pero que no lo hagmao digog
    [JsonIgnore]
    public List<string> priorities { get; set; } = ["High", "Medium", "Low", "NateHiggers"];
}

public class Serializer
{
    public static string? currentpath;
    public static async Task<List<Tareas>> Deserialize()
    {
        var window = Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window == null)
            return [];

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a JSON file",
            AllowMultiple = false,
            FileTypeFilter =
            [
                //You can put whatever you want here:
                new FilePickerFileType("JSON file") { Patterns = new[] { "*.json"} }
            ]
        });


        if (files.Count == 0)
            return [];


        var file = files[0];
        string json = await File.ReadAllTextAsync(file.Path.LocalPath);
        currentpath = file.Path.LocalPath;
        return JsonSerializer.Deserialize<List<Tareas>>(json) ?? [];

    }

    public static async Task Serialize(List<Tareas> lista)
    {
        if (currentpath == null) Console.WriteLine("No file opened!");
        string json = JsonSerializer.Serialize(lista);
        await File.WriteAllTextAsync(currentpath!, json);
    }

    public static List<Tareas> KanYe(List<Tareas> tareas)
    {
        tareas = tareas.OrderBy(t => t.priorities.IndexOf(t.priority)).ToList();
        return tareas;
    }

    public static async Task<List<Tareas>> NewListAsync()
    {
        var window = Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window == null)
            return [];

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Create new list",
            SuggestedFileName = "List.json",
            FileTypeChoices = new List<FilePickerFileType>
            {
                new FilePickerFileType("JSON file") { Patterns = new[] { "*.json" } }
            }
        });
        string? filePath = file!.Path.LocalPath;
        File.WriteAllText(filePath, "");
        System.Console.WriteLine(filePath);
        currentpath = filePath;
        return new List<Tareas>
        {
            new Tareas { name = "New task", priority = "Medium", container = "No-named list" },
            new Tareas { name = "New task 2", priority = "Medium", container = "No-named list" },
            new Tareas { name = "Change name5", priority = "Medium", container = "No-named list" },
            new Tareas { name = "Change name6", priority = "Medium", container = "No-named list" }
        };
    }
}