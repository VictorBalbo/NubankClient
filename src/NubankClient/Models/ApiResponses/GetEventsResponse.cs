using System.Collections.Generic;

namespace NubankClient.Models.ApiResponses
{
    public class GetEventsResponse : BaseResponse
    {
        public List<Event> Events { get; set; }
    }
}