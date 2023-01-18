using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
	public class Picture
	{
		[Key]
		public string PictureId { get; set; }

		[Required]
		public string PhotoUrl { get; set; }

		[ForeignKey(nameof(User))]
		public int UserId { get; set; }

		[NotMapped]
		public virtual SiteUser User { get; set; }

		public Picture()
		{
			PictureId = Guid.NewGuid().ToString();
		}
	}
}
