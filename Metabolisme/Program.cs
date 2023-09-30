var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.MapPost("/calculatebmr", async (HttpContext context) =>
{
    // Reading JSON input
    var input = await context.Request.ReadFromJsonAsync<UserInput>();
    if (input == null)
    {
        context.Response.StatusCode = 400; // Bad Request
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("Invalid input");
    return;
    }

    double bmr;
    if (input.Sex == "male")
    {
    bmr =  (13.707 * input.Weight) + (492.3 * ((double)input.Height/100)) - (6.673 * input.Age) + 77.607 ;
    }
    else if (input.Sex == "female")
    {
    bmr =  (9.740 * input.Weight) + (172.9 * input.Height) - (4.737 * input.Age) + 667.051 ;
    }
    else
    {
    context.Response.StatusCode = 400; // Bad Request
    await context.Response.WriteAsync("Invalid sex");
    return;
    }

    await context.Response.WriteAsJsonAsync(new { BMR = bmr });

});

app.Run();

public class UserInput
{
    public double Weight { get; set; } // in kg
    public double Height { get; set; } // in cm
    public int Age { get; set; }
    public string Sex { get; set; } // male or female
}