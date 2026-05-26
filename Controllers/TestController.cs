using Microsoft.AspNetCore.Mvc;
using SemanticSearchApi.Data;

namespace SemanticSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly DbConnectionFactory _dbFactory;

    public TestController(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public IActionResult TestDbConnection()
    {
        using var connection = _dbFactory.CreateConnection();

        connection.Open();

        return Ok("Database connection successful");
    }
}