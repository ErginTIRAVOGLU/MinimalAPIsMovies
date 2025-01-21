using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPIsMovies.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
        {
            group.MapPost("/register", Register)
                .AddEndpointFilter<ValidationFilter<UserCredentialsDto>>();
            group.MapPost("/login", Login)
                .AddEndpointFilter<ValidationFilter<UserCredentialsDto>>();
            return group;
        }
        static async Task<Results<Ok<AuthenticationResponseDto>, BadRequest<IEnumerable<IdentityError>>>> Register(UserCredentialsDto userCredentialsDto, [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialsDto.Email,
                Email = userCredentialsDto.Email
            };

            var result = await userManager.CreateAsync(user, userCredentialsDto.Password);

            if (result.Succeeded)
            {
                var authenticationResponse = await BuildToken(userCredentialsDto, configuration, userManager);
                
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest(result.Errors);
            }
        }

        static async Task<Results<Ok<AuthenticationResponseDto>, BadRequest<string>>> Login( UserCredentialsDto userCredentialsDto, 
            [FromServices] UserManager<IdentityUser> userManager, 
            [FromServices] SignInManager<IdentityUser> signInManager, 
            IConfiguration configuration)
        {
           var user = await userManager.FindByEmailAsync(userCredentialsDto.Email);
            if (user is null)
            {
                return TypedResults.BadRequest("There was a problem with th email or the password");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, userCredentialsDto.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var authenticationResponse = await BuildToken(userCredentialsDto, configuration, userManager);
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest("There was a problem with th email or the password");
            }
        }
        private async static Task<AuthenticationResponseDto> BuildToken(UserCredentialsDto userCredentialsDto, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userCredentialsDto.Email)
            };

            var user = await userManager.FindByEmailAsync(userCredentialsDto.Email);
            var claimsFromDB = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimsFromDB);

            var key = KeysHandler.GetKey(configuration).First();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDto
            {
                Token = token,
                Expiration = expiration
            };
        }
    }
}
