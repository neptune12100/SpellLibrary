//
//  SpellImage.cs
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
using System.Drawing;
using fNbt;
using TextNbt;
using System.Drawing.Imaging;

namespace SpellLibrary
{
    public abstract class SpellImage
    {
        public static readonly int Width = 9, Height = 9;
        public static readonly int Scale = 4;
        public static readonly int TileWidth = 16 * Scale, TileHeight = 16 * Scale;
        public static readonly int Spacing = 1 * Scale;
        public static readonly int ImageWidth = Width * (TileWidth + Spacing), ImageHeight = Height * (TileHeight + Spacing);

        public static readonly Color BackgroundColor = Color.FromArgb(0x1c, 0x1c, 0x1c);


        private static readonly int[] Backwards = new int[] { 0, 2, 1, 4, 3 };

        private static readonly Point[] ParameterOffset = {
            new Point (0 * Scale, 0 * Scale),
            new Point (4 * Scale, -4 * Scale),
            new Point (4 * Scale, 13 * Scale),
            new Point (-5 * Scale, 4 * Scale),
            new Point (13 * Scale, 4 * Scale)
        };

        private static readonly int[] XOffsets = { 0, 0, 0, -1, 1 };
        private static readonly int[] YOffsets = { 0, -1, 1, 0, 0 };

        private static Image _blankImage;

        public static Image BlankImage
        {
            get
            {
                if (_blankImage == null)
                {
                    _blankImage = new Bitmap(ImageWidth, ImageHeight);
                    Graphics.FromImage(_blankImage).Clear(BackgroundColor);
                }
                return _blankImage;
            }
        }

        private static String GetKey(NbtCompound piece)
        {
            return piece.Contains("data") ? piece["data"]["key"].StringValue : piece["spellData"]["spellKey"].StringValue;
        }

        private static int GetX(NbtCompound piece)
        {
            return piece.Contains("data") ? piece["x"].IntValue : piece["spellPosX"].IntValue;
        }

        private static int GetY(NbtCompound piece)
        {
            return piece.Contains("data") ? piece["y"].IntValue : piece["spellPosY"].IntValue;
        }

        private static NbtCompound GetParams(NbtCompound piece)
        {
            if (piece.Contains("data"))
                return (NbtCompound)piece["data"]["params"];
            return (NbtCompound)piece["spellData"]["params"];
        }

        private static string GetConstantValue(NbtCompound piece)
        {
            return (piece["spellData"] != null ? piece["spellData"] : piece["data"])["constantValue"].StringValue;
        }

        private static int GetConnectorTarget(NbtCompound connector)
        {
            if (connector.Contains("data"))
                return connector["data"]["params"]["_target"].IntValue;
            return connector["spellData"]["params"]["psi.spellparam.target"].IntValue;
        }

        private static int Reverse(int dir)
        {
            return Backwards[dir];
        }

        private static bool HasConnection(NbtList pieces, NbtCompound connector, int dir)
        {
            NbtCompound piece = GetPieceAtSide(pieces, connector, dir);
            if (piece == null)
            {
                return false;
            }
            NbtCompound pieceParams = GetParams(piece);
            if (pieceParams != null)
            {
                foreach (var param in pieceParams)
                {
                    if (param.IntValue == Reverse(dir))
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        public static NbtCompound GetPieceAtSide(NbtList pieces, NbtCompound piece, int side)
        {
            int targetX = GetX(piece) + XOffsets[side];
            int targetY = GetY(piece) + YOffsets[side];
            NbtCompound result = null;
            if (targetX >= 0 && targetX < 9 && targetY >= 0 && targetY < 9)
            {
                foreach (NbtCompound other in pieces)
                {
                    if (GetX(other) == targetX && GetY(other) == targetY)
                    {
                        result = other;
                        break;
                    }
                }
            }
            return result;
        }

        public static Image RenderSpell(Spell spell)
        {
            NbtCompound spellNbt = (NbtCompound)TextNbtParser.Parse(spell.Source);
            NbtList pieces = (NbtList)spellNbt["spellList"];

            Bitmap image = new Bitmap(ImageWidth, ImageHeight);
            Graphics g = Graphics.FromImage(image);
            g.Clear(BackgroundColor);

            //Piece pass
            foreach (NbtCompound piece in pieces)
            {
                int x = GetX(piece);
                int y = GetY(piece);
                string key = GetKey(piece);
                x *= (TileWidth + Spacing);
                y *= (TileHeight + Spacing);
                
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(PieceImage.Get(key), x, y, TileWidth, TileHeight);

                if (key == "constantNumber")
                {
                    String value = GetConstantValue(piece);
                    g.DrawString(value,
                        new Font(FontFamily.GenericMonospace, 40 / value.Length),
                        new SolidBrush(Color.White),
                        new RectangleF(x, y, TileWidth, TileHeight));
                }
            }

            //Connector pass
            foreach (NbtCompound piece in pieces)
            {
                int x = GetX(piece);
                int y = GetY(piece);
                string key = GetKey(piece);
                int pixx = x * (TileWidth + Spacing);
                int pixy = y * (TileHeight + Spacing);
                if (key == "connector")
                {
                    g.DrawImage(PieceImage.ConnectorImages[GetConnectorTarget(piece)], pixx, pixy, TileWidth, TileHeight);

                    for (int i = 1; i <= 4; i++)
                    {
                        bool con = HasConnection(pieces, piece, i);
                        if (con)
                        {
                            g.DrawImage(PieceImage.ConnectorImages[i], pixx, pixy, TileWidth, TileHeight);
                        }

                    }
                }
            }

            //Parameter pass
            foreach (NbtCompound piece in pieces)
            {
                int x = GetX(piece);
                int y = GetY(piece);
                string key = GetKey(piece);
                x *= (TileWidth + Spacing);
                y *= (TileHeight + Spacing);
                NbtCompound par = GetParams(piece);
                if (par != null)
                {
                    foreach (var param in par)
                    {
                        int i = param.IntValue;
                        if (i > 0 && i <= 4)
                        {
                            Point p = new Point(x, y);
                            p.Offset(ParameterOffset[i]);
                            g.DrawImage(PieceImage.ParameterImages[i], new Rectangle(p, new Size(TileWidth / 2, TileHeight / 2)));
                        }
                    }
                }
            }
            return image;
        }
    }
}

