using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Pages.Tests
{
    public class ViewTestsModel : PageModel
    {
        private readonly MyNUnitWeb.Data.MyNUnitWebContext _context;

        public ViewTestsModel(MyNUnitWeb.Data.MyNUnitWebContext context)
        {
            _context = context;
        }

        public IList<Test> Test { get;set; }

        public async Task OnGetAsync()
        {
            Test = await _context.Test.ToListAsync();
        }
    }
}
