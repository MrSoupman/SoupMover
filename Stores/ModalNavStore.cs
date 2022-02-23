namespace SoupMover.Stores
{
    public class ModalNavStore : NavStore
    {
        public bool IsOpen => CurrentVM != null;

        public void Close()
        {
            CurrentVM = null;
        }
    }
}
