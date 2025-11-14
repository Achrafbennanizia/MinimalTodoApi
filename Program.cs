using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-Memory "Datenbank"
List<TodoItem> todos = new();

// Health Check
app.MapGet("/", () => "Minimal REST API is running!");

// Alle Todos
app.MapGet("/todos", () => todos);

// Einzelnes Todo
app.MapGet("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

// Neues Todo anlegen
app.MapPost("/todos", (TodoCreateRequest request) =>
{
    int newId = todos.Count == 0 ? 1 : todos.Max(t => t.Id) + 1;

    var todo = new TodoItem
    {
        Id = newId,
        Title = request.Title,
        IsDone = false
    };

    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

// Todo aktualisieren
app.MapPut("/todos/{id:int}", (int id, TodoUpdateRequest request) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null)
        return Results.NotFound();

    if (!string.IsNullOrWhiteSpace(request.Title))
        todo.Title = request.Title;

    if (request.IsDone.HasValue)
        todo.IsDone = request.IsDone.Value;

    return Results.Ok(todo);
});

// Todo löschen
app.MapDelete("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null)
        return Results.NotFound();

    todos.Remove(todo);
    return Results.NoContent();
});

app.Run();

// Datentypen

class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}

record TodoCreateRequest(string Title);
record TodoUpdateRequest(string? Title, bool? IsDone);
