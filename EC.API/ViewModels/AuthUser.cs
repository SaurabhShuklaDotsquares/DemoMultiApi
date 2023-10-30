using System;
using System.Linq;
using System.Security.Claims;

namespace ToDo.WebApi.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string Name { get; set;}

        public AuthUser(ClaimsPrincipal user)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                throw new InvalidOperationException("User is not authorized");

            var claims = user.Claims;
            Id = Convert.ToInt32(claims.First(c => c.Type == ClaimTypes.Sid).Value);
            Name =claims.First(c => c.Type == ClaimTypes.Name).Value;

        }

    }
}
