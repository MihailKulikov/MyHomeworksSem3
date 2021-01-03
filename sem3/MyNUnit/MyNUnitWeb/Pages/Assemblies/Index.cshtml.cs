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
    public class IndexModel : PageModel
    {
        private readonly MyNUnitWeb.Data.MyNUnitWebContext _context;

        public IndexModel(MyNUnitWeb.Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public IList<Assembly> Assembly { get;set; }

        public async Task OnGetAsync()
        {
            Assembly = await _context.Assemblies.ToListAsync();
        }
    }
}
