using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudentFunctions.Models.School;

namespace School.Function;
public class HttpStudentsAPI
{
    private readonly ILogger<HttpStudentsAPI> _logger;
    private readonly SchoolContext _context;    
    public HttpStudentsAPI(ILogger<HttpStudentsAPI> logger, SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }



    [Function("Welcome")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("********* C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("GetStudents")]
    public HttpResponseData GetStudents(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request in GetStudents().");

        var students = _context.Students.ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        response.WriteStringAsync(JsonConvert.SerializeObject(students));

        return response;
    }

}

