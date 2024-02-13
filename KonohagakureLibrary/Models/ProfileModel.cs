using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonohagakureLibrary.Models
{
	public class ProfileModel
	{
		public ulong MemberId { get; set; }
		public string Username { get; set; }
		public ulong ServerId { get; set; }
		public string ServerName { get; set; }
		public string AvatarURL { get; set; }
		public string ProfileImage { get; set; }
		public string InGameName { get; set; }
		public int Level { get; set; } = 0;
		public string Clan { get; set; } = "—";
		public string Organization { get; set; } = "—";
		public string OrganizationRank { get; set; } = "—";
		public int Raids { get; set; } = 0;
		public int Fame { get; set; } = 0;
		public int ProctoredMissions { get; set; } = 0;
		public string[] Masteries { get; set; } = new string[] { "—" };
		public string[] Alts { get; set; } = new string[] { "—" };
	}
}
