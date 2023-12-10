﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace VisualSortingItems.SortingAlgorithm
{
	/// <summary>
	/// Describes a sort algorithm
	/// </summary>
	public interface ISortStrategy
	{
        CancellationToken SortCancellationToken { get; set; }

        /// <summary>
        /// Reports the actual progress of the sorting 
        /// </summary>
        event Action<IList<int>> ReportProgress; 
        
        /// <summary>
        /// The name of the sort algorithm
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Sort´s the collection
        /// </summary>
        /// <param name="input">collection to be sorted</param>
        void Sort(IList<int> input);
	}
}
