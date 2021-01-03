using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyNUnitWeb.Data;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class CreateModel : PageModel
    {
        private readonly MyNUnitWeb.Data.MyNUnitWebContext _context;

        public CreateModel(MyNUnitWeb.Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Assembly Assembly { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Assemblies.Add(Assembly);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
