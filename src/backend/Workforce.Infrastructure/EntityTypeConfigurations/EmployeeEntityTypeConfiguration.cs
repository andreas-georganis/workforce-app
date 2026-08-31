using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workforce.Domain.Model;

namespace Workforce.Infrastructure.EntityTypeConfigurations;

class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x=>x.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => new EmployeeId(v))
            .HasColumnName("Id")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .IsRequired();

        builder.Property(x=>x.FirstName)
            .HasConversion(
                firstname=> firstname.Value,
                value => new FirstName(value))
            .HasMaxLength(75)
            .HasColumnName("FirstName");

        builder.Property(x=>x.LastName)
            .HasConversion(
                lastname => lastname.Value,
                value => new LastName(value))
            .HasMaxLength(150)
            .HasColumnName("LastName");

        builder.Property(x=>x.Email)
            .HasConversion(
                email=> email.Value,
                value => new Email(value))
            .HasMaxLength(200)
            .HasColumnName("Email");

        builder.HasIndex(x=>x.Email)
            .IsUnique();

        builder.OwnsMany(x=>x.Skills, skill =>
        {
            skill.Property<long>("Id").UseHiLo("employeeskillseq");

            skill.HasKey("Id");

            skill.UsePropertyAccessMode(PropertyAccessMode.Field);

            skill.WithOwner().HasForeignKey("EmployeeId");

            skill.HasIndex("EmployeeId", "SkillId").IsUnique();

            skill.Property(s => s.SkillId)
                .HasConversion(
                    id => id.Value,
                    guid => new SkillId(guid));

            skill.HasOne<Skill>()
                    .WithMany()
                    .HasForeignKey(s => s.SkillId)
                    .HasPrincipalKey(s => s.Id);

            skill.Property(x=>x.Proficiency).HasConversion<int>();

            skill.Property(x=>x.YearsOfExperience)
                .HasConversion(
                    x=>x.Value,
                    y => new YearsOfExperience(y));
            });
    }
}