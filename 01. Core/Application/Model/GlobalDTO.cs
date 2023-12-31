using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class AuthorizeDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class PrivateActionDTO
    {
        public int Age { get; set; }
        public string FullName { get; set; }
    }

    public class PublicActionDTO
    {
        public int Age { get; set; }
        public string FullName { get; set; }
    }
}
