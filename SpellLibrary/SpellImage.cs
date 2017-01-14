using fNbt;
using System;
using System.Drawing;
using TextNbt;

namespace SpellLibrary
{
    /// <summary>
    /// Renders previews of Spells
    /// </summary>
    public abstract class SpellImage
    {
        public static readonly int Width = 9, Height = 9;
        public static readonly int Scale = 4;
        public static readonly int TileWidth = 16 * Scale, TileHeight = 16 * Scale;
        public static readonly int Spacing = 1 * Scale;
        public static readonly int ImageWidth = Width * (TileWidth + Spacing), ImageHeight = Height * (TileHeight + Spacing);

        public static readonly Color BackgroundColor = Color.FromArgb(0x1c, 0x1c, 0x1c);

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

        /// <summary>
        /// A blank, spell-sized image
        /// </summary>
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


        /// <summary>
        /// Render a spell preview
        /// </summary>
        /// <param name="spell">The spell to render</param>
        /// <returns>An Image of the spell's grid</returns>
        public static Image RenderSpell(Spell spell)
        {
            NbtCompound spellNbt = null;
            try
            {
                spellNbt = (NbtCompound)TextNbtParser.Parse(spell.Source);
            }
            catch (Exception)
            {
                //Console.WriteLine(e);
            }
            if (spellNbt == null)
                return BlankImage;

            NbtList piecesNbt = (NbtList)spellNbt["spellList"];
            Piece[] pieces = new Piece[piecesNbt.Count];
            int index = 0;
            foreach (NbtCompound p in piecesNbt)
            {
                pieces[index++] = new Piece(p);
            }

            Bitmap image = new Bitmap(ImageWidth, ImageHeight);
            Graphics g = Graphics.FromImage(image);
            g.Clear(BackgroundColor);

            //Piece pass
            foreach (Piece piece in pieces)
            {
                int x = piece.X;
                int y = piece.Y;
                string key = piece.Key;
                x *= (TileWidth + Spacing);
                y *= (TileHeight + Spacing);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(PieceImage.Get(key), x, y, TileWidth, TileHeight);

                if (key == "constantNumber")
                {
                    string value = piece.ConstantValue;
                    g.DrawString(value,
                        new Font(FontFamily.GenericMonospace, 40 / value.Length),
                        new SolidBrush(Color.White),
                        new RectangleF(x, y, TileWidth, TileHeight));
                }
            }

            //Connector pass
            foreach (Piece piece in pieces)
            {
                int x = piece.X;
                int y = piece.Y;
                string key = piece.Key;
                int pixx = x * (TileWidth + Spacing);
                int pixy = y * (TileHeight + Spacing);
                if (key == "connector")
                {
                    g.DrawImage(PieceImage.ConnectorImages[(int)piece.Parameters[0]], pixx, pixy, TileWidth, TileHeight);

                    foreach (Piece.Side i in Enum.GetValues(typeof(Piece.Side)))
                    {
                        bool con = piece.HasConnection(pieces, i);
                        if (con)
                        {
                            g.DrawImage(PieceImage.ConnectorImages[(int)i], pixx, pixy, TileWidth, TileHeight);
                        }

                    }
                }
            }

            //Parameter pass
            foreach (Piece piece in pieces)
            {
                int x = piece.X;
                int y = piece.Y;
                string key = piece.Key;
                x *= (TileWidth + Spacing);
                y *= (TileHeight + Spacing);
                Piece.Side[] par = piece.Parameters;
                if (par != null)
                {
                    foreach (int param in par)
                    {
                        if (param > 0 && param <= 4)
                        {
                            Point p = new Point(x, y);
                            p.Offset(ParameterOffset[param]);
                            g.DrawImage(PieceImage.ParameterImages[param], new Rectangle(p, new Size(TileWidth / 2, TileHeight / 2)));
                        }
                    }
                }
            }
            return image;
        }
    }
}

