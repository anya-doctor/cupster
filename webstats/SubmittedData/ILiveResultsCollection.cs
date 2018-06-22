using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmittedData
{
    public interface ILiveResultsCollection : IResultCollection
    {
        new ILiveResults Current { get; set; }

        new ILiveResults Previous { get; set; }

        Task UpdateResultsAsync(TimeSpan interval);
    }

}
