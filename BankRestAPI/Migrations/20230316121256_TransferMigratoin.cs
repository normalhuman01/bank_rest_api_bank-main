using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankRestAPI.Migrations
{
    /// <inheritdoc />
    public partial class TransferMigratoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromBankName = table.Column<string>(type: "text", nullable: false),
                    FromBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromCustomerDocumentNumber = table.Column<string>(type: "text", nullable: true),
                    ToBankName = table.Column<string>(type: "text", nullable: false),
                    ToBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToCustomerDocumentNumber = table.Column<string>(type: "text", nullable: true),
                    OperationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionState = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Bank_FromBankId",
                        column: x => x.FromBankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Bank_ToBankId",
                        column: x => x.ToBankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Customer_FromCustomerDocumentNumber",
                        column: x => x.FromCustomerDocumentNumber,
                        principalTable: "Customer",
                        principalColumn: "DocumentNumber");
                    table.ForeignKey(
                        name: "FK_Transfer_Customer_ToCustomerDocumentNumber",
                        column: x => x.ToCustomerDocumentNumber,
                        principalTable: "Customer",
                        principalColumn: "DocumentNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromAccountId",
                table: "Transfer",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromBankId",
                table: "Transfer",
                column: "FromBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromCustomerDocumentNumber",
                table: "Transfer",
                column: "FromCustomerDocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToAccountId",
                table: "Transfer",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToBankId",
                table: "Transfer",
                column: "ToBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToCustomerDocumentNumber",
                table: "Transfer",
                column: "ToCustomerDocumentNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Bank_BankId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Customer_CustomerDocumentNumber",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionState",
                table: "Transfer",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Account",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerDocumentNumber",
                table: "Account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "Account",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Bank_BankId",
                table: "Account",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Customer_CustomerDocumentNumber",
                table: "Account",
                column: "CustomerDocumentNumber",
                principalTable: "Customer",
                principalColumn: "DocumentNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Bank_BankId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Customer_CustomerDocumentNumber",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionState",
                table: "Transfer",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Number",
                table: "Account",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerDocumentNumber",
                table: "Account",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "Account",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Bank_BankId",
                table: "Account",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Customer_CustomerDocumentNumber",
                table: "Account",
                column: "CustomerDocumentNumber",
                principalTable: "Customer",
                principalColumn: "DocumentNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
