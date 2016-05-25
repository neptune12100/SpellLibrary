using fNbt;
using System;

namespace SpellLibrary
{
    /// <summary>
    /// Encapsulates a spell piece to abstract from any changes to vazkii's spell NBT format
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// Sides for parameters
        /// </summary>
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
            return Backwards[(int)side];
        }


        private static readonly int[] XOffsets = { 0, 0, 0, -1, 1 };
        private static readonly int[] YOffsets = { 0, -1, 1, 0, 0 };

        public String Key;
        public Side[] Parameters;
        public int X, Y;
        public string ConstantValue = "";

        /// <summary>
        /// Construct a Piece with arbitrary properties
        /// </summary>
        /// <param name="key">The piece's key, e.g. operatorSum</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="parameters">The piece's parameters</param>
        public Piece(String key, int x, int y, Side[] parameters)
        {
            Key = key;
            Parameters = (Side[])parameters.Clone();
            X = x;
            Y = y;
        }

        /// <summary>
        /// Construct a piece from an NbtCompound containing the piece's data
        /// </summary>
        /// <param name="nbt">The NbtCompound conatining the piece's data</param>
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
                    Parameters[i++] = (Side)p.IntValue;
                }
            }
        }

        /// <summary>
        /// Given a list of other pieces in a spell, find the piece at the given side of this one
        /// </summary>
        /// <param name="pieces">Array of other pieces in the spell</param>
        /// <param name="side">The side to search at</param>
        /// <returns>The found Piece, or null if there is none</returns>
        public Piece GetPieceAtSide(Piece[] pieces, Side side)
        {
            int targetX = X + XOffsets[(int)side];
            int targetY = Y + YOffsets[(int)side];
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

        /// <summary>
        /// For connectors, finds if the Piece at the given side uses this one as an input
        /// </summary>
        /// <param name="pieces">Array of other pieces in the spell</param>
        /// <param name="side">The side to search at</param>
        /// <returns>True if this piece should connect to this side, false otherwise</returns>
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
            return string.Format("{0} at ({1},{2})", Key, X, Y);
        }
    }
}

