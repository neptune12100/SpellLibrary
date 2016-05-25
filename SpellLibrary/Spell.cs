using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SpellLibrary
{

    /// <summary>
    /// Encapsulates an exported spell and provides functions to load spells in bulk.
    /// </summary>
    public class Spell
	{
        private const string Extension = ".txt";
        /// <summary>
        /// The exported spell
        /// </summary>
        public string Source;
		/// <summary>
        /// The Spell's name
        /// </summary>
        public String Title;

		private static Regex ntfsIllegalChars = new Regex (@"[\\/:*?""<>|]");

		/// <summary>
        /// Construct a Spell form the exported source
        /// </summary>
        /// <param name="json">Source, exported from Psi</param>
        public Spell (string json)
		{
			Source = json;

			try {
				Title = TextNbt.TextNbtParser.Parse (json) ["spellName"].StringValue;
			} catch (Exception) {
				Title = "";
			}
			if (Title == "")
				Title = "INVALID SPELL " + json.GetHashCode ().ToString("x");
		}

		/// <summary>
        /// Load a spell from a file
        /// </summary>
        /// <param name="path">The file</param>
        /// <returns>The loaded spell</returns>
        public static Spell LoadFile (string path)
		{
			return new Spell (File.ReadAllText (path));
		}

		/// <summary>
        /// Load all spells from a folder
        /// </summary>
        /// <param name="path">Where to load from</param>
        /// <returns>The spells</returns>
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

		/// <summary>
        /// The filename the spell should be saved as
        /// </summary>
        public string FileName {
			get {
				return ntfsIllegalChars.Replace (Title, "_") + Extension; //TODO: there's probably something in the .NET stdlib that does this. find it.
			}
		}

		/// <summary>
        /// Save the spell in the specified directory
        /// </summary>
        /// <param name="dir">Where to save it</param>
        public void SaveIn (string dir)
		{
			File.WriteAllText (Path.Combine (dir, FileName), Source);
		}

	}
}

