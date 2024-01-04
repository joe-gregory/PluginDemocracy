using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.Translations;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.Models;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(PluginDemocracyContext context, ApiUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly ApiUtilityClass _utilityClass = utilityClass;

        [HttpPost("signup")]
        public async Task<ActionResult<PDAPIResponse>> SignUp(UserDto registeringUser)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Check validity of input
            if (!ModelState.IsValid || !(registeringUser.Password.Length >= 7)) return BadRequest(ModelState);

            //Create new User object
            User newUser = UserDto.ReturnUserFromUserDto(registeringUser);
            
            //hash password && assign
            PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
            newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registeringUser.Password);

            // Save the new user to the context
            _context.Users.Add(newUser);
            try 
            {
                await _context.SaveChangesAsync();
                apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.NewUserCreated, newUser.Culture));
            }
            catch(Exception ex)
            {
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.UnableToCreateNewUser, newUser.Culture) + $"Error: {ex.Message}");
                //Return here if unable to save user. 
                return StatusCode(503, apiResponse);
            }

            //Send confirmation email
            await _utilityClass.SendConfirmationEmail(newUser, apiResponse);

            //Attach user data to response object
            apiResponse.User = UserDto.ReturnUserDtoFromUser(newUser);

            return Ok(apiResponse);
        }
        [HttpPost("login")]
        public async Task<ActionResult<PDAPIResponse>> LogIn(LoginInfoDto loginInfo)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Look up user by email
            User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginInfo.Email);
            if(existingUser == null)
            {
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.EmailNotFound, loginInfo.Culture));
                return BadRequest(apiResponse);
            }
            loginInfo.Password ??= string.Empty;

            //Compare passwords
            PasswordHasher<User> _passwordHasher = new();
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.HashedPassword, loginInfo.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.PasswordMismatch, loginInfo.Culture));
                return BadRequest(apiResponse);
            }
            else
            {
                apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.YouHaveLoggedIn, loginInfo.Culture));
                //Convert User to UserDto
                apiResponse.User = UserDto.ReturnUserDtoFromUser(existingUser);
                //Redirect to home feed after login in or join community page if no community
                apiResponse.RedirectTo = FrontEndPages.Community;
                return Ok(apiResponse);
            }
        }
        [HttpGet("confirmemail")]
        public async Task<ActionResult<PDAPIResponse>> ConfirmEmail([FromQuery] int userId, [FromQuery] string emailConfirmationToken)
        {
            PDAPIResponse apiResponse = new()
            {
                RedirectTo = FrontEndPages.GenericMessage
            };

            User? existingUser = await _context.FindAsync<User>(userId);

            if(existingUser == null)
            {
                apiResponse.AddAlert("error", "User not found");
                apiResponse.RedirectParameters["Title"] = _utilityClass.GetAllTranslationsInNewLines("UserNotFound");
                apiResponse.RedirectParameters["Body"] = _utilityClass.GetAllTranslationsInNewLines("UserNotFoundBody");
                return BadRequest(apiResponse);
            }

            //If user does exist and confirmationToken does match
            if (existingUser.EmailConfirmationToken == emailConfirmationToken)
            {
                existingUser.EmailConfirmed = true;
                try
                {
                    await _context.SaveChangesAsync();
                    
                    //Add Messages saying the email was confirmed
                    apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.YourEmailHasBeenConfirmed, existingUser.Culture));
                    apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture);
                    apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture);
                    apiResponse.User = UserDto.ReturnUserDtoFromUser(existingUser);

                    //Send an email saying that the email address has been confirmed
                    try
                    {
                        await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture));
                        return Ok(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        apiResponse.AddAlert("error", $"Unable to send thank you for confirming email\nError:\n{ex.Message}");
                        return Ok(apiResponse);
                    }
                }
                catch(Exception ex)
                {
                    apiResponse.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                    return StatusCode(500, apiResponse);
                }
                
            }
            //If user exists but the confirmationToken does not match
            else
            {
                apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture);
                apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchBody, existingUser.Culture);
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture));
                return Ok(apiResponse);
            }
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
