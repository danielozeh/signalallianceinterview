namespace test.Models
{
    public class UserWithToken
    {
        public UserWithToken(string token, string username)
        {
            this.token = token;
            this.username = username;
        }
        public string token { get; set; }
        public string username { get; set; }
    }
}
