using System;
using System.IO;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    /// <summary>
    /// Provides methods for writing to a stream and reading from a stream.
    /// </summary>
    public interface IFtpClientStreamHandler : IDisposable
    {
        /// <summary>
        /// Asynchronously writes a string to the stream, followed by a line terminator.
        /// </summary>
        /// <param name="input">The string to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteLineAsync(string input);

        /// <summary>
        /// Reads a line of characters asynchronously from the current stream and returns the data as a string.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the next line from the stream,
        /// or is null if all the characters have been read.</returns>
        public Task<string?> ReadLineAsync();

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the total number of bytes read into the buffer.
        /// The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number,
        /// or it can be 0 (zero) if the end of the stream has been reached.</returns>
        public Task<int> ReadAsync(byte[] buffer, int offset, int count);
        
        /// <summary>
        /// Asynchronously reads specified amount of bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="destination">The stream to which the contents of the current stream will be written.</param>
        /// <param name="count">Specified amount of bytes.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        public Task CopyToAsync(Stream destination, long count);
    }
}