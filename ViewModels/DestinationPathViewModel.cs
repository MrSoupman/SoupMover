using SoupMover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.ViewModels
{
    public class DestinationPathViewModel
    {
        private readonly DestinationPath DestPath;

        public string Path => DestPath.Path;
        public List<ModFile> modFiles => DestPath.GetFiles();

        public DestinationPathViewModel(DestinationPath DestPath)
        {
            this.DestPath = DestPath;
        }

    }
}
