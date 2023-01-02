using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public enum Gender
    {
        Female, Male, NonBinary
    }
    public enum Education
    {
        None, Elementary, HighSchool, Bachelor, Master, PhD
    }
    public class SiteUser
    {
        public string Id { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        [Range(18, int.MaxValue)]
        public int Age { get; set; }

        public int Height { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public Dictionary<Education, string> Education { get; set; } //ex.: Bachelor, University of Obuda

        [Required]
        public List<Picture> Pictures { get; set; }

        public SiteUser()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
    public class Picture
    {
        public string PhotoContentType { get; set; }

        public byte[] PhotoData { get; set; }
    }
}
