using API.Dto;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{

    [Authorize]
    public class MessageController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageController(IUnitOfWork unitOfWork
            , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            string userName = User.GetUserName();
            if (userName == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot message yourself");

            var seneder = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userName);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound("Recipient not found!");
            }

            var message = new Message
            {
                Sender = seneder,
                SenderUsername = seneder.UserName,
                RecipientUsername = recipient.UserName,
                Recipient = recipient,
                Content = createMessageDto.Content,
            };
            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.CompletedAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet("thread/{username}")]
        public async Task<IActionResult> GetMessageThread(string username)
        {
            string currentUsername = User.GetUserName();

            return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPage);

            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            string username = User.GetUserName();

            var message = await _unitOfWork.MessageRepository.GetMessageAsync(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == username)
            {
                message.SenderDeleted = true;
            }

            if (message.Recipient.UserName == username)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitOfWork.CompletedAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting message!");
        }
    }
}
