using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using LibOwin;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    /// <summary>
    /// Middleware to read the end user’s identity from a header on requests between microservices
    /// </summary>
    public class IdToken
    {
        public static Func<IDictionary<string, object>, Task> Middleware(Func<IDictionary<string, object>, Task> next)
        {
            return env =>
            {
                var ctx = new OwinContext(env);
                //Check for the header that should contain the end user's identity
                if (!ctx.Request.Headers.ContainsKey("library-member")) return next(env);

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token;
                var userPrincipal =
                    //Reads and validates the end user's identity
                    tokenHandler.ValidateToken(ctx.Request.Headers["library-member"],
                        new TokenValidationParameters(), out token);
                //Creates a user object based on the claims in the end user's identity
                //and adds it to the OWIN context                
                ctx.Set("pos-member", userPrincipal);
                return next(env);
            };
        }
    }
}