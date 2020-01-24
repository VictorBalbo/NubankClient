using System.Collections.Generic;

namespace NubankClient.Models.ApiResponses
{
    public class GetEventsResponse : BaseResponse
    {
        public IEnumerable<Event> Events { get; set; }
    }
}