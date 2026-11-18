using BookingManagement.MVC.Data;
using BookingManagement.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.MVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.FullName)
                .ToListAsync();

            return View(customers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Id == id);

            return customer is null ? NotFound() : View(customer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FullName,Email,Phone,Address")] Customer customer)
        {
            Normalize(customer);
            ModelState.Clear();

            if (!TryValidateModel(customer))
            {
                return View(customer);
            }

            if (await EmailExists(customer.Email))
            {
                ModelState.AddModelError(nameof(Customer.Email), "A customer with this email already exists.");
                return View(customer);
            }

            var now = DateTime.UtcNow;
            customer.AddedAt = now;
            customer.UpdatedAt = now;
            customer.Status = true;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            return customer is null ? NotFound() : View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,FullName,Email,Phone,Address")] Customer input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            Normalize(input);
            ModelState.Clear();

            if (!TryValidateModel(input))
            {
                return View(input);
            }

            if (await EmailExists(input.Email, input.Id))
            {
                ModelState.AddModelError(nameof(Customer.Email), "A customer with this email already exists.");
                return View(input);
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            customer.FullName = input.FullName;
            customer.Email = input.Email;
            customer.Phone = input.Phone;
            customer.Address = input.Address;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Id == id);

            return customer is null ? NotFound() : View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                TempData["ErrorMessage"] = "The customer no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Appointments.AnyAsync(appointment => appointment.CustomerId == id))
            {
                TempData["ErrorMessage"] = "This customer has appointments and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmailExists(string email, int? excludedCustomerId = null)
        {
            return await _context.Customers.AnyAsync(customer =>
                customer.Email == email &&
                (!excludedCustomerId.HasValue || customer.Id != excludedCustomerId.Value));
        }

        private static void Normalize(Customer customer)
        {
            customer.FullName = customer.FullName.Trim();
            customer.Email = customer.Email.Trim().ToLowerInvariant();
            customer.Phone = customer.Phone.Trim();
            customer.Address = customer.Address.Trim();
        }
    }
}
