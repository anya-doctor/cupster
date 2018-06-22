using System;
namespace SubmittedData
{
	public class ResultCollection : IResultCollection
	{

		public IResults Current { get; set; }

		public IResults Previous { get; set; }

        public IResults Backup { get; set; }
    }
}

