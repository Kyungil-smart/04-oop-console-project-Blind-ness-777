using System;

public class TownScene : Scene
{
    private Tile[,] _field = new Tile[10, 20];
    private PlayerCharacter _player;
    private HotKeyBar _hotKeyBar;

    private TriggerService _triggerService;
    private LocationType _locationType = LocationType.Town;

    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        _player.Inventory.Owner = _player;
        _hotKeyBar = new HotKeyBar(_player.Inventory);

        _triggerService = new TriggerService();

        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
            }
        }

        // 테스트용: 바닥 아이템 + 루팅 포인트
        _field[3, 5].ItemOnTile = new Drink(10, "수상한 탄산음료", "마시면 머리가 맑아지는 느낌이다.", 2);
        _field[2, 3].IsLootSpot = true;
        _field[2, 4].IsLootSpot = true;
        _field[5, 10].IsLootSpot = true;


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

        // ✅ 인벤토리를 열지 않아도 1~6(또는 넘패드)로 아이템 사용 가능
        if (HandleHotKeyUse())
            return;

        HandleMoveKeys();
        HandleInteractKey();
    }

    private bool HandleHotKeyUse()
    {
        int slotIndex = -1;

        // 숫자키 1~6
        if (InputManager.GetKey(ConsoleKey.D1)) slotIndex = 0;
        else if (InputManager.GetKey(ConsoleKey.D2)) slotIndex = 1;
        else if (InputManager.GetKey(ConsoleKey.D3)) slotIndex = 2;
        else if (InputManager.GetKey(ConsoleKey.D4)) slotIndex = 3;
        else if (InputManager.GetKey(ConsoleKey.D5)) slotIndex = 4;
        else if (InputManager.GetKey(ConsoleKey.D6)) slotIndex = 5;

        // 넘패드 1~6
        else if (InputManager.GetKey(ConsoleKey.NumPad1)) slotIndex = 0;
        else if (InputManager.GetKey(ConsoleKey.NumPad2)) slotIndex = 1;
        else if (InputManager.GetKey(ConsoleKey.NumPad3)) slotIndex = 2;
        else if (InputManager.GetKey(ConsoleKey.NumPad4)) slotIndex = 3;
        else if (InputManager.GetKey(ConsoleKey.NumPad5)) slotIndex = 4;
        else if (InputManager.GetKey(ConsoleKey.NumPad6)) slotIndex = 5;

        if (slotIndex == -1)
            return false;

        Item item = _player.Inventory.GetItem(slotIndex);
        if (item == null)
        {
            Console.Clear();
            Console.WriteLine("그 슬롯은 비어있다.");
            Console.WriteLine("[Enter] 계속");
            Console.ReadLine();
            return true;
        }

        // 성수/십자가는 '자동 소모' 컨셉이라, 실수로 낭비하지 않게 단축키 사용을 막는다.
        if (item is HolyWater || item is Cross)
        {
            Console.Clear();
            Console.WriteLine(item + " 는(은) 위기 상황에서 자동으로 소모되는 보호 아이템이다.");
            Console.WriteLine("(단축키로 직접 사용할 수 없다)");
            Console.WriteLine("[Enter] 계속");
            Console.ReadLine();
            return true;
        }

        bool used = _player.Inventory.UseAt(slotIndex);

        Console.Clear();
        if (used)
            Console.WriteLine("아이템을 사용했다: " + item);
        else
            Console.WriteLine("지금은 사용할 수 없다.");

        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
        return true;
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

        // 이동 1회 = 스텝 1회
        EventContext context = new EventContext(_player, _locationType);
        _triggerService.OnStep(context);

        if (_player.IsDead())
            GameOver();
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

        // 루팅 포인트(조사)
        if (tile.IsLootSpot)
        {
            EventContext context = new EventContext(_player, _locationType);

            Console.Clear();
            Console.WriteLine("주변을 조사한다...");
            Console.WriteLine();

            LootResult loot = LootSystem.RollLoot(context);
            if (!loot.FoundSomething)
            {
                Console.WriteLine("아무것도 찾지 못했다.");
            }
            else
            {
                bool added = _player.Inventory.TryAdd(loot.Item);
                if (added)
                {
                    Console.WriteLine("무언가를 얻었다: " + loot.Item);

                    // 일회성 루팅 포인트(원하면 나중에 재생성/쿨다운으로 확장)
                    tile.IsLootSpot = false;
                }
                else
                {
                    Console.WriteLine("가방이 가득 찼다. 아무것도 챙기지 못했다.");
                }
            }

            // 루팅 이벤트 체크(성물 수에 따라 확률 상승)
            _triggerService.OnLootAttempt(context);

            if (_player.IsDead())
                GameOver();

            Console.WriteLine("[Enter] 계속");
            Console.ReadLine();
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