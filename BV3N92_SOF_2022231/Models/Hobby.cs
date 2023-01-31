using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
	public class Hobby
	{
		[Key]
		public string HobbyId { get; set; }

		public string Name { get; set; }

		public bool IsChecked { get; set; }

		[ForeignKey(nameof(User))]
		public string UserId { get; set; }

		[NotMapped]
		public virtual SiteUser User { get; set; }

		public Hobby()
		{
			HobbyId = Guid.NewGuid().ToString();
		}
	}
}
