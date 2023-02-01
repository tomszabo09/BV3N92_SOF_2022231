namespace Backend.Models
{
	public class MessageModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Text { get; set; }

		public DateTime Timestamp { get; set; }

		public string ChatId { get; set; }

		public virtual ChatModel Chat { get; set; }

		public MessageModel()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
