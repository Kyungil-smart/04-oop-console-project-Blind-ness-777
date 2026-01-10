using System;

public class TownScene : Scene
{
    private Tile[,] _field = new Tile[25, 60];
    private PlayerCharacter _player;
    private HotKeyBar _hotKeyBar;

    // 1) 글자 규칙:
    //    - '#': 벽(통과 불가)
    //    - '.': 바닥(이동 가능)
    //    - '+': 문(통과 가능 + Enter 상호작용, DoorTargetScene은 아래 주석 참고)
    //    - '?': 루팅 후보(= IsLootSpot = true)
    //    - 'S': 플레이어 시작 위치(스폰)
    //    - 'E': 정문/엔딩 포인트(연출상 바깥 외벽에 두는 것을 추천)
    //
    // ⚠️ 주의
    // - TownTemplate는 반드시 _field 크기(세로 25, 가로 60)와 같은 크기여야 한다.
    // - 이 파일에서는 "타이틀 메뉴의 조작법 화면" 및 "조작 안내 문구"는 건드리지 않는다.
    // =============================================================

    // true면 타운을 "직접 찍은 템플릿"으로 로드한다.
    // false면 기존 PlaceBuilding 자동 배치를 사용한다(권장하지 않음).
    private const bool USE_MANUAL_TOWN_TEMPLATE = true;

    // 직접 찍을 수 있는 타운 템플릿(25줄, 각 줄 60글자)
    // 현재는 '뼈대'만 제공한다. 너가 townmap.png를 보고 여기 내용을 직접 완성하면 된다.
    private static readonly string[] TownTemplate =
    {
        "############################################################",
        "#?..#.......#?......#.?.##########........................?#",
        "#...#...#...#...#...#...####C#####...##################....#",
        "#.......#...#...#...#...##########...########1#########....#",
        "#####...#...#...#.......####+#####...##################....#",
        "#.......#...#...#...#...#......?.....########+#########....#",
        "#...#####...#...#...#...#..................................#",
        "#*..#?..........#.?.#......................................#",
        "#########################...............###############....#",
        "#.......................................###############....#",
        "#..................################.....#######2#######....#",
        "#..................################.....###############....#",
        "#..########?.......#######H########?....#######+#######....#",
        "#..########........################........................#",
        "#..########........#######+########........................#",
        "#..########................................................#",
        "#..###B###+.............................#######+#######....#",
        "#..########.............................###############....#",
        "#..########....######+########..........###############....#",
        "#..########....###############..........#######3#######....#",
        "#..########....######4########..........###############....#",
        "#...?..........###############..........###############....#",
        "#..............###############.............................#",
        "#.................................S........................#",
        "##################################E#########################",
    };

    private TriggerService _triggerService;
    private LocationType _locationType = LocationType.Town;

    // ===== 추격(혼합 모드): 적 X, 흔적 : =====
    private System.Collections.Generic.List<Vector> _chaseEnemies = new System.Collections.Generic.List<Vector>();
    private System.Collections.Generic.Dictionary<Vector, int> _enemyTrails = new System.Collections.Generic.Dictionary<Vector, int>();
    private bool _climaxChaseActive;

    // 결계(엔딩 조건)
    private bool _barrierRemoved;
    private bool _barrierMessageShown;

    // 다른 씬(부서진 집 등) 다녀왔다가 돌아올 때 위치 보존
    private bool _hasReturnPosition;
    private Vector _returnPosition;
    private bool _spawnedOnce;

    // 템플릿에서 읽은 스폰/정문 위치(없으면 기본값)
    private int _spawnX = 1;
    private int _spawnY = 1;
    private int _exitX = 46;
    private int _exitY = 20;

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

