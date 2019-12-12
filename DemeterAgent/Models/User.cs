using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demeter.Agent.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(3)]
        public int Age { get; set; }
    }
}
