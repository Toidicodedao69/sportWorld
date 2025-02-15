﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using sportWorld.DataAccess.Data;

#nullable disable

namespace sportWorld.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("sportWorld.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Badminton Racquet"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Badminton Shoe"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Badminton Bag"
                        });
                });

            modelBuilder.Entity("sportWorld.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price20")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Brand = "Yonex",
                            CategoryId = 1,
                            Description = "The Yonex Nanoflare 700 Pro is designed to make your clears—those deep, high shots that push your opponent to the backcourt—easier and more consistent. Its unique frame design helps you hit these shots with less effort, giving you better control of the game.",
                            ListPrice = 300.0,
                            Name = "Yonex Nanoflare 700 PRO",
                            Price = 289.0,
                            Price20 = 260.0
                        },
                        new
                        {
                            Id = 2,
                            Brand = "Victor",
                            CategoryId = 1,
                            Description = "The New Victor Thruster F Ultra (2024) is made to be user friendly, yet being able to product powerful and pin point accuracy smashes. \r\n\r\nVictor's signature Free Core Handle is used to maximise the racquet's shock absorption. This provides you with a comfortable hitting experience.",
                            ListPrice = 290.0,
                            Name = "Victor Thruster F Ultra",
                            Price = 279.0,
                            Price20 = 250.0
                        },
                        new
                        {
                            Id = 3,
                            Brand = "Yonex",
                            CategoryId = 2,
                            Description = "The new update for the Yonex Comfort Z Performance badminton shoes features a couple of upgrades to make the shoes more comfortable, with increased performance.",
                            ListPrice = 250.0,
                            Name = "Yonex Power Cushion Comfort Z 3 (Black/Mint)",
                            Price = 239.0,
                            Price20 = 210.0
                        });
                });

            modelBuilder.Entity("sportWorld.Models.Product", b =>
                {
                    b.HasOne("sportWorld.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
