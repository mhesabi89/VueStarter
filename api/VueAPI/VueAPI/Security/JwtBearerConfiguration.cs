using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace VueAPI.Security
{
    public static class JwtBearerConfiguration
    {
        public static AuthenticationBuilder AddJwtBearerConfiguration(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            return builder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JWT:ValidIssuer").Value,
                    ValidAudience = configuration.GetSection("JWT:ValidAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Secret")?.Value ?? throw new Exception("JWT Config is not defined")))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        var serviceProvider = context.HttpContext.RequestServices;
                        var token = context.HttpContext.Request.Headers["Authorization"].ToString()?.Split(' ').LastOrDefault();

                        //if (string.IsNullOrEmpty(token))
                        //{
                        //    context.HttpContext.Response.StatusCode = 401;
                        //    return context.HttpContext.Response.WriteAsJsonAsync(new { error = "invalid_token" });
                        //}

                        // var securityService = serviceProvider.GetRequiredService<SecurityService>();
                        // var dbToken = securityService.ValidateToken(token, Common.GetUserIP(context.HttpContext));

                        //if (dbToken == null)
                        //{
                        //    context.HttpContext.Response.StatusCode = 401;
                        //    return context.HttpContext.Response.WriteAsJsonAsync(new { error = "invalid_token" });
                        //}

                        //if (dbToken.NeedRefresh == true)
                        //{
                        //    context.HttpContext.Response.StatusCode = 401;
                        //    return context.HttpContext.Response.WriteAsJsonAsync(new
                        //    {
                        //        error = "refresh_token",
                        //        access_token = dbToken.AccessToken
                        //    });
                        //}

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
