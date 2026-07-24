using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workforce.Domain.Model;

namespace Workforce.Infrastructure.EntityTypeConfigurations;

class SkillEntityTypeConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(x=>x.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => new SkillId(v))
            .HasColumnName("Id")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .IsRequired();

        builder.Property(x=>x.Name)
            .HasConversion(
                name=> name.Value,
                value => new SkillName(value))
            .HasMaxLength(75)
            .HasColumnName("Name");
    }
}

