using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Test.Grpc.DAL.Entities;

[Table(nameof(User), Schema = "data")]
[Index(nameof(Id), IsUnique = true)]
public class User
{
    public User(string nickName, string firstName = "", int age = default)
    {
        NickName = nickName;
        FirstName = firstName;
        Age = age;
    }

    [Key]
    [ConcurrencyCheck]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; protected set; }

    [Required] public string NickName { get; set; }
    public string FirstName { get; set; }
    public int Age { get; set; }
}