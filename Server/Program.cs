using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ServerConfig
{
	public static int port;
	public static string dbFilename;
	
	public static int backupInterval;
}

class Program
{
	static TcpListener listener;
	static int port;

	public static void Main(string[] args)
	{
		if (args.Length != 2 && args.Length != 3)
		{
			Console.WriteLine("Usage: " + "<port> <filename> [<backup interval>]");
			return;
		}

		ServerConfig.port = int.Parse(args[0]);
		ServerConfig.dbFilename = args[1];

		if (args.Length == 3)
		{
			ServerConfig.backupInterval = int.Parse(args[2]);

			new Thread(Query_Backup.BackupWorker).Start();
		}

		Scoreboard board = new Scoreboard(ServerConfig.dbFilename);

		port = int.Parse(args[0]);

		new Thread(Query.QueryWorker).Start();
		Thread accept = new Thread(AcceptThread);

		accept.Start();
		accept.Join();
	}

	static void AcceptThread()
	{
		try
		{
			listener = new TcpListener(IPAddress.Any, port);
			listener.Start();

			Console.WriteLine("---- SERVER STARTED ----");
			Console.WriteLine("---- Port " + port + " ----");

			while (true)
			{
				Socket s = listener.AcceptSocket();
				Session session = new Session(s);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("Failed to start server: " + e.Message);
		}
	}
}



class Session
{
	Socket socket;

	public Session(Socket socket)
	{
		this.socket = socket;
		new Thread(HandleSession).Start();
	}

	void HandleSession()
	{
		try
		{
			while (true)
			{
				byte[] buffer = new byte[1024];
				int received = socket.Receive(buffer);

				//Client disconnected
				if (received <= 0)
					break;

				MemoryStream outBuffer = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(outBuffer);

				Query[] queryList = Query.Parse(new MemoryStream(buffer, 0, received));

				//Handle queries while data available
				foreach (Query query in queryList)
				{
					//New entry
					if (query is Query_Add)
					{
						Query.AddQueryBlocking(query);

						//Write resulting placing
						writer.Write((query as Query_Add).result.Placing);
					}

					//Getting entries
					else if (query is Query_Get)
					{
						Query.AddQueryBlocking(query);

						//Get result
						Entry[] result = (query as Query_Get).result.ToArray();

						//Write nmbr of results
						writer.Write(result.Length);

						//Write each entry
						foreach (Entry e in result)
							e.WriteToStream(writer);
					}
					else
					{
						ConsoleColor prevColor = Console.ForegroundColor;

						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("INVALID QUERY REQUEST");
						Console.ForegroundColor = prevColor;
					}
				}

				//Send the result buffer
				socket.Send(outBuffer.ToArray());
			}
		}
		catch (Exception e)
		{
			//Console.WriteLine(e);
		}
	}
}
