using System;
using System.Collections.Generic;

public static class SceneManager
{
    public static Action OnChangeScene;

    public static Scene Current { get; private set; }
    private static Scene _prev;

    private static Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

    // 다음 씬 입장 시 스폰 기준 심볼(예: '+', 'S', 'U', 'D')
    public static char NextSpawnSymbol = '\0';

    public static void SetNextSpawnSymbol(char symbol)
    {
        NextSpawnSymbol = symbol;
    }

    public static void AddScene(string key, Scene scene)
    {
        if (_scenes.ContainsKey(key)) return;
        if (scene == null) return;

        _scenes.Add(key, scene);
    }

    public static void ChangePrevScene()
    {
        if (_prev == null) return;
        Change(_prev);
    }

    public static void Change(string key)
    {
        if (!_scenes.ContainsKey(key)) return;
        Change(_scenes[key]);
    }

    public static void Change(Scene next)
    {
        if (next == null) return;
        if (Current == next) return;

        Scene old = Current;
        _prev = old;

        if (old != null) old.Exit();

        Current = next;
        Current.Enter();

        if (OnChangeScene != null)
            OnChangeScene.Invoke();
    }

    public static void Update()
    {
        if (Current != null)
            Current.Update();
    }

    public static void Render()
    {
        Console.Clear();

        if (Current != null)
            Current.Render();
    }
}