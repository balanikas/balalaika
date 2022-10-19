using System.ComponentModel.DataAnnotations;


namespace BlazorApp.Pages;


public class CodeExecutionInputModel
{
    [Required]
    public string Code { get; set; } = "";
}