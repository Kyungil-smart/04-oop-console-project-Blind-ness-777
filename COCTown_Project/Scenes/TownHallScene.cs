using System;

// 2층 집(촌장집/마을회관) - 일단 틀만
public class TownHallScene : IndoorSceneBase
{
	public TownHallScene(PlayerCharacter player)
		: base(player, LocationType.House, "촌장집 / 마을회관")
	{
		// 'U' : 2층으로 올라가는 계단
		BuildFromStrings(new string[]
		{
			"########################",
			"#......U....#..........#",
			"#..?........#..?.......#",
			"#...........#..........#",
			"#....#####..#######....#",
			"#....#...#..#?....#....#",
			"#.......?#..#.....#....#",
			"#....#...#..#..........#",
			"#....#####..#######....#",
			"#......................#",
			"###########+############"
		});
	}

	protected override void OnSpecialInteract(char symbol)
	{
		if (symbol == 'U')
		{
			Console.Clear();
			Console.WriteLine("2층으로 올라간다...");
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            SceneManager.SetNextSpawnSymbol('D');
            SceneManager.Change("TownHall2F");

            return;
		}

		base.OnSpecialInteract(symbol);
	}
}
