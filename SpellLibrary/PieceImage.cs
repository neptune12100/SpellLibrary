﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

namespace SpellLibrary
{
    /// <summary>
    /// Provides static methods to load images from the Psi jar file
    /// </summary>
    public abstract class PieceImage
    {
        /// <summary>
        /// Finds the Psi jar
        /// </summary>
        /// <returns>the name of the jar file</returns>
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

        /// <summary>
        /// Images for the parameter arrows
        /// </summary>
        public static Image[] ParameterImages;
        /// <summary>
        /// Images for connector lines
        /// </summary>
        public static Image[] ConnectorImages;

        private static Dictionary<string, Image> Images = new Dictionary<string, Image>();

        /// <summary>
        /// finds the Psi jar, loads a few images. Must be called befoore using the class.
        /// </summary>
        /// <returns>true if Psi jar was found, false otherwise</returns>
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

        /// <summary>
        /// Gets the image for a spell piece
        /// </summary>
        /// <param name="key">Which piece</param>
        /// <returns>the image</returns>
        public static Image Get(string key)
        {
            if (Images.ContainsKey(key))
            {
                //Console.WriteLine ("Had " + name + " already loaded.");
                return Images[key];
            }
            else {
                Image i = Bitmap.FromStream(zf.GetEntry(basePath + key + ".png").Open());
                Images[key] = i;
                //Console.WriteLine ("Loaded " + name + ".");
                return i;
            }
        }
    }
}

