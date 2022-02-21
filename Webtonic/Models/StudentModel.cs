using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Webtonic.Models
{
    public class StudentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }
        [DisplayName("Student Number")]
        [Required]
        public string StudentNumber { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [DisplayName("Course Code")]
        public string CourseCode { get; set; }
        [Required]
        [DisplayName("Course Description")]
        public string CourseDescription { get; set; }
        [Required]
        public string Grade { get; set; }
    }
}
