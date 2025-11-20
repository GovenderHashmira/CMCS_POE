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
        public string DocumentName { get; set; } 

        [Required]
        public string DocumentPath { get; set; }  

        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
