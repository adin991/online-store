using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.User.Commands.Create
{
    public class CreateUserCommand : IRequest<CreateUserResultDto>
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }   
        public required string Password { get; set; }
        public Gender Gender { get; set; }
    }
}
