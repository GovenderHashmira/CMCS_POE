using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_POE.Models
{
    public class DocumentUpload
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public Claim Claim { get; set; }

        [Required]
        public string DocumentName { get; set; }  // Name of the uploaded file

        [Required]
        public string DocumentPath { get; set; }  // Path where the file is stored

        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
