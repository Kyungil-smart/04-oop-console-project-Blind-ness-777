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
        if (HandleInventoryKey())
            return;

        HandleMoveKeys();
        HandleInteractKey();
    }

    private bool HandleInventoryKey()
    {
        if (InputManager.GetKey(ConsoleKey.B))
        {
            _player.Inventory.ShowInventoryScreen();
            Console.Clear();
            return true;
        }
        return false;
    }

    private void HandleMoveKeys()
    {
        if (InputManager.GetKey(ConsoleKey.UpArrow))
        {
            TryMove(0, -1);
            return;
        }

        if (InputManager.GetKey(ConsoleKey.DownArrow))
        {
            TryMove(0, 1);
            return;
        }

        if (InputManager.GetKey(ConsoleKey.LeftArrow))
        {
            TryMove(-1, 0);
            return;
        }

        if (InputManager.GetKey(ConsoleKey.RightArrow))
        {
            TryMove(1, 0);
            return;
        }
    }

    private void HandleInteractKey()
    {
        if (!InputManager.GetKey(ConsoleKey.Enter))
            return;

        InteractAtPlayerPosition();
    }

    private void TryMove(int deltaX, int deltaY)
    {
    }

    private void InteractAtPlayerPosition()
    {
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