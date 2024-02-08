using KonohagakureLibrary.Models;

namespace KonohagakureLibrary.Data
{
	public interface IDatabaseProfileData
	{
		Task<bool> isEmptyAsync(ulong ServerId);
		string QueryableConversion(string profileData);
		Task<bool> StoreVillagerApplicationAsync(ProfileModel profile);
		Task<bool> UserExistsAsync(ulong MemberId);
	}
}