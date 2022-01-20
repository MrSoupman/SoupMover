using SoupMover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.ViewModels
{
    public class DestinationPathViewModel : IComparable
    {
        private readonly DestinationPath DestPath;

        public string Path => DestPath.Path;
        public List<ModFile> modFiles => DestPath.GetFiles();

        public override string ToString()
        {
            return Path;
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                return DestPath.Path.CompareTo((obj as DestinationPath).Path);
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

    }
}
