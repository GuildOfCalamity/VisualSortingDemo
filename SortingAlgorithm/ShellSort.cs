using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSortingItems.SortingAlgorithm
{
    /// <summary>
    /// This algorithm was created by Donald Shell in 1959. It works based on the premise that sorting further elements 
    /// first effectively reduces the interval between the elements that still need to undergo the sorting process. This 
    /// makes the shell sort algorithm work in the same way as a generalized version of the insertion sort algorithm. The 
    /// algorithm sorts elements at a specific interval first and reduces that interval gradually until it is equal to one.
    /// </summary>
    /// <remarks>
    /// Some portions borrowed from https://code-maze.com
    /// </remarks>
    public class ShellSort : SortAlgorithmBase
    {
        public override string Caption
        {
            get => "Shell Sort";
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int interval = _collection.Count / 2; interval > 0; interval /= 2)
            {
                for (int i = interval; i < _collection.Count; i++)
                {
                    var currentKey = _collection[i];
                    var k = i;
                    while (k >= interval && _collection[k - interval] > currentKey)
                    {
                        _collection[k] = _collection[k - interval];
                        k -= interval;
                    }
                    _collection[k] = currentKey;
                    
                    if (i % 10 == 0)
                        OnReportProgress();
                }


                if (SortCancellationToken.IsCancellationRequested)
                {
                    //SortCancellationToken.ThrowIfCancellationRequested();
                    break;
                }
            }

            OnReportProgress();
        }
    }
}
