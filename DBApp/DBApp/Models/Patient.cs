using System.ComponentModel.DataAnnotations;

namespace DBApp.Models
{
    public class Patient
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]  
        public DateTime CreatedDate { get; set;}

        [Required]
        public int duration { get; set; }

        [Required]
        public string doctorname { get; set; }

    
    }
}
