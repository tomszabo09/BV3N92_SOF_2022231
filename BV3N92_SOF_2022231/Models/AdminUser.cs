using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
	public class AdminUser
	{
		public string Id { get; set; }

		[Required]
		public string Name { get; set; }

		public AdminUser()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
