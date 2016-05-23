using System;
using System.Collections;
using fNbt;

namespace TextNbt
{
    public class TextNbtParser
    {
        private int Position = -1;
        private string Source;

        private TextNbtParser(string src)
        {
            Source = src.Trim();
        }

        private char NextChar()
        {
            Position += 1;
            if (Position >= Source.Length)
            {
                Position = Source.Length - 1;
                throw new Exception("Unexpected EOF");
            }
            return Source[Position];
        }

        /**
		 * Called AFTER reading the opening "
		 */
        private NbtString ParseString()
        {
            bool more = true;
            string literal = "";
            while (more)
            {
                char next;
                try
                {
                    next = NextChar();
                }
                catch (Exception)
                {
                    throw new Exception(@"Unexpected EOF, expected '""'");
                }
                if (next == '\\')
                {
                    literal += NextChar(); //Backslash escapes "'s
                }
                else if (next == '"')
                {
                    more = false;
                }
                else {
                    literal += next;
                }
            }
            return new NbtString(literal);
        }

        /**
		 * BEFORE any digits, requiring backtracking.
		 */
        private NbtTag ParseNumber()
        {
            bool more = true;
            NbtTagType type = NbtTagType.Int;
            String literal = "";
            while (more)
            {
                char next;
                try
                {
                    next = NextChar();
                }
                catch (Exception)
                {
                    next = ' '; //just end the number
                }
                if (next <= '9' && next >= '0' || next == '-')
                {
                    literal += next;
                }
                else if (next == '.')
                {
                    literal += next;
                    type = NbtTagType.Double;
                }
                else {
                    more = false;
                    switch (next)
                    {
                        case 'l':
                        case 'L':
                            type = NbtTagType.Long;
                            break;
                        case 'b':
                        case 'B':
                            type = NbtTagType.Byte;
                            break;
                        case 'f':
                        case 'F':
                            type = NbtTagType.Float;
                            break;
                        default:
                            //End of number, put it back.
                            Position--;
                            break;
                    }
                }
            }
            NbtTag ret;
            switch (type)
            {
                case NbtTagType.Byte:
                    ret = new NbtByte(byte.Parse(literal));
                    break;
                case NbtTagType.Int:
                    ret = new NbtInt(int.Parse(literal));
                    break;
                case NbtTagType.Long:
                    ret = new NbtLong(long.Parse(literal));
                    break;
                case NbtTagType.Float:
                    ret = new NbtFloat(float.Parse(literal));
                    break;
                case NbtTagType.Double:
                    ret = new NbtDouble(double.Parse(literal));
                    break;
                default:
                    ret = null;
                    break;
            }
            return ret;
        }

        /**
		 * AFTER the opening [
		 */
        private NbtList ParseList()
        {
            bool more = true; //I'm sensing a pattern here...
            NbtList list = new NbtList();
            NbtTag current = null;
            int index = -1;
            while (more)
            {
                char next;
                try
                {
                    next = NextChar();
                }
                catch (Exception)
                {
                    throw new Exception(@"Unexpected EOF, expected ']'");
                }
                switch (next)
                {
                    case ']':
                        more = false;
                        goto case ','; //fuckin fall through.
                    case ',':
                        if (current != null)
                        {
                            if (index != -1)
                            {
                                if (index >= list.Count)
                                {
                                    for (int i = list.Count; i <= index; i++)
                                    {
                                        list.Add((NbtTag)current.GetType().GetConstructor(new Type[0]).Invoke(new object[0])); //HOLY FUCKNUGGETS
                                    }
                                }
                                list[index] = current;
                            }
                            else {
                                list.Add(current);
                            }
                            index = -1;
                            current = null;
                        }
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    case ':': //oh noes explicit index, doesn't even have to be in order.
                        index = current.IntValue;
                        current = null;
                        break;
                    default:
                        Position--;
                        current = ParseTag();
                        break;
                }
            }
            return list;
        }

        private NbtCompound ParseCompound()
        {
            bool more = true;
            NbtCompound compound = new NbtCompound();
            NbtTag current = null;
            string currentName = "";
            bool afterName = false;
            while (more)
            {
                char next;
                try
                {
                    next = NextChar();
                }
                catch (Exception)
                {
                    throw new Exception(@"Unexpected EOF, expected '}'");
                }
                switch (next)
                {
                    case '}':
                        more = false;
                        goto case ','; //fuckin fall through.
                    case ',':
                        if (current != null)
                        {
                            current.Name = currentName;
                            compound.Add(current);
                            afterName = false;
                            current = null;
                            currentName = "";
                        }
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    case ':':
                        afterName = true;
                        break;
                    default:
                        if (afterName)
                        {
                            Position--;
                            current = ParseTag();
                        }
                        else {
                            currentName += next;
                        }
                        break;
                }
            }
            return compound;
        }

        /**
		 * Before ANY of the tag is parsed (including opening ", [ and {)
		 */
        private NbtTag ParseTag()
        {
            char next;
            try
            {
                next = NextChar();
            }
            catch (Exception)
            {
                throw new Exception(@"Unexpected EOF, expected prety much anything but that.");
            }
            NbtTag ret = null;
            switch (next)
            {
                case '"':
                    ret = ParseString();
                    break;
                case '[':
                    ret = ParseList();
                    break;
                case '{':
                    ret = ParseCompound();
                    break;
                default:
                    if (next >= '0' && next <= '9' || next == '-')
                    { //Fuckin seriously, I cant just put a case '0':'9' or something?
                        Position--; //blundered into a number.
                        ret = ParseNumber();
                    }
                    break;
            }
            return ret;
        }

        public static NbtTag Parse(string src)
        {
            TextNbtParser p = new TextNbtParser(src);
            return p.ParseTag();
        }
    }
}
