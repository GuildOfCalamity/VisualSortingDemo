using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// As the name suggests, insertion sort is an algorithm that sorts a list of elements by taking each element and 
	/// adding it to the correct position in the list. The algorithm iterates through the list until the array is sorted. 
    /// </summary>
    public class InsertionSort : SortAlgorithmBase
	{
		public override string Caption
		{
			get => "Insertion Sort";
		}

		public override void Sort(IList<int> input)
		{
			_collection = new List<int>(input);
			OnReportProgress();

			for (int i = 1; i < _collection.Count; i++)
			{
				int index = _collection[i];
				int j = i;

				while (j > 0 && _collection[j - 1].CompareTo(index) > 0)
				{
					_collection[j] = _collection[j - 1];
					j--;

                    if (SortCancellationToken.IsCancellationRequested)
                        break;
                }
                
				_collection[j] = index;
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
