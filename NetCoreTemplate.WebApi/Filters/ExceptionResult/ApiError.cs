using Newtonsoft.Json;

namespace NetCoreTemplate.WebApi.Filters.ExceptionResult
{
  public class ApiError
  {
    public string Error { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string StackTrace { get; set; }
  }
}
