using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using test.Models;

namespace test.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private JWTSettings _jwtSettings;
        private readonly ILogger _logger;
        private HttpClient client = new HttpClient();
        public string Message { get; set; }

        public TestController(IOptions<JWTSettings> jwtSettings, ILogger<TestController> logger)
        {
            this._jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet("{cardBin}")]
        public async Task<CardDetails> GetCardDetails(int cardBin)
        {
            string cardDetails = await client.GetStringAsync("https://lookup.binlist.net/" + cardBin);

            CardDetails details = JsonConvert.DeserializeObject<CardDetails>(cardDetails);

            Message = $"details for {cardBin} requested at {DateTime.UtcNow.ToLongTimeString()}";

            _logger.LogInformation(Message);

            return details;
        }

        [HttpPost("auth")]
        public ActionResult<UserWithToken> Auth(User user)
        {
            if(user.username == "administrator" && user.password == "password")
            {
                string token = GenerateJwtToken("administrator");

                UserWithToken userWithToken = new UserWithToken(token, "administrator");

                return userWithToken;
            }
            else
            {
                return BadRequest(new { error = "Invalid details." });
            }
        }

        //Generate Token
        string GenerateJwtToken(string username)
        {
            //Initialize the token handler here
            var tokenHandler = new JwtSecurityTokenHandler();

            //Get the bytes of the secret key
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            //Token Descriptor gathers the details to create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //UserID
                Subject = new ClaimsIdentity(new Claim[]
               {
                   new Claim(ClaimTypes.Name, username)
               }),

                //Expiry Date
                Expires = DateTime.UtcNow.AddDays(1),

                //Hashed SecretKey
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
               SecurityAlgorithms.HmacSha256Signature)
            };

            //TokenHandler creates the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //TokenHandler stores the token
            return tokenHandler.WriteToken(token);
        }
    }
}
