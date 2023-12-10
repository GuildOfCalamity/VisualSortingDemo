using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// Compares two positions at a time and then orders them by weight, swapping or 
    /// leaving as is. After one pass is complete the entire process is started over.
    /// </summary>
    public class BubbleSort : SortAlgorithmBase
    {
		public override string Caption
        {
            get => "Bubble Sort";
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int i = _collection.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    if (_collection[j - 1].CompareTo(_collection[j]) > 0)
                        SwapIndex(j - 1, j);
                    if (SortCancellationToken.IsCancellationRequested)
                    {
                        //SortCancellationToken.ThrowIfCancellationRequested();
                        break;
                    }
                }
                OnReportProgress();

                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }
            }
        }
    }
}
