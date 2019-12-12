using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demeter.Agent.Object
{
    public class LoginResult
    {
        public string stat { get; set; }
        public LoginError errors { get; set; }
    }
    public class LoginError
    {
        public string action_error { get; set; }
    }
}
