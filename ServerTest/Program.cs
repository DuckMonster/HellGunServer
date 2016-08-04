using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
	static IPEndPoint serverEndpoint;

	static void Main(string[] args)
	{
		if (args.Length != 3)
		{
			Console.WriteLine("usage: <ip> <port> <nmbr of tests>");
			return;
		}

		serverEndpoint = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));

		try
		{
			new TcpClient().Connect(serverEndpoint);
		} catch(Exception e)
		{
			Console.WriteLine("Couldn't connect to " + serverEndpoint.ToString());
			return;
		}

		int n = int.Parse(args[2]);

		Thread[] clients = new Thread[n];
		for (int i = 0; i < clients.Length; i++)
		{
			clients[i] = new Thread(FooClient);
			clients[i].Start();
		}

		for (int i = 0; i < clients.Length; i++)
			clients[i].Join();
	}

	static Random rng = new Random();

	static void FooClient()
	{
		TcpClient client = new TcpClient();
		client.Connect(serverEndpoint);

		MemoryStream stream = new MemoryStream();
		BinaryWriter writer = new BinaryWriter(stream);

		//Test adding
		writer.Write((byte)0);

		byte[] name = new byte[10];
		for (int i = 0; i < name.Length; i++)
			name[i] = (byte)rng.Next('A', 'Z' + 1);

		writer.Write(name.Length);
		writer.Write(name);
		writer.Write(rng.Next(0, 50));

		byte[] data = stream.ToArray();

		client.Client.Send(stream.ToArray());
		stream.SetLength(0);

		//Test getting
		byte[] buffer = new byte[1024 * 1024];
		client.Client.Receive(buffer);

		int myPosition = BitConverter.ToInt32(buffer, 0);

		writer.Write((byte)1);
		writer.Write((int)0);
		writer.Write((byte)50);
		writer.Write((byte)1);
		writer.Write((int)myPosition - 25);
		writer.Write((byte)50);

		data = stream.ToArray();

		client.Client.Send(stream.ToArray());
		stream.SetLength(0);

		client.Client.Receive(buffer);

		Thread.Sleep(100);

		client.Close();
	}
}