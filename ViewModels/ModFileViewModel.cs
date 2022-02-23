using SoupMover.Models;

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
