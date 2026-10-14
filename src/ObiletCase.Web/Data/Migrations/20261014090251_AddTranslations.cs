using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ObiletCase.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Culture = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Culture", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "tr-TR", "BusTickets", "Otobüs Bileti" },
                    { 2, "en-US", "BusTickets", "Bus Tickets" },
                    { 3, "tr-TR", "From", "Nereden" },
                    { 4, "en-US", "From", "From" },
                    { 5, "tr-TR", "To", "Nereye" },
                    { 6, "en-US", "To", "To" },
                    { 7, "tr-TR", "SwapLocations", "Nereden ve Nereye alanlarını değiştir" },
                    { 8, "en-US", "SwapLocations", "Swap From and To" },
                    { 9, "tr-TR", "Date", "Tarih" },
                    { 10, "en-US", "Date", "Date" },
                    { 11, "tr-TR", "Today", "Bugün" },
                    { 12, "en-US", "Today", "Today" },
                    { 13, "tr-TR", "Tomorrow", "Yarın" },
                    { 14, "en-US", "Tomorrow", "Tomorrow" },
                    { 15, "tr-TR", "FindTickets", "Bileti Bul" },
                    { 16, "en-US", "FindTickets", "Find Tickets" },
                    { 17, "tr-TR", "SelectOrigin", "Kalkış noktası seçiniz." },
                    { 18, "en-US", "SelectOrigin", "Please select a departure point." },
                    { 19, "tr-TR", "SelectDestination", "Varış noktası seçiniz." },
                    { 20, "en-US", "SelectDestination", "Please select an arrival point." },
                    { 21, "tr-TR", "SameOriginAndDestination", "Kalkış ve varış noktası aynı olamaz." },
                    { 22, "en-US", "SameOriginAndDestination", "Departure and arrival points cannot be the same." },
                    { 23, "tr-TR", "DateInPast", "Yolculuk tarihi bugünden önce olamaz." },
                    { 24, "en-US", "DateInPast", "The travel date cannot be before today." },
                    { 25, "tr-TR", "BackToSearch", "Aramaya dön" },
                    { 26, "en-US", "BackToSearch", "Back to search" },
                    { 27, "tr-TR", "NoJourneys", "Bu tarihte sefer bulunamadı." },
                    { 28, "en-US", "NoJourneys", "No journeys were found on this date." },
                    { 29, "tr-TR", "Departure", "KALKIŞ" },
                    { 30, "en-US", "Departure", "DEPARTURE" },
                    { 31, "tr-TR", "Arrival", "VARIŞ" },
                    { 32, "en-US", "Arrival", "ARRIVAL" },
                    { 33, "tr-TR", "NextDay", "Ertesi gün" },
                    { 34, "en-US", "NextDay", "Next day" },
                    { 35, "tr-TR", "ErrorTitle", "Bir sorun oluştu" },
                    { 36, "en-US", "ErrorTitle", "Something went wrong" },
                    { 37, "tr-TR", "ErrorHeading", "Şu anda işleminizi gerçekleştiremiyoruz" },
                    { 38, "en-US", "ErrorHeading", "We cannot complete your request right now" },
                    { 39, "tr-TR", "ErrorTryLater", "Lütfen biraz sonra tekrar deneyin." },
                    { 40, "en-US", "ErrorTryLater", "Please try again a little later." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Key_Culture",
                table: "Translations",
                columns: new[] { "Key", "Culture" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");
        }
    }
}
