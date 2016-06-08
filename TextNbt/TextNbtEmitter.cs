using fNbt;
using System;
using System.Text;

namespace TextNbt
{
    /// <summary>
    /// Provides a static method to serialize an NbtTag to a text format Minecraft can use
    /// </summary>
    public class TextNbtEmitter
    {
        /// <summary>
        /// Serialize an fNbt.NbtTag to a JSON-ish representation Minecraft likes
        /// </summary>
        /// <param name="tag">The tag to serialize</param>
        /// <returns>The serialized tag</returns>
        public static string Serialize(NbtTag tag)
        {
            StringBuilder sb = new StringBuilder();
            switch (tag.TagType) {
                case NbtTagType.List:
                    if (((NbtList)tag).Count == 0)
                    {
                        sb.Append("[]");
                    }
                    else {
                        sb.Append("[");
                        NbtList list = (NbtList)tag;
                        for (int i = 0; i < list.Count; i++)
                        {
                            NbtTag subTag = list[i];
                            sb.Append(i);
                            sb.Append(":");
                            sb.Append(Serialize(subTag));
                            sb.Append(",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("]");
                    }
                    break;
                case NbtTagType.Compound:
                    if (((NbtCompound)tag).Count == 0)
                    {
                        sb.Append("{}");
                    }
                    else {
                        sb.Append("{");
                        foreach (NbtTag subTag in (NbtCompound)tag)
                        {
                            sb.Append(subTag.Name); //fine mojang. you win. dont quote your suckin names.
                                                    //btw did vs seriously just autocorrect that to suckin or am i high?
                            sb.Append(":");
                            sb.Append(Serialize(subTag));
                            sb.Append(",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("}");
                    }
                    break;
                case NbtTagType.String:
                    sb.Append('"');
                    sb.Append(tag.StringValue.Replace("\"", "\\\"").Replace("\\", "\\\\"));
                    sb.Append('"');
                    break;
                case NbtTagType.Int:
                case NbtTagType.Double:
                    sb.Append(tag.StringValue);
                    break;
                case NbtTagType.Long:
                    sb.Append(tag.StringValue);
                    sb.Append("l");
                    break;
                case NbtTagType.Byte:
                    sb.Append(tag.StringValue);
                    sb.Append("b");
                    break;
                case NbtTagType.Float:
                    sb.Append(tag.StringValue);
                    sb.Append("f");
                    break;
                default:
                    //Not supposed to happen
                    throw new NotImplementedException("Emitting a " + tag.GetType().ToString());
            }
            return sb.ToString();
        }
    }
}
