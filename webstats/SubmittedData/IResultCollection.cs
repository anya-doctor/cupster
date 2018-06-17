using System;
namespace SubmittedData
{
	public interface IResultCollection
	{
		IResults Current { get; set; }
		IResults Previous { get; set; }
	}
}

