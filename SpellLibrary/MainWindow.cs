using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpellLibrary
{
    public class MainWindow : Form
    {
        private ListBox list;
        private TableLayoutPanel panel;
        private Label preview;
		private bool HasJar = false;
        public static readonly int PreviewScale = 4;
        public static readonly string libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Psi Spell Library");

        public MainWindow()
        {
            //GUI setup

            panel = new TableLayoutPanel();

            list = new ListBox();
            list.MinimumSize = new Size(278, 500);
            panel.Controls.Add(list, 0, 0);
            panel.SetColumnSpan(list, 3);

            list.SelectedValueChanged += HandleSelectionChange;

            Button copy = new Button();
            copy.Text = "Copy";
            copy.Click += new EventHandler(DoCopy);
            panel.Controls.Add(copy, 0, 1);

            Button paste = new Button();
            paste.Text = "Paste";
            paste.Click += new EventHandler(DoPaste);
            panel.Controls.Add(paste, 1, 1);

            Button delete = new Button();
            delete.Text = "Delete";
            delete.Click += DoDelete;
            panel.Controls.Add(delete, 2, 1);

            preview = new Label();
            //Loading images
			HasJar = PieceImage.Init ();
            if (HasJar)
			{
                preview.Image = SpellImage.BlankImage;
                preview.MinimumSize = new Size(SpellImage.ImageWidth, SpellImage.ImageHeight);
				preview.BackColor = Color.Black;
				preview.Location = new Point (278, 0);
				preview.AutoSize = true;
				Controls.Add (preview);
                //panel.Controls.Add(preview, 3, 0);
                //panel.SetRowSpan(preview, 2);
            }
            else
            {
                preview.Text = "No Psi jar found, previews disabled.\nTo enable previews, download the latest Psi jar\nand put it in the same folder as this program.";
                preview.AutoSize = true;
                panel.Controls.Add(preview, 0, 2);
                panel.SetColumnSpan(preview, 3);
            }

            Controls.Add(panel);
            panel.AutoSize = true;
			AutoSize = true;
			MinimumSize = new Size (278 + (HasJar ? 612 : 0), 612);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "Psi Spell Library";

            //Loading spells
            Console.WriteLine("Loading spells...");
            DateTime begin = DateTime.Now;
            if (!Directory.Exists(libraryPath))
            {
                Directory.CreateDirectory(libraryPath);
            }
            else {
                LoadAllSpells();
            }
            TimeSpan loadingTime = DateTime.Now - begin;
            Console.WriteLine("Loaded " + list.Items.Count + " spells in " + loadingTime.TotalSeconds + "s");
        }

        private void HandleSelectionChange(Object sender, EventArgs e)
        {
            Spell spell = (Spell)list.SelectedItem;
            if (spell != null)
            {
                try
                {
                    Image image = SpellImage.RenderSpell(spell);
                    preview.Image = image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void HandleClosing(Object sender, FormClosingEventArgs e)
        {
            SaveAllSpells();
        }

        private void SaveAllSpells()
        {
            foreach (var spell in list.Items)
            {
                SaveSpell((Spell)spell);
            }
        }

        private void SaveSpell(Spell spell)
        {
            ((Spell)spell).SaveIn(libraryPath);
        }

        private void LoadAllSpells()
        {
            object[] spells = Spell.LoadFromDir(libraryPath);
            list.Items.AddRange(spells);
        }

        private void DoCopy(Object sender, EventArgs e)
        {
            Clipboard.SetText(((Spell)list.SelectedItem).Source);
        }

        private void DoPaste(Object sender, EventArgs e)
        {
            Spell spell = new Spell(Clipboard.GetText(TextDataFormat.Text));
            object[] roSpells = new object[list.Items.Count];
            list.Items.CopyTo(roSpells, 0);
            foreach (var s in roSpells)
            {
                Spell sp = (Spell)s;
                if (sp.Title == spell.Title)
                {
                    DeleteSpell(sp);
                }
            }
            SaveSpell(spell);
            list.Items.Add(spell);
        }

        private void DeleteSpell(Spell spell)
        {
            string path = Path.Combine(libraryPath, spell.FileName);
            File.Delete(path);
            list.Items.Remove(spell);
        }

        private void DoDelete(Object sender, EventArgs e)
        {
            DeleteSpell((Spell)list.SelectedItem);
            preview.Image = SpellImage.BlankImage;
        }
        
        [STAThread]
        public static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new MainWindow());
            return 0;
        } 
    }
}

