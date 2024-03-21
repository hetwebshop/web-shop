using API.Data;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private readonly IUnitOfWork _uow;
        public AdminController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet("moderate/admin-role")]
        //public async Task<ActionResult> GetModeratorsForAdmin([FromQuery] BaseRoleParams roleParams)
        //{
        //    var userId = HttpContext.User.GetUserId();
        //    return Ok(await _uow.RoleRepository.GetModeratorsForAdmin(userId, roleParams));
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost("moderate/admin-role")]
        //public async Task<ActionResult> AddModeratorByAdmin(BaseRoleDto roleDto)
        //{
        //    var userId = HttpContext.User.GetUserId();
        //    return Ok(await _uow.RoleRepository.AddModeratorByAdmin(userId, roleDto));
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpDelete("moderate/admin-role")]
        //public async Task<ActionResult> RemoveModeratorByAdmin(BaseRoleDto roleDto)
        //{
        //    var userId = HttpContext.User.GetUserId();

        //    if (roleDto.UserId == userId)
        //        return BadRequest("You cannot remove your role");

        //    await _uow.RoleRepository.RemoveModeratorByAdmin(userId, roleDto);
        //    return Ok();
        //}
    }
}
