using System.Net;

namespace NetCoreTemplate.Application.Exceptions {
    public interface IApiException<TContent> {
        HttpStatusCode StatusCode { get; set; }
        TContent Content { get; set; }
    }
}
