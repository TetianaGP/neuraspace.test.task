public class MessageWarningTests
    {
        private const string BaseUrl = "https://your-api-endpoint.com"; // Replace with actual base URL

        private async Task<JObject> PostMessageAsync(double poc, int missDistance, DateTime tcaUtc)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("/message", Method.Post);
            request.AddJsonBody(new
            {
                satellite_id = "abc234sghu54646jgh6mbgy367",
                chaser_object_id = "abc234hhtyhtyhyth23367123",
                probability_of_collision = poc,
                miss_distance = missDistance,
                expected_date_of_collision = tcaUtc.ToString("o") // ISO 8601 format
            });

            var response = await client.ExecuteAsync(request);
            Assert.That(response.IsSuccessful, Is.True);
            return JObject.Parse(response.Content);
        }

        private DateTime DaysFromNow(double days)
        {
            return DateTime.UtcNow.AddDays(days);
        }

        [TestCase(0.0001, 10000, 14, TestName = "LowAndHighBoundaries_Condition1_ReturnsWarningStatus")]
        [TestCase(0.01, 5000, 5, TestName = "HighBoundaryCondition1_UpperBoundsCondition2_ReturnsWarningStatus")]
        [TestCase(0.0002, 5000, 4.9, TestName = "MidRangeCondition1_BoundsCondition2_ReturnsWarningStatus")]
        [TestCase(0.00009, 4999, 4.9, TestName = "HighBoundsCondition2_ReturnsWarningStatus")]
        [TestCase(0.00009, 1000, 1, TestName = "LowValues_InsideCondition2_ReturnsWarningStatus")]
        public async Task WarningStatusTest(double poc, int missDistance, double daysUntilTca)
        {
            var tcaDate = DaysFromNow(daysUntilTca);
            var result = await PostMessageAsync(poc, missDistance, tcaDate);
            Assert.That(result["status"]?.ToString(), Is.EqualTo("Warning"));
        }
}
