using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NubankClient.Models.ApiResponses
{
    public class GetSavingsResponse
    {
        public GetSavingsResponse()
        {
            Data = new DataResponse();
        }

        [JsonPropertyName("data")]
        public DataResponse Data { get; set; }

        [JsonIgnore]
        public List<Saving> Savings => Data.Viewer.SavingsAccount.Feed;
    }

    public class DataResponse
    {
        public DataResponse()
        {
            Viewer = new ViewerResponse();
        }

        [JsonPropertyName("viewer")]
        public ViewerResponse Viewer { get; set; }
    }

    public class ViewerResponse
    {
        public ViewerResponse()
        {
            SavingsAccount = new SavingsAccount();
        }

        [JsonPropertyName("savingsAccount")]
        public SavingsAccount SavingsAccount { get; set; }
    }

    public class SavingsAccount
    {
        public SavingsAccount()
        {
            Feed = new List<Saving>();
        }

        [JsonPropertyName("feed")]
        public List<Saving> Feed { get; set; }
    }
}