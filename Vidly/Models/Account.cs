namespace Vidly.Models
{
  public class Role
  {
    public int Id { get; set; }
    public string Description { get; set; }
  }

  public class Account
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
  }
}