using Core.Shared.ByteHandler.Interfaces;

namespace Core.Shared.ByteHandler
{
    public sealed class ByteWriter : BinaryWriter, IDisposable, IByteWriter
    {
        #region Fields

        private readonly MemoryStream _memoryStream;
        private bool _disposed;

        #endregion

        #region Constructors

        public ByteWriter()
            : base(new MemoryStream())
        {
            _memoryStream = (MemoryStream)BaseStream;
        }

        #endregion

        #region Implementation of IByteWriter

        ByteWriter IByteWriter.FetchByteWriter() => this;

        #endregion

        #region Methods

        public int GetLength()
        {
            return (int)_memoryStream.Length;
        }

        public byte[] GetBuffer()
        {
            return _memoryStream.ToArray(); // Use ToArray() instead of GetBuffer() for safety
        }

        #endregion

        #region IDisposable Support

        public new void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _memoryStream.Dispose();
            }
            _disposed = true;
        }

        public override async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            await _memoryStream.DisposeAsync();
            _disposed = true;
        }

        #endregion
    }
}