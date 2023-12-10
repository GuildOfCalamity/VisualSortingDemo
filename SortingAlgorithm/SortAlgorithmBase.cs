using System;
using System.Collections.Generic;
using System.Threading;

namespace VisualSortingItems.SortingAlgorithm
{
    public abstract class SortAlgorithmBase : ISortStrategy
    {
        protected IList<int> _collection;

        /// <summary>
        /// Reports the actual progress of the sorting 
        /// </summary>
        public event Action<IList<int>> ReportProgress;

        /// <summary>
        /// The name of the sort algorithm
        /// </summary>
        public abstract string Caption { get; }

        public CancellationToken SortCancellationToken { get; set; }

        /// <summary>
        /// Sort´s the collection
        /// </summary>
        /// <param name="input">collection to be sorted</param>
        public abstract void Sort(IList<int> input);

        protected void OnReportProgress()
        {
            if (ReportProgress != null)
                ReportProgress(_collection);

        }

        protected void OnReportProgress(IList<int> listToReport)
        {
            if (ReportProgress != null)
                ReportProgress(listToReport);
        }

        /// <summary>
        /// A very common routine for sorting algorithms.
        /// </summary>
        protected void SwapIndex(int indexX, int indexY)
        {
            int tmp = _collection[indexX];
            _collection[indexX] = _collection[indexY];
            _collection[indexY] = tmp;
        }
    }
}
