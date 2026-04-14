// ─────────────────────────────────────────────────────────────────────────────
// Program.cs — Punto di avvio minimale dell'applicazione.
// Durante la demo questo file verrà esteso per aggiungere i servizi e il middleware.
// ─────────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi HTTP di base
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


