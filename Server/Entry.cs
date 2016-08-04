using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Entry : IComparable
{
	public int Placing
	{
		get { return Scoreboard.current.GetPlacing(this); }
	}

	public string name;
	public int score;

	public Entry(string name, int score)
	{
		name.Replace("\n", "");
		this.name = name;
		this.score = score;
	}

	public void WriteToStream(BinaryWriter writer)
	{
		byte[] nameBytes = Encoding.ASCII.GetBytes(name);
		writer.Write((int)nameBytes.Length);
		writer.Write(nameBytes);
		writer.Write(score);
		writer.Write(Placing);
	}

	public int CompareTo(object other)
	{
		if (other == null || !(other is Entry)) return 1;

		Entry entry = other as Entry;
		return entry.score - score;
	}

	public override int GetHashCode()
	{
		return score.GetHashCode() ^ name.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Entry)) return false;

		Entry asEntry = obj as Entry;
		return asEntry.name == name;
	}

	public override string ToString()
	{
		return Placing + ". " + name + " - " + score;
	}
}