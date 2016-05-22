using System;

namespace SpellLibrary
{
	public class Piece
	{
		public String Type;
		public int[] Inputs;
		public Piece (String type, int[] inputs)
		{
			Type = type;
			Inputs = inputs;
		}
	}
}

