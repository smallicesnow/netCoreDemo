using Demo.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Demo
{
    [Route("api/Repost")]
    [Authorize]
    public class RepostController : Controller
    {
        private readonly ILogger<RepostController> _logger;

        public RepostController(ILogger<RepostController> logger)
        {
            _logger = logger;
        }
        [HttpGet, Route("Test")]
        [AllowAnonymous]
        public string Test(string request)
        {
            _logger.LogDebug("Test " + request);
            return request;
        }
        [HttpPost, Route("Send")]
        public RepostResponse Send(string request)
        {
            _logger.LogDebug("Send a request "+request);
            var repostObj = JsonConvert.DeserializeObject<RepostRequest>(request);
            _logger.LogDebug("Request :{0}", repostObj);
            return Send(repostObj);
        }

        public RepostResponse Send(RepostRequest request)
        {
            RepostResponse repostResult = new RepostResponse();
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", request.email),
                    new KeyValuePair<string, string>("password", request.password),
                    new KeyValuePair<string, string>("access_token", request.accessToken)
                });

                var handler = new HttpClientHandler();
                handler.UseCookies = true;
                handler.CookieContainer = new CookieContainer();
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var _httpClient = new HttpClient(handler);
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8,zh-CN;q=0.6,zh-TW;q=0.4");
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.2.0");
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                HttpResponseMessage response = _httpClient.PostAsync("", content).Result;
                var responseStr = response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation(responseStr);
                var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseStr);
                if (loginResult.state.Equals("ok", StringComparison.OrdinalIgnoreCase))
                {
                    
                }
                else
                {
                    repostResult.Status = 1;
                }
            }
            catch (Exception e)
            {
                repostResult.Status = 1;
            }
            return repostResult;
        }
    }
}
