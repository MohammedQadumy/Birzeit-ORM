using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BirzeitUniversity.Data;
using BirzeitUniversity.Models;
using Microsoft.Extensions.Configuration;


namespace BirzeitUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly BirzeitUniversity.Data.SchoolContext _context;
        private readonly IConfiguration Configuration;


        public IndexModel(BirzeitUniversity.Data.SchoolContext context , IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

     

        public PaginatedList<Student> Students { get;set; }

        public async Task OnGetAsync(string sortOrder,string searchString , string currentFilter , int? pageIndex)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            CurrentSort = sortOrder;
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Student> studentsIQ = from s in _context.Students select s;

            if (!string.IsNullOrEmpty(searchString)) {
                studentsIQ = studentsIQ.Where(s => s.LastName.Contains(searchString) 
                || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder) {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName);
                break;
                case "Date":
                    studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
                break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName);
                break;
                default:
                    studentsIQ = studentsIQ.OrderBy(s=> s.LastName);
                break;
            }

            var pageSize = Configuration.GetValue("PageSize", 4);
            Students = await PaginatedList<Student>.CreateAsync(
                studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
