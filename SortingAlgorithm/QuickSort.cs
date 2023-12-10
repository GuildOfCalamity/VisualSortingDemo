using System.Collections.Generic;
using System.Linq;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// Just like merge sort, quicksort uses the “divide and conquer” strategy to sort elements in arrays or lists. 
	/// It implements this strategy by choosing an element as a pivot and using it to partition the array.
    /// </summary>
    public class QuickSort : SortAlgorithmBase
	{

		public override string Caption
		{
			get => "Quick Sort";
		}

		public override void Sort(IList<int> input)
		{
			_collection = new List<int>(input);
			OnReportProgress();
			
			if(_collection.Count > 1)
				QuickSortCore(_collection.Min(), _collection.Max());
		}

		private void QuickSortCore(int left, int right)
		{
            if (left.CompareTo(right) < 0)
			{
				int part = Separate(left, right);
				QuickSortCore(left, part - 1);
				QuickSortCore(part + 1, right);
            }
        }

		private int Separate(int left, int right)
		{
			int i = left;
			int j = right - 1;
			int pivot = _collection[right];

		    do
		    {
                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }

				while (_collection[i].CompareTo(pivot) <= 0 && i.CompareTo(right) < 0)
				{
					i++;
                    
					if (SortCancellationToken.IsCancellationRequested)
                        break;
                }

				while (_collection[j].CompareTo(pivot) > 0 && j.CompareTo(left) > 0)
				{
					j--;
                
					if (SortCancellationToken.IsCancellationRequested)
                        break;
                }

                if (i.CompareTo(j) < 0)
                {
                	SwapIndex(i, j);
                    OnReportProgress();
                }
			

			} while (i.CompareTo(j) < 0);

			SwapIndex(i, right);
            OnReportProgress();

			return i;
		}
	}
}
