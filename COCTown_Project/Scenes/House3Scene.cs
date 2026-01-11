public class House3Scene : IndoorSceneBase
{
	public House3Scene(PlayerCharacter player)
		: base(player, LocationType.House, "평범한 민가(1층)")
	{
		BuildFromStrings(new string[]
		{
			"#################",
			"#...............#",
			"#..R...#####....#",
			"#......#.?.#....#",
			"#......#........#",
			"#......#...#....#",
			"#..###########..#",
			"#...............#",
			"#.....?.........#",
			"#...............#",
			"########+########"
		});
	}
}
