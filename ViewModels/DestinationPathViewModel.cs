using SoupMover.Models;
using System;
using System.Collections.Generic;

namespace SoupMover.ViewModels
{
    public class DestinationPathViewModel : ViewModelBase, IComparable
    {
        public DestinationPath DestPath { get; init; }

        public string Path => DestPath.Path;

        public override string ToString()
        {
            return Path;
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                return Path.CompareTo(obj.ToString());
            }
            throw new ArgumentException("Null Object");
        }

        public override bool Equals(object obj)
        {
            return Path.Equals(obj.ToString());
        }

        public DestinationPathViewModel(DestinationPath DestPath)
        {
            this.DestPath = DestPath;
        }

        public DestinationPath GetDestinationPath() => DestPath;

        public List<ModFile> GetFiles() => DestPath.GetFiles();

        public void AddFile(string file) => DestPath.Add(file);

        public void RemoveFile(string file) => DestPath.Remove(file);

        public void Clear() => DestPath.Clear();

    }
}
