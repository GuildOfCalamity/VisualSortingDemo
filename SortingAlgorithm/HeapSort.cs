using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// The heap sort algorithm is a sorting technique that belongs to the family of comparison-based sorting algorithms. 
    /// It uses a data structure called a heap, which is essentially a binary tree with some special properties. 
    /// The heap sort algorithm has two phases:
    ///   1) The heap phase: In this phase, we transform the input array into a max heap – a binary tree in which the value of
    ///      each node is greater than or equal to the value of its children nodes.This can be done by starting at the last 
    ///      non-leaf node in the tree and working backward towards the root, ensuring that each node satisfies the max heap property.
    ///   2) The sort phase: In this phase, the max heap is repeatedly removed until only one element remains. This is done by 
    ///      swapping the root node with the last element in the heap, and then ensuring that the new root node satisfies the 
    ///      max heap property.This process is repeated until only one element remains in the heap.
    /// </summary>
    /// <remarks>
    /// Some portions borrowed from https://code-maze.com
    /// </remarks>
    public class HeapSort : SortAlgorithmBase
    {
        public override string Caption
        {
            get => "Heap Sort";
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int i = _collection.Count / 2 - 1; i >= 0; i--)
            {
                HeapIt(_collection, _collection.Count, i);

                OnReportProgress();
                if (SortCancellationToken.IsCancellationRequested)
                    break;
            }

            for (int i = _collection.Count - 1; i >= 0; i--)
            {
                var tempVar = _collection[0];
                _collection[0] = _collection[i];
                _collection[i] = tempVar;
                HeapIt(_collection, i, 0);

                OnReportProgress();
                if (SortCancellationToken.IsCancellationRequested)
                    break;
            }
        }

        void HeapIt(IList<int> array, int size, int index)
        {
            var largestIndex = index;
            var leftChild = 2 * index + 1;
            var rightChild = 2 * index + 2;
            if (leftChild < size && array[leftChild] > array[largestIndex])
            {
                largestIndex = leftChild;
            }
            if (rightChild < size && array[rightChild] > array[largestIndex])
            {
                largestIndex = rightChild;
            }
            if (largestIndex != index)
            {
                var tempVar = array[index];
                array[index] = array[largestIndex];
                array[largestIndex] = tempVar;
                HeapIt(array, size, largestIndex);
            }
        }
    }
}
