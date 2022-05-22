using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    public class Validate_JWT
    {
        public bool isValid { get; }
        public string username { get; }
        public Validate_JWT(HttpRequest request)
        {
            Console.WriteLine("Validate_JWT");
            if (!request.Headers.ContainsKey("Authorization")) {
            Console.WriteLine("Validate_JWT Authorization");

                isValid = false;
                return;
            }
            string authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader)) {
                Console.WriteLine("Validate_JWT Authorization header");
                isValid = false;
                return;
            }

            try{
                if(authorizationHeader.StartsWith("Bearer")){
                    authorizationHeader = authorizationHeader.Substring(7);
                }
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                jwtTokenHandler.ValidateToken(authorizationHeader, getTokenValidationParams(), out validatedToken);
                JwtSecurityToken token = (JwtSecurityToken) validatedToken;

                Console.WriteLine(validatedToken != null);

                isValid = validatedToken != null;
                return;
            } catch(Exception ex){
                    isValid = false;
                    return;
            }
        }

        private static TokenValidationParameters getTokenValidationParams()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretsecretsecretsecretsecret"))
            };
        }
    }
}
