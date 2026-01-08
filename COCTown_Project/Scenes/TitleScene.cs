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
        _titleMenu.Add("조작법", ViewConrtols);
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
        Console.SetCursorPosition(5, 1);
        GameManager.GameName.Print(ConsoleColor.Yellow);

        _titleMenu.Render(8, 5);
    }

    public override void Exit()
    {

    }

    

    public void GameStart()
    {
        SceneManager.Change("Story");
    }

    public void ViewCredits()
    {
    }
}