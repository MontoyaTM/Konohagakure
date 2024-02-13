
using KonohagakureLibrary.Models;

namespace KonohagakureLibrary.Data
{
	public interface IDatabaseHokageData
	{
		Task<bool> DeleteApplication(ulong MemberId);
		Task<(bool, string[])> RetrieveAlts(ulong MemberId);
	}
}