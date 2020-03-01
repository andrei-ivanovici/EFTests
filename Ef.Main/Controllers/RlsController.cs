using System;
using System.Collections.Generic;
using System.Linq;
using Ef.Main.Data;
using Microsoft.AspNetCore.Mvc;

namespace Ef.Main.Controllers
{
    [ApiController]
    [Route("rls")]
    public class RlsController : ControllerBase
    {
        private readonly EfTestsContext _context;

        public RlsController(EfTestsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public void AddHtoHistory(History history)
        {
            history.EventTime = DateTime.Now;
            _context.Add(history);
        }

        [HttpGet]
        public List<History> History()
        {
            return _context.History.ToList();
        }
    }
}