using Birpors.API.Helpers.ActionResults;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Birpors.API.Filters
{
    public class Auth : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserRoleEnum[] _roles;
        public Auth(params UserRoleEnum[] roles)
        {
            _roles = roles;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var result = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token);

            if (!result)
            {
                context.Result = new UnAuthActionResult();
                return;
            }


            var authToken = token.ToString();

            token = authToken.ToString().Contains("Bearer") ? authToken.ToString().Replace("Bearer ", "") : authToken.ToString().Replace("bearer ", "");
            var tokenValidationParams = context.HttpContext.RequestServices.GetService<TokenValidationParameters>();
            var principal = GetPrincipalFromToken(token, tokenValidationParams);
            if (principal == null)
            {
                context.Result = new UnAuthActionResult();
                return;
            }

            bool hasRole = false;
            if (!_roles.Contains(UserRoleEnum.All))
            {
                var role = principal.Claims.Where(c => c.Type == "roleId").FirstOrDefault().Value;

                foreach (var item in _roles)
                {
                    if ((int)item == Convert.ToInt32(role))
                    {
                        hasRole = true;
                        break;
                    }
                }

                if (!hasRole)
                {
                    context.Result = new UnAuthActionResult();
                    return;
                }
            }


        }
        private ClaimsPrincipal GetPrincipalFromToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken)) return null;

                return principal;
            }
            catch { return null; }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        =>
            (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);

    }
}
