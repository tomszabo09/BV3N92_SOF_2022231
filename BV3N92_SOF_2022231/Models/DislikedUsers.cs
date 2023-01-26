using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
	public class DislikedUser
	{
		[Key]
		public string Id { get; set; }

		public string DislikedById { get; set; }

		[NotMapped]
		public virtual SiteUser DislikedBy { get; set; }

		public string WhoDislikedId { get; set; }

		[NotMapped]
		public virtual SiteUser WhoDisliked { get; set; }

		public DislikedUser()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
