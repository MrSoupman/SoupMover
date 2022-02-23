using System;
using System.Collections.Generic;

namespace SoupMover.Models
{
    /// <summary>
    /// DestinationPath holds information regarding the path to move files to, and the current path to the files themselves
    /// </summary>
    public class DestinationPath : IComparable
    {
        public string Path { get; init; }
        public List<ModFile> Files { get; init; }

        public int CompareTo(object obj)
        {
            DestinationPath temp = obj as DestinationPath;
            if (temp != null && temp is DestinationPath)
            {
                return Path.CompareTo(temp.Path);
            }
            else
                throw new ArgumentException("Invalid object passed");
        }

        /// <summary>
        /// Checks if the object being passed in is the same directory as what this current object points to
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the two point to the same directory, false otherwise</returns>
        public override bool Equals(object obj) //TODO: At some inexplicable point, this method is called and a null object is passed in. I don't know why or how.
        {
            var temp = obj as DestinationPath;
            if (temp != null)
                return temp.ToString() == this.ToString();
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public DestinationPath(string Path)
        {
            this.Path = Path;
            Files = new List<ModFile>();
        }

        /// <summary>
        /// Adds a file to the directory, if it doesn't already exist. 
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if file was successfully added to the queue, false if it is already included</returns>
        public bool Add(string file)
        {
            ModFile modFile = new ModFile(file);
            if (!Files.Contains(modFile))
            {
                Files.Add(modFile);
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
            ModFile modFile = new ModFile(file);
            if (Files.Contains(modFile))
            {
                Files.Remove(modFile);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Removes a file from the directory given an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// Returns the FilesToMove object as a string, or rather, its directory.
        /// </summary>
        /// <returns>The directory that this object points to</returns>
        public override string ToString()
        {
            return Path;
        }

        /// <summary>
        /// Gets the index of the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int IndexOf(string file)
        {
            return Files.IndexOf(new ModFile(file));
        }

        /// <summary>
        /// Checks if a file exists in this file db
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if the file exists, false otherwise</returns>
        public bool Contains(string file)
        {
            return Files.Contains(new ModFile(file));
        }

        /// <summary>
        /// Looks for a file, and changes its name, if it exists in the list.
        /// </summary>
        /// <param name="FileToFind"></param>
        /// <param name="NewName"></param>
        /// <returns>True if the file exists and was sucessfully updated, false otherwise.</returns>
        public bool UpdateFileName(string FileToFind, string NewName)
        {
            ModFile modFile = new ModFile(FileToFind);
            int index = Files.IndexOf(modFile);
            if (index > -1)
            {
                Files[index].NewName = NewName;
                return true;
            }
            return false;
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
        /// Returns a List of all files queued to this directory.
        /// </summary>
        /// <returns>List containing all files queued to this directory.</returns>
        public List<ModFile> GetFiles()
        {
            return Files;
        }
        public ModFile GetFile(int index)
        {
            if (index > Files.Count - 1)
                throw new IndexOutOfRangeException();
            return Files[index];
        }
    }
}
