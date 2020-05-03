using System;

namespace RoomL21.Common.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public DateTime ExpirationLocal => Expiration.ToLocalTime();
    }
}
