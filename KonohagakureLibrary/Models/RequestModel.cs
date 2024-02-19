using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonohagakureLibrary.Models
{
	public class RequestModel
	{
		public ulong RequestId { get; set; }
		public ulong MemberId { get; set; }
		public string Username { get; set; }
		public ulong ServerId { get; set; }
		public string ServerName { get; set; }
		public string InGameName { get; set; }
		public string Mission {  get; set; }
		public string Attendees { get; set; }
		public string Timezone { get; set; }
	}
}
