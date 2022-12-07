using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivesHelpdesk.Ui.WebApp.Data;
using VivesHelpdesk.Ui.WebApp.Models;

namespace VivesHelpdesk.Ui.WebApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly VivesHelpdeskDbContext _dbContext;

        public TicketController(VivesHelpdeskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(int? assignedToId)
        {
            //var tickets = _dbContext.Tickets
            //    .Where(t => !assignedToId.HasValue || t.AssignedToId == assignedToId)
            //    .Include(t => t.AssignedTo)
            //    .ToList();

            var query = _dbContext.Tickets.AsQueryable();

            if (assignedToId.HasValue)
            {
                query = query.Where(t => t.AssignedToId == assignedToId);
            }

            var tickets = query
                .Include(t => t.AssignedTo)
                .ToList();

            return View(tickets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm]Ticket ticket)
        {
            //Validate
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            //Execute
            ticket.CreatedDate = DateTime.UtcNow;

            _dbContext.Tickets.Add(ticket);

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit([FromRoute]int id)
        {
            var ticket = _dbContext.Tickets
                .SingleOrDefault(t => t.Id == id);

            if(ticket is null)
            {
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute]int id, [FromForm]Ticket ticket)
        {
            //Validate
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            var dbTicket = _dbContext.Tickets
                .SingleOrDefault(t => t.Id == id);

            if(dbTicket is null)
            {
                return RedirectToAction("Index");
            }

            dbTicket.Title = ticket.Title;
            dbTicket.Description = ticket.Description;
            dbTicket.Author = ticket.Author;

            _dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ticket = _dbContext.Tickets
                .SingleOrDefault(t => t.Id == id);

            return View(ticket);
        }

        [HttpPost]
        [Route("[controller]/Delete/{id:int?}")]
        public IActionResult DeleteConfirmed(int id)
        {
            var ticket = _dbContext.Tickets
                .SingleOrDefault(t => t.Id == id);

            if(ticket is null)
            {
                return RedirectToAction("Index");
            }

            _dbContext.Tickets.Remove(ticket);

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
