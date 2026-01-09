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

        // 필요하면 시작 아이템 테스트로 하나 넣기(선택)
        // player.AddItem(new EnergyDrink(...));  // 너 Item 구조 완성 후에
        SceneManager.AddScene("Title", new TitleScene());
        SceneManager.AddScene("Story", new StoryScene());
        SceneManager.AddScene("Town", new TownScene(player));

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

