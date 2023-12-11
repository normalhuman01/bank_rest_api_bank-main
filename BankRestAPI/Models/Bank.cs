using System.Text.Json.Serialization;

namespace BankRestAPI.Models
{
    public class Bank
    {

        [JsonIgnore]
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

    }
}
