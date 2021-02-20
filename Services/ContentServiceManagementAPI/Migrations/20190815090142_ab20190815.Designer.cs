﻿// <auto-generated />
using System;
using ContentServiceManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContentServiceManagementAPI.Migrations
{
    [DbContext(typeof(ANQContentServiceManageDb))]
    [Migration("20190815090142_ab20190815")]
    partial class ab20190815
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContentServiceManagementAPI.Models.Content", b =>
                {
                    b.Property<long>("ContentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ApplicationId");

                    b.Property<long>("CommandRecordId");

                    b.Property<int>("ContentPrivacyType");

                    b.Property<int>("ContentProviderId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.HasKey("ContentId");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.ContentProvider", b =>
                {
                    b.Property<long>("ContentProviderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountEmail");

                    b.Property<string>("ApplicationId");

                    b.Property<long>("AuthenticationId");

                    b.Property<string>("ContactPersonEmail");

                    b.Property<string>("ContactPersonFirstName");

                    b.Property<string>("ContactPersonLastName");

                    b.Property<string>("ContactPersonPhoneNumber");

                    b.Property<string>("ContentProviderName");

                    b.Property<int>("Status");

                    b.HasKey("ContentProviderId");

                    b.ToTable("ContentProvider");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.DomainModels.Client", b =>
                {
                    b.Property<long>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountEmail");

                    b.Property<string>("ApplicationId");

                    b.Property<long>("AuthenticationId");

                    b.Property<string>("ClientName");

                    b.Property<string>("ContactPersonEmail");

                    b.Property<string>("ContactPersonFirstName");

                    b.Property<string>("ContactPersonLastName");

                    b.Property<string>("ContactPersonPhoneNumber");

                    b.Property<int>("Status");

                    b.HasKey("ClientId");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.DomainModels.CommandRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ClientId");

                    b.Property<long>("CommandRecordId");

                    b.Property<string>("Description");

                    b.Property<string>("Group");

                    b.Property<string>("SystemName");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("CommandRecords");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.DomainModels.Service", b =>
                {
                    b.Property<long>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<DateTime?>("ActiveDate");

                    b.Property<long>("ApplicationId");

                    b.Property<int?>("BillBasisId");

                    b.Property<long?>("BillPerTransactionCount");

                    b.Property<int?>("Category");

                    b.Property<string>("Channel");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("ExpiryDate");

                    b.Property<long>("Frequency");

                    b.Property<int?>("Industry");

                    b.Property<string>("Keyword");

                    b.Property<long?>("MobileOriginatingCount");

                    b.Property<long?>("MobileTerminatingCount");

                    b.Property<string>("Note");

                    b.Property<string>("OperatorIds");

                    b.Property<int?>("Periodicity");

                    b.Property<decimal?>("Price");

                    b.Property<string>("ServiceCode");

                    b.Property<string>("ServiceName");

                    b.Property<long>("ServiceProviderId");

                    b.Property<string>("ServiceProviderName");

                    b.Property<int>("ServiceStatus");

                    b.Property<string>("ShortCode");

                    b.Property<int?>("SubscriptionType");

                    b.HasKey("ServiceId");

                    b.ToTable("Service");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.DomainModels.ServiceContentMapp", b =>
                {
                    b.Property<long>("ServiceContentMappId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ContentId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<long>("ServiceId");

                    b.Property<long>("ServiceProviderId");

                    b.HasKey("ServiceContentMappId");

                    b.ToTable("ServiceContentMap");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.MapContentToServiceProvider", b =>
                {
                    b.Property<long>("MapContentToServiceProviderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ContentId");

                    b.Property<int>("ContentProviderId");

                    b.Property<string>("ContentProviderName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<long?>("DedicatedRateId");

                    b.Property<string>("DedicatedRateName");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("MappedBy");

                    b.Property<int>("RateCurrency");

                    b.Property<long?>("RateId");

                    b.Property<string>("RateName");

                    b.Property<string>("RateType");

                    b.Property<long>("ServiceProviderId");

                    b.HasKey("MapContentToServiceProviderId");

                    b.ToTable("MapContentToServiceProvider");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.MapServiceToClient", b =>
                {
                    b.Property<long>("MapServiceToClientId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ClientAuthId");

                    b.Property<long>("ClientId");

                    b.Property<string>("ClientName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<long?>("DedicatedRateId");

                    b.Property<string>("DedicatedRateName");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("MappedBy");

                    b.Property<int>("RateCurrency");

                    b.Property<long>("RateId");

                    b.Property<string>("RateName");

                    b.Property<string>("RateType");

                    b.Property<long>("ServiceId");

                    b.Property<string>("ServiceName");

                    b.Property<long>("ServiceProviderId");

                    b.Property<string>("ServiceProviderName");

                    b.HasKey("MapServiceToClientId");

                    b.ToTable("MapServiceToClient");
                });

            modelBuilder.Entity("ContentServiceManagementAPI.Models.ServiceProvider", b =>
                {
                    b.Property<long>("ServiceProviderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountEmail");

                    b.Property<string>("ApplicationId");

                    b.Property<bool?>("Approved");

                    b.Property<long>("AuthenticationId");

                    b.Property<string>("ContactPersonEmail");

                    b.Property<string>("ContactPersonFirstName");

                    b.Property<string>("ContactPersonLastName");

                    b.Property<string>("ContactPersonPhoneNumber");

                    b.Property<string>("ServiceProviderName");

                    b.Property<int>("Status");

                    b.Property<DateTime?>("VasLicenseActiveDate");

                    b.Property<DateTime?>("VasLicenseExpiryDate");

                    b.Property<string>("VasLicenseId");

                    b.HasKey("ServiceProviderId");

                    b.ToTable("ServiceProvider");
                });
#pragma warning restore 612, 618
        }
    }
}
