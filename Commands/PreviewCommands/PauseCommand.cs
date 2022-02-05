using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.PreviewCommands
{
    public class PauseCommand : BaseCommand
    {
        private readonly MediaPlayer Media;
        public override void Execute(object parameter)
        {
            Media.Pause();
        }
        public PauseCommand(MediaPlayer Media)
        {
            this.Media = Media;
        }
    }
}
