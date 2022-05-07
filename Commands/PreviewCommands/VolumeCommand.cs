using LibVLCSharp.Shared;
using SoupMover.ViewModels;

namespace SoupMover.Commands.PreviewCommands
{
    public class VolumeCommand : BaseCommand
    {
        private MediaPlayer Media;
        private readonly PreviewViewModel PVM;
        public override void Execute(object parameter)
        {
            if (Media.Volume == 0)
            {
                Media.Volume = 100;
                PVM.VolumeImage = @"../Images/volume-up.png";
            }
            else
            {
                Media.Volume = 0;
                PVM.VolumeImage = @"../Images/volume-off.png";
            }
        }

        public VolumeCommand(PreviewViewModel PVM)
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
