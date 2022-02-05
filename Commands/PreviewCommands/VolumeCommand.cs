using LibVLCSharp.Shared;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.PreviewCommands
{
    public class VolumeCommand : BaseCommand
    {
        private readonly MediaPlayer Media;
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

        public VolumeCommand(PreviewViewModel PVM, MediaPlayer Media)
        {
            this.Media = Media;
            this.PVM = PVM;
        }
    }
}
