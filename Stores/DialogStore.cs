using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Stores
{
    public class DialogStore
    {
        public event Action<int> FileCompareResult;
        public void GetResult(int Result)
        {
            FileCompareResult?.Invoke(Result);
        }
    }
}
