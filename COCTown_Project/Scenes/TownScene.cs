using System;

public class TownScene : Scene
{
    private Tile[,] _field = new Tile[10, 20];
    private PlayerCharacter _player;
    private HotKeyBar _hotKeyBar;

    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        _hotKeyBar = new HotKeyBar(_player.Inventory);

        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
            }
        }
    }

    public override void Enter()
    {
        _player.Field = _field;
        _player.Position = new Vector(1, 1);
        _field[_player.Position.Y, _player.Position.X].OnTileObject = _player;
    }

    public override void Update()
    {
        _player.Update();

        if (InputManager.GetKey(ConsoleKey.Z))
        {
            _player.DecreaseSanity(1);

            if (_player.IsDead())
            {
                GameOver();
                // 지금 구조에 맞는 종료/전환 방식으로 바꾸기
                // 예: GameManager.ChangeScene(new TitleScene()); 또는 GameManager.Quit()
            }
        }
    }

    public override void Render()
    {
        Console.SetCursorPosition(0, 0);
        PrintField();

        int hotkeyY = Console.WindowHeight - 2;
        _hotKeyBar.Render(0, hotkeyY);

        _player.DrawSanityGauge();
    }

    public override void Exit()
    {
        _field[_player.Position.Y, _player.Position.X].OnTileObject = null;
        _player.Field = null;
    }

    private void GameOver()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("정신력이 0이 되었다. 사망!");
        Console.WriteLine("아무 키나 누르면 종료합니다...");
        Console.ReadKey(true);
        Environment.Exit(0);
    }

    private void PrintField()
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                _field[y, x].Print();
            }
            Console.WriteLine();
        }
    }
}