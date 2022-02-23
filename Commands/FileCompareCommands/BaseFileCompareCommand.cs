using SoupMover.Services;
using SoupMover.Stores;

namespace SoupMover.Commands.FileCompareCommands
{
    public abstract class BaseFileCompareCommand : BaseCommand
    {
        public ModalNavSvc Modal { get; init; }
        public DialogStore Store { get; init; }

        public void CloseModal()
        {
            Modal.GetModalNavStore().Close();
        }


    }
}
