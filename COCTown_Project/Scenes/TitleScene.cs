using System;

public class TitleScene : Scene
{
    private MenuList _titleMenu;

    public TitleScene()
    {
        Init();
    }

    public void Init()
    {
        _titleMenu = new MenuList();
        _titleMenu.Add("게임 시작", GameStart);
        _titleMenu.Add("조작법", ViewControls);
        _titleMenu.Add("크레딧", ViewCredits);
        _titleMenu.Add("게임 종료", GameQuit);
    }
    public override void Enter()
    {
        _titleMenu.Reset();
    }

    public static class GameKeys
    {
        public static readonly ConsoleKey Up = ConsoleKey.UpArrow;
        public static readonly ConsoleKey Down = ConsoleKey.DownArrow;
        public static readonly ConsoleKey Confirm = ConsoleKey.Enter;
    }

    public override void Update()
    {
        if (InputManager.GetKey(GameKeys.Up))
            _titleMenu.SelectUp();

        if (InputManager.GetKey(GameKeys.Down))
            _titleMenu.SelectDown();

        if (InputManager.GetKey(GameKeys.Confirm))
            _titleMenu.Select();
    }

    public override void Render()
    {
        // 1) 배경(눈) - 나중에
        // DrawEyeGlow();

        // 2) 타이틀
        Console.SetCursorPosition(10, 5);
        Console.Write("COC TOWN");

        // 3) 메뉴 박스(외곽) - 네 맘대로 커스텀할 자리
        DrawBox(8, 7, 30, 8);

        // 4) 메뉴 텍스트(세로 메뉴)
        _titleMenu.Render(10, 8);
    }

    public override void Exit() { }

    private void DrawBox(int x, int y, int width, int height)
    {
        Console.SetCursorPosition(x, y);
        Console.Write("+" + new string('-', width - 2) + "+");

        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write("|" + new string(' ', width - 2) + "|");
        }

        Console.SetCursorPosition(x, y + height - 1);
        Console.Write("+" + new string('-', width - 2) + "+");
    }

    public void GameStart()
    {
        SceneManager.Change("Story");
    }

    public void ViewCredits()
    {
    }

    public void ViewControls()
    {
        Console.Clear();
        Console.WriteLine("=== 조작법 ===");
        Console.WriteLine("방향키: 이동");
        Console.WriteLine("Enter: 선택/다음");
        Console.WriteLine("I: 인벤토리");
        Console.WriteLine();
        Console.WriteLine("[Enter] 돌아가기");

        // Enter가 눌릴 때까지 대기
        while (true)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Enter)
                break;
        }
    }

    public void GameQuit()
    {
        GameManager.RequestQuit();
    }
}