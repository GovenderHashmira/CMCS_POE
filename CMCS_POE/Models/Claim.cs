using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CMCS_POE.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LecturerId { get; set; }

        [ForeignKey("LecturerId")]
        public AppUser Lecturer { get; set; }

        [Required]
        [Range(0, 180, ErrorMessage = "Hours worked cannot exceed 180 per month.")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPayment { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; 

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public DateTime? ApprovalDate { get; set; }

        public string? Notes { get; set; }

        public ICollection<DocumentUpload> DocumentUploads { get; set; }

        [NotMapped]
        public IFormFile Document { get; set; }

        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }

    }
}
