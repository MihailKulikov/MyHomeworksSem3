using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string uploadedFilePath;
        private readonly string testedFilePath;
        private readonly IAssemblyHandler assemblyHandler;
        private readonly IRunner runner;

        [BindProperty] public FileUpload FileUpload { get; set; }
        public string? ResultOfUploading { get; private set; }
        public IEnumerable<Test> Tests { get; private set; } = new List<Test>();

        public List<string> SavedFileNames => Directory.EnumerateFiles(uploadedFilePath)
            .Select(fileName => fileName.Split('\\')[^1])
            .ToList();

        public LoadModel(MyNUnitWebContext context, IConfiguration config, IAssemblyHandler assemblyHandler,
            IRunner runner)
        {
            fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            uploadedFilePath = config.GetValue<string>("UploadedFilesPath");
            testedFilePath = config.GetValue<string>("TestedFilesPath");
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

                var filePath = Path.Combine(uploadedFilePath, formFile.FileName);
                await using var fileStream = System.IO.File.Create(filePath);
                await fileStream.WriteAsync(formFileContent);
                SavedFileNames.Add(filePath);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRunTestsAsync()
        {
            var testClasses = assemblyHandler.GetTestClassesFromAssemblies(uploadedFilePath);
            var testResults = runner.RunTests(testClasses).ToList();
            var assemblies = testResults
                .Select(result => result.ClassType.Assembly)
                .Distinct()
                .Select(assembly =>
                {
                    var results = testResults.ToList();
                    return new Assembly
                    {
                        Name = assembly.FullName ?? "",
                        Tests = results.Where(testResult => testResult.ClassType.Assembly.Equals(assembly))
                            .SelectMany(MapTestResultToTestDbs).ToList()
                    };
                }).ToList();
            Tests = assemblies.SelectMany(assembly => assembly.Tests);
            await context.Assemblies.AddRangeAsync(assemblies);
            await context.Tests.AddRangeAsync(Tests);
            await context.SaveChangesAsync();

            MoveAllFilesInDirectory(uploadedFilePath);
            return Page();
        }

        private void MoveAllFilesInDirectory(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                System.IO.File.Move(file, Path.Combine(testedFilePath, Path.GetRandomFileName()));
            }
        }

        private static ICollection<Test> MapTestResultToTestDbs(TestResult testResult)
        {
            var tests = new List<Test>();
            foreach (var testMethod in testResult.TestMethods)
            {
                switch (testMethod)
                {
                    case IgnoredTestMethod ignoredTestMethod:
                        tests.Add(new Test
                        {
                            ElapsedTime = TimeSpan.Zero,
                            ReasonForIgnoring = ignoredTestMethod.ReasonForIgnoring, Name = ignoredTestMethod.Name,
                            Status = TestStatus.Ignored
                        });
                        break;
                    case SuccessfulTestMethod successfulTestMethod:
                        tests.Add(new Test
                        {
                            ElapsedTime = successfulTestMethod.ElapsedTime,
                            Name = successfulTestMethod.Name,
                            ReasonForIgnoring = string.Empty,
                            Status = TestStatus.Success
                        });
                        break;
                    case FailedTestMethod failedTestMethod:
                        tests.Add(new Test
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
}
