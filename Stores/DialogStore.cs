using System;

namespace SoupMover.Stores
{
    public class DialogStore
    {
        public event Action<int> FileCompareResult;
        public void GetResult(int Result)
        {
            FileCompareResult?.Invoke(Result);
        }


        public string Title { get; set; }
        public string Message { get; set; }
    }
}
