//
//  PieceImage.cs
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
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing.Imaging;

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

		private static ZipArchive zf;
		public static readonly string basePath = "assets/psi/textures/spell/";

		public static Image[] ParameterImages, ConnectorImages;
		public static Dictionary<string, Image> Images = new Dictionary<string, Image> ();

		public static void Init ()
		{
			zf = ZipFile.Open ("psi.jar", ZipArchiveMode.Read);
			Bitmap programmer = (Bitmap)Bitmap.FromStream (zf.GetEntry ("assets/psi/textures/gui/programmer.png").Open ());
			ParameterImages = new Image[5]; //NONE UP DOWN LEFT RIGHT
			ParameterImages [0] = null;
			ParameterImages [2] = programmer.Clone (new Rectangle (230, 8, 8, 8), PixelFormat.DontCare); //for some reason "PixelFormat.DontCare" cracks me up
			ParameterImages [1] = programmer.Clone (new Rectangle (222, 8, 8, 8), PixelFormat.DontCare);
			ParameterImages [4] = programmer.Clone (new Rectangle (222, 0, 8, 8), PixelFormat.DontCare);
			ParameterImages [3] = programmer.Clone (new Rectangle (230, 0, 8, 8), PixelFormat.DontCare);

			Bitmap connectors = (Bitmap)Bitmap.FromStream (zf.GetEntry ("assets/psi/textures/spell/connectorLines.png").Open ());
			ConnectorImages = new Image[5];
			ConnectorImages [0] = null;
			ConnectorImages [2] = connectors.Clone (new Rectangle (16, 16, 16, 16), PixelFormat.DontCare);
			ConnectorImages [1] = connectors.Clone (new Rectangle (0, 16, 16, 16), PixelFormat.DontCare);
			ConnectorImages [4] = connectors.Clone (new Rectangle (0, 0, 16, 16), PixelFormat.DontCare);
			ConnectorImages [3] = connectors.Clone (new Rectangle (16, 0, 16, 16), PixelFormat.DontCare);

		}

		public static Image Get (string name)
		{
			if (Images.ContainsKey (name)) {
				//Console.WriteLine ("Had " + name + " already loaded.");
				return Images [name];
			} else {
				Image i = Bitmap.FromStream (zf.GetEntry (basePath + name + ".png").Open ());
				Images [name] = i;
				//Console.WriteLine ("Loaded " + name + ".");
				return i;
			}
		}
	}
}

