using Core.Shared.ByteHandler.Interfaces;
using System.IO; // Certifique-se de incluir os namespaces necessários

namespace Core.Shared.ByteHandler
{
    public class ByteReader(MemoryStream stream) : BinaryReader(stream), IByteReader
    {
        #region Constructors

        public ByteReader(byte[] buffer)
            : this(new MemoryStream(buffer))
        {
        }

        #endregion

        #region Implementation of IByteReader

        ByteReader IByteReader.FetchByteReader() => this;

        #endregion

        #region Overrides

        public ushort ReadUShort()
        {
            return this.ReadUInt16();
        }

        public int ReadInt()
        {
            return this.ReadInt32();
        }

        #endregion
    }
}