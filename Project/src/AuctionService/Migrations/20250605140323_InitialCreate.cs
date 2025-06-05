using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuctionService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservePrice = table.Column<int>(type: "integer", nullable: false),
                    Seller = table.Column<string>(type: "text", nullable: false),
                    Winner = table.Column<string>(type: "text", nullable: false),
                    SoldAmount = table.Column<int>(type: "integer", nullable: true),
                    CurrentHighBid = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuctionEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Mileage = table.Column<int>(type: "integer", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "AuctionEnd", "CreatedAt", "CurrentHighBid", "ReservePrice", "Seller", "SoldAmount", "Status", "UpdatedAt", "Winner" },
                values: new object[,]
                {
                    { new Guid("155225c1-4448-4066-9886-6786536e05ea"), new DateTime(2025, 5, 26, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642), null, 50000, "tom", null, 2, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642), "" },
                    { new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"), new DateTime(2025, 7, 23, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665), null, 20000, "bob", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665), "" },
                    { new Guid("40490065-dac7-46b6-acc4-df507e0d6570"), new DateTime(2025, 6, 25, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663), null, 20000, "tom", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663), "" },
                    { new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"), new DateTime(2025, 7, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644), null, 20000, "alice", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644), "" },
                    { new Guid("47111973-d176-4feb-848d-0ea22641c31a"), new DateTime(2025, 6, 18, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659), null, 150000, "alice", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659), "" },
                    { new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"), new DateTime(2025, 6, 24, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661), null, 0, "bob", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661), "" },
                    { new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"), new DateTime(2025, 6, 15, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5368), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5491), null, 20000, "bob", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5563), "" },
                    { new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"), new DateTime(2025, 6, 9, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640), null, 0, "bob", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640), "" },
                    { new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"), new DateTime(2025, 8, 4, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5637), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5638), null, 90000, "alice", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5638), "" },
                    { new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"), new DateTime(2025, 7, 20, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657), null, 20000, "bob", null, 0, new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657), "" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AuctionId", "Color", "ImageURL", "Make", "Mileage", "Model", "Year" },
                values: new object[,]
                {
                    { new Guid("071af034-1bfd-45d3-a9f5-a9c8a3d4c773"), new Guid("47111973-d176-4feb-848d-0ea22641c31a"), "Red", "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg", "Ferrari", 5000, "F-430", 2022 },
                    { new Guid("142d4d9d-bc5e-4ddb-846f-30b325a91828"), new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"), "White", "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg", "Ford", 50000, "GT", 2020 },
                    { new Guid("1fd017e8-fce5-4fe2-89ff-d6d58f846e19"), new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"), "White", "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg", "BMW", 90000, "X1", 2017 },
                    { new Guid("31cf12e2-dd6f-427e-82f2-9556bc747f77"), new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"), "White", "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg", "Audi", 10050, "R8", 2021 },
                    { new Guid("755553b1-5d0c-4d06-9d5c-20e4be7438fe"), new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"), "Red", "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg", "Ferrari", 50000, "Spider", 2015 },
                    { new Guid("81385bc5-a5ec-41dd-9e68-dae88c835265"), new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"), "Black", "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg", "Bugatti", 15035, "Veyron", 2018 },
                    { new Guid("86b4c3db-1e12-4eac-b7d1-08ec0332507b"), new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"), "Rust", "https://cdn.pixabay.com/photo/2017/08/02/19/47/vintage-2573090_960_720.jpg", "Ford", 150150, "Model T", 1938 },
                    { new Guid("9ba6aae5-99ea-4353-a23a-90e9bd269b62"), new Guid("40490065-dac7-46b6-acc4-df507e0d6570"), "Black", "https://cdn.pixabay.com/photo/2016/09/01/15/06/audi-1636320_960_720.jpg", "Audi", 25400, "TT", 2020 },
                    { new Guid("dc03f139-6f48-46b5-9459-fe0c0bfd8cb8"), new Guid("155225c1-4448-4066-9886-6786536e05ea"), "Silver", "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png", "Mercedes", 15001, "SLK", 2020 },
                    { new Guid("e0df736a-5d1f-47c3-a21e-d2555470dfa5"), new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"), "Black", "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg", "Ford", 65125, "Mustang", 2023 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_AuctionId",
                table: "Items",
                column: "AuctionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Auctions");
        }
    }
}
