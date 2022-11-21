using System.Diagnostics.Eventing.Reader;
using TicketSystemAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<SQLRepo>();
var connString = builder.Configuration.GetValue<string>("ConnectionString:Azure");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//gets all employees
app.MapGet("/employees", (SQLRepo repo) =>
    repo.getAll(connString));

//gets specific employee
app.MapGet("/employees/{id}", (SQLRepo repo, int id) =>
    repo.getEmployee(id, connString));

//creates employee
app.MapPost("/employees", (SQLRepo repo, Employee employee) =>
{
    repo.insertEmployee(employee, connString);
    return Results.Created($"/employees/{employee.iD}", employee);
});

//creates ticket
app.MapPost("/tickets", (SQLRepo repo, int employeeID,Ticket ticket) =>
{
    repo.insertTicket(employeeID, ticket, connString);
    return Results.Created($"/tickets/{ticket.Id}", ticket);
});

//updates specific employee
app.MapPut("/employees/{id}", (int id, Employee employee, SQLRepo repo) =>
{
    repo.updateEmployee(employee, id, connString);
    return Results.NoContent();
});

//deletes specific employee
app.MapDelete("/employees/{id}", (int id, SQLRepo repo) =>
{
    repo.deleteEmployee(connString, id);
    return Results.Ok(id);
});

//gets specific employee TICKET
app.MapGet("/employeeTicket/{id}", (SQLRepo repo,int id) =>
    repo.seeOwnTickets(connString, id));

//login, still working
app.MapGet("/login/{userName}/{password}", (string userName,string password,SQLRepo repo) =>
{
    return repo.login(connString, userName, password);
});

//manager methods
app.MapGet("/tickets", (SQLRepo repo) =>
    repo.getAllPendingTickets(connString));

//updates specific ticket, only the status is allowed to to be changed
app.MapPut("/tickets/{id}", (SQLRepo repo,Employee employee,int ticketId,string status) =>
{
    repo.updateTicket(employee, ticketId, status, connString);
    return Results.NoContent();
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}