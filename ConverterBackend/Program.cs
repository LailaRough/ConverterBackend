using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});


builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors("AllowAll"); 


app.MapGet("/convert", async (string from, string to, decimal amount, IHttpClientFactory clientFactory) =>
{
    
    var client = clientFactory.CreateClient();
    string url = $"https://api.exchangerate-api.com/v4/latest/{from}";

    try
    {
        
        var response = await client.GetStringAsync(url);

        
        var json = JsonNode.Parse(response);

        
        decimal rate = (decimal)json["rates"][to];

        
        decimal result = amount * rate;

        
        return Results.Ok(new
        {
            From = from,
            To = to,
            Rate = rate,
            OriginalAmount = amount,
            ConvertedAmount = result
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = "Не удалось получить курс валют", Details = ex.Message });
    }
});

app.Run();