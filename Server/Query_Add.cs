using System.Net;

class Query_Add : Query
{
	string entryName;
	int entryScore;
	public Entry result;

	override protected string QueryInfo
	{
		get
		{
			return "Entry Added\n" + result;
		}
	}

	public override bool Verbal
	{
		get { return true; }
	}

	public Query_Add(Scoreboard board, string name, int score)
		: base(board)
	{
		entryName = name;
		entryScore = score;
	}

	protected override void Execute()
	{
		Entry e = new Entry(entryName, entryScore);
		scoreboard.AddEntry(e);
		scoreboard.SaveToFile();

		result = e;

		base.Execute();
	}

	public override string ToString()
	{
		return base.ToString();
	}
}