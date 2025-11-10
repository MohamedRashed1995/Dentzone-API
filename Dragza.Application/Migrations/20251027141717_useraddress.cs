using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class useraddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);


         

           

         
        }

        /// <inheritdoc />
      
    }
}
