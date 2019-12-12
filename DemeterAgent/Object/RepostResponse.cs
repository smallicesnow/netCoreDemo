using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demeter.Agent.Object
{
    public class RepostResponse
    {
        public List<RepostResult> Result { get; set; }
        /// <summary>
        /// Success:0
        /// Error:1
        /// CompleteWithError:2
        /// </summary>
        public int Status { get; set; }
    }
    public class RepostResult
    {
        /// <summary>
        /// Success:0
        /// Error:1
        /// </summary>
        public int Status { get; set; }
        public string Message { get; set; }
        public string ListingId { get; set; }
    }
}
