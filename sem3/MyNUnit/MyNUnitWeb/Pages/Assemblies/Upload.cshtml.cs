using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;
using MyNUnitWeb.Data;
using MyNUnitWeb.Models;
using MyNUnitWeb.Utilities;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class LoadModel : PageModel
    {
        private readonly MyNUnitWebContext context;
        private readonly long fileSizeLimit;
        private readonly string[] permittedExtensions;
        private readonly string targetFilePath;
        private readonly IAssemblyHandler assemblyHandler;
        private readonly IRunner runner;

        [BindProperty] public FileUpload FileUpload { get; set; }
        public string ResultOfUploading { get; private set; }
        public IEnumerable<TestDb> Tests { get; private set; } = new List<TestDb>();

        public List<string> SavedFileNames => Directory.EnumerateFiles(targetFilePath)
            .Select(fileName => fileName.Split('\\')[^1])
            .ToList();

        public LoadModel(MyNUnitWebContext context, IConfiguration config, IAssemblyHandler assemblyHandler,
            IRunner runner)
        {
            fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            targetFilePath = config.GetValue<string>("StoredFilesPath");
            permittedExtensions = config.GetValue<string[]>("PermittedExtensions");
            this.context = context;
            this.assemblyHandler = assemblyHandler;
            this.runner = runner;
            permittedExtensions = new[] {".dll"};
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                ResultOfUploading = "Please correct the form";

                return Page();
            }

            foreach (var formFile in FileUpload.FormFiles)
            {
                var formFileContent = await FileHelpers.ProcessFormFile<LoadModel>(
                    formFile, ModelState, permittedExtensions, fileSizeLimit);
                if (!ModelState.IsValid)
                {
                    ResultOfUploading = "File extension should be .dll";

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

        public async Task<IActionResult> OnPostRunTestsAsync()
        {
            var testClasses = assemblyHandler.GetTestClassesFromAssemblies(targetFilePath).ToList();
            var testResults = runner.RunTests(testClasses).ToList();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendJoin(", ", SavedFileNames);
            var assembly = new AssemblyDb {Name = stringBuilder.ToString()};
            var tests = MapTestResultsToTestDbs(testResults);
            assembly.Tests = tests;
            await context.Assemblies.AddAsync(assembly);
            await context.Tests.AddRangeAsync(tests);
            await context.SaveChangesAsync();
            var savedAssemblies = context.Assemblies.ToList();
            Tests = tests;

            return Page();
        }

        private static ICollection<TestDb> MapTestResultsToTestDbs(IEnumerable<TestResult> testResults)
        {
            var tests = new List<TestDb>();
            foreach (var testMethod in testResults.SelectMany(result => result.TestMethods))
            {
                switch (testMethod)
                {
                    case IgnoredTestMethod ignoredTestMethod:
                        tests.Add(new TestDb
                        {
                            ElapsedTime = TimeSpan.Zero,
                            ReasonForIgnoring = ignoredTestMethod.ReasonForIgnoring, Name = ignoredTestMethod.Name,
                            Status = TestStatus.Ignored
                        });
                        break;
                    case SuccessfulTestMethod successfulTestMethod:
                        tests.Add(new TestDb
                        {
                            ElapsedTime = successfulTestMethod.ElapsedTime,
                            Name = successfulTestMethod.Name,
                            ReasonForIgnoring = string.Empty,
                            Status = TestStatus.Success
                        });
                        break;
                    case FailedTestMethod failedTestMethod:
                        tests.Add(new TestDb
                        {
                            ElapsedTime = failedTestMethod.ElapsedTime,
                            Name = failedTestMethod.Name,
                            ReasonForIgnoring = string.Empty,
                            Status = TestStatus.Failed
                        });
                        break;
                }
            }

            return tests;
        }
    }

    public class FileUpload
    {
        [Required] public IFormFileCollection FormFiles { get; set; }
    }
}
