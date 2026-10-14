using ObiletCase.Web.Localization;

namespace ObiletCase.Web.Data;

/// <summary>The texts the database starts with. A change here needs a new migration to reach the database.</summary>
public static class TranslationSeed
{
    public static readonly Translation[] Translations =
    {
        // Search page
        new Translation { Id = 1, Key = "BusTickets", Culture = "tr-TR", Value = "Otobüs Bileti" },
        new Translation { Id = 2, Key = "BusTickets", Culture = "en-US", Value = "Bus Tickets" },
        new Translation { Id = 3, Key = "From", Culture = "tr-TR", Value = "Nereden" },
        new Translation { Id = 4, Key = "From", Culture = "en-US", Value = "From" },
        new Translation { Id = 5, Key = "To", Culture = "tr-TR", Value = "Nereye" },
        new Translation { Id = 6, Key = "To", Culture = "en-US", Value = "To" },
        new Translation { Id = 7, Key = "SwapLocations", Culture = "tr-TR", Value = "Nereden ve Nereye alanlarını değiştir" },
        new Translation { Id = 8, Key = "SwapLocations", Culture = "en-US", Value = "Swap From and To" },
        new Translation { Id = 9, Key = "Date", Culture = "tr-TR", Value = "Tarih" },
        new Translation { Id = 10, Key = "Date", Culture = "en-US", Value = "Date" },
        new Translation { Id = 11, Key = "Today", Culture = "tr-TR", Value = "Bugün" },
        new Translation { Id = 12, Key = "Today", Culture = "en-US", Value = "Today" },
        new Translation { Id = 13, Key = "Tomorrow", Culture = "tr-TR", Value = "Yarın" },
        new Translation { Id = 14, Key = "Tomorrow", Culture = "en-US", Value = "Tomorrow" },
        new Translation { Id = 15, Key = "FindTickets", Culture = "tr-TR", Value = "Bileti Bul" },
        new Translation { Id = 16, Key = "FindTickets", Culture = "en-US", Value = "Find Tickets" },

        // Search form validation
        new Translation { Id = 17, Key = "SelectOrigin", Culture = "tr-TR", Value = "Kalkış noktası seçiniz." },
        new Translation { Id = 18, Key = "SelectOrigin", Culture = "en-US", Value = "Please select a departure point." },
        new Translation { Id = 19, Key = "SelectDestination", Culture = "tr-TR", Value = "Varış noktası seçiniz." },
        new Translation { Id = 20, Key = "SelectDestination", Culture = "en-US", Value = "Please select an arrival point." },
        new Translation { Id = 21, Key = "SameOriginAndDestination", Culture = "tr-TR", Value = "Kalkış ve varış noktası aynı olamaz." },
        new Translation { Id = 22, Key = "SameOriginAndDestination", Culture = "en-US", Value = "Departure and arrival points cannot be the same." },
        new Translation { Id = 23, Key = "DateInPast", Culture = "tr-TR", Value = "Yolculuk tarihi bugünden önce olamaz." },
        new Translation { Id = 24, Key = "DateInPast", Culture = "en-US", Value = "The travel date cannot be before today." },

        // Journey page
        new Translation { Id = 25, Key = "BackToSearch", Culture = "tr-TR", Value = "Aramaya dön" },
        new Translation { Id = 26, Key = "BackToSearch", Culture = "en-US", Value = "Back to search" },
        new Translation { Id = 27, Key = "NoJourneys", Culture = "tr-TR", Value = "Bu tarihte sefer bulunamadı." },
        new Translation { Id = 28, Key = "NoJourneys", Culture = "en-US", Value = "No journeys were found on this date." },
        new Translation { Id = 29, Key = "Departure", Culture = "tr-TR", Value = "KALKIŞ" },
        new Translation { Id = 30, Key = "Departure", Culture = "en-US", Value = "DEPARTURE" },
        new Translation { Id = 31, Key = "Arrival", Culture = "tr-TR", Value = "VARIŞ" },
        new Translation { Id = 32, Key = "Arrival", Culture = "en-US", Value = "ARRIVAL" },
        new Translation { Id = 33, Key = "NextDay", Culture = "tr-TR", Value = "Ertesi gün" },
        new Translation { Id = 34, Key = "NextDay", Culture = "en-US", Value = "Next day" },

        // Error page
        new Translation { Id = 35, Key = "ErrorTitle", Culture = "tr-TR", Value = "Bir sorun oluştu" },
        new Translation { Id = 36, Key = "ErrorTitle", Culture = "en-US", Value = "Something went wrong" },
        new Translation { Id = 37, Key = "ErrorHeading", Culture = "tr-TR", Value = "Şu anda işleminizi gerçekleştiremiyoruz" },
        new Translation { Id = 38, Key = "ErrorHeading", Culture = "en-US", Value = "We cannot complete your request right now" },
        new Translation { Id = 39, Key = "ErrorTryLater", Culture = "tr-TR", Value = "Lütfen biraz sonra tekrar deneyin." },
        new Translation { Id = 40, Key = "ErrorTryLater", Culture = "en-US", Value = "Please try again a little later." }
    };
}
