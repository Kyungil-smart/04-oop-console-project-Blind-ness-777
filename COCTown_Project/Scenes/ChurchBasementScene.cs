using System;

// 성당 지하
// - 내부 모양과 루팅 포인트(?) 배치는 자유롭게 수정 가능
// - 'S' : 1층(성당 내부)로 올라가는 문/계단
public class ChurchBasementScene : IndoorSceneBase
{
	public ChurchBasementScene(PlayerCharacter player)
		: base(player, LocationType.Church, "성당 지하")
	{
		BuildFromStrings(new string[]
		{
			"####################",
			"#..S...............#",
			"#..................#",
			"#.........F........#",
            "#..................#",
            "#..................#",
			"####################",
		});

		// 스폰 위치는 'S' 위치로 맞춘다.
		TrySetSpawnOnSymbol('S');
	}

	protected override void OnSpecialInteract(char symbol)
	{
        if (symbol == 'F')
        {
            Console.Clear();
            Console.WriteLine("성수대 앞에 섰다.");
            Console.WriteLine();
            if (_player.Inventory.GetHolyRelicCount() < 5)
            {
                Console.WriteLine("(아직은 부족하다... 성물 5개가 필요하다)");
            }
            else
            {
                Console.WriteLine("성물 5개가 모였다.\n여기서 무언가를 할 수 있을 것 같다...(미구현)");
            }
            Console.WriteLine();
            Console.WriteLine("[Enter] 계속");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            return;
        }

        if (symbol == 'S')
		{
			Console.Clear();
			Console.WriteLine("1층으로 올라간다...");
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            SceneManager.SetNextSpawnSymbol('S'); // 1층 맵의 S 위로 스폰
            SceneManager.Change("Church");

            return;

		}

        base.OnSpecialInteract(symbol);
	}

	private void TrySetSpawnOnSymbol(char symbol)
	{
		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)
			{
				if (_field[y, x].SpecialSymbol == symbol)
				{
					_spawn = new Vector(x, y);
					return;
				}
			}
		}
	}
}
