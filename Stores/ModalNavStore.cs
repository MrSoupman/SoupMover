using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
