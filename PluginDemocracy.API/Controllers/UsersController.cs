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
        private UtilityClass _utilityClass;

        public UsersController(PluginDemocracyContext context, UtilityClass utilityClass)
        {
            _context = context;
            _utilityClass = utilityClass;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUp(UserDto registeringUser)
        {
            //CREATE RESPONSE OBJECT
            PDAPIResponse apiResponse = new();

            //CHECK VALIDITY OF INPUT
            if (!ModelState.IsValid || registeringUser.Password.Length >= 7) return BadRequest(ModelState);

            //CREATE & SAVE NEW USER
            //Create new User object
            User newUser = new User(
            firstName: registeringUser.FirstName, 
            lastName: registeringUser.LastName, 
            email: registeringUser.Email, 
            hashedPassword: string.Empty, 
            phoneNumber: registeringUser.PhoneNumber, 
            address: registeringUser.Address, 
            dateOfBirth: registeringUser.DateOfBirth, 
            culture: registeringUser.Culture, 
            middleName: registeringUser.MiddleName, 
            secondLastName: registeringUser.SecondLastName);
            //hash password && assign
            PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
            newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registeringUser.Password);
            // Save the new user to the context
            _context.Users.Add(newUser);
            apiResponse.User = UserDto.ReturnUserDtoFromUser(newUser);
            try 
            {
                await _context.SaveChangesAsync();
                apiResponse.AddAlert("success", _utilityClass.Translate("NewUserCreated", newUser.Culture));
            }
            catch(Exception ex)
            {
                apiResponse.AddAlert("error", _utilityClass.Translate("UnableToCreateNewUser", newUser.Culture) + $"Error: {ex.Message}");
                //Return here if unable to save user. 
                return StatusCode(503, apiResponse);
            }
            

            //CONFIRMATION EMAIL
            // Send a confirmation email or perform other actions
            string emailConfirmationToken = Guid.NewGuid().ToString();
            newUser.EmailConfirmationToken = emailConfirmationToken;
            //Pick HTML message to send given the Culture of the user
            string emailConfirmationLink = $"{Request.Scheme}://{Request.Host}/user/{newUser.Id}/confirmemail/{newUser.EmailConfirmationToken}";
            string emailBody = $"<h1 style=\"text-align: center; color:darkgreen\">{_utilityClass.Translate("ConfirmEmailTitle", newUser.Culture)}</h1>\r\n<img src=\"https://pdstorageaccountname.blob.core.windows.net/pdblobcontainer/PluginDemocracyImage.png\" style=\"max-height: 200px; margin-left: auto; margin-right: auto; display:block;\"/>\r\n<p style=\"text-align: center;\">{_utilityClass.Translate("EmailConfirmP1", newUser.Culture)}</p>\r\n<p style=\"text-align: center;\">{_utilityClass.Translate("EmailConfirmP2", newUser.Culture)}:</p>\r\n<p style=\"text-align: center;\"><a href={emailConfirmationLink}>{_utilityClass.Translate("ConfirmEmailLink", newUser.Culture)}</a></p>";
            //Send Email
            try 
            {
                await _utilityClass.SendEmailAsync(toEmail: newUser.Email, subject: _utilityClass.Translate("ConfirmEmailTitle", newUser.Culture), body: emailBody);
            }
            catch(Exception ex)
            {
                apiResponse.AddAlert("error", ex.Message);
            }
            
            //SEND FINAL RESPONSE
            apiResponse.RedirectParameters["Title"] = _utilityClass.Translate("EmailConfirmLink", newUser.Culture);
            apiResponse.RedirectParameters["Body"] = _utilityClass.Translate("ConfirmEmailCheckInbox", newUser.Culture);
            return Ok(apiResponse);
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
