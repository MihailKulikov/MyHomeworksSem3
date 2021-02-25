using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Pages.Assemblies
{
    public class IndexModel : PageModel
    {
        private readonly Data.MyNUnitWebContext _context;

        public IndexModel(Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public IList<Assembly> Assembly { get;set; }

        public async Task OnGetAsync()
        {
            Assembly = await _context.Assemblies.Include("Tests").ToListAsync();
        }
    }
}
