using KonohagakureLibrary.Models;

namespace KonohagakureLibrary.Data
{
	public interface IDatabaseRequestData
	{
		Task<bool> StoreRPRequestAsync(RequestModel request);
		Task<bool> DeleteRequestAsync(ulong RequestId);
	}
}