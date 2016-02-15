using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Casio.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student")]
        public string StudentId { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Range(0, 5)]
        [Display(Name = "Grade")]
        public int? Grade { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime DateModified { get; set; }

        public virtual Course Course { get; set; }

        public virtual Student Student { get; set; }
    }
}