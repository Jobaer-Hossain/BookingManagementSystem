using BookingManagement.MVC.Data;
using BookingManagement.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.MVC.Controllers
{
    public class ConsultantsController : Controller
    {
        private readonly AppDbContext _context;

        public ConsultantsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var consultants = await _context.Consultants
                .AsNoTracking()
                .OrderBy(consultant => consultant.Name)
                .ToListAsync();

            return View(consultants);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants
                .AsNoTracking()
                .FirstOrDefaultAsync(consultant => consultant.Id == id.Value);

            return consultant is null ? NotFound() : View(consultant);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("Name,Email,Phone,Specialization,AppointmentDuration")] Consultant consultant)
        {
            if (!ModelState.IsValid)
            {
                return View(consultant);
            }

            Normalize(consultant);

            if (await EmailExistsAsync(consultant.Email))
            {
                ModelState.AddModelError(
                    nameof(Consultant.Email),
                    "A consultant with this email already exists.");

                return View(consultant);
            }

            var now = DateTime.UtcNow;
            consultant.AddedAt = now;
            consultant.UpdatedAt = now;
            consultant.Status = true;

            _context.Consultants.Add(consultant);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consultant added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants.FindAsync(id.Value);

            return consultant is null ? NotFound() : View(consultant);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Email,Phone,Specialization,AppointmentDuration")] Consultant input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var consultant = await _context.Consultants.FindAsync(id);

            if (consultant is null)
            {
                return NotFound();
            }

            Normalize(input);

            if (await EmailExistsAsync(input.Email, id))
            {
                ModelState.AddModelError(
                    nameof(Consultant.Email),
                    "A consultant with this email already exists.");

                return View(input);
            }

            consultant.Name = input.Name;
            consultant.Email = input.Email;
            consultant.Phone = input.Phone;
            consultant.Specialization = input.Specialization;
            consultant.AppointmentDuration = input.AppointmentDuration;
            consultant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consultant updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants
                .AsNoTracking()
                .FirstOrDefaultAsync(consultant => consultant.Id == id.Value);

            return consultant is null ? NotFound() : View(consultant);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consultant = await _context.Consultants.FindAsync(id);

            if (consultant is null)
            {
                TempData["ErrorMessage"] = "The consultant no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Appointments.AnyAsync(
                appointment => appointment.ConsultantId == id))
            {
                TempData["ErrorMessage"] =
                    "This consultant has appointments and cannot be deleted.";

                return RedirectToAction(nameof(Index));
            }

            _context.Consultants.Remove(consultant);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consultant deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> EmailExistsAsync(string email, int? excludedId = null)
        {
            return _context.Consultants.AnyAsync(consultant =>
                consultant.Email == email &&
                (!excludedId.HasValue || consultant.Id != excludedId.Value));
        }

        private static void Normalize(Consultant consultant)
        {
            if (consultant.Name is not null)
            {
                consultant.Name = consultant.Name.Trim();
            }

            if (consultant.Email is not null)
            {
                consultant.Email = consultant.Email.Trim().ToLowerInvariant();
            }

            if (consultant.Phone is not null)
            {
                consultant.Phone = consultant.Phone.Trim();
            }

            if (consultant.Specialization is not null)
            {
                consultant.Specialization = consultant.Specialization.Trim();
            }
        }
    }
}