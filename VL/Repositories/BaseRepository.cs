using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Contexts;

namespace Video_Library_Api.Repositories
{
    public class BaseRepository
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
