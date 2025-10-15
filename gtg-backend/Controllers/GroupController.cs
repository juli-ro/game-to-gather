using System.Security.Claims;
using AutoMapper;
using gtg_backend.Data;
using gtg_backend.Dtos;
using gtg_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace gtg_backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GroupController : BaseController<Group, GroupDto>
{
    public GroupController(GameDbContext context, IMapper mapper) : base(context, mapper, context.Groups)
    {
    }

    [HttpGet("UserGroup")]
    public async Task<IActionResult> GetUserGroupsByUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        
        List<Group> groupList = await _dbSet
            .Include(group => group.GroupUsers)
            .Where(group => group.GroupUsers.Any(groupUser => groupUser.UserId == new Guid(userId)))
            .ToListAsync();
        
        List<GroupDto>? groupDtoList = _mapper.Map<List<GroupDto>>(groupList);
        
        return Ok(groupDtoList);
    }

    [HttpPost("UserGroup")]
    public async Task<IActionResult> CreateGroup()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == new Guid(userId));

        if (user == null)
        {
            return BadRequest();
        }

        var group = new Group{Name = "New Group"};
        group.GroupUsers = new List<GroupUser>{new GroupUser{UserId = user.Id}};
        
        EntityEntry<Group> entry = _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return Ok(entry.Entity.Id);
    }
}