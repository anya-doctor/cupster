using System;
using System.Collections.Generic;
using SubmittedData.LiveModels;

namespace SubmittedData
{
    public interface ILiveResults : IResults
    {
        void Copy(ILiveResults results);
        IEnumerable<Group> Groups { get;  }

        Match Final { get;  }
        Match ThirdPlacePlayoff { get;  }
        IEnumerable<Match> SemiFinals { get;  }
        IEnumerable<Match> Last8 { get;  }
        IEnumerable<Match> Last16 { get;  }
        DateTime Timestamp { get;  }
    }
}
