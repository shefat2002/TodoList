using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }
    internal TodoViewModel GetAllTodos()
    {
        List<TodoItem> todoList = new();
        using (SqliteConnection con = new SqliteConnection("Data Source=DB.sqlite")){
            using( var tableCmd = con.CreateCommand()){
                con.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using(var reader = tableCmd.ExecuteReader()){
                    if(reader.HasRows){
                        while(reader.Read()){
                            todoList.Add(
                                new TodoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                }
                            );
                        }
                    }
                    else{
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                }
            }
        }
         return new TodoViewModel
            {
                TodoList = todoList
            };
    }

    public JsonResult Delete(int id)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=DB.sqlite")){
            using( var tableCmd = con.CreateCommand()){
                con.Open();
                tableCmd.CommandText = $"DELETE FROM todo WHERE Id= '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }
        return Json(new{});
    }

    public RedirectResult Insert(TodoItem todo)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=DB.sqlite")){
            using( var tableCmd = con.CreateCommand()){
                con.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES('{todo.Name}')";
                try{
                    tableCmd.ExecuteNonQuery();
                }
                catch(Exception ex){
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5206");
    }


}
