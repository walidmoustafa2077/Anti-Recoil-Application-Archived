using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Anti_Recoil_Application.Models
{
    internal class TokenResponse
    {
        [JsonPropertyName("token")]

        public string Token { get; set; }

    }
}
