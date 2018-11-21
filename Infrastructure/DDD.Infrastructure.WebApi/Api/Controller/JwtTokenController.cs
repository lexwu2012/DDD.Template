using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DDD.Infrastructure.Web.Application;
using Microsoft.IdentityModel.Tokens;

namespace DDD.Infrastructure.WebApi.Api.Controller
{
    public class JwtTokenController: ApiController
    {
        private static readonly string secretKey = "secretkey!123";

        [HttpPost]
        [Route("RequestTokenAsync")]
        public async Task<Result<string>> RequestTokenAsync(string userName, string pwd)
        {
            if (!Verify(userName, pwd))
                return Result.FromCode<string>(ResultCode.Forbidden);

            var token  = await GenerateToken(userName);

            return Result.FromData(token);
        }


        private bool Verify(string userName, string pwd)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pwd))
                return false;

            //todo: verify user in db
            return true;
        }

        private async Task<string> GenerateToken(string userName)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await Task.FromResult(Guid.NewGuid().ToString())),
                new Claim(JwtRegisteredClaimNames.Iat,  new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: "ExampleIssuer",
                audience: "ExampleAudience",
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
