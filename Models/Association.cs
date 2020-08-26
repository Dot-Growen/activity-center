using System.ComponentModel.DataAnnotations;

namespace belt_exam.Models {
    public class Association {

        [Key]
        public int AssociationId { get; set; }
        
        public int ActivityId { get; set; }

        public Activity ActivityUsers { get; set; }

        public int UserId { get; set; }

        public User UsersActivity { get; set; }

    }
}