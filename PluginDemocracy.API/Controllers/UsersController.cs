using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.Models;
using PluginDemocracy.Data;
using PluginDemocracy.Models;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PluginDemocracyContext _context;

        public UsersController(PluginDemocracyContext context)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUp(UserDto registeringUser)
        {
            if (!ModelState.IsValid || registeringUser.Password.Length >= 7) return BadRequest(ModelState);

            //Create new User object
            User newUser = new User(
            firstName: registeringUser.FirstName, 
            lastName: registeringUser.LastName, 
            email: registeringUser.Email, 
            hashedPassword: null, 
            phoneNumber: registeringUser.PhoneNumber, 
            address: registeringUser.Address, 
            dateOfBirth: registeringUser.DateOfBirth, 
            culture: registeringUser.Culture, 
            middleName: registeringUser.MiddleName, 
            secondLastName: registeringUser.SecondLastName);

            //hash password
            PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
            newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registeringUser.Password);


            // Add the new user to the context
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Optionally, send a confirmation email or perform other actions
            // ...

            // Return the created user with a 'CreatedAtAction' response
            return CreatedAtAction("GetUser", new { id = newUser.Id }, newUser);
        }

        //SCAFFOLDING: 
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
