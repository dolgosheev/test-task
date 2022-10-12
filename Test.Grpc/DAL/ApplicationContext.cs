using Microsoft.EntityFrameworkCore;

using Test.Grpc.DAL.Entities;

namespace Test.Grpc.DAL;

public class ApplicationContext : DbContext
{
    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public virtual DbSet<User>? Users { get; set; }
}