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
        private readonly RlsSecurityContext _securityContext;

        public RlsController(EfTestsContext context, RlsSecurityContext securityContext)
        {
            _context = context;
            _securityContext = securityContext;
        }

        [HttpPost]
        public History AddHtoHistory(History history)
        {
            history.EventTime = DateTime.Now;
            history.Owner = _securityContext.Owner;
            _context.Add(history);
            _context.SaveChanges();
            return history;
        }

        [HttpGet]
        public List<History> History()
        {
            return _context.History.ToList();
        }
    }
}