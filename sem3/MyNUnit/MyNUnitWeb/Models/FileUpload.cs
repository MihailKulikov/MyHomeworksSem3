using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyNUnitWeb.Models
{
    public class FileUpload
    {
        [Required] public IFormFileCollection FormFiles { get; set; }
    }
}