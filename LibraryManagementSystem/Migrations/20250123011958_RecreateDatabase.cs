using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabloları oluşturduğunuz kodlar zaten burada olmalı.

            // Admin kullanıcısını seed olarak ekle
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "FirstName", "LastName", "Email", "Password", "Role", "IsActive", "CreatedDate" },
                values: new object[]
                {
            "Admin", // FirstName
            "User",  // LastName
            "admin@example.com", // Email
            HashPassword("Admin@123"), // Şifreleme için metod
            "Admin", // Role
            true,    // IsActive
            DateTime.UtcNow // CreatedDate
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Admin kullanıcısını silmek için kod
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValue: "admin@example.com"
            );
        }

        // Password hashing method
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
