using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Models
{
    /// <summary>
    /// Mod(ified)File contains information about the file itself; whether its name needs to be changed or if it should/shouldn't overwrite an existing file
    /// </summary>
    public class ModFile : IComparable
    {
        public readonly string FileName; //The original file name that currently exists on disk

        public string NewName { get; set; } //The name it should be renamed to upon being moved

        public bool OverwriteExist { get; set; } //Whether this entry is marked to be overwritten

        public ModFile(string FileName)
        {
            this.FileName = FileName;
        }

        public int CompareTo(object obj)
        {
            if (obj is ModFile)
            {
                return (obj as ModFile).FileName.CompareTo(FileName);
            }
            throw new ArgumentException("Invalid object passed");
        }

        public override bool Equals(object obj)
        {
            return FileName.Equals(obj.ToString());
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
