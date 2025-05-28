using System;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Extensions;
using API.Helpers;

namespace API.Controllers;


[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }

    /*[HttpGet("{username}")]   // /api/users/3
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await unitOfWork.UserRepository.GetMemberAsync(username);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }*/
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        /*var currentUsername = User.GetUsername();
        return await unitOfWork.UserRepository.GetMemberAsync(username,
        isCurrentUser: currentUsername == username
        );*/
        var currentUsername = User.GetUsername();
        var isCurrentUser = currentUsername == username;

        var user = await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);

        if (user == null) return NotFound();

        return Ok(user);
    }

    // api/users
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {


        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Cound not find user");

        mapper.Map(memberUpdateDto, user);

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await
 unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot update user");
        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        user.Photos.Add(photo);
        if (await unitOfWork.Complete())
            return CreatedAtAction(nameof(GetUser),
            new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem setting main photo");
    }


    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await
       unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Problem deleting photo");
    }


    [Authorize(Policy = "RequireVipRole")]
    [HttpPost("visited/{username}")]
    public async Task<ActionResult> TrackVisit(string username)
    {
        var sourceUsername = User.GetUsername();

        if (sourceUsername == username.ToLower()) return BadRequest("You cannot visit your own profile");

        var sourceUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(sourceUsername);
        var targetUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (targetUser == null || sourceUser == null) return NotFound("User not found");

        var visit = await unitOfWork.VisitRepository.GetVisitAsync(sourceUser.Id, targetUser.Id);

        if (visit == null)
        {
            visit = new Visit
            {
                SourceUserId = sourceUser.Id,
                TargetUserId = targetUser.Id,
                LastVisited = DateTime.UtcNow
            };

            unitOfWork.VisitRepository.AddVisit(visit);
        }
        else
        {
            visit.LastVisited = DateTime.UtcNow;
        }

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem tracking profile visit");
    }

    [Authorize]
    [HttpGet("visited")]
    public async Task<ActionResult<IEnumerable<VisitDto>>> GetVisits()
    {
        var userId = User.GetUserId();
        var visits = await unitOfWork.VisitRepository.GetVisitsByUserIdAsync(userId);

        return Ok(visits.Select(v => new VisitDto
        {
            Username = v.TargetUser!.UserName,
            KnownAs = v.TargetUser.KnownAs,
            VisitedOn = v.LastVisited
        }));
    }
    [Authorize(Roles = "VIP")]
    [HttpGet("visits")]
    public async Task<ActionResult<PagedList<VisitDto>>> GetUserVisits([FromQuery] VisitsParams visitsParams)
    {
        var userId = User.GetUserId();
        var visits = await unitOfWork.UserRepository.GetUserVisits(visitsParams, userId);

        //Response.AddPaginationHeader(visits.CurrentPage, visitsParams.PageSize, visits.TotalCount, visits.TotalPages);
        Response.AddPaginationHeader(visits);
        return Ok(visits);
    }


    [Authorize]
    [HttpPost("visit/{username}")]
    public async Task<ActionResult> AddVisit(string username)
    {
        var sourceUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        var targetUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (targetUser == null || sourceUser == null) return NotFound();

        var visit = await unitOfWork.UserRepository.GetVisit(sourceUser.Id, targetUser.Id);

        if (visit == null)
        {
            visit = new Visit
            {
                SourceUserId = sourceUser.Id,
                TargetUserId = targetUser.Id,
                LastVisited = DateTime.UtcNow
            };
            unitOfWork.UserRepository.AddVisit(visit); 
        }
        else
        {
            visit.LastVisited = DateTime.UtcNow;
        }

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem tracking visit");
    }



    /*[Authorize(Roles = "Member,VIP")]
    [HttpPost("visit/{username}")]
    public async Task<ActionResult> AddVisit(string username)
    {
        var sourceUserId = User.GetUserId();
        var targetUser = await _userRepository.GetUserByUsernameAsync(username);

        if (targetUser == null) return NotFound();

        var visit = await _userRepository.GetVisit(sourceUserId, targetUser.Id);

        if (visit == null)
        {
            visit = new Visit
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUser.Id,
                LastVisited = DateTime.UtcNow
            };
            context.Visits.Add(visit);
        }
        else
        {
            visit.LastVisited = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return Ok();
    }*/
}
