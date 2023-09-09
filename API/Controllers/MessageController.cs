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
        private readonly IUserReposetory _userReposetory;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageController(IUserReposetory userReposetory
            , IMessageRepository messageRepository
            , IMapper mapper)
        {
            _userReposetory = userReposetory;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            string userName =  User.GetUserName();
            if (userName == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot message yourself");

            var seneder = await _userReposetory.GetUserByUsernameAsync(userName);
            var recipient = await _userReposetory.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

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
            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet("thread/{username}")]
        public async Task<IActionResult> GetMessageThread(string username)
        {
            string currentUsername = User.GetUserName();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPage);

            return Ok(messages);
        }
    }
}
