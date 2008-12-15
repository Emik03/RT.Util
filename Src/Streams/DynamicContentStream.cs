﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RT.Util.ExtensionMethods;

namespace RT.Util.Streams
{
    /// <summary>
    /// Provides a read-only stream that can "read from" an <c>IEnumerable&lt;string&gt;</c>.
    /// In particular, an intended application is to "read from" a method that uses <c>yield return</c> to return strings as execution proceeds.
    /// This enables generation of, for example, HTML for dynamic web pages.
    /// </summary>
    public class DynamicContentStream : Stream
    {
        private IEnumerator<string> _enumerator = null;
        private byte[] _lastUnprocessedBytes = null;
        private int _lastUnprocessedBytesIndex = 0;

        /// <summary>
        /// Instantiates a buffered <see cref="DynamicContentStream"/>.
        /// </summary>
        /// <param name="enumerable">The object that provides the content for this stream to read from.</param>
        public DynamicContentStream(IEnumerable<string> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
            Buffered = true;
        }

        /// <summary>
        /// Instantiates a <see cref="DynamicContentStream"/> and lets you configure whether it's buffered or not.
        /// </summary>
        /// <param name="enumerable">The object that provides the content for this stream to read from.</param>
        /// <param name="buffered">Provides an initial value for the <see cref="Buffered"/> property.</param>
        public DynamicContentStream(IEnumerable<string> enumerable, bool buffered)
        {
            _enumerator = enumerable.GetEnumerator();
            Buffered = buffered;
        }

        /// <summary>
        /// If true, each call to <see cref="Read"/> will move the enumerator forward as far as necessary to fill the buffer.
        /// If false, each call to <see cref="Read"/> returns only the text produced by a single MoveNext() of the enumerator.
        /// </summary>
        public bool Buffered { get; set; }

        /// <summary>
        /// Reads some text into the specified buffer. The behaviour depends on the <see cref="Buffered"/> property.
        /// The bytes returned respresent the text returned by the underlying enumerator, UTF-8-encoded.
        /// Although DynamicContentStream makes no effort to keep multi-byte characters within a single invocation
        /// of Read(), all output will be valid UTF-8 when concatenated.
        /// </summary>
        /// <param name="buffer">The buffer to copy the data into.</param>
        /// <param name="offset">The offset at which to start copying into buffer.</param>
        /// <param name="count">The maximum number of bytes to copy. The stream may return less than this.</param>
        /// <returns>The number of bytes actually copied into the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_lastUnprocessedBytes != null && _lastUnprocessedBytes.Length > 0)
            {
                if (_lastUnprocessedBytes.Length - _lastUnprocessedBytesIndex > count)
                {
                    Array.Copy(_lastUnprocessedBytes, _lastUnprocessedBytesIndex, buffer, offset, count);
                    _lastUnprocessedBytesIndex += count;
                    return count;
                }
                else
                {
                    int howMany = _lastUnprocessedBytes.Length - _lastUnprocessedBytesIndex;
                    Array.Copy(_lastUnprocessedBytes, _lastUnprocessedBytesIndex, buffer, offset, howMany);
                    _lastUnprocessedBytes = null;
                    _lastUnprocessedBytesIndex = 0;
                    return howMany;
                }
            }

            if (Buffered)
            {
                StringBuilder b = new StringBuilder();
                long bytesSoFar = 0;
                while (bytesSoFar < count)
                {
                    if (!_enumerator.MoveNext())
                        break;
                    b.Append(_enumerator.Current);
                    bytesSoFar += _enumerator.Current.Utf8Length();
                }
                if (b.Length == 0)
                    return 0;

                byte[] bigBuffer = b.ToString().ToUtf8();
                if (bigBuffer.Length > count)
                {
                    Array.Copy(bigBuffer, 0, buffer, offset, count);
                    _lastUnprocessedBytes = bigBuffer;
                    _lastUnprocessedBytesIndex = count;
                    return count;
                }
                else
                {
                    Array.Copy(bigBuffer, 0, buffer, offset, bigBuffer.Length);
                    _lastUnprocessedBytes = null;
                    return bigBuffer.Length;
                }
            }
            else
            {
                do
                {
                    if (!_enumerator.MoveNext())
                        return 0;
                } while (_enumerator.Current.Length == 0);
                byte[] encoded = _enumerator.Current.ToUtf8();
                if (encoded.Length > count)
                {
                    Array.Copy(encoded, 0, buffer, 0, count);
                    _lastUnprocessedBytes = encoded;
                    _lastUnprocessedBytesIndex = count;
                    return count;
                }
                else
                {
                    Array.Copy(encoded, 0, buffer, 0, encoded.Length);
                    return encoded.Length;
                }
            }
        }

#pragma warning disable 1591    // Missing XML comment for publicly visible type or member
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override void Flush() { }

        // Things you can't do
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
#pragma warning restore 1591    // Missing XML comment for publicly visible type or member
    }
}
