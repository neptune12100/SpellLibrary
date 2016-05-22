using fNbt;
using System;

namespace SpellLibrary
{
    public class Piece
    {
        public enum Side
        {
            NONE = 0,
            UP = 1,
            DOWN = 2,
            LEFT = 3,
            RIGHT = 4
        }

        private static readonly Side[] Backwards = new Side[] { Side.NONE, Side.DOWN, Side.UP, Side.RIGHT, Side.LEFT };

        private static Side Reverse(Side side)
        {
            return Backwards[(int) side];
        }


        private static readonly int[] XOffsets = { 0, 0, 0, -1, 1 };
        private static readonly int[] YOffsets = { 0, -1, 1, 0, 0 };

        public String Key;
        public Side[] Parameters;
        public int X, Y;
        public string ConstantValue = "";

        public Piece(String key, int x, int y, Side[] parameters)
        {
            Key = key;
            Parameters = (Side[]) parameters.Clone();
            X = x;
            Y = y;
        }

        public Piece(NbtCompound nbt)
        {
            NbtCompound data;
            if (nbt.Contains("data")) //New NBT format
            {
                X = nbt["x"].IntValue;
                Y = nbt["y"].IntValue;
                data = (NbtCompound)nbt["data"];
                Key = data["key"].StringValue;
            }
            else //Old NBT format
            {
                X = nbt["spellPosX"].IntValue;
                Y = nbt["spellPosY"].IntValue;
                data = (NbtCompound)nbt["spellData"];
                Key = data["spellKey"].StringValue;
            }

            if (data.Contains("constantValue"))
            {
                ConstantValue = data["constantValue"].StringValue;
            }

            if (data.Contains("params"))
            {
                NbtCompound paramsTag = (NbtCompound)data["params"];
                Parameters = new Side[paramsTag.Count];
                int i = 0;
                foreach (NbtInt p in paramsTag)
                {
                    Parameters[i++] = (Side) p.IntValue;
                }
            }
        }

        public Piece GetPieceAtSide(Piece[] pieces, Side side)
        {
            int targetX = X + XOffsets[(int) side];
            int targetY = Y + YOffsets[(int) side];
            Piece result = null;
            if (targetX >= 0 && targetX < 9 && targetY >= 0 && targetY < 9)
            {
                foreach (Piece other in pieces)
                {
                    if (other.X == targetX && other.Y == targetY)
                    {
                        result = other;
                        break;
                    }
                }
            }
            return result;
        }

        public bool HasConnection(Piece[] pieces, Side side)
        {
            Piece other = GetPieceAtSide(pieces, side);
            if (other == null)
            {
                return false;
            }
            Side[] pieceParams = other.Parameters;
            if (pieceParams != null)
            {
                foreach (Side param in pieceParams)
                {
                    if (param == Reverse(side))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} at ({1},{2}), parameters {3}", Key, X, Y, Parameters);
        }
    }
}

