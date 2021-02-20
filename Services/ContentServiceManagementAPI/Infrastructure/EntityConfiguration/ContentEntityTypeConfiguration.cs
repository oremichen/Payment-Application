using ContentServiceManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ContentServiceManagementAPI.Infrastructure.EntityConfiguration
{
    public class ContentEntityTypeConfiguration
        : IEntityTypeConfiguration <Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ToTable("Content");

            builder.HasKey(ci => ci.ContentId);

            builder.Property(cb => cb.ContentProviderId)
                .IsRequired();
            builder.Property(cb => cb.ContentPrivacyType)
               .IsRequired();


        }
    }
}
