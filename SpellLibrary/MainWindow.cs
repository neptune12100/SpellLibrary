using fNbt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpellLibrary
{
    public class MainWindow : Form
    {
		private bool HasJar = false;
        public static readonly int PreviewScale = 4;
        private ListBox SpellList;
        private Label PreviewLabel;
        private Button CopyButton;
        private Button PasteButton;
        private Button DeleteButton;
        private Button ImportButton;
        public static readonly string libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Psi Spell Library");
        private string OldText;

        public MainWindow()
        {
            InitializeComponent();
            HasJar = PieceImage.Init();
            if (HasJar)
            {
                PreviewLabel.Text = "";
                PreviewLabel.Image = SpellImage.BlankImage;
            }
            //buttonDownloadPsi.Text = "Download psi-" + PsiJarHelper.GetLatestPsiVersion("1.9") + ".jar";
            LoadAllSpells();
        }

        public static string MinecraftFolder
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");
                    case PlatformID.MacOSX:
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Application Support", "minecraft"); //Don't have a mac, don't know if this is right
                    default: // ~/.minecraft on Linux
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft");
                }
            }
        }

        private void SaveSpell(Spell spell)
        {
            spell.SaveIn(libraryPath);
        }

        private void LoadAllSpells()
        {
            if (!Directory.Exists(libraryPath))
            {
                Directory.CreateDirectory(libraryPath);
                return;
            }
            Spell[] spells = Spell.LoadFromDir(libraryPath);
            AddSpells(spells);
        }

        private void DeleteSpell(Spell spell)
        {
            string path = Path.Combine(libraryPath, spell.FileName);
            File.Delete(path);
            SpellList.Items.Remove(spell);
        }
        
        

        #region stuff that should be in a separate designer file but isn't so bite me
        private void InitializeComponent()
        {
            this.SpellList = new System.Windows.Forms.ListBox();
            this.PreviewLabel = new System.Windows.Forms.Label();
            this.CopyButton = new System.Windows.Forms.Button();
            this.PasteButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SpellList
            // 
            this.SpellList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.SpellList.ForeColor = System.Drawing.Color.White;
            this.SpellList.FormattingEnabled = true;
            this.SpellList.ItemHeight = 20;
            this.SpellList.Location = new System.Drawing.Point(12, 12);
            this.SpellList.Name = "SpellList";
            this.SpellList.Size = new System.Drawing.Size(170, 484);
            this.SpellList.TabIndex = 0;
            this.SpellList.SelectedIndexChanged += new System.EventHandler(this.SpellList_SelectedIndexChanged);
            // 
            // PreviewLabel
            // 
            this.PreviewLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.PreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewLabel.ForeColor = System.Drawing.Color.Red;
            this.PreviewLabel.Location = new System.Drawing.Point(188, 12);
            this.PreviewLabel.Name = "PreviewLabel";
            this.PreviewLabel.Size = new System.Drawing.Size(612, 612);
            this.PreviewLabel.TabIndex = 1;
            this.PreviewLabel.Text = "No Psi jar found.\r\nDownload Psi and place the file in the same folder as SpellLib" +
    "rary.exe";
            this.PreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CopyButton
            // 
            this.CopyButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.CopyButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CopyButton.Location = new System.Drawing.Point(12, 503);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(170, 23);
            this.CopyButton.TabIndex = 2;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = false;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // PasteButton
            // 
            this.PasteButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.PasteButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PasteButton.Location = new System.Drawing.Point(12, 532);
            this.PasteButton.Name = "PasteButton";
            this.PasteButton.Size = new System.Drawing.Size(170, 23);
            this.PasteButton.TabIndex = 2;
            this.PasteButton.Text = "Paste";
            this.PasteButton.UseVisualStyleBackColor = false;
            this.PasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.DeleteButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DeleteButton.Location = new System.Drawing.Point(12, 561);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(170, 23);
            this.DeleteButton.TabIndex = 2;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = false;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ImportButton
            // 
            this.ImportButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ImportButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ImportButton.Location = new System.Drawing.Point(12, 590);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(170, 23);
            this.ImportButton.TabIndex = 2;
            this.ImportButton.Text = "Import from Save";
            this.ImportButton.UseVisualStyleBackColor = false;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // MainWindow
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(799, 624);
            this.Controls.Add(this.ImportButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.PasteButton);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.PreviewLabel);
            this.Controls.Add(this.SpellList);
            this.Name = "MainWindow";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ResumeLayout(false);

        }
        #endregion

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (SpellList.SelectedItem != null)
                Clipboard.SetText(((Spell)SpellList.SelectedItem).Source);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {

            Spell spell = new Spell(Clipboard.GetText(TextDataFormat.Text));
            AddSpell(spell);
        }

        private void AddSpells(Spell[] spells)
        {
            foreach (Spell spell in spells)
            {
                AddSpell(spell);
            }
        }

        private void AddSpell(Spell spell)
        {
            object[] roSpells = new object[SpellList.Items.Count];
            SpellList.Items.CopyTo(roSpells, 0);
            foreach (var s in roSpells)
            {
                Spell sp = (Spell)s;
                if (sp.FileName == spell.FileName)
                {
                    DeleteSpell(sp);
                }
            }
            SaveSpell(spell);
            SpellList.Items.Add(spell);
            SpellList.SelectedItem = spell;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {

            int selected = SpellList.SelectedIndex;
            if (SpellList.SelectedItem == null)
                return;

            DeleteSpell((Spell)SpellList.SelectedItem);
            PreviewLabel.Image = SpellImage.BlankImage;

            if (selected == SpellList.Items.Count)
            {
                SpellList.SelectedIndex = selected - 1;
            } else if (SpellList.Items.Count != 0)
            {
                SpellList.SelectedIndex = selected;
            } //else there's nothing left
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            ImportButton.Enabled = false;
            OldText = ImportButton.Text;
            ImportButton.Text = "Importing...";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path.Combine(MinecraftFolder, "saves");
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += Worker_ScanWorld;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerAsync(path);
            } else
            {
                ImportButton.Text = OldText;
                ImportButton.Enabled = true;
            }

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ImportButton.Text = "Importing, " + e.ProgressPercentage.ToString() + "%";
        }

        private void Worker_ScanWorld(object sender, DoWorkEventArgs e)
        {
            List<NbtTag> l = new List<NbtTag>();
            string[] files = WorldScanner.FindFilesToScan((string)e.Argument);
            double numFiles = files.Length;
            double filesScanned = 0;

            foreach (string filePath in files)
            {
                l.AddRange(WorldScanner.ScanFile(filePath, "spell"));
                filesScanned++;
                ((BackgroundWorker)sender).ReportProgress((int)Math.Floor((filesScanned / numFiles) * 100.0));
            }
            Spell[] spells = new Spell[l.Count];
            for (int i = 0; i < spells.Length; i++)
            {
                spells[i] = new Spell(l[i]);
            }
            e.Result = spells;
            //e.Result = WorldScanner.ScanWorldForSpells((string)e.Argument);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AddSpells((Spell[])e.Result);
            ImportButton.Text = OldText;
            ImportButton.Enabled = true;
        }

        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HasJar)
                return;

            Spell spell = (Spell)SpellList.SelectedItem;
            if (spell != null)
            {
                try
                {
                    Image image = SpellImage.RenderSpell(spell);
                    PreviewLabel.Image = image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}

