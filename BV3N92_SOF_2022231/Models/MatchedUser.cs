using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class MatchedUser
    {
        [Key]
        public string Id { get; set; }

        public string LikedById { get; set; }

        [NotMapped]
        public virtual SiteUser LikedBy { get; set; }

        public string WhoLikedId { get; set; }

        [NotMapped]
        public virtual SiteUser WhoLiked { get; set; }

        public MatchedUser()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
