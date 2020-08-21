using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
using Soccer.Common.Enums;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[Controller]")]
    public class AccountController: ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;
        private readonly IImageHelper _imageHelper;

        public AccountController(DataContext dataContext,IUserHelper userHelper,
            IMailHelper mailHelper,IConfiguration configuration,IConverterHelper converterHelper, IImageHelper imageHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _converterHelper = converterHelper;
            _configuration = configuration;
            _imageHelper = imageHelper;
        }

       [HttpPost]
       public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess=false,
                    Message="Bad Request",
                    Result=ModelState
                });
            }

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = Resource.UserAlreadyExists
                });
            }

            string picturePath = string.Empty;
            if(request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            user = new UserEntity
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email,
                PicturePath = picturePath,
                UserType = UserType.User,
                Team = await _dataContext.Teams.FindAsync(request.TeamId)
            };
            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if(result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            UserEntity userNew = await _userHelper.GetUserAsync(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, user.UserType.ToString());

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email, Resource.ConfirmEmail, $"<h1>{Resource.ConfirmEmail}</h1>" +
               $"{Resource.ConfirmEmailSubject}</br></br><a href = \"{tokenLink}\">{Resource.ConfirmEmail}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = Resource.ConfirmEmailMessage
            });
        }
    }
}
