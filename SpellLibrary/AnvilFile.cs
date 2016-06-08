using fNbt;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace SpellLibrary
{
    /// <summary>
    /// Encapsulates an Anvil file and allows loading of individual chunks from it, handling compressed NBT.
    /// </summary>
    public class AnvilFile
    {
        private const int NumberOfChunks = 32 * 32;
        private const int SizeOfHeader = 8 * NumberOfChunks;
        private const int SizeOfChunkPiece = 4 * 1024;
        string FileName;
        Stream file;
        uint[] chunkOffsets = new uint[NumberOfChunks]; //Chunk offsets, in 4KiB pieces
        byte[] chunkSizes = new byte[NumberOfChunks]; //Chunk sizes, in 4KiB pieces
        public AnvilFile(string filename)
        {
            FileName = filename;
            file = File.OpenRead(filename);
            ReadHeader();
        }

        private void ReadHeader()
        {
            byte[] buf = new byte[SizeOfHeader];
            file.Seek(0, SeekOrigin.Begin);
            file.Read(buf, 0, SizeOfHeader);

            for (int i = 0; i < NumberOfChunks; i++)
            {
                int headerPos = i * 4; //Size of header entry
                uint offset = (uint)buf[headerPos] * 256 * 256 + (uint)buf[headerPos + 1] * 256 + (uint)buf[headerPos + 2]; //before you say something, left-shifting wasn't working
                byte size = buf[headerPos + 3];
                chunkOffsets[i] = offset;
                chunkSizes[i] = size;
            }
        }

        public NbtTag GetChunkData(int chunk)
        {
            if (chunkSizes[chunk] == 0) //Chunk is not generated
            {
                return null;
            }

            file.Seek(chunkOffsets[chunk] * SizeOfChunkPiece, SeekOrigin.Begin);

            byte[] sizeAndCompression = new byte[5];
            file.Read(sizeAndCompression, 0, 5); // First 4 are exact chunk size, last is compression scheme (1 = GZip, not used; 2 = ZLib). BIG FUCKING ENDIAN.
            uint size = (uint)sizeAndCompression[0] * 256 * 256 * 256 + (uint)sizeAndCompression[1] * 256 * 256 + (uint)sizeAndCompression[2] * 256 + (uint)sizeAndCompression[3];
            byte compression = sizeAndCompression[4];

            if (compression == 1)
            {
                throw new NotImplementedException("GZip Not Supported");
            }
            byte[] compressed = new byte[size];
            file.Read(compressed, 0, (int)size);
            Stream cStream = new MemoryStream(compressed, false);
            Stream rawStream = new InflaterInputStream(cStream);
            NbtTag nbt = new NbtReader(rawStream).ReadAsTag();
            return nbt;
        }
    }
}
