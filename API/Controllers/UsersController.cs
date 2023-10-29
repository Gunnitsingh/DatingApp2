using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper,IPhotoService photoService)
        {
           
            UserRepository = userRepository;
            _mapper = mapper;
            this.photoService = photoService;
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
            var user = await UserRepository.GetUserByNameAsync(User.GetUsername());
            if(user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            if(await UserRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fail to save changes");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("delete-photo/{id}")]
        public async Task<ActionResult> DeletePhoto(int id)
        {
            var user = await UserRepository.GetUserByNameAsync(User.GetUsername());
            if(user == null) return NotFound();
            var photo = user.Photos.Find(x=>x.Id == id);
            if(photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Main photo cannot be deleted");
            user.Photos.Remove(photo);
            await UserRepository.SaveAllAsync();
            return Ok();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await UserRepository.GetUserByNameAsync(User.GetUsername());

            if(user == null) return NotFound();

             var results  = await photoService.AddPhotoAsync(file);

            if (results.Error != null) return BadRequest();

            var photo = new Photo
            {
                Url = results.Url.AbsoluteUri,
                PublicId = results.PublicId,
            };

            if(user.Photos.Count == 0)
            {
               photo.IsMain = true;
            }
            user.Photos.Add(photo);

            if(await UserRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName },
                    _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Photos cannot be save");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await UserRepository.GetMemberByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("Already main");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            

            await UserRepository.SaveAllAsync();

            return BadRequest("Issue setting main photo");
        }

       
    }
}
