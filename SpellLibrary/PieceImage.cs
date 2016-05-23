using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

namespace SpellLibrary
{
    public abstract class PieceImage
    {
        public enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        public static string FindPsiJar()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*Psi*.jar");
            if (files.Length == 0)
            {
                return null;
            }
            return files[0];
        }

        private static ZipArchive zf;
        public static readonly string basePath = "assets/psi/textures/spell/";

        public static Image[] ParameterImages, ConnectorImages;
        public static Dictionary<string, Image> Images = new Dictionary<string, Image>();

        public static bool Init()
        {
            string jarPath = FindPsiJar();
            if (jarPath == null)
            {
                return false;
            }
            zf = ZipFile.Open(jarPath, ZipArchiveMode.Read);
            Bitmap programmer = (Bitmap)Bitmap.FromStream(zf.GetEntry("assets/psi/textures/gui/programmer.png").Open());
            ParameterImages = new Image[5]; //NONE UP DOWN LEFT RIGHT
            ParameterImages[0] = null;
            ParameterImages[2] = programmer.Clone(new Rectangle(230, 8, 8, 8), PixelFormat.DontCare); //for some reason "PixelFormat.DontCare" cracks me up
            ParameterImages[1] = programmer.Clone(new Rectangle(222, 8, 8, 8), PixelFormat.DontCare);
            ParameterImages[4] = programmer.Clone(new Rectangle(222, 0, 8, 8), PixelFormat.DontCare);
            ParameterImages[3] = programmer.Clone(new Rectangle(230, 0, 8, 8), PixelFormat.DontCare);

            Bitmap connectors = (Bitmap)Bitmap.FromStream(zf.GetEntry("assets/psi/textures/spell/connectorLines.png").Open());
            ConnectorImages = new Image[5];
            ConnectorImages[0] = null;
            ConnectorImages[2] = connectors.Clone(new Rectangle(16, 16, 16, 16), PixelFormat.DontCare);
            ConnectorImages[1] = connectors.Clone(new Rectangle(0, 16, 16, 16), PixelFormat.DontCare);
            ConnectorImages[4] = connectors.Clone(new Rectangle(0, 0, 16, 16), PixelFormat.DontCare);
            ConnectorImages[3] = connectors.Clone(new Rectangle(16, 0, 16, 16), PixelFormat.DontCare);
            return true;
        }

        public static Image Get(string name)
        {
            if (Images.ContainsKey(name))
            {
                //Console.WriteLine ("Had " + name + " already loaded.");
                return Images[name];
            }
            else {
                Image i = Bitmap.FromStream(zf.GetEntry(basePath + name + ".png").Open());
                Images[name] = i;
                //Console.WriteLine ("Loaded " + name + ".");
                return i;
            }
        }
    }
}

