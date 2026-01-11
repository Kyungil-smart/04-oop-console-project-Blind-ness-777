using System;
using System.Threading;

public class GameManager
{
    public static bool IsGameOver { get; private set; }

    public static void RequestQuit()
    {
        IsGameOver = true;
    }
    
    public void Run()
    {
        Init();

        while (!IsGameOver)
        {
            InputManager.Poll();
            SceneManager.Update();
            SceneManager.Render();

            Thread.Sleep(33);
        }
    }

    private void Init()
    {
        IsGameOver = false;
        Console.CursorVisible = false;

        // 콘솔 사이즈는 안전하게 (순서: Window -> Buffer)
        try
        {
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);
        }
        catch { }

        SceneManager.OnChangeScene += () => Console.Clear();

        PlayerCharacter player = new PlayerCharacter();

        SceneManager.AddScene("Title", new TitleScene());
        SceneManager.AddScene("Story", new StoryScene());
        SceneManager.AddScene("Town", new TownScene(player));
		SceneManager.AddScene("BrokenHouse", new BrokenHouseScene(player));
		
		// 실내(집/성당/회관) 씬들
		SceneManager.AddScene("House1", new House1Scene(player));
		SceneManager.AddScene("House2", new House2Scene(player));
		SceneManager.AddScene("House3", new House3Scene(player));
		SceneManager.AddScene("House4", new House4Scene(player));
		SceneManager.AddScene("Church", new ChurchScene(player));
		SceneManager.AddScene("ChurchBasement", new ChurchBasementScene(player));
		SceneManager.AddScene("TownHall", new TownHallScene(player));
		SceneManager.AddScene("TownHall2F", new TownHall2FScene(player));

        // 첫 씬 시작
        SceneManager.Change("Title");
    }

    private void TrySetConsoleSize(int targetWidth, int targetHeight)
    {
        try
        {
            int width = Math.Min(targetWidth, Console.LargestWindowWidth);
            int height = Math.Min(targetHeight, Console.LargestWindowHeight);

            if (width < 1) width = 1;
            if (height < 1) height = 1;

            // 창 크기 먼저
            Console.SetWindowSize(width, height);

            // 버퍼는 창보다 작으면 안 됨
            Console.SetBufferSize(width, height);
        }
        catch
        {
            // 실패해도 게임은 실행되게 그냥 무시
        }
    }
}

