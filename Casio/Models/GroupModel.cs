using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Casio.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "Group Name")]
        public string Name { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}