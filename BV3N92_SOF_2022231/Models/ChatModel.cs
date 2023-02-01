namespace Backend.Models
{
	public class ChatModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public virtual ICollection<MessageModel> Messages { get; set; }

		public virtual ICollection<SiteUser> Users { get; set; }

		public ChatModel()
		{
			Id= Guid.NewGuid().ToString();
			Messages = new List<MessageModel>();
			Users = new List<SiteUser>();
		}
	}
}
