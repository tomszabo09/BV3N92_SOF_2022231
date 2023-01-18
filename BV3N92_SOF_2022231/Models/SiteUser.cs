using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
	public enum Gender
	{
		Female, Male, NonBinary
	}

	public enum Orientation
	{
		Straight, Gay, Lesbian, Bisexual, Heterosexual, NonBinary, Asexual, Pansexual, Queer
	}
	public enum Education
	{
		None, Elementary, HighSchool, Bachelor, Master, PhD
	}

	public class SiteUser : IdentityUser
	{
		[Key]
		public string UId { get; set; }

		[Required]
		[StringLength(20)]
		public string FirstName { get; set; }

		[Required]
		[Range(18, int.MaxValue)]
		public int Age { get; set; }

		[Required]
		public Orientation Orientation { get; set; }

		[Required]
		public Gender Gender { get; set; }

		[StringLength(500)]
		public string Bio { get; set; }

		[Required]
		public Picture ProfilePicture { get; set; }

		[NotMapped]
		public ICollection<Picture> Pictures { get; set; }

		public SiteUser()
		{
			UId = Guid.NewGuid().ToString();
			this.Pictures = new List<Picture>();
		}
	}
}