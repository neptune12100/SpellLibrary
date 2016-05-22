//
//  Spell.cs
//
//  Author:
//       Levi Arnold <arnojla@gmail.com>
//
//  Copyright (c) 2016 Levi Arnold
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace SpellLibrary
{
	/*
	 * Encapsulates an exported spell and provides functions to load spells in bulk.
	 */
	public class Spell
	{
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
			String[] files = Directory.GetFiles (path, "*.json");
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
				return ntfsIllegalChars.Replace (Title, "_") + ".json"; //TODO: there's probably something in the .NET stdlib that does this. find it.
			}
		}

		public void SaveIn (string dir)
		{
			File.WriteAllText (Path.Combine (dir, FileName), Source);
		}

	}
}

