using System;
using System.Collections.Generic;
using SubmittedData.LiveModels;

namespace SubmittedData
{
    public interface ILiveResults : IResults
    {
        void Copy(ILiveResults results);
        IEnumerable<Group> Groups { get;  }
        DateTime Timestamp { get;  }
    }
}