		// ===== 타운 맵 초기 배치(레트로 ASCII) =====
		// === 템플릿을 직접 찍어서 타운을 고정 레이아웃으로 유지 ===
		if (USE_MANUAL_TOWN_TEMPLATE)
		{
			LoadTownFromTemplate(TownTemplate);
			// 문(+)은 템플릿에서 '표시'만 한다.
			// 실제로 어느 씬으로 들어가는지는 여기에서 좌표로 연결한다.
			ConfigureTownDoors_ManualTemplate();
		}
		else
		{
			// 바깥 테두리는 벽으로 막아서 맵 밖으로 못 나가게 한다.
			for (int y = 0; y < _field.GetLength(0); y++)
			{
				for (int x = 0; x < _field.GetLength(1); x++)
				{
					if (y == 0 || y == _field.GetLength(0) - 1 || x == 0 || x == _field.GetLength(1) - 1)
						_field[y, x].IsBlocked = true;
				}
			}

			// 테스트용: 바닥 아이템 1개
			_field[4, 6].ItemOnTile = new Drink(10, "수상한 탄산음료", "마시면 머리가 맑아지는 느낌이다.", 2);

			// ✅ '포켓몬 마을' 느낌: 건물은 덩어리(#)로 그리고, 문(+) 딱 1칸만 열린다.
			// 부서진 집(하이리스크)
			PlaceBuilding(leftX: 6, topY: 6, width: 9, height: 6, doorX: 10, doorY: 11, doorTargetScene: "BrokenHouse");

			// 일반 집들(내부 구현)
			PlaceBuilding(leftX: 22, topY: 5, width: 8, height: 5, doorX: 25, doorY: 9, doorTargetScene: "House1");
			PlaceBuilding(leftX: 34, topY: 5, width: 8, height: 5, doorX: 37, doorY: 9, doorTargetScene: "House2");
			PlaceBuilding(leftX: 22, topY: 12, width: 8, height: 5, doorX: 25, doorY: 16, doorTargetScene: "House3");
			PlaceBuilding(leftX: 34, topY: 12, width: 8, height: 5, doorX: 37, doorY: 16, doorTargetScene: "House4");

			// 성당(틀만)
			PlaceBuilding(leftX: 42, topY: 2, width: 12, height: 7, doorX: 48, doorY: 8, doorTargetScene: "Church");

			// 2층 집(촌장집/마을회관 느낌, 틀만)
			PlaceBuilding(leftX: 42, topY: 11, width: 12, height: 7, doorX: 48, doorY: 17, doorTargetScene: "TownHall");

			// ✅ 마을 정문(엔딩)
			_field[20, 46].SpecialSymbol = 'E';

			// ✅ 마을 루팅 포인트(임의 배치)
			_field[14, 18].IsLootSpot = true;
			_field[15, 27].IsLootSpot = true;
			_field[17, 40].IsLootSpot = true;
			_field[10, 50].IsLootSpot = true;
			_field[19, 10].IsLootSpot = true;
			_field[7, 15].IsLootSpot = true;
			_field[18, 33].IsLootSpot = true;
		}

