using System.ComponentModel.DataAnnotations;


namespace BlazorApp.Pages;


public class ExampleModel
{
    [Required]
    public string? Code { get; set; }
}