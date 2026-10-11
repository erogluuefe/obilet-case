# obilet Case

obilet.com Sr. Full Stack Developer programlama ödevi: obilet'in iş ortağı API'sini kullanarak otobüs seferi arayan bir ASP.NET Core MVC uygulaması.

- **Arama sayfası:** kalkış ve varış noktası (yazarak aranabilir), tarih, Bugün/Yarın kısayolları ve yer değiştirme butonu.
- **Sefer sayfası:** seçilen rota ve gün için seferler, kalkış saatine göre sıralı.

## Gereksinimler

- .NET 8 SDK
- obilet API istemci anahtarı (client token)

## Kurulum ve çalıştırma

API anahtarı depoda tutulmaz. Yerelde `dotnet user-secrets` ile verilir:

```bash
dotnet user-secrets set "ObiletApi:ClientToken" "<anahtar>" --project src/ObiletCase.Web
dotnet run --project src/ObiletCase.Web
```

Uygulama `http://localhost:5023` adresinde açılır. Anahtar verilmemişse uygulama başlarken açık bir hata mesajıyla durur.

Testler:

```bash
dotnet test
```

Testler obilet'e istek atmaz; API cevapları gerçek cevaplardan kırpılmış örneklerle taklit edilir.

## Proje yapısı

```
src/ObiletCase.Web
├── Configuration/      obilet API ayarları (adres, anahtar, dil)
├── Controllers/        HomeController (arama), JourneyController (seferler)
├── Filters/            obilet hatalarını tek yerde ele alan exception filter
├── Models/             sayfaların view model'leri ve doğrulama kuralları
├── Services/           TurkeyTime (Türkiye saati); Obilet/ altında API istemcisi, session servisi ve API modelleri
├── Views/              Razor sayfaları
└── wwwroot/            CSS ve iki küçük JavaScript dosyası
tests/ObiletCase.Tests  xUnit testleri
```

## Bir isteğin akışı

1. Tarayıcı yalnızca bu uygulamaya istek atar; obilet'e giden bütün istekler sunucudan çıkar.
2. Controller, `IObiletSessionService` üzerinden ziyaretçinin obilet session'ını ister.
   - İlk istekte obilet'e `GetSession` atılır ve dönen session, ASP.NET Core Session'da saklanır.
   - Sonraki isteklerde saklanan session kullanılır; her ziyaretçinin session'ı kendine aittir.
3. Controller, `IObiletApiClient` ile `GetBusLocations` ya da `GetJourneys` çağrısını bu session'la yapar.
4. Sonuç Razor view'a verilir; arama kutusu için JSON olarak döner.

## Tasarım kararları

- **Doğrulama sunucuda.** Kurallar `JourneySearchViewModel` içinde: kalkış ve varış aynı olamaz, tarih bugünden önce olamaz. Sefer sayfası aynı modeli kullandığı için adres elle değiştirilse de aynı kurallar çalışır. Tarayıcıdaki `required` ve `min` yalnızca kolaylıktır.
- **Tek kutulu lokasyon araması.** Kutu, Bootstrap'in açılır listesine bağlı bir yazı alanıdır. Yazılan metin kısa bir beklemeden sonra sunucuya, oradan obilet'e gider. Seçilen lokasyonun id'si ve adı gizli alanlarda tutulur; form bunları gönderir.
- **Son arama tarayıcıda saklanır.** Kayıt `localStorage`'a sefer sayfası açıldığında yazılır; böylece yalnızca doğrulamadan geçmiş aramalar saklanır. Saklanan tarih geçmişte kaldıysa form varsayılan tarihle (yarın) açılır.
- **Seferler sunucuda sıralanır.** obilet seferleri kalkış saatine göre sıralı göndermez.
- **Gece yarısından sonraki seferler işaretlenir.** obilet, aranan günün sonucunda ertesi günün ilk saatlerinde kalkan seferleri de döndürür; bu seferlerde kalkış saati farklı renkte ve hilal işaretiyle gösterilir.
- **Para birimi obilet'ten gelir.** Türk lirası "TL" olarak, diğer para birimleri kodlarıyla yazılır (yurt dışı seferlerde farklı para birimi gelebiliyor).
- **Hatalar tek yerde ele alınır.** obilet hata döndüğünde, ulaşılamadığında (örneğin çok fazla istek sonrası 429) ya da 20 saniye içinde cevap vermediğinde `ObiletExceptionFilter` hatayı kaydeder ve ziyaretçiye hata sayfasını gösterir.
- **Arayüz kütüphanesi olarak yalnızca Bootstrap var.** Şablondan gelen jQuery kaldırıldı; tarayıcı tarafı düz JavaScript ile yazıldı.
- **Tarih ve fiyatlar Türkçe biçimde yazılır.** Kültür `Program.cs`'te tek yerde tanımlıdır.
- **"Bugün" Türkiye saatine göre hesaplanır.** Seferlerin neredeyse tamamı Türkiye'den kalktığı için Bugün/Yarın tarihleri ve "tarih bugünden önce olamaz" kuralı, sunucunun ya da ziyaretçinin saat diliminden bağımsız olarak Türkiye'deki günü kullanır (`TurkeyTime`).

## Bilinen sınırlar

- **Session bellekte tutulur.** Tek sunucu için yeterlidir; birden fazla sunucuda çalıştırmak için `Program.cs`'teki bellek içi önbellek Redis gibi ortak bir önbellekle değiştirilmelidir.
- **"Bugün" Türkiye'deki gündür.** Kalkış yeri yurt dışındaysa oranın günü gece yarısı civarında bir-iki saat boyunca Türkiye'nin gününden farklıdır; bu sürede "Bugün" ve en erken tarih kalkış yerinin gününe uymaz. obilet lokasyonların saat dilimini gönderdiği için kural gerekirse kalkış yerine göre hesaplanabilir.
- **Ziyaretçinin IP adresi bağlantıdan okunur.** Uygulama bir proxy ya da yük dengeleyici arkasında çalışacaksa forwarded headers ayarı eklenmelidir.
- **obilet'in istek sınırı vardır.** Kısa sürede çok sayıda yeni session istenirse obilet bir süre 429 döndürür; uygulama bu durumda hata sayfasını gösterir ve isteği kendiliğinden tekrarlamaz.
