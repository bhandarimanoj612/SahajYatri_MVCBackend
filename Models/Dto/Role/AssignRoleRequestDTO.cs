namespace Sahaj_Yatri.Models.Dto.Role
{
    public class AssignRoleRequestDTO
    {
        public string UserName { get; set; }
        public string Role { get; set; }

        public bool AssignRole { get; set; } // Indicates whether to assign or remove the role
    
}
}
