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
        _player.Inventory.Owner = _player;
        _hotKeyBar = new HotKeyBar(_player.Inventory);

        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
            }
        }

        _field[3, 5].ItemOnTile = new Drink(10, "수상한 탄산음료", "마시면 머리가 맑아지는 느낌이다.", 2);
        _field[2, 3].ItemOnTile = new HolyWater(20, "성수", "한 번, 광기의 손을 떼어낸다.");
        _field[2, 4].ItemOnTile = new Cross(21, "십자가", "한 번, 죽음을 되돌린다.");


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
        int newX = _player.Position.X + deltaX;
        int newY = _player.Position.Y + deltaY;

        // 맵 범위 체크
        if (newX < 0 || newX >= _field.GetLength(1)) return;
        if (newY < 0 || newY >= _field.GetLength(0)) return;

        // 현재 타일 비우기
        _field[_player.Position.Y, _player.Position.X].OnTileObject = null;

        // 위치 이동
        _player.Position = new Vector(newX, newY);

        // 새 타일에 플레이어 배치
        _field[newY, newX].OnTileObject = _player;
    }

    private void InteractAtPlayerPosition()
    {
        Vector pos = _player.Position;
        Tile tile = _field[pos.Y, pos.X];

        if (tile.ItemOnTile != null)
        {
            Item found = tile.ItemOnTile;

            bool added = _player.Inventory.TryAdd(found);
            if (added)
            {
                tile.ItemOnTile = null;

                Console.Clear();
                Console.WriteLine("아이템을 획득했다: " + found.ToString());
                Console.WriteLine("[Enter] 계속");
                Console.ReadLine();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("가방이 가득 찼다.");
                Console.WriteLine("[Enter] 계속");
                Console.ReadLine();
            }
            return;
        }

        Console.Clear();
        Console.WriteLine("아무것도 없다...");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
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