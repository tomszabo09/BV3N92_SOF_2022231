using Microsoft.Build.Framework;

namespace Backend.Models
{
	public class MessageModel
	{
		public string Id { get; set; }

		public string UserId { get; set; }

		public virtual SiteUser SiteUser { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Text { get; set; }

		public DateTime When { get; set; }
	}
}
