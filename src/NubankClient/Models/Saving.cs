using System;
using System.Text.Json.Serialization;

namespace NubankClient.Models
{
    public class Saving
    {
        public string Id { get; set; }

        [JsonPropertyName("__typename")]
        public string TypeName { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTimeOffset PostDate { get; set; }
        public float Amount { get; set; }
        public Account OriginAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }

    public class Account
    {
        public string Name { get; set; }
    }
}