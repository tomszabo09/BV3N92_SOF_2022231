namespace Backend.Models
{
    public class ChatModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ChatMessageModel> Messages { get; set; }

        public virtual ICollection<SiteUser> Users { get; set; }
    }
}
