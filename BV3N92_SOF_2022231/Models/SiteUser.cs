using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public enum Gender
    {
        Female, Male, NonBinary
    }

    public enum Orientation
    {
        Straight,Gay, Lesbian, Bisexual, Heterosexual, NonBinary,Asexual,Pansexual,Queer
    }
    public enum Education
    {
        None, Elementary, HighSchool, Bachelor, Master, PhD
    }
    public class SiteUser : IdentityUser
    {
        public string Id { get; set; }

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
        public string ProfilePicture { get; set; }

        [Required]
        public ICollection<string> Pictures { get; set; }

        public SiteUser()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
