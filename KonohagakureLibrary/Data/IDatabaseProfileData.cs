using KonohagakureLibrary.Models;

namespace KonohagakureLibrary.Data
{
	public interface IDatabaseProfileData
	{
		Task<bool> isEmptyAsync(ulong ServerId);
		Task<bool> StoreVillagerApplicationAsync(ProfileModel profile);
		Task<bool> UserExistsAsync(ulong MemberId);
		Task<(bool, List<ProfileModel>)> RetrieveProfileAsync(ulong MemberId);
		Task<bool> UpdateProfileImageAsync(ulong MemberId, string ImageURL);
		Task<(bool, List<string>)> GetProfileImageAsync(ulong MemberId);
		Task<bool> UpdateFameAsync(ulong MemberID);
		Task<bool> UpdateClanAsync(ulong MemberID, string Clan);
		Task<bool> UpdateMasteriesAsync(ulong MemberID, string[] Masteries);
		Task<bool> UpdateOrganizationAsync(ulong MemberID, string Organization, string Rank);
		Task<bool> UpdateLevelAsync(ulong MemberID, int Level);
		Task<bool> UpdateIngameName(ulong MemberID, string IngameName);
		Task<bool> AddAltAsync(ulong MemberID, string Alt);
		Task<bool> UpdateRaidAsync(ulong MemberID);
		Task<(bool, List<string>)> GetRaidMasteries(List<ulong> MemberIDs);
	}
}