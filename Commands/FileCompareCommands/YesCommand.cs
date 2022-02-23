using SoupMover.Services;
using SoupMover.Stores;

namespace SoupMover.Commands.FileCompareCommands
{
    public class YesCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(0);
            CloseModal();
        }

        public YesCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
