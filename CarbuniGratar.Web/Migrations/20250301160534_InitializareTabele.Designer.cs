﻿// <auto-generated />
using System;
using CarbuniGratar.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarbuniGratar.Web.Migrations
{
    [DbContext(typeof(NepalezBazaDate))]
    [Migration("20250301160534_InitializareTabele")]
    partial class InitializareTabele
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarbuniGratar.Web.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adresa")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nume")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clienti");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Comanda", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataPlasare")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Comenzi");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.ComandaProdus", b =>
                {
                    b.Property<int>("ComandaId")
                        .HasColumnType("int");

                    b.Property<int>("ProdusId")
                        .HasColumnType("int");

                    b.Property<int>("Cantitate")
                        .HasColumnType("int");

                    b.Property<decimal>("PretUnitate")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("ComandaId", "ProdusId");

                    b.HasIndex("ProdusId");

                    b.ToTable("ComenziProduse");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Produs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descriere")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagineUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("InStoc")
                        .HasColumnType("bit");

                    b.Property<string>("Nume")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<decimal>("Pret")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.ToTable("Produse");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Comanda", b =>
                {
                    b.HasOne("CarbuniGratar.Web.Models.Client", "Client")
                        .WithMany("Comenzi")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.ComandaProdus", b =>
                {
                    b.HasOne("CarbuniGratar.Web.Models.Comanda", "Comanda")
                        .WithMany("ProduseComandate")
                        .HasForeignKey("ComandaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarbuniGratar.Web.Models.Produs", "Produs")
                        .WithMany("ComenziProduse")
                        .HasForeignKey("ProdusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comanda");

                    b.Navigation("Produs");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Client", b =>
                {
                    b.Navigation("Comenzi");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Comanda", b =>
                {
                    b.Navigation("ProduseComandate");
                });

            modelBuilder.Entity("CarbuniGratar.Web.Models.Produs", b =>
                {
                    b.Navigation("ComenziProduse");
                });
#pragma warning restore 612, 618
        }
    }
}
