using System;
using System.Collections.Generic;
using System.IO;
using fNbt;

namespace SpellLibrary
{
    /// <summary>
    /// Scans Minecraft save files for Psi spells.
    /// </summary>
    public class WorldScanner
    {
        
        public static string[] FindFilesToScan(string saveBase)
        {
            List<string> files = new List<string>();

            files.AddRange(Directory.GetFiles(Path.Combine(saveBase, "playerdata"), "*.dat")); //Players' individual files
            files.AddRange(Directory.GetFiles(saveBase, "*.mca", SearchOption.AllDirectories)); //World files in all dimensions. Woot!

            return files.ToArray();
        }

        public static Spell[] ScanWorldForSpells(string path)
        {
            List<NbtTag> l = new List<NbtTag>();
            string[] files = FindFilesToScan(path);
            foreach (string filePath in files)
            {
                l.AddRange(ScanFile(filePath, "spell"));
            }
            Spell[] spells = new Spell[l.Count];
            for (int i = 0; i < spells.Length; i++)
            {
                spells[i] = new Spell(l[i]);
            }
            return spells;
        }

        public static NbtTag[] ScanFile(string path, string needleName)
        {
            if (path.EndsWith(".mca"))
            {
                return FindTagsNamedInAnvil(new AnvilFile(path), needleName);
            }
            else if (path.EndsWith(".dat"))
            {
                NbtTag t = new NbtFile(path).RootTag;
                return FindTagsNamed(t, needleName);
            }
            return new NbtTag[0];
        }

        public static NbtTag[] FindTagsNamedInAnvil(AnvilFile file, string needleName)
        {
            List<NbtTag> l = new List<NbtTag>();
            for (int i = 0; i < 32 * 32; i++)
            {
                NbtTag t = file.GetChunkData(i);
                if (t != null)
                {
                    l.AddRange(FindTagsNamed(t, needleName));
                }
            }
            return l.ToArray();
        }

        public static NbtTag[] FindTagsNamed(NbtTag haystack, string needleName)
        {
            List<NbtTag> l = new List<NbtTag>();
            if (!haystack.HasValue)
            {
                ICollection<NbtTag> tags = ((ICollection<NbtTag>)haystack); //I DON'T CARE WHETHER ITS A LIST OR A COMPOUND.

                foreach (NbtTag tag in tags)
                {
                    if (!tag.HasValue)
                    {
                        if (tag.Name == needleName)
                        {
                            l.Add(tag);
                        }
                        else
                        {
                            l.AddRange(FindTagsNamed(tag, needleName)); //GETTIN RECURSIVE UP IN THIS MOFO. BETTER WATCH OUT.
                        }
                    }
                }
            }
            return l.ToArray();
        }
    }
}
