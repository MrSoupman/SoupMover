using SoupMover.Services;
using SoupMover.Stores;

namespace SoupMover.Commands.FileCompareCommands
{
    public class NoCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(2);
            CloseModal();
        }

        public NoCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
