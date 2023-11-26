using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VueAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public AuthController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        [HttpPost, Route("[controller]/otp/send")]
        public JsonResult SendOTP(string mobile)
        {
            var cache = _memoryCache.Get("OTP") as Dictionary<string, int> ?? [];
            var random = new Random();
            var code = random.Next(10000, 99999);
            cache.Remove(mobile);
            cache.Add(mobile, code);
            _memoryCache.Set("OTP", cache);
            return Json(new { code });
        }
        [HttpPost, Route("[controller]/otp/verify")]
        public JsonResult VerifyOTP(string mobile, int code)
        {
            var cache = _memoryCache.Get("OTP") as Dictionary<string, int> ?? [];
            if (!cache.Any(x => x.Key == mobile && x.Value == code))
            {
                return Json(new
                {
                    success = false,
                    error = "کد وارد شده معتبر نمی باشد"
                });
            }

            var user = new
            {
                id = Guid.NewGuid().ToString(),
                mobile,
                fullname = "محمدصادق حسابی"
            };

            // generate token that is valid for 3 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWT:Secret")?.Value ?? throw new Exception("JWT Secret key is not defined"));
            var ExpireAt = DateTime.Now.AddDays(3);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration.GetSection("JWT:ValidAudience")?.Value ?? throw new Exception("JWT ValidAudience is not defined"),
                Issuer = _configuration.GetSection("JWT:ValidIssuer")?.Value ?? throw new Exception("JWT ValidIssuer is not defined"),
                IssuedAt = DateTime.Now,
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.mobile),
                    new Claim("id", user.id),
                    new Claim("fullname", user.fullname)
                }),
                Expires = ExpireAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var a = tokenHandler.CreateToken(tokenDescriptor);
            var Token = tokenHandler.WriteToken(a);

            // invalidate old tokens
            // InvalidateTokens(user.Username);

            // save new token in database
            //var jwt = new JwtToken()
            //{
            //    AccessToken = Token,
            //    Username = user.Username,
            //    CreatedAt = DateTime.Now,
            //    ExpireAt = ExpireAt,
            //    IP = IP,
            //    IsPersistent = IsPersistent
            //};
            //this.jwt.Add(jwt);
            //db.SaveChanges();

            return Json(new
            {
                success = true,
                access_token = Token
            });
        }
    }
}