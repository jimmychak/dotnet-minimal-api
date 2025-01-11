using Carter;
using Microsoft.EntityFrameworkCore;

namespace dotnet_minimal_api
{
    public class TodoModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var todoItems = app.MapGroup("/todoitems");

            todoItems.MapGet("/", GetAllTodos);

            todoItems.MapGet("/complete", GetCompleteTodos);

            todoItems.MapGet("/{id}", GetTodo);

            todoItems.MapPost("/", CreateTodo);

            todoItems.MapPut("/{id}", UpdateTodo);

            todoItems.MapDelete("/{id}", DeleteTodo);
        }

        static async Task<IResult> GetAllTodos(TodoDb db)
        {
            return TypedResults.Ok(await db.Todos.ToListAsync());
        }

        static async Task<IResult> GetCompleteTodos(TodoDb db)
        {
            return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
        }

        static async Task<IResult> GetTodo(int id, TodoDb db)
        {
            return await db.Todos.FindAsync(id)
                is Todo todo
                    ? TypedResults.Ok(todo)
                    : TypedResults.NotFound();
        }

        static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
        {
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/todoitems/{todo.Id}", todo);
        }

        static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return TypedResults.NotFound();

            todo.Name = inputTodo.Name;
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteTodo(int id, TodoDb db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return TypedResults.NotFound();

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    }
}
