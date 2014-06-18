using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace AuthZero.API.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {

        private readonly string clientID;
        private readonly string clientSecret;
        private readonly string domain;

        public AccountController()
        {
            this.clientID = System.Configuration.ConfigurationManager.AppSettings["auth0:ClientId"]; 
            this.clientSecret = System.Configuration.ConfigurationManager.AppSettings["auth0:ClientSecret"]; 
            this.domain = "https://" + System.Configuration.ConfigurationManager.AppSettings["auth0:Domain"] + "/";
        }

        // POST api/account/signup
        [AllowAnonymous]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupModel signupModel)
        {

            string accessToken = await this.GetAccessToken();

            if (accessToken == string.Empty)
            {
                return BadRequest("Invalid clientid, secret or domain");
            }


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.domain);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await client.PostAsJsonAsync("api/users/", signupModel);

                if (response.IsSuccessStatusCode)
                {
                    var userObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(userObject);

                }

                return BadRequest("Invalid request: " + response.ReasonPhrase);
            }

        }


        private async Task<string> GetAccessToken()
        {

            string accessToken = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.domain);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var payLoad = new
                {
                    client_id = this.clientID,
                    client_secret = this.clientSecret,
                    grant_type = "client_credentials"
                };

                HttpResponseMessage response = await client.PostAsJsonAsync("oauth/token", payLoad);

                if (response.IsSuccessStatusCode)
                {
                    var JsonObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                    accessToken = JsonObject["access_token"].ToString();

                    return accessToken;

                }

                return string.Empty;
            }
        }


    }

   public class SignupModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool email_verified { get; set; }
        public string connection { get; set; }
    }
}
