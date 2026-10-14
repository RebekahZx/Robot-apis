using System.Text.Json.Serialization;

namespace robot_controller_api.Models
{
    public class UserModel
    {
        public int Id { get; set; }
       
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        /// <summary>Plain password on create; never included in responses.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PasswordHash { get; set; }
        public string? Description { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
