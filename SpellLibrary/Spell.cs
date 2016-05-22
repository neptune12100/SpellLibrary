using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SpellLibrary
{
    /*
	 * Encapsulates an exported spell and provides functions to load spells in bulk.
	 */
    public class Spell
	{
        private const string Extension = ".txt";
        public string Source;
		public String Title;
		private static Regex ntfsIllegalChars = new Regex (@"[\\/:*?""<>|]");

		public Spell (string json)
		{
			Source = json;

			try {
				Title = TextNbt.TextNbtParser.Parse (json) ["spellName"].StringValue;
			} catch (Exception) {
				Title = "";
			}
			if (Title == "")
				Title = "INVALID SPELL " + json.GetHashCode ();
		}

		public static Spell LoadFile (string path)
		{
			return new Spell (File.ReadAllText (path));
		}

		public static object[] LoadFromDir (string path)
		{
			String[] files = Directory.GetFiles (path, "*" + Extension);
			Spell[] spells = new Spell[files.Length];
			for (int i = 0; i < files.Length; i++) {
				spells [i] = LoadFile (files [i]);
			}
			return spells;
		}

		public override string ToString ()
		{
			return Title;
		}

		public string FileName {
			get {
				return ntfsIllegalChars.Replace (Title, "_") + Extension; //TODO: there's probably something in the .NET stdlib that does this. find it.
			}
		}

		public void SaveIn (string dir)
		{
			File.WriteAllText (Path.Combine (dir, FileName), Source);
		}

	}
}