		// ✅ B안: 성물 4개를 "시작 시점에" 루팅 포인트 4곳에만 배정한다(셔플).
		RelicPlacementSystem.InitializeIfNeeded(GetAllLootSpotPositions(), 4);
    }

	// =============================================================
	// 템플릿 로더
	// =============================================================
	private void LoadTownFromTemplate(string[] template)
	{
		int height = _field.GetLength(0);
		int width = _field.GetLength(1);

		// 1) 기본 바닥(.)으로 초기화
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				_field[y, x].IsBlocked = false;
				_field[y, x].IsLootSpot = false;
				_field[y, x].SpecialSymbol = '\0';
				_field[y, x].DoorTargetScene = null;
			}
		}

		// 2) 크기 검사(틀리면 최소한의 보호)
		if (template == null || template.Length != height)
		{
			// 템플릿이 잘못됐으면 기존 기본값(테두리 벽)만 적용
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
						_field[y, x].IsBlocked = true;
				}
			}
			_spawnX = 1;
			_spawnY = 1;
			return;
		}

		for (int y = 0; y < height; y++)
		{
			string line = template[y];
			if (line == null || line.Length != width)
				continue;

			for (int x = 0; x < width; x++)
			{
				char ch = line[x];

				if (ch == '#')
				{
					_field[y, x].IsBlocked = true;
				}
				else if (ch == '+')
				{
					_field[y, x].IsBlocked = false;
					_field[y, x].SpecialSymbol = '+';
				}
				else if (ch == '?')
				{
					_field[y, x].IsBlocked = false;
					_field[y, x].IsLootSpot = true;
				}
				else if (ch == 'S')
				{
					_field[y, x].IsBlocked = false;
					_spawnX = x;
					_spawnY = y;
				}
				else if (ch == 'E')
				{
					_field[y, x].IsBlocked = false;
					_exitX = x;
					_exitY = y;
					_field[y, x].SpecialSymbol = 'E';
				}
				else
				{
					// 그 외 문자는 바닥(.) 취급
					_field[y, x].IsBlocked = false;
				}
			}
		}
	}

	// =============================================================
	// [직접 찍는 타운 템플릿] 문(+) -> 씬 연결
	//
	// TownTemplate에서 +를 찍어둔 좌표에, 어떤 씬으로 들어갈지 지정하는 곳.
	// 너는 다음 턴에 "타운을 직접 찍고 최신 ZIP"을 줄 거라고 했으니,
	// 그 때 + 좌표가 확정되면 이 메서드의 (y,x) 좌표만 바꾸면 된다.
	//
	// ⚠️ 주의: _field는 [y,x] 순서다.
	//
	// 문 대상 씬 이름(문자열)은 기존 구조를 유지한다:
	// - "BrokenHouse" / "House1"~"House4" / "Church" / "TownHall"
	// =============================================================
	private void ConfigureTownDoors_ManualTemplate()
	{
		// 아래 좌표는 '예시'다. (현재는 기본값)
		// 너가 TownTemplate를 완성하고 난 뒤, 실제 + 좌표로 바꿔서 고정해라.
		//
		// 예) 부서진 집 문
		// SetDoorTarget(doorX: 10, doorY: 11, targetScene: "BrokenHouse");
		//
		// 예) 일반 집 4채
		// SetDoorTarget(doorX: 25, doorY: 9,  targetScene: "House1");
		// SetDoorTarget(doorX: 37, doorY: 9,  targetScene: "House2");
		// SetDoorTarget(doorX: 25, doorY: 16, targetScene: "House3");
		// SetDoorTarget(doorX: 37, doorY: 16, targetScene: "House4");
		//
		// 예) 성당 / 촌장집(마을회관)
		// SetDoorTarget(doorX: 48, doorY: 8,  targetScene: "Church");
		// SetDoorTarget(doorX: 48, doorY: 17, targetScene: "TownHall");
	}

	private void SetDoorTarget(int doorX, int doorY, string targetScene)
	{
		if (doorY < 0 || doorY >= _field.GetLength(0) || doorX < 0 || doorX >= _field.GetLength(1))
			return;

		// 템플릿에 +를 안 찍었더라도, 안전하게 문으로 만들어준다.
		_field[doorY, doorX].IsBlocked = false;
		_field[doorY, doorX].SpecialSymbol = '+';
		_field[doorY, doorX].DoorTargetScene = targetScene;
	}

	// 건물 외형을 타운 맵에 그린다.
	// - 건물은 전부 벽(#) 판정
	// - 문(+) 딱 1칸만 이동 가능 + Enter 상호작용
	private void PlaceBuilding(int leftX, int topY, int width, int height, int doorX, int doorY, string doorTargetScene)
	{
		for (int y = topY; y < topY + height; y++)
		{
			for (int x = leftX; x < leftX + width; x++)
			{
				if (y < 0 || y >= _field.GetLength(0) || x < 0 || x >= _field.GetLength(1))
					continue;

				if (x == doorX && y == doorY)
				{
					_field[y, x].IsBlocked = false;
					_field[y, x].SpecialSymbol = '+';
					_field[y, x].DoorTargetScene = doorTargetScene;
				}
				else
				{
					_field[y, x].IsBlocked = true;
				}
			}
		}
	}

    public override void Enter()
    {
        _player.Field = _field;

        // 처음 들어올 때만 템플릿의 'S' 위치에서 시작(없으면 기본 1,1)
        if (!_spawnedOnce)
        {
            _player.Position = new Vector(_spawnX, _spawnY);
            _spawnedOnce = true;
        }
        else if (_hasReturnPosition)
        {
            _player.Position = _returnPosition;
            _hasReturnPosition = false;
        }
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

        // 벽/건물 외벽 체크
        if (_field[newY, newX].IsBlocked) return;

        // 현재 타일 비우기
        _field[_player.Position.Y, _player.Position.X].OnTileObject = null;

        // 위치 이동
        _player.Position = new Vector(newX, newY);

        // 새 타일에 플레이어 배치
        _field[newY, newX].OnTileObject = _player;

        // 이동 1회 = 스텝 1회
        EventContext context = new EventContext(_player, _locationType);
        _triggerService.OnStep(context);

        // ✅ 성물 5개를 모으면(혹은 모은 상태면) 클라이맥스 추격을 시작/유지
        TryStartClimaxChaseIfReady();
        if (_climaxChaseActive)
        {
            UpdateChaseEnemies();
        }

        if (_player.IsDead())
            GameOver();
    }

    private void InteractAtPlayerPosition()
    {
        Vector pos = _player.Position;
        Tile tile = _field[pos.Y, pos.X];
		// ✅ 문(+) 상호작용
		if (tile.SpecialSymbol == '+')
		{
			// 문 타일이면: DoorTargetScene으로 이동(씬이 없으면 문구)
			if (!string.IsNullOrEmpty(tile.DoorTargetScene))
			{
				_returnPosition = _player.Position;
				_hasReturnPosition = true;

				Console.Clear();
				if (tile.DoorTargetScene == "BrokenHouse")
					Console.WriteLine("부서진 집 안으로 들어간다...");
				else if (tile.DoorTargetScene == "Church")
					Console.WriteLine("성당 안으로 들어간다...");
				else if (tile.DoorTargetScene == "TownHall")
					Console.WriteLine("촌장집(마을회관) 안으로 들어간다...");
				else
					Console.WriteLine("집 안으로 들어간다...");
				Console.WriteLine("[Enter] 계속");
				Console.ReadLine();

				SceneManager.Change(tile.DoorTargetScene);
				return;
			}

			Console.Clear();
			Console.WriteLine("문이 잠겨있다.");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			return;
		}

		// ✅ 마을 정문(엔딩)
		if (tile.SpecialSymbol == 'E')
		{
			Console.Clear();

			if (!_barrierRemoved)
			{
				Console.WriteLine("정문은 결계에 막혀있다.");
				Console.WriteLine("아직 나갈 수 없다...");
				Console.WriteLine("[Enter] 계속");
				Console.ReadLine();
				return;
			}

			Console.WriteLine("결계가 사라진 정문이 열려있다.");
			Console.WriteLine("너는 마을을 빠져나간다...");
			Console.WriteLine();
			Console.WriteLine("=== ENDING ===");
			Console.WriteLine("[Enter] 종료");
			Console.ReadLine();
			GameManager.RequestQuit();
			return;
		}

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

            // ✅ B안: 이 루팅 포인트가 성물 배정 지점이라면, 무조건 성물을 준다.
            if (RelicPlacementSystem.HasRelicAt(pos))
            {
                Item relic = new HolyRelicPiece(100, "파편 성물", "정화에 필요한 성물 조각이다.");

                bool addedRelic = _player.Inventory.TryAdd(relic);
                if (addedRelic)
                {
                    Console.WriteLine("무언가를 발견했다: " + relic);

                    RelicPlacementSystem.MarkRelicCollected(pos);

                    // 성물은 1회성(그 지점에서 더는 안 나옴)
                    tile.IsLootSpot = false;

                    // 루팅 시 이벤트 체크
                    _triggerService.OnLootAttempt(context);

                    Console.WriteLine();
                    Console.WriteLine("[Enter] 계속");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("가방이 가득 찼다. 성물을 챙기지 못했다.");
                Console.WriteLine();
                Console.WriteLine("[Enter] 계속");
                Console.ReadLine();
                return;
            }

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

	        // ✅ 루팅 시 이벤트 체크(실패여도 조사하면 이벤트가 뜰 수 있음)
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
        // 0번째 줄: 정신력 UI
        _player.DrawSanityGauge();

        // 1번째 줄부터: 맵 출력
        Console.SetCursorPosition(0, 1);
        PrintField();

        int hotkeyY = Console.WindowHeight - 2;
        _hotKeyBar.Render(0, hotkeyY);

        // (정신력 UI는 이미 위에서 출력함)
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


	private System.Collections.Generic.List<Vector> GetAllLootSpotPositions()
	{
		System.Collections.Generic.List<Vector> list = new System.Collections.Generic.List<Vector>();

		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)
			{
				if (_field[y, x].IsLootSpot)
					list.Add(_field[y, x].Position);
			}
		}

		return list;
	}

    private void PrintField()
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);

                // 1) 플레이어(타일 오브젝트)
                if (_field[y, x].HasGameObject)
                {
                    _field[y, x].OnTileObject.Symbol.Print();
                    continue;
                }

                // 2) 적(추격 중)
                if (_climaxChaseActive && IsEnemyAt(pos))
                {
                    'X'.Print();
                    continue;
                }

                // 3) 흔적(추격 중)
                if (_climaxChaseActive && _enemyTrails.ContainsKey(pos))
                {
                    ':'.Print();
                    continue;
                }

                // 4) 나머지 기본 타일
                _field[y, x].Print();
            }
            Console.WriteLine();
        }
    }

    // =====================
    // 추격(클라이맥스) 로직
    // =====================

    private void TryStartClimaxChaseIfReady()
    {
        if (_climaxChaseActive) return;

        // 성물(정화용) 5개를 모은 순간부터 추격 시작
        if (_player.Inventory.GetHolyRelicCount() >= 5)
        {
            if (!_barrierRemoved)
            {
                _barrierRemoved = true;

                Console.Clear();
                Console.WriteLine("성물 5개가 모두 모였다.");
                Console.WriteLine();
                Console.WriteLine("마을을 감싸던 결계가… 흔들린다.");
                Console.WriteLine("정문이 열릴지도 모른다.");
                Console.WriteLine();
                Console.WriteLine("[Enter] 계속");
                Console.ReadLine();
            }

            StartClimaxChase(enemyCount: 3);
        }
    }

    private void StartClimaxChase(int enemyCount)
    {
        _climaxChaseActive = true;
        _chaseEnemies.Clear();
        _enemyTrails.Clear();

        int width = _field.GetLength(1);
        int height = _field.GetLength(0);

        // 아주 단순한 스폰: 맵 가장자리 3곳(플레이어와 겹치면 다른 자리)
        System.Collections.Generic.List<Vector> candidates = new System.Collections.Generic.List<Vector>();
        candidates.Add(new Vector(width - 2, height - 2));
        candidates.Add(new Vector(width - 2, 1));
        candidates.Add(new Vector(1, height - 2));
        candidates.Add(new Vector(1, 1));

        for (int i = 0; i < candidates.Count && _chaseEnemies.Count < enemyCount; i++)
        {
            Vector c = candidates[i];
            if (c.X == _player.Position.X && c.Y == _player.Position.Y) continue;
            _chaseEnemies.Add(c);
        }

        // 안내 텍스트(한 번만)
        Console.Clear();
        Console.WriteLine("마지막 성물이 모였다...");
        Console.WriteLine("멀리서 발소리가 들려온다.");
        Console.WriteLine("(추격이 시작됐다!)");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }

    private void UpdateChaseEnemies()
    {
        // 흔적 감소(턴마다)
        DecayTrails();

        for (int i = 0; i < _chaseEnemies.Count; i++)
        {
            Vector from = _chaseEnemies[i];
            Vector to = MoveOneStepTowards(from, _player.Position);

            // 이동했다면 흔적 남김
            if (to.X != from.X || to.Y != from.Y)
            {
                AddTrail(from, life: 2);
            }

            _chaseEnemies[i] = to;

            // 충돌 체크
            if (to.X == _player.Position.X && to.Y == _player.Position.Y)
            {
                HandleEnemyHitPlayer(i, from);
                return;
            }
        }
    }

    private void HandleEnemyHitPlayer(int enemyIndex, Vector enemyPrevPos)
    {
        // 십자가가 있으면 1회 방어
        bool saved = _player.Inventory.TryConsumeCross();

        if (saved)
        {
            Console.Clear();
            Console.WriteLine("십자가가 부서지며, 죽음을 막아냈다!");
            Console.WriteLine("하지만 다리가 풀린다...");

            // 플레이어 정신력을 1로 유지(즉사 방어 느낌)
            if (_player.Sanity.Value <= 0)
                _player.Sanity.Value = 1;
            else if (_player.Sanity.Value > 1)
                _player.Sanity.Value = 1;

            // 적을 한 칸 뒤로(원래 자리로 되돌림) - 최소한의 구출 연출
            _chaseEnemies[enemyIndex] = enemyPrevPos;

            Console.WriteLine("[Enter] 계속");
            Console.ReadLine();
            return;
        }

        // 방어 수단 없음 → 게임 오버
        GameOver();
    }

    private void AddTrail(Vector pos, int life)
    {
        // 이미 있으면 더 오래 남게
        if (_enemyTrails.ContainsKey(pos))
        {
            if (_enemyTrails[pos] < life)
                _enemyTrails[pos] = life;
            return;
        }
        _enemyTrails.Add(pos, life);
    }

    private void DecayTrails()
    {
        if (_enemyTrails.Count == 0) return;

        System.Collections.Generic.List<Vector> keys = new System.Collections.Generic.List<Vector>(_enemyTrails.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            Vector k = keys[i];
            _enemyTrails[k] = _enemyTrails[k] - 1;
            if (_enemyTrails[k] <= 0)
                _enemyTrails.Remove(k);
        }
    }

    private bool IsEnemyAt(Vector pos)
    {
        for (int i = 0; i < _chaseEnemies.Count; i++)
        {
            Vector e = _chaseEnemies[i];
            if (e.X == pos.X && e.Y == pos.Y) return true;
        }
        return false;
    }

    private Vector MoveOneStepTowards(Vector from, Vector target)
    {
        int dx = target.X - from.X;
        int dy = target.Y - from.Y;

        int stepX = 0;
        int stepY = 0;

        // 더 멀리 떨어진 축으로 1칸 이동(아주 단순)
        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            if (dx > 0) stepX = 1;
            else if (dx < 0) stepX = -1;
        }
        else
        {
            if (dy > 0) stepY = 1;
            else if (dy < 0) stepY = -1;
        }

        Vector next = new Vector(from.X + stepX, from.Y + stepY);

        // 맵 밖이면 제자리
        if (next.X < 0 || next.X >= _field.GetLength(1)) return from;
        if (next.Y < 0 || next.Y >= _field.GetLength(0)) return from;

        return next;
    }
}