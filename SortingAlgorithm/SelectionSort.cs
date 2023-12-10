using System;
using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// The goal of the selection sort algorithm is to find the minimum value of an array by iterating through each 
    /// element. For each iteration, the algorithm compares the current minimum value of the array with the current 
    /// element. If the current value is smaller than the minimum value, a swap process occurs. This process continues 
    /// until the array is completely sorted.
    /// </summary>
    public class SelectionSort : SortAlgorithmBase
    {
		public override string Caption
        {
            get => "Selection Sort";
        }

		public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int i = 0; i < _collection.Count -1; i++)
            {
                if (SortCancellationToken.IsCancellationRequested)
                    break;

                int minimum = i;
                for(int j = i + 1; j < _collection.Count; j++)
                {
                    if (_collection[j].CompareTo(_collection[minimum])  < 0)
                        minimum = j;

                    if (SortCancellationToken.IsCancellationRequested)
                    {
                        //SortCancellationToken.ThrowIfCancellationRequested();
                        break;
                    }
                }

                SwapIndex(minimum, i);
                OnReportProgress();
            }
        }
    }
}
