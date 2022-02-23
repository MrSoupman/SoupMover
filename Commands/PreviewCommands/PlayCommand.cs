using LibVLCSharp.Shared;

namespace SoupMover.Commands.PreviewCommands
{
    public class PlayCommand : BaseCommand
    {
        private readonly MediaPlayer Media;
        public override void Execute(object parameter)
        {
            Media.Play();
        }

        public PlayCommand(MediaPlayer Media)
        {
            this.Media = Media;
        }
    }
}
