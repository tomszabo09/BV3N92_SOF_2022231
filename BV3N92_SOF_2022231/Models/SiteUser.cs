using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
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
		Straight, Gay, Bisexual
	}

	public class SiteUser : IdentityUser
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

		public string ProfilePictureUrl { get; set; }

		[NotMapped]
		public virtual List<Hobby> Hobbies { get; set; }

		[NotMapped]
		public virtual ICollection<Picture> Pictures { get; set; }

		[NotMapped]
		public virtual ICollection<LikedUser> LikedUsers { get; set; }

		[NotMapped]
		public virtual ICollection<MatchedUser> MatchedUsers { get; set; }

		[NotMapped]
		public virtual ICollection<DislikedUser> DislikedUsers { get; set; }

		public SiteUser()
		{
			this.Pictures = new List<Picture>();
			this.LikedUsers = new List<LikedUser>();
			this.DislikedUsers = new List<DislikedUser>();
			this.Hobbies = new List<Hobby>();
			this.MatchedUsers = new List<MatchedUser>();
		}
	}
}