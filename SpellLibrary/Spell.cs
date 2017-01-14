using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TextNbt;

namespace SpellLibrary
{
    /// <summary>
    /// Encapsulates an exported spell and provides functions to load spells in bulk.
    /// </summary>
    public class Spell
	{
        private const string Extension = ".txt";
        private const string NoteExtension = ".note.txt";
        /// <summary>
        /// The exported spell
        /// </summary>
        public string Source;
        /// <summary>
        /// Any notes attached to the spell
        /// </summary>
        public string Note;
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
        /// Construct a Spell with source code and a note
        /// </summary>
        /// <param name="json"></param>
        /// <param name="notes"></param>
        public Spell(string json, string note) : this(json)
        {
            Note = note;
        }

        /// <summary>
        /// Construct a spell from raw NBT
        /// </summary>
        /// <param name="nbt"></param>
        public Spell(NbtTag nbt) : this(TextNbtEmitter.Serialize(nbt)) { }

		/// <summary>
        /// Load a spell from a file
        /// </summary>
        /// <param name="path">The file</param>
        /// <returns>The loaded spell</returns>
        public static Spell LoadFile (string path)
		{
			Spell spell = new Spell (File.ReadAllText (path));
            if (File.Exists(path + ".note.txt"))
            {
                spell.Note = File.ReadAllText(path + ".note.txt");
            }
            return spell;
		}

		/// <summary>
        /// Load all spells from a folder
        /// </summary>
        /// <param name="path">Where to load from</param>
        /// <returns>The spells</returns>
        public static Spell[] LoadFromDir (string path)
		{
			String[] files = Directory.GetFiles (path, "*" + Extension);
			List<Spell> spells = new List<Spell>();
			foreach (String fn in files) {
				if (!fn.EndsWith(NoteExtension))
                {
                    spells.Add(LoadFile(fn));
                }
			}
			return spells.ToArray();
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
				return ntfsIllegalChars.Replace (Title, "_") + " " + Source.GetHashCode().ToString("x") + Extension; //TODO: there's probably something in the .NET stdlib that does this. find it.
			}
		}

		/// <summary>
        /// Save the spell in the specified directory
        /// </summary>
        /// <param name="dir">Where to save it</param>
        public void SaveIn (string dir)
		{
			File.WriteAllText (Path.Combine (dir, FileName), Source);
            if (Note != "")
            {
                File.WriteAllText(Path.Combine(dir, FileName + NoteExtension), Note);
            } else
            {
                File.Delete(Path.Combine(dir, FileName + NoteExtension));
            }
		}

	}
}

