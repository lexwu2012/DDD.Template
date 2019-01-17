using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace DDD.Infrastructure.WebApi.Api.Helper
{
    public static class TokenHelper
    {
        /// <summary>
        /// 解码字符串token，并返回秘钥的信息对象
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler(); // 创建一个JwtSecurityTokenHandler类，用来后续操作
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken; // 将字符串token解码成token对象
                if (jwtToken == null)
                    return null;

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenSecretKey.SecretKey));

                var validationParameters = new TokenValidationParameters() // 生成验证token的参数
                {
                    RequireExpirationTime = true, // token是否包含有效期
                    ValidateIssuer = false, // 验证秘钥发行人，如果要验证在这里指定发行人字符串即可
                    ValidateAudience = false, // 验证秘钥的接受人，如果要验证在这里提供接收人字符串即可
                    IssuerSigningKey = signingKey // 生成token时的安全秘钥
                };
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal; // 返回秘钥的主体对象，包含秘钥的所有相关信息
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 生成JWT
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static string CreateAccessToken(CliamProperties properties, TimeSpan? expiration = null)
        {
            var claims = CreateJwtClaims(properties);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenSecretKey.SecretKey));
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "ExampleIssuer",
                audience: "ExampleAudience",
                claims: claims,
                notBefore: now,
                //5分钟有效
                expires: now.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(CliamProperties properties)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, properties.UserId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, properties.Name));
            identity.AddClaim(new Claim(ClaimTypes.Role, properties.RoleName));
            
            //权限Id
            //identity.AddClaims(properties.DepartmentIdList.Select(o => new Claim(ServiceContinuationClaimTypes.Department, o.ToString())));

            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}
