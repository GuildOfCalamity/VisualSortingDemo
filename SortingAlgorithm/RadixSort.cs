using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// Radix sort is an efficient sorting algorithm that sorts numbers by checking their digits one at a time, 
    /// starting with the least significant digit and moving to the most significant digit. Besides that, it
    /// is a non-comparative sorting algorithm, meaning that it does not compare the values of the numbers 
    /// being sorted. However, it uses counting sort as its “inner algorithm” to perform the sorting process.
    /// Since radix sort is not a comparative algorithm, we have to check the number of digits that the 
    /// largest number in the array has.
    /// </summary>
    /// <remarks>
    /// Some portions borrowed from https://code-maze.com
    /// </remarks>
    public class RadixSort : SortAlgorithmBase
    {
        public override string Caption
        {
            get => "Radix Sort";
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            var maxVal = GetMaxVal(_collection, _collection.Count);
            OnReportProgress();

            for (int exponent = 1; maxVal / exponent > 0; exponent *= 10)
            {
                CountingSort(_collection, _collection.Count, exponent);
                if (SortCancellationToken.IsCancellationRequested)
                    break;
            }

            OnReportProgress();
        }

        void CountingSort(IList<int> array, int size, int exponent)
        {
            var outputArr = new int[size];
            var occurences = new int[10];
            
            for (int i = 0; i < 10; i++)
                occurences[i] = 0;
            
            for (int i = 0; i < size; i++)
                occurences[(array[i] / exponent) % 10]++;
            
            for (int i = 1; i < 10; i++)
                occurences[i] += occurences[i - 1];
            
            for (int i = size - 1; i >= 0; i--)
            {
                outputArr[occurences[(array[i] / exponent) % 10] - 1] = array[i];
                occurences[(array[i] / exponent) % 10]--;
            }

            for (int i = 0; i < size; i++)
            {
                array[i] = outputArr[i];

                if (i % 2 == 0)
                    OnReportProgress();
            }
        }

        int GetMaxVal(IList<int> array, int size)
        {
            var maxVal = array[0];
            for (int i = 1; i < size; i++)
            {
                if (array[i] > maxVal)
                    maxVal = array[i];
            }
            return maxVal;
        }
    }
}
