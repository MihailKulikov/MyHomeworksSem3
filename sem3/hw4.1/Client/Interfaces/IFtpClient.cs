using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    /// <summary>
    /// Provides client connections for SimpleFTP network services.
    /// </summary>
    public interface IFtpClient : IDisposable
    {
        /// <summary>
        /// Makes file listing in the directory with specified path on the server.
        /// </summary>
        /// <param name="path">Path to the directory relative to where the server is running.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the sequence of file's names and flags
        /// indicating whether the file is a directory.</returns>
        public Task<IEnumerable<(string name, bool isDirectory)>> ListAsync(string path);

        /// <summary>
        /// Downloads file from server.
        /// </summary>
        /// <param name="path">Path to the file relative to where the server is running.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains path to the downloaded file relative to where the client is running.</returns>
        public Task<string> GetAsync(string path);
    }
}