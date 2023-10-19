using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Authorize]   
    public class UsersController : BaseApiController
    {

        public readonly IUserRepository UserRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
           
            UserRepository = userRepository;
            _mapper = mapper;
        }
        // GET: api/<UsersController>
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            return Ok(await UserRepository.GetMembersAsync());

            
        }

        // GET api/<UsersController>/5
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
           var user =  await UserRepository.GetMemberByUsernameAsync(username);  

            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await UserRepository.GetUserByNameAsync(username);
            if(user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            if(await UserRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fail to save changes");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
