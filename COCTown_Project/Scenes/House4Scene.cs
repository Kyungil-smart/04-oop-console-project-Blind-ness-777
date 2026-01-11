using System;

public class House4Scene : IndoorSceneBase
{
	private bool _safeOpened;

	public House4Scene(PlayerCharacter player)
		: base(player, LocationType.House, "버려진 민가(1층)")
	{
		BuildFromStrings(new string[]
		{
			"###############",
			"#...#.?...#...#",
			"#.........#...#",
			"#...#..#####..#",
			"#...#..#2.....#",
			"#...#..#####..#",
			"#...#....?#...#",
			"#...#######...#",
			"#......G......#",
			"#............?#",
			"#######+#######"
		});
	}

	protected override void OnSpecialInteract(char symbol)
	{
		if (symbol == 'G')
		{
			Console.Clear();

			// 금고 상호작용 대사는 아래 문구를 자유롭게 수정 가능.
			// - 연출을 더 넣고 싶으면 줄을 추가하면 된다.
			if (_safeOpened)
			{
				Console.WriteLine("금고는 이미 열려 있다.");
				Console.WriteLine("[Enter] 계속");
				Console.ReadLine();
				return;
			}

			// 금고 열쇠 보유 확인
			if (!_player.Inventory.HasKeyNameContains("금고"))
			{
				Console.WriteLine("금고가 잠겨 있다.");
				Console.WriteLine("열쇠가 필요하다.");
				Console.WriteLine("[Enter] 계속");
				Console.ReadLine();
				return;
			}

			_safeOpened = true;

			Item relic = new HolyRelicPiece(100, "파편 성물", "정화에 필요한 성물 조각이다.");
			bool added = _player.Inventory.TryAdd(relic);
			if (added)
			{
				Console.WriteLine("금고를 열었다.");
				Console.WriteLine("안쪽에서 무언가를 발견했다: " + relic);
				// 금고는 1회성: 열린 뒤에는 기호를 지워도 되지만, 맵을 수정하지 않기 위해
				// 여기서는 상태만으로 처리한다.
			}
			else
			{
				Console.WriteLine("가방이 가득 찼다. 금고 속 물건을 챙기지 못했다.");
				_safeOpened = false;
			}

			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			return;
		}

		base.OnSpecialInteract(symbol);
	}
}
