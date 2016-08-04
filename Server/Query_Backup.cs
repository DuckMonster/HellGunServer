using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

class Query_Backup : Query
{
	public static void BackupWorker()
	{
		Console.WriteLine("-- BACKUP WORKER STARTED --");

		while (true)
		{
			Thread.Sleep(1000 * 60 * ServerConfig.backupInterval);
			Query.AddQuery(new Query_Backup(Scoreboard.current));
		}
	}

	string filename = "";

	override protected string QueryInfo
	{
		get
		{
			return "Backupped to: " + filename;
		}
	}

	public Query_Backup(Scoreboard board)
		: base(board)
	{
	}

	protected override void Execute()
	{
		filename = ServerConfig.dbFilename + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bckup";
		scoreboard.SaveToFile(filename);

		base.Execute();
	}
}