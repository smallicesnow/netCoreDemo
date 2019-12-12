using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demeter.Agent.Object
{
    public class RepostRequest
    {
        public string email { get; set; }
        public string password { get; set; }
        public List<string> listings { get; set; }
        public string accessToken { get; set; }
    }
}
