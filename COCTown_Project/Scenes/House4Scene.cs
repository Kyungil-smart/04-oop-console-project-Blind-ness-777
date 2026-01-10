public class House4Scene : IndoorSceneBase
{
	public House4Scene(PlayerCharacter player)
		: base(player, LocationType.House, "버려진 민가(1층)")
	{
		BuildFromStrings(new string[]
		{
			"###############",
			"#...#.?...#...#",
			"#.........#...#",
			"#...#..#####..#",
			"#...#..#?.....#",
			"#...#..#####..#",
			"#...#....?#...#",
			"#...#######...#",
			"#.............#",
			"#............?#",
			"#######+#######"
		});
	}
}
