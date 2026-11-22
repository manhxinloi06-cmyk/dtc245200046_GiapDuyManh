using System.Net.Http.Json;

var apiBaseUrl = args.Length > 0 ? args[0] : "http://localhost:5000";

using var http = new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
};

// 1. Lấy danh sách sự kiện và số vé còn lại
var events = await http.GetFromJsonAsync<List<EventItem>>("/events");

Console.WriteLine("Danh sach su kien:");
foreach (var ev in events ?? new List<EventItem>())
{
    Console.WriteLine($"- {ev.Id}: {ev.Name} ({ev.Location}) - Con lai: {ev.RemainingTickets} ve");
}

// 2. Gửi yêu cầu đặt vé mới
var request = new CreateBookingRequest(
    EventId: 1,
    UserId: "SV001",
    Quantity: 2);

var response = await http.PostAsJsonAsync("/bookings", request);
response.EnsureSuccessStatusCode();

var result = await response.Content.ReadFromJsonAsync<BookingResult>();

Console.WriteLine($"\nDa gui yeu cau dat ve. Ma dat ve: {result?.Id}");
Console.WriteLine($"Trang thai: {result?.Status}");

// 3. Tra cứu lại trạng thái đặt vé vừa tạo
if (result is not null)
{
    var checkBooking = await http.GetFromJsonAsync<BookingResult>($"/bookings/{result.Id}");
    Console.WriteLine($"Kiem tra trang thai /bookings/{result.Id}: {checkBooking?.Status}");
}

// 4. Tra cứu danh sách vé của người dùng SV001
var userBookings = await http.GetFromJsonAsync<List<BookingResult>>("/users/SV001/bookings");
Console.WriteLine($"\nTong so yeu cau dat ve cua SV001: {userBookings?.Count ?? 0}");

record EventItem(int Id, string Name, string Location, int RemainingTickets);

record CreateBookingRequest(int EventId, string UserId, int Quantity);

record BookingResult(
    Guid Id,
    int EventId,
    string UserId,
    int Quantity,
    string Status,
    DateTimeOffset CreatedAt);