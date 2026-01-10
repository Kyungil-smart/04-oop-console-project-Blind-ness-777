using System;

public class TitleScene : Scene
{
    private MenuList _titleMenu;
    private bool showMenu = false;

    private const int BannerX = 6;
    private const int BannerY = 2;
    private const int BannerWidth = 68;
    private const int BannerHeight = 18;

    private int _titleCenterX;
    private int _titleBottomY;

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
        showMenu = false;
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
        if (!showMenu)
        {
            if (InputManager.GetKey(ConsoleKey.Enter))
                showMenu = true;

            return;
        }

        if (InputManager.GetKey(GameKeys.Up))
            _titleMenu.SelectUp();

        if (InputManager.GetKey(GameKeys.Down))
            _titleMenu.SelectDown();

        if (InputManager.GetKey(GameKeys.Confirm))
            _titleMenu.Select();
    }

    public override void Render()
    {
        Console.Clear();
        DrawTitleBanner();

        if(!showMenu)
        {
            string msg = "계속하려면 [Enter]...";
            int msgX = BannerX + (BannerWidth - msg.Length) / 2;
            int msgY = BannerY + BannerHeight - 2;   // 배너 안쪽 아래

            Console.SetCursorPosition(msgX, msgY);
            Console.Write(msg);
        }

        if (showMenu)
        {
            int menuBoxWidth = 30;
            int menuBoxHeight = 8;

            int menuX = _titleCenterX - (menuBoxWidth / 2);
            int menuY = _titleBottomY + 3;

            int minX = BannerX + 2;
            int maxX = BannerX + BannerWidth - menuBoxWidth - 2;
            if (menuX < minX) menuX = minX;
            if (menuX > maxX) menuX = maxX;

            int minY = BannerY + 2;
            int maxY = BannerY + BannerHeight - menuBoxHeight - 2;
            if (menuY < minY) menuY = minY;
            if (menuY > maxY) menuY = maxY;

            DrawBox(menuX, menuY, menuBoxWidth, menuBoxHeight);
            _titleMenu.Render(menuX + 2, menuY + 1);
        }

    }

    private void DrawTitleBanner()
    {
        DrawBox(BannerX, BannerY, BannerWidth, BannerHeight);

        string title = "C O C   T O W N";
        int titleX = BannerX + (BannerWidth - title.Length) / 2;
        int titleY = BannerY + 2;

        Console.SetCursorPosition(titleX, titleY);
        Console.Write(title);

        _titleCenterX = titleX + (title.Length / 2);
        _titleBottomY = titleY + 3;

        string tagline = "YOU SHOULD NOT HAVE COME HERE";
        int tagX = BannerX + (BannerWidth - tagline.Length) / 2;
        int tagY = BannerY + 4;

        Console.SetCursorPosition(tagX, tagY);
        Console.Write(tagline);
    }

    private void WaitForEnter()
    {
        string msg = "계속하려면 [Enter]...";
        int msgX = BannerX + (BannerWidth - msg.Length) / 2;
        int msgY = BannerY + BannerHeight + 1;

        Console.SetCursorPosition(msgX, msgY);
        Console.Write(msg);

        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
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

        string[] lines =
        {
            "┌──────────────────────────────┐",
            "│          조 작 법            │",
            "│                              │",
            "│   ↑ ↓ ← → : 이동             │",
            "│   Enter  : 상호작용 / 선택   │",
            "│   B      : 소지품(인벤)      │",
            "│   1~6    : 아이템 사용       │",
            "│            넘버패드 가능     │",
            "│                              │",
            "│                              │",
            "│   [ Enter ] 돌아가기         │",
            "└──────────────────────────────┘"
        };

        int boxWidth = lines[0].Length;
        int boxHeight = lines.Length;

        int startX = (Console.WindowWidth - boxWidth) / 2;
        int startY = (Console.WindowHeight - boxHeight) / 2;

        for (int i = 0; i < lines.Length; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            Console.Write(lines[i]);
        }

        // Enter 대기
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
    }

    public void GameQuit()
    {
        GameManager.RequestQuit();
    }
}