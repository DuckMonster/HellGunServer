using System.Collections.Generic;
using System.IO;

class Scoreboard
{
	public static Scoreboard current = null;
	List<Entry> entryList = new List<Entry>();

	string filename;

	public int EntryCount { get { return entryList.Count; } }

	public Scoreboard(string filename)
	{
		current = this;
		this.filename = filename;

		LoadFromFile();
	}

	public Entry GetEntry(int placing)
	{
		if (placing < 0 || placing >= entryList.Count)
			return null;

		return entryList[placing];
	}
	public Entry GetEntry(string name)
	{
		return entryList.Find((e) => e.name == name);
	}
	public int GetPlacing(Entry entry)
	{
		return entryList.IndexOf(entry);
	}

	public IEnumerable<Entry> GetEntries()
	{
		foreach (Entry e in entryList)
			yield return e;
	}

	public IEnumerable<Entry> GetEntries(int index, int count)
	{
		for (int i = index; i < index + count; i++)
		{
			if (i < 0 || i >= entryList.Count)
				break;

			yield return entryList[i];
		}
	}

	public void AddEntry(Entry e)
	{
		int currentIndex = entryList.IndexOf(e);

		if (currentIndex == -1)
		{
			InsertEntry(e);
		}
		else if (e.score > entryList[currentIndex].score)
		{
			entryList.RemoveAt(currentIndex);
			InsertEntry(e);
		}

		//Sort();
	}

	void InsertEntry(Entry e)
	{
		for (int i = 0; i < entryList.Count; i++)
		{
			if (entryList[i].score < e.score)
			{
				entryList.Insert(i, e);
				return;
			}
		}

		entryList.Add(e);
	}

	public void Sort()
	{
		entryList.Sort();
	}

	public void SaveToFile() { SaveToFile(filename); }
	public void SaveToFile(string filename)
	{
		StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create));

		foreach (Entry e in GetEntries())
		{
			writer.WriteLine(e.name);
			writer.WriteLine(e.score);
		}

		writer.Flush();
		writer.Close();
	}

	public void LoadFromFile() { LoadFromFile(filename); }
	public void LoadFromFile(string filename)
	{
		entryList.Clear();
		StreamReader reader = new StreamReader(new FileStream(filename, FileMode.OpenOrCreate));

		int index = 0;

		while (!reader.EndOfStream)
		{
			string name = reader.ReadLine();
			int score = int.Parse(reader.ReadLine());
			AddEntry(new Entry(name, score));

			index++;
		}

		reader.Close();
	}

	public override string ToString()
	{
		string s = "";
		foreach (Entry e in entryList)
			s += e;

		return s;
	}
}