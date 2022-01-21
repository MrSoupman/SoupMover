using SoupMover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.ViewModels
{
    public class ModFileViewModel : ViewModelBase
    {
        private readonly ModFile file;

        public string FileName => file.FileName;

        public ModFileViewModel(ModFile file)
        {
            this.file = file;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
