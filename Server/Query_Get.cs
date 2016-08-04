using System;
using System.Collections.Generic;
using System.Net;

class Query_Get : Query
{
	int offset, count;
	public List<Entry> result;

	override protected string QueryInfo
	{
		get
		{
			if (result.Count > 0)
				return "Sent entries " + offset + " - " + (offset + result.Count - 1);
			else
				return "Send no entries";
		}
	}

	public Query_Get(Scoreboard board, int offset, int count)
		: base(board)
	{
		if (offset < 0)
			offset = 0;

		this.offset = offset;
		this.count = count;
	}

	protected override void Execute()
	{
		result = new List<Entry>(count);
		foreach (Entry e in scoreboard.GetEntries(offset, count))
			result.Add(e);

		base.Execute();
	}

	public override string ToString()
	{
		return base.ToString();
	}
}