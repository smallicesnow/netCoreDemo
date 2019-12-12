using Demeter.Agent.Object;
using DemeterBackend.Business.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;

namespace Demeter.Agent.Api
{
    [Route("api/Repost")]
    [Authorize]
    public class RepostController : Controller
    {
        private readonly ILogger<RepostController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        //private readonly CoreDbContext _coreDbContext;
        public RepostController(ILogger<RepostController> logger, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

            //_coreDbContext = coreDbContext;
        }
        [HttpGet, Route("Test")]
        [AllowAnonymous]
        public string Test(string request)
        {
            _logger.LogDebug("Test " + request);
            return request;
        }

        [HttpGet, Route("ShowImg")]
        [AllowAnonymous]
        public FileResult ShowImg(string fileName)
        {
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string filePath = Path.Combine(webRootPath, "Images", fileName);
            FileStream fs = new FileStream(filePath, FileMode.Open);
            return File(fs, "image/png");
        }

        [HttpPost, Route("Send")]
        [Authorize]
        public string Send(string request)
        {
            _logger.LogDebug("Send a request " + request);
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<BsonDocument>("bar");
            var document = new BsonDocument
                                {
                                    { "name", "MongoDB" },
                                    { "type", "Database" },
                                    { "count", 1 },
                                    { "info", new BsonDocument
                                        {
                                            { "x", 203 },
                                            { "y", 102 }
                                        }}
                                };
            collection.InsertOne(document);
            return request;
        }
        [HttpGet, Route("SendABC")]
        [Authorize]
        public IActionResult SendABC(string request)
        {
            _logger.LogDebug("Send a request " + request);
            //var result = _coreDbContext.Set<User>().ToList();
            return new CreatedResult("api/Repost/SendABC", new {status="success", data=request });
        }


        [HttpPost, Route("Upload")]
        [AllowAnonymous]
        public bool ImportFile([FromForm] ImportParameterModel importParameter)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            foreach (var file in files)
            {
                var fileName = file.FileName;
                var fileDir = Path.Combine(webRootPath, "UploadFiles");
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string filePath = fileDir + $@"\{fileName}";
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return true;
        }
    }
}
