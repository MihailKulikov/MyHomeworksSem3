using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MyNUnitWeb.Data;
using MyNUnitWeb.Utilities;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class LoadModel : PageModel
    {
        private readonly MyNUnitWebContext context;
        private readonly long fileSizeLimit;
        private readonly string[] permittedExtensions;
        private readonly string targetFilePath;

        [BindProperty]
        public FileUpload FileUpload { get; set; }

        public string Result { get; private set; }

        public List<string> SavedFileNames => Directory.EnumerateFiles(targetFilePath)
            .Select(fileName => fileName.Split('\\')[^1])
            .ToList();

        public LoadModel(MyNUnitWebContext context, IConfiguration config)
        {
            fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            targetFilePath = config.GetValue<string>("StoredFilesPath");
            permittedExtensions = config.GetValue<string[]>("PermittedExtensions");
            this.context = context;
            permittedExtensions = new[] { ".dll" };
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form";

                return Page();
            }

            foreach (var formFile in FileUpload.FormFiles)
            {
                var formFileContent = await FileHelpers.ProcessFormFile<LoadModel>(
                    formFile, ModelState, permittedExtensions, fileSizeLimit);
                if (!ModelState.IsValid)
                {
                    Result = "File extension should be .dll";

                    return Page();
                }

                var filePath = Path.Combine(targetFilePath, formFile.FileName);
                await using var fileStream = System.IO.File.Create(filePath);
                await fileStream.WriteAsync(formFileContent);
                SavedFileNames.Add(filePath);
            }

            return Page();
            //_context.Assemblies.Add(Assembly);
            //await _context.SaveChangesAsync();

            //return RedirectToPage("./Index");
        }
    }

    public class FileUpload
    {
        [Required]
        public IFormFileCollection FormFiles { get; set; }
    }
}
