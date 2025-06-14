using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuctionService.Migrations
{
    /// <inheritdoc />
    public partial class AddMassTransitOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("071af034-1bfd-45d3-a9f5-a9c8a3d4c773"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("142d4d9d-bc5e-4ddb-846f-30b325a91828"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("1fd017e8-fce5-4fe2-89ff-d6d58f846e19"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("31cf12e2-dd6f-427e-82f2-9556bc747f77"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("755553b1-5d0c-4d06-9d5c-20e4be7438fe"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("81385bc5-a5ec-41dd-9e68-dae88c835265"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("86b4c3db-1e12-4eac-b7d1-08ec0332507b"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("9ba6aae5-99ea-4353-a23a-90e9bd269b62"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("dc03f139-6f48-46b5-9459-fe0c0bfd8cb8"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("e0df736a-5d1f-47c3-a21e-d2555470dfa5"));

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiveCount = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnqueueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("155225c1-4448-4066-9886-6786536e05ea"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 4, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7517), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7517), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7518) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 1, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7529), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7529), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7530) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("40490065-dac7-46b6-acc4-df507e0d6570"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 4, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7527), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7527), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7528) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7519), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7520), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7520) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("47111973-d176-4feb-848d-0ea22641c31a"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 27, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7523), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7524), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7524) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7525), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7525), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7526) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7191), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7346), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7420) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7497), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7498), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7498) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 13, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7493), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7494), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7495) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7521), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7522), new DateTime(2025, 6, 14, 17, 14, 0, 963, DateTimeKind.Utc).AddTicks(7522) });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AuctionId", "Color", "ImageURL", "Make", "Mileage", "Model", "Year" },
                values: new object[,]
                {
                    { new Guid("0ebb71ab-f759-41a2-a3ad-66ac7b0bdd79"), new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"), "Black", "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg", "Bugatti", 15035, "Veyron", 2018 },
                    { new Guid("16fec7db-0fc8-49d1-a3f3-f6d73ec67ae7"), new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"), "White", "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg", "Ford", 50000, "GT", 2020 },
                    { new Guid("1d0f3268-7963-43c3-b9bb-a655f33c2a84"), new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"), "White", "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg", "Audi", 10050, "R8", 2021 },
                    { new Guid("252ae859-3491-4451-b024-7e81a266dfb3"), new Guid("47111973-d176-4feb-848d-0ea22641c31a"), "Red", "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg", "Ferrari", 5000, "F-430", 2022 },
                    { new Guid("3da6b0f4-b721-46a2-88e6-dcfe0bfc15ff"), new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"), "Black", "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg", "Ford", 65125, "Mustang", 2023 },
                    { new Guid("6985c1dc-c0b9-4a04-8537-fce0c9a5e066"), new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"), "Rust", "https://cdn.pixabay.com/photo/2017/08/02/19/47/vintage-2573090_960_720.jpg", "Ford", 150150, "Model T", 1938 },
                    { new Guid("9ca2efe6-9530-4028-8083-7c8d54c1aa7e"), new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"), "White", "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg", "BMW", 90000, "X1", 2017 },
                    { new Guid("a6210a0e-0067-4e20-b28e-a65ce86964c7"), new Guid("155225c1-4448-4066-9886-6786536e05ea"), "Silver", "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png", "Mercedes", 15001, "SLK", 2020 },
                    { new Guid("a7208ca4-9f3b-4dde-b8cb-729b214c76b9"), new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"), "Red", "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg", "Ferrari", 50000, "Spider", 2015 },
                    { new Guid("c2b0cdd2-ac73-4afa-9d61-179b429b8e21"), new Guid("40490065-dac7-46b6-acc4-df507e0d6570"), "Black", "https://cdn.pixabay.com/photo/2016/09/01/15/06/audi-1636320_960_720.jpg", "Audi", 25400, "TT", 2020 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("0ebb71ab-f759-41a2-a3ad-66ac7b0bdd79"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("16fec7db-0fc8-49d1-a3f3-f6d73ec67ae7"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("1d0f3268-7963-43c3-b9bb-a655f33c2a84"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("252ae859-3491-4451-b024-7e81a266dfb3"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("3da6b0f4-b721-46a2-88e6-dcfe0bfc15ff"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("6985c1dc-c0b9-4a04-8537-fce0c9a5e066"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("9ca2efe6-9530-4028-8083-7c8d54c1aa7e"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("a6210a0e-0067-4e20-b28e-a65ce86964c7"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("a7208ca4-9f3b-4dde-b8cb-729b214c76b9"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("c2b0cdd2-ac73-4afa-9d61-179b429b8e21"));

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("155225c1-4448-4066-9886-6786536e05ea"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5642) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 23, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5665) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("40490065-dac7-46b6-acc4-df507e0d6570"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5663) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5644) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("47111973-d176-4feb-848d-0ea22641c31a"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5659) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5661) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 15, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5368), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5491), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5563) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5640) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 4, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5637), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5638), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5638) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"),
                columns: new[] { "AuctionEnd", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 20, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657), new DateTime(2025, 6, 5, 14, 3, 23, 528, DateTimeKind.Utc).AddTicks(5657) });

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
        }
    }
}
