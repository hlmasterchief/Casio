using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Casio.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "Course code")]
        public string Code { get; set; }

        [Required]
        [StringLength(80)]
        [Display(Name = "Course name")]
        public string Name { get; set; }

        public string FullName { get { return Code + " " + Name; } }

        [Range(1, 30)]
        [Display(Name = "ETCS credits")]
        public int Credit { get; set; }

        [StringLength(160)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Teacher")]
        public string TeacherId { get; set; }

        public virtual Teacher Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}