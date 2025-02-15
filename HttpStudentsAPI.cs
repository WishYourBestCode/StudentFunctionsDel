using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudentFunctions.Models;


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

  
    [Function("GetStudentById")]
    public HttpResponseData GetStudentById
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req,
        int id
    )
    {
        _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response. WriteStringAsync("Not Found");
            return response;
        }
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2. WriteStringAsync(JsonConvert.SerializeObject(student));
        return response2;
    }


    [Function("CreateStudent")]
    public HttpResponseData CreateStudent
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req
    )
    {
        _logger.LogInformation("C# HTTP POST/posts trigger function processed a request.");
        var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
        _context.Students.Add(student);
        _context.SaveChanges();
        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");
        response. WriteStringAsync(JsonConvert.SerializeObject(student));
        return response;
    }

    [Function("UpdateStudent")]
    public HttpResponseData UpdateStudent
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req,
        int id
    )
    {
        _logger.LogInformation("C# HTTP PUT/posts trigger function processed a request.");
        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response. WriteStringAsync("Not Found");
            return response;
        }
        var student2 = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
        student.FirstName = student2.FirstName;
        student.LastName = student2.LastName;
        student.School = student2.School;
        _context.SaveChanges();
        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2. WriteStringAsync(JsonConvert.SerializeObject(student));
        return response2;
    }

    [Function("DeleteStudent")]
    public HttpResponseData DeleteStudent
    (
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req,
    int id
    )
    {
    _logger.LogInformation("C# HTTP DELETE/posts trigger function processed a request.");
    var student = _context.Students.FindAsync(id).Result;
    if (student == null)
    {
        var response = req.CreateResponse(HttpStatusCode.NotFound);
        response.Headers.Add("Content-Type", "application/json");
        response. WriteStringAsync("Not Found");
        return response;
    }
    _context.Students.Remove(student);
    _context.SaveChanges();
    var response2 = req.CreateResponse(HttpStatusCode.OK);
    response2.Headers.Add("Content-Type", "application/json");
    response2. WriteStringAsync(JsonConvert.SerializeObject(student));
    return response2;
    }

    // -------------------------
    //   GAMES ENDPOINTS
    // -------------------------

        [Function("GetGames")]
        public async Task<HttpResponseData> GetGames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")] HttpRequestData req)
        {
            _logger.LogInformation("GET: Retrieving all games.");
            
            var games = _context.Games.ToArray();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteStringAsync(JsonConvert.SerializeObject(games)); 
            return response;
        }

        // Add these methods in the same class where you have GetGames,
    // typically below the GetGames method.

    [Function("GetGameById")]
    public async Task<HttpResponseData> GetGameById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{id}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"GET: Retrieving game by ID = {id}.");

        var game = await _context.Games.FindAsync(id);
        if (game == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            notFoundResponse.Headers.Add("Content-Type", "application/json");
            await notFoundResponse.WriteStringAsync("Game not found");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonConvert.SerializeObject(game));
        return response;
    }

    [Function("CreateGame")]
    public async Task<HttpResponseData> CreateGame(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "games")] HttpRequestData req)
    {
        _logger.LogInformation("POST: Creating a new game.");

        // Read request body and deserialize into Game object
        var requestBody = await req.ReadAsStringAsync();
        var newGame = JsonConvert.DeserializeObject<Game>(requestBody);

        // Add to database and save
        _context.Games.Add(newGame);
        await _context.SaveChangesAsync();

        // Return newly created record
        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonConvert.SerializeObject(newGame));
        return response;
    }

    [Function("UpdateGame")]
    public async Task<HttpResponseData> UpdateGame(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "games/{id}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"PUT: Updating game with ID = {id}.");

        var existingGame = await _context.Games.FindAsync(id);
        if (existingGame == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            notFoundResponse.Headers.Add("Content-Type", "application/json");
            await notFoundResponse.WriteStringAsync("Game not found");
            return notFoundResponse;
        }

        // Read the updated data from the body
        var requestBody = await req.ReadAsStringAsync();
        var updateGame = JsonConvert.DeserializeObject<Game>(requestBody);

        // Overwrite fields
        existingGame.Year = updateGame.Year;
        existingGame.Gender = updateGame.Gender;
        existingGame.City = updateGame.City;
        existingGame.Country = updateGame.Country;
        existingGame.Continent = updateGame.Continent;
        existingGame.Winner = updateGame.Winner;
        existingGame.Created = updateGame.Created;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonConvert.SerializeObject(existingGame));
        return response;
    }

    [Function("DeleteGame")]
    public async Task<HttpResponseData> DeleteGame(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "games/{id}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"DELETE: Removing game with ID = {id}.");

        var existingGame = await _context.Games.FindAsync(id);
        if (existingGame == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            notFoundResponse.Headers.Add("Content-Type", "application/json");
            await notFoundResponse.WriteStringAsync("Game not found");
            return notFoundResponse;
        }

        _context.Games.Remove(existingGame);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonConvert.SerializeObject(existingGame));
        return response;
    }


}

