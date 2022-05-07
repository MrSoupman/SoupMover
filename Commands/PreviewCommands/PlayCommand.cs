using LibVLCSharp.Shared;
using SoupMover.ViewModels;

namespace SoupMover.Commands.PreviewCommands
{
    public class PlayCommand : BaseCommand
    {
        private MediaPlayer Media;
        private readonly PreviewViewModel PVM;
        public override void Execute(object parameter)
        {
            Media.Play();
        }

        public PlayCommand(PreviewViewModel PVM)
        {
            this.PVM = PVM;
            Media = this.PVM.GetMedia();
            this.PVM.PropertyChanged += PVM_PropertyChanged;
        }

        private void PVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Media))
                Media = PVM.GetMedia();
        }
    }
}
