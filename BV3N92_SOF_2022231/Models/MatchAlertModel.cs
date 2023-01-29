namespace Backend.Models
{
	public class MatchAlertModel
	{
		public string MatchId { get; set; }

		public MatchAlertModel(string userId)
		{
			this.MatchId = userId;
		}
	}
}
