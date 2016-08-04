using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

enum QueryType
{
	Add = 0,
	Get = 1,
	Backup = 2
};

class Query
{
	static object lockObject = new object();
	static List<Query> queryList = new List<Query>();

	static int queriesHandled = 0;
	public static int QueriesHandled { get { return queriesHandled; } }
	public static int QueryCount { get { return queryList.Count; } }

	public static void QueryWorker()
	{
		Console.WriteLine("-- QUERY WORKER STARTED --");

		while (true)
		{
			lock (lockObject)
			{
				while (queryList.Count > 0)
				{
					queryList[0].Execute();
					queryList.RemoveAt(0);

					queriesHandled++;
				}
			}

			Thread.Sleep(2);
		}
	}

	public static void AddQuery(Query q)
	{
		lock (lockObject)
		{
			queryList.Add(q);
		}
	}
	public static void AddQueryBlocking(Query q)
	{
		AddQuery(q);
		q.Wait();
	}

	public static Query[] Parse(MemoryStream stream)
	{
		List<Query> queryList = new List<Query>();
		BinaryReader reader = new BinaryReader(stream);

		while (reader.PeekChar() != -1)
		{
			try
			{
				QueryType type = (QueryType)reader.ReadByte();

				switch (type)
				{
					case QueryType.Add:
						int nameLen = reader.ReadInt32();
						byte[] name = reader.ReadBytes(nameLen);
						int score = reader.ReadInt32();

						queryList.Add(new Query_Add(Scoreboard.current, Encoding.ASCII.GetString(name, 0, nameLen), score));
						break;

					case QueryType.Get:
						queryList.Add(new Query_Get(Scoreboard.current, reader.ReadInt32(), reader.ReadByte()));
						break;

					default:
						queryList.Add(new Query(Scoreboard.current));
						break;
				}
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("!!!!!!!!!!!");
				Console.WriteLine("Query error");
				Console.WriteLine(e);
				Console.WriteLine("!!!!!!!!!!!");
			}
		}

		return queryList.ToArray();
	}

	bool finished = false;
	protected Scoreboard scoreboard;

	virtual protected string QueryInfo
	{
		get
		{
			return "Base Entry";
		}
	}

	virtual public bool Verbal
	{
		get { return false; }
	}

	public Query(Scoreboard board)
	{
		scoreboard = board;
	}

	public void Wait()
	{
		while (!finished) { Thread.Sleep(1); }
	}

	protected virtual void Execute()
	{
		if (Verbal)
			Console.WriteLine(this);

		finished = true;
	}

	public override string ToString()
	{
		return "///////////\n" + QueryInfo + "\n///////////\n";
	}
}
