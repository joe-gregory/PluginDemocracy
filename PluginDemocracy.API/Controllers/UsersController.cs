﻿using System;
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
    public class UsersController(PluginDemocracyContext context, UtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private UtilityClass _utilityClass = utilityClass;

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

            //Email confirmation token
            newUser.EmailConfirmationToken = Guid.NewGuid().ToString();
            
            // Save the new user to the context
            _context.Users.Add(newUser);
            apiResponse.User = UserDto.ReturnUserDtoFromUser(newUser);
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
            string emailConfirmationLink = $"{Request.Scheme}://{Request.Host}/user/{newUser.Id}/confirmemail/{newUser.EmailConfirmationToken}";
            string emailBody = $"<h1 style=\"text-align: center; color:darkgreen\">{_utilityClass.Translate(ResourceKeys.ConfirmEmailTitle, newUser.Culture)}</h1>\r\n<img src=\"https://pdstorageaccountname.blob.core.windows.net/pdblobcontainer/PluginDemocracyImage.png\" style=\"max-height: 200px; margin-left: auto; margin-right: auto; display:block;\"/>\r\n<p style=\"text-align: center;\">{_utilityClass.Translate(ResourceKeys.ConfirmEmailP1, newUser.Culture)}</p>\r\n<p style=\"text-align: center;\">{_utilityClass.Translate(ResourceKeys.ConfirmEmailP2, newUser.Culture)}:</p>\r\n<p style=\"text-align: center;\"><a href={emailConfirmationLink}>{_utilityClass.Translate(ResourceKeys.ConfirmEmailLink, newUser.Culture)}</a></p>";
            //Send Email
            try 
            {
                await _utilityClass.SendEmailAsync(toEmail: newUser.Email, subject: _utilityClass.Translate(ResourceKeys.ConfirmEmailTitle, newUser.Culture), body: emailBody);
            }
            catch(Exception ex)
            {
                apiResponse.AddAlert("error", $"Error sending confirmation email: {ex.Message}");
            }
            
            //Redirect to generic message page and add a message
            apiResponse.RedirectTo = FrontEndPages.GenericMessage;
            apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.ConfirmEmailTitle, newUser.Culture);
            apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.ConfirmEmailCheckInbox, newUser.Culture);
            
            return Ok(apiResponse);
        }

        [HttpGet("{id}/confirmemail/{emailConfirmationToken}")]
        public async Task<ActionResult<PDAPIResponse>> ConfirmEmail(int id, string emailConfirmationToken)
        {
            PDAPIResponse apiResponse = new();
            apiResponse.RedirectTo = FrontEndPages.GenericMessage;

            User? existingUser = await _context.FindAsync<User>(id);

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
                apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.YourEmailHasBeenConfirmed, existingUser.Culture));

                //Add message parameters & redirect to generic message page 
                
                apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture);
                apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture);

                //Send an email saying that the email address has been confirmed
                try
                {
                    await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture));
                    return Ok(apiResponse);
                }
                catch(Exception ex)
                {
                    apiResponse.AddAlert("error", $"Unable to send thank you for confirming email\nError:\n{ex.Message}");
                    return Ok(apiResponse);
                }
            }
            //If user is not null but the confirmationToken does not match
            else
            {
                apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture);
                apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchBody, existingUser.Culture);
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture));
                return BadRequest(apiResponse);
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
