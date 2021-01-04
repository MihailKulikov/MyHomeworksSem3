using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class DetailsModel : PageModel
    {
        private readonly Data.MyNUnitWebContext _context;

        public DetailsModel(Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public Assembly Assembly { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Assembly = await _context.Assemblies.Include("Tests").FirstOrDefaultAsync(m => m.AssemblyId == id);

            if (Assembly == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
