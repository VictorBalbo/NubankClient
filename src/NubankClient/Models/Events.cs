using System;

namespace NubankClient.Models
{
    public class Event
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public float CurrencyAmount => (float) Amount / 100;
        public DateTimeOffset Time { get; set; }
        public string Title { get; set; }
    }
}