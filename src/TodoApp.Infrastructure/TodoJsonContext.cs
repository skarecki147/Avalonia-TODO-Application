using System.Text.Json.Serialization;
using TodoApp.Core.Models;

namespace TodoApp.Infrastructure;

[JsonSerializable(typeof(List<TodoItem>))]
internal partial class TodoJsonContext : JsonSerializerContext
{
}
