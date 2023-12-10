using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// Bucket/Bin sort is a sorting algorithm that works by partitioning an array into several buckets. The bucket 
    /// sort algorithm then sorts the contents of each bucket recursively or by applying different sorting algorithms. 
    /// The implementation of the bucket sort algorithm requires the creation of buckets to hold the array elements
    /// during the sorting process. Therefore, the space complexity of bucket sort is O(n+k), with n being the 
    /// number of elements in the array and k being the number of buckets. 
    /// </summary>
    /// <remarks>
    /// Some portions borrowed from https://code-maze.com
    /// </remarks>
    public class BucketSort : SortAlgorithmBase
    {
        public override string Caption
        {
            get => "Bucket Sort";
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            int maxValue = _collection[0];
            int minValue = _collection[0];
            for (int i = 1; i < _collection.Count; i++)
            {
                if (_collection[i] > maxValue)
                {
                    maxValue = _collection[i];
                }
                if (_collection[i] < minValue)
                {
                    minValue = _collection[i];
                }
                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }
            }
            OnReportProgress();

            LinkedList<int>[] bucket = new LinkedList<int>[maxValue - minValue + 1];
            for (int i = 0; i < _collection.Count; i++)
            {
                if (bucket[_collection[i] - minValue] == null)
                {
                    bucket[_collection[i] - minValue] = new LinkedList<int>();
                }
                bucket[_collection[i] - minValue].AddLast(_collection[i]);
                
                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }
            }
            OnReportProgress();

            var index = 0;
            for (int i = 0; i < bucket.Length; i++)
            {
                if (bucket[i] != null)
                {
                    LinkedListNode<int> node = bucket[i].First;
                    while (node != null)
                    {
                        _collection[index] = node.Value;
                        node = node.Next;
                        index++;
                    }
                }
                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }
                OnReportProgress();
            }
        }
    }
}
