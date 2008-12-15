﻿using System;
using System.IO;

namespace RT.Util.Streams
{
    /// <summary>
    /// Provides methods to read from a stream in small chunks at a time.
    /// </summary>
    public class SlowStream : Stream
    {
        /// <summary>Gets or sets the current chunk size (number of bytes read at a time).</summary>
        public int ChunkSize { get; set; }

        private Stream _stream;

        /// <summary>Initialises a new SlowStream instance.</summary>
        /// <param name="stream">The underlying stream to read in chunks from.</param>
        public SlowStream(Stream stream) { _stream = stream; }

        /// <summary>Initialises a new SlowStream instance.</summary>
        /// <param name="stream">The underlying stream to read in chunks from.</param>
        /// <param name="chunkSize">The number of bytes to read per chunk.</param>
        public SlowStream(Stream stream, int chunkSize) { _stream = stream; ChunkSize = chunkSize; }

#pragma warning disable 1591    // Missing XML comment for publicly visible type or member

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            _stream.Close();
        }

#pragma warning restore 1591    // Missing XML comment for publicly visible type or member

        /// <summary>Reads at most <see cref="ChunkSize"/> bytes from the underlying stream.</summary>
        /// <param name="buffer">Buffer to store results into.</param>
        /// <param name="offset">Offset in buffer to store results at.</param>
        /// <param name="count">Maximum number of bytes to read.</param>
        /// <returns>Number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, Math.Min(count, ChunkSize));
        }
    }
}
