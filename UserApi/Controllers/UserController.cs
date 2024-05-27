using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UserApi.Models;
using UserApi.Services;

namespace UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IdentityService _identityUserService;
    private readonly ProfileService _profileService;
    private readonly ILogger<UserController> _logger;

    public UserController(IdentityService identityUserService, ProfileService profileService, ILogger<UserController> logger)
    {
        _identityUserService = identityUserService;
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet("identity_users")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllIdentityUsers()
    {
        var users = await _identityUserService.GetAllUsersAsync();
        if (users == null || !users.Any())
        {
            _logger.LogWarning("No users found in Identity database.");
            return NotFound("No users found in Identity database.");
        }
        return Ok(users);
    }

    [HttpGet("account_users")]
    public async Task<ActionResult<IEnumerable<ProfileModel>>> GetAllAccountUsers()
    {
        var profiles = await _profileService.GetAllProfilesAsync();
        if (profiles == null || !profiles.Any())
        {
            _logger.LogWarning("No profiles found in AccountUser database.");
            return NotFound("No profiles found in AccountUser database.");
        }
        return Ok(profiles);
    }

    [HttpGet("all_users")]
    public async Task<ActionResult<object>> GetAllUsers()
    {
        var identityUsers = await _identityUserService.GetAllUsersAsync();
        var accountUsers = await _profileService.GetAllProfilesAsync();

        if (identityUsers == null || !identityUsers.Any())
        {
            _logger.LogWarning("No users found in Identity database.");
        }

        if (accountUsers == null || !accountUsers.Any())
        {
            _logger.LogWarning("No profiles found in AccountUser database.");
        }

        var allUsers = accountUsers.Join(identityUsers,
       profile => profile.IdentityUserId,
       identity => identity.Id,
       (profile, identity) => new ProfileModel
       {
           IdentityUserId = profile.IdentityUserId,
           FirstName = profile.FirstName,
           LastName = profile.LastName,
           Email = identity.Email
       }).ToList();

        if (!allUsers.Any())
        {
            _logger.LogWarning("No matched users found after join operation.");
        }


        return Ok(allUsers);
    }
}

