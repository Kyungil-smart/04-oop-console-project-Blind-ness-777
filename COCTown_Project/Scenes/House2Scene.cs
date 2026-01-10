public class House2Scene : IndoorSceneBase
{
	public House2Scene(PlayerCharacter player)
		: base(player, LocationType.House, "작은 민가(1층)")
	{
		BuildFromStrings(new string[]
		{
			"###############",
			"#.....#......##",
			"#..?..#..?...##",
			"#.....#......##",
			"#.....#####...#",
			"#.............#",
			"###..######...#",
			"#.........#...#",
			"#...#.....#...#",
			"#...#.....#...#",
			"#######+#######"
		});
	}
}
