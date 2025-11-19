using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_POE.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to the Lecturer (AppUser)
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
        public string Status { get; set; } = "Pending"; // Default status

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public DateTime? ApprovalDate { get; set; }

        public string? Notes { get; set; }

        // Navigation property for uploaded documents
        public ICollection<DocumentUpload> DocumentUploads { get; set; }
    }
}
