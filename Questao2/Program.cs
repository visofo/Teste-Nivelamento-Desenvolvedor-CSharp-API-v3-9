using Newtonsoft.Json;

public class Program
{
	private const string ApiUrl = "https://jsonmock.hackerrank.com/api/football_matches";

	public static async Task Main()
    {
		await GetAndPrintTotalGoals("Paris Saint-Germain", 2013);
		await GetAndPrintTotalGoals("Chelsea", 2014);

		// Output expected:
		// Team Paris Saint - Germain scored 109 goals in 2013
		// Team Chelsea scored 92 goals in 2014
	}

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
		using (var client = new HttpClient())
		{
			int totalGoals = 0;
			int currentPage = 1;
			int totalPages = 1; 

			while (currentPage <= totalPages)
			{
				string url = $"{ApiUrl}?year={year}&team1={team}&page={currentPage}";
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				var matchesResponse = JsonConvert.DeserializeObject<FootballMatchesResponse>(responseBody);
				totalPages = matchesResponse.TotalPages;

				foreach (var match in matchesResponse.Data)
				{
					totalGoals += match.Team1Goals;
				}

				currentPage++;
			}

			return totalGoals;
		}
    }

	public static async Task GetAndPrintTotalGoals(string teamName, int year)
	{
		int totalGoals = await GetTotalScoredGoals(teamName, year);

		Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");
	}

	public class FootballMatchesResponse
	{
		[JsonProperty("total_pages")]
		public int TotalPages { get; set; }

		[JsonProperty("data")]
		public List<FootballMatchData> Data { get; set; }
	}

	public class FootballMatchData
	{
		[JsonProperty("team1")]
		public string Team1 { get; set; }

		[JsonProperty("team2")]
		public string Team2 { get; set; }

		[JsonProperty("team1goals")]
		public int Team1Goals { get; set; }

		[JsonProperty("team2goals")]
		public int Team2Goals { get; set; }
	}

}