//
//  MainWindow.cs
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
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using fNbt;

namespace SpellLibrary
{
	public class MainWindow : Form
	{
		private ListBox list;
		private TableLayoutPanel panel;
		private Label preview;

		public static readonly int PreviewScale = 4;
		public static readonly string libraryPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "Psi Spell Library");

		public MainWindow ()
		{
			//GUI setup

			panel = new TableLayoutPanel ();

			list = new ListBox ();
			list.MinimumSize = new Size (278, 500);
			panel.Controls.Add (list, 0, 0);
			panel.SetColumnSpan (list, 3);

			list.SelectedValueChanged += HandleSelectionChange;

			Button copy = new Button ();
			copy.Text = "Copy";
			copy.Click += new EventHandler (DoCopy);
			panel.Controls.Add (copy, 0, 1);

			Button paste = new Button ();
			paste.Text = "Paste";
			paste.Click += new EventHandler (DoPaste);
			panel.Controls.Add (paste, 1, 1);

			Button delete = new Button ();
			delete.Text = "Delete";
			delete.Click += DoDelete;
			panel.Controls.Add (delete, 2, 1);

			preview = new Label ();
			preview.Image = SpellImage.BlankImage;
			preview.MinimumSize = new Size (SpellImage.ImageWidth, SpellImage.ImageHeight);
			panel.Controls.Add (preview, 3, 0);
			panel.SetRowSpan (preview, 2);

			Controls.Add (panel);
			panel.AutoSize = true;
			AutoSize = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Text = "Psi Spell Library";

			//Loading images
			PieceImage.Init ();

			//Loading spells
			Console.WriteLine ("Loading spells...");
			DateTime begin = DateTime.Now;
			if (!Directory.Exists (libraryPath)) {
				Directory.CreateDirectory (libraryPath);
			} else {
				LoadAllSpells ();
			}
			TimeSpan loadingTime = DateTime.Now - begin;
			Console.WriteLine ("Loaded " + list.Items.Count + " spells in " + loadingTime.TotalSeconds + "s");
		}

		private void HandleSelectionChange (Object sender, EventArgs e)
		{
			Spell spell = (Spell)list.SelectedItem;
			if (spell != null) {
				try {
					Image image = SpellImage.RenderSpell (spell);
					preview.Image = image;
				} catch (Exception ex) {
					Console.WriteLine (ex.ToString ());
				}
			}
		}

		private void HandleClosing (Object sender, FormClosingEventArgs e)
		{
			SaveAllSpells ();
		}

		private void SaveAllSpells ()
		{
			foreach (var spell in list.Items) {
				SaveSpell ((Spell)spell);
			}
		}

		private void SaveSpell (Spell spell)
		{
			((Spell)spell).SaveIn (libraryPath);
		}

		private void LoadAllSpells ()
		{
			object[] spells = Spell.LoadFromDir (libraryPath);
			list.Items.AddRange (spells);
		}

		private void DoCopy (Object sender, EventArgs e)
		{
			Clipboard.SetText (((Spell)list.SelectedItem).Source);
		}

		private void DoPaste (Object sender, EventArgs e)
		{
			Spell spell = new Spell (Clipboard.GetText (TextDataFormat.Text));
			object[] roSpells = new object[list.Items.Count];
			list.Items.CopyTo (roSpells, 0);
			foreach (var s in roSpells) {
				Spell sp = (Spell)s;
				if (sp.Title == spell.Title) {
					DeleteSpell (sp);
				}
			}
			SaveSpell (spell);
			list.Items.Add (spell);
		}

		private void DeleteSpell (Spell spell)
		{
			string path = Path.Combine (libraryPath, spell.FileName);
			File.Delete (path);
			list.Items.Remove (spell);
		}

		private void DoDelete (Object sender, EventArgs e)
		{
			DeleteSpell ((Spell)list.SelectedItem);
			preview.Image = SpellImage.BlankImage;
		}

		[STAThread]
		public static int Main (string[] args)
		{
			Application.EnableVisualStyles ();
			Application.Run (new MainWindow ());
			return 0;
		}
		//		public static int Main (string[] args)
		//		{
		//			Spell spell = Spell.LoadFile (Path.Combine (libraryPath, "21x21.json"));
		//			SpellImage.RenderSpell (spell).Save("test.png");
		//			return 0;
		//		}
	}
}

