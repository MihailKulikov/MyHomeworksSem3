using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class DetailsModel : PageModel
    {
        private readonly MyNUnitWeb.Data.MyNUnitWebContext _context;

        public DetailsModel(MyNUnitWeb.Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public Assembly Assembly { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Assembly = await _context.Assemblies.FirstOrDefaultAsync(m => m.Id == id);

            if (Assembly == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
