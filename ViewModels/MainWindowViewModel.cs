using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace Importy.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<Tareas> listaTareas = [] ;

    [ObservableProperty]
    public string? currentlist = "";

    [ObservableProperty]
    public string? text;

    [ObservableProperty]
    public string status = "No file opened yet, please open a JSON file or create a new list";

    [ObservableProperty]
    public string? container;

    [ObservableProperty]
    public bool saveEnabled = false;

    [ObservableProperty]
    public bool addEnabled = false;

    [ObservableProperty]
    public bool nameEnabled = false;

    [RelayCommand]
    public async Task OpenList()
    {
        Serializer.currentpath = null;
        var temp = await Task.Run(Serializer.Deserialize);
        ListaTareas = new ObservableCollection<Tareas>(temp);
        if (ListaTareas != null && Serializer.currentpath != null)
        {
            Status = $"File opened succesfully, tasks in the list: {ListaTareas.Count}";
            NameEnabled = true;
            SaveEnabled = true;
            AddEnabled = true;
        }
        List<Tareas> tempi = new(ListaTareas!);
        tempi = Serializer.KanYe(tempi);
        ListaTareas = new(tempi);
        if (ListaTareas[0].container == string.Empty || ListaTareas[0].container == null)
        {
            ListaTareas[0].container = "No-named list";
        }
        Container = ListaTareas[0].container;
    }

    [RelayCommand]
    public async Task SaveList()
    {
        ListaTareas[0].container = Container ??= "No-named list";
        List<Tareas> tempi = new(ListaTareas!);
        tempi = Serializer.KanYe(tempi);
        ListaTareas = new(tempi);
        List<Tareas> temp = new(ListaTareas); //Esto es una referencia a la otra lista
        await Serializer.Serialize(temp);
        Status = $"File saved succesfully, tasks in the list: {ListaTareas.Count}";
    }

    [RelayCommand]
    public async Task RemoveTask(Tareas tarea)
    {
        ListaTareas.Remove(tarea);
        await SaveList();
    }

    [RelayCommand]
    public void AddTask()
    {
        ListaTareas.Add(new Tareas { name = "Change name", priority = "Medium", container = Container??= "No-named list" });
    }

    [RelayCommand]
    public async Task NewList()
    {
        ListaTareas.Clear();
        ListaTareas = new ObservableCollection<Tareas>(await Serializer.NewListAsync());
        if (ListaTareas.Count == 0 || ListaTareas == null)
        {
            return;
        }
        Container = "No-named list";
        Status = $"New list created with {ListaTareas.Count} tasks, don't forget to save your changes!";
        NameEnabled = true;
        SaveEnabled = true;
        AddEnabled = true;

    }
}