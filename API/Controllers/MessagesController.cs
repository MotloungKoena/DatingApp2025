using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController(IMessageRepository messageRepository, 
        IUserRepository userRepository, IMapper mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot message yourself");

            var sender = await userRepository.GetUserByUsernameAsync(username);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null || sender == null) return BadRequest("Cannot send message at this time");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Failed to save message");
        }


        /*[HttpPost]
public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
{
    var username = User.GetUsername();

    if (string.IsNullOrWhiteSpace(createMessageDto.RecipientUsername))
        return BadRequest("Recipient username is required");

    if (username.ToLower() == createMessageDto.RecipientUsername.ToLower())
        return BadRequest("You cannot message yourself");

    // 👇 Get sender and recipient from DB
    var sender = await userRepository.GetUserByUsernameAsync(username);
    var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

    // 👇 Print debug info to terminal
    Console.WriteLine("Sender username from token: " + username);
    Console.WriteLine("Recipient from DTO: " + createMessageDto.RecipientUsername);
    Console.WriteLine("Sender loaded from DB: " + (sender?.UserName ?? "null"));
    Console.WriteLine("Recipient loaded from DB: " + (recipient?.UserName ?? "null"));

    if (sender == null || recipient == null)
        return BadRequest("Cannot send message at this time");

    var message = new Message
    {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDto.Content
    };

    messageRepository.AddMessage(message);

    if (await messageRepository.SaveAllAsync())
        return Ok(mapper.Map<MessageDto>(message));

    return BadRequest("Failed to save message");
}*/



        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await messageRepository.GetMessageThread(currentUsername, username));
        }

         [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await messageRepository.GetMessage(id);

            if (message == null) return BadRequest("Cannot delete this message");

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Forbid();

            if (message.SenderUsername == username) message.SenderDeleted = true;

            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message is {SenderDeleted: true, RecipientDeleted: true})
            { 
                messageRepository.DeleteMessage(message);
            }

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}