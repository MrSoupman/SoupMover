using System;
using System.Collections.Generic;
using System.Text;

namespace SoupMover
{
    /// <summary>
    /// A class used to store data about a directory and the files that are queued to be moved to it.
    /// </summary>
    internal class FilesToMove : IComparable
    {
        private string Directory;
        private List<string> Files;
        public FilesToMove() 
        {
            Directory = "";
        }
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

        public bool RemoveAt(int index)
        {
            if (index < Files.Count)
            { 
                Files.RemoveAt(index);
                return true;
            }
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
        /// <summary>
        /// Returns the FilesToMove object as a string, or rather, its directory.
        /// </summary>
        /// <returns>The directory that this object points to</returns>
        override public string ToString()
        {
            return GetDirectory();
        }
        /// <summary>
        /// Checks if the object being passed in is the same directory as what this current object points to
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the two point to the same directory, false otherwise</returns>
        public override bool Equals(object obj) //TODO: At some inexplicable point, this method is called and a null object is passed in. I don't know why or how.
        {
            FilesToMove temp = obj as FilesToMove;
            if (temp != null)
                return temp.ToString() == this.ToString();
            return false;
        }

        public int CompareTo(object obj)
        {
            FilesToMove temp = obj as FilesToMove;
            if (temp != null)
            {
                return this.Directory.CompareTo(temp.GetDirectory());
            }
            else
                throw new ArgumentException("Invalid object passed");
        }

        /// <summary>
        /// Returns a List of all files queued to this directory.
        /// </summary>
        /// <returns>List containing all files queued to this directory.</returns>
        public List<string> GetFiles()
        {
            return Files;
        }
        public string GetFile(int index)
        {
            if (index > Files.Count - 1)
                throw new IndexOutOfRangeException();
            return Files[index];
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
        /// <summary>
        /// Looks for a file, and changes its name, if it exists in the list.
        /// </summary>
        /// <param name="FileToFind"></param>
        /// <param name="NewName"></param>
        /// <returns>True if the file exists and was sucessfully updated, false otherwise.</returns>
        public bool UpdateFileName(string FileToFind, string NewName)
        {
            int index = Files.IndexOf(FileToFind);
            if (index > -1)
            { 
                Files[index] = NewName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a file exists in this file db
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if the file exists, false otherwise</returns>
        public bool Contains(string file)
        {
            return Files.Contains(file);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the index of the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int IndexOf(string file)
        {
            return Files.IndexOf(file);
        }
    }
}
