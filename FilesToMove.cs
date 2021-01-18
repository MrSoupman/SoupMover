using System;
using System.Collections.Generic;
using System.Text;

namespace SoupMover
{
    /// <summary>
    /// A class used to store data about a directory and the files that are queued to be moved to it.
    /// </summary>
    class FilesToMove
    {
        string Directory;
        List<string> Files;
        public FilesToMove() { }
        /// <summary>
        /// Stores a directory and the files to be moved to it.
        /// </summary>
        /// <param name="Directory"></param>
        public FilesToMove(string Directory) {
            this.Directory = Directory;
            Files = new List<string>();
        }
        /// <summary>
        /// Adds a file to the directory, if it doesn't already exist.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if file was successfully added to the queue, false if it is already included</returns>
        public bool Add(string file)
        {
            if (!Files.Contains(file))
            {
                Files.Add(file);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Removes a file from the directory, if it exists.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if file was successfully removed from the queue, false if it doesn't exist</returns>
        public bool Remove(string file)
        {
            if (Files.Contains(file))
            {
                Files.Remove(file);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks if the current directory has no files to be moved to it.
        /// </summary>
        /// <returns>true if no files are queued to be moved to it, false otherwise.</returns>
        public bool IsEmpty()
        {
            if (Files.Count == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the directory
        /// </summary>
        /// <returns>string Directory</returns>
        public string GetDirectory()
        {
            return Directory;
        }
        /// <summary>
        /// Sets a directory to the inputted string.
        /// </summary>
        /// <param name="Directory"></param>
        public void SetDirectory(string Directory)
        {
            this.Directory = Directory;
        }
        override public string ToString()
        {
            return GetDirectory();
        }
        /// <summary>
        /// Returns a List of all files queued to this directory.
        /// </summary>
        /// <returns>List containing all files queued to this directory.</returns>
        public List<string> GetFiles()
        {
            return Files;
        }
        /// <summary>
        /// Returns the amount of files that are queued to this directory.
        /// </summary>
        /// <returns>An integer amount of files queued to this directory.</returns>
        public int Count()
        {
            return Files.Count;
        }
        /// <summary>
        /// Removes all files from this directory's queue
        /// </summary>
        public void Clear()
        {
            Files.Clear();
        }
    }
}
