using HPVTesting.Business.Helpers;
using HPVTesting.Business.ViewModels;
using HPVTesting.Business.ViewModels.Account;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HPVTesting.API.Helpers
{
    public class ApiTokenHelper
    {
        public static string GenerateJSONWebToken(ApplicationUser user, UserViewModel userViewModel)
        {
            var appSettings = new Jwt();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim("AspNetUserId", Convert.ToString(user.Id)),
            new Claim("UserId", Convert.ToString(userViewModel.Id)),
            new Claim("Name",userViewModel.Name),
            new Claim("Email",user.Email),
            new Claim("Roles",userViewModel.UserRole),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: appSettings.Issuer,
                audience: appSettings.Issuer,
                claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials);

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }

        public static ClaimsPrincipal ValidateToken(string jwtToken)
        {
            var appSettings = new Jwt();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            //  validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = appSettings.Issuer;
            validationParameters.ValidIssuer = appSettings.Issuer;
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Key));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}
