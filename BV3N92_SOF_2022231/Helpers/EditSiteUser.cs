using Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Helpers
{
	//[ModelBinder(BinderType = typeof(EditSiteUserModelBinder))]
	public class EditSiteUser
	{
		[Required]
		[StringLength(20)]
		[DisplayName("First name")]
		public string FirstName { get; set; }

		[Required]
		[Range(18, 100)]
		public int Age { get; set; }

		[Required]
		public Orientation Orientation { get; set; }

		[Required]
		public Gender Gender { get; set; }

		[StringLength(500)]
		public string Bio { get; set; }

		public List<Hobby> Hobbies { get; set; }

		public List<string> HobbyNames { get; set; }

		public string ProfilePictureUrl { get; set; }

		public IFormFile? ProfilePicture { get; set; }

		public ICollection<IFormFile?>? UserPictures { get; set; }

		public EditSiteUser()
		{
			UserPictures = new List<IFormFile?>();
			Hobbies = new List<Hobby>();
			HobbyNames = new List<string>();
		}
	}
}
