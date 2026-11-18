using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingManagement.MVC.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeCustomerPrimaryKeyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.key_constraints
                    WHERE [parent_object_id] = OBJECT_ID(N'[dbo].[Customers]')
                      AND [name] = N'PK_Students'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM sys.key_constraints
                    WHERE [parent_object_id] = OBJECT_ID(N'[dbo].[Customers]')
                      AND [name] = N'PK_Customers'
                )
                BEGIN
                    EXEC sp_rename N'[dbo].[PK_Students]', N'PK_Customers', N'OBJECT';
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The normalized key name is intentionally retained on rollback.
        }
    }
}
