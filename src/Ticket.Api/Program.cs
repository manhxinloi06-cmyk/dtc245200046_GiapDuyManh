var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Dữ liệu mẫu trong bộ nhớ (In-memory data theo Chương 1)
var events = new List<EventItem>
{
    new(1, "Concert Anh Trai Vuot Ngan Chong Gai", "TP.HCM", 150),
    new(2, "Tech Conference .NET 10 Distributed Systems", "Ha Noi", 80),
    new(3, "Indie Music Night 2026", "Da Nang", 50)
};

var bookings = new List<BookingResult>();

// 2. Endpoint kiểm tra trạng thái API đang chạy
app.MapGet("/", () => Results.Ok(new
{
    service = "Ticket.Api",
    status = "running"
}));

// 3. Tra cứu danh sách sự kiện và số vé còn lại
app.MapGet("/events", () => Results.Ok(events));

// 4. Tra cứu chi tiết 1 sự kiện theo Id
app.MapGet("/events/{id:int}", (int id) =>
{
    var ev = events.FirstOrDefault(e => e.Id == id);
    return ev is not null
        ? Results.Ok(ev)
        : Results.NotFound(new { message = $"Khong tim thay su kien {id}" });
});

// 5. Gửi yêu cầu đặt vé -> Tạo trạng thái Pending & trả về 201 Created
app.MapPost("/bookings", (CreateBookingRequest request) =>
{
    if (request.EventId <= 0 ||
        string.IsNullOrWhiteSpace(request.UserId) ||
        request.Quantity <= 0)
    {
        return Results.BadRequest(new { message = "Du lieu dat ve khong hop le" });
    }

    var ev = events.FirstOrDefault(e => e.Id == request.EventId);
    if (ev is null)
    {
        return Results.NotFound(new { message = "Su kien khong ton tai" });
    }

    var booking = new BookingResult(
        Id: Guid.NewGuid(),
        EventId: request.EventId,
        UserId: request.UserId,
        Quantity: request.Quantity,
        Status: "Pending",
        CreatedAt: DateTimeOffset.UtcNow);

    bookings.Add(booking);

    return Results.Created($"/bookings/{booking.Id}", booking);
});

// 6. Tra cứu trạng thái yêu cầu đặt vé
app.MapGet("/bookings/{id:guid}", (Guid id) =>
{
    var booking = bookings.FirstOrDefault(b => b.Id == id);
    return booking is not null
        ? Results.Ok(booking)
        : Results.NotFound(new { message = "Khong tim thay yeu cau dat ve" });
});

// 7. Tra cứu danh sách vé của một người dùng
app.MapGet("/users/{userId}/bookings", (string userId) =>
{
    var userBookings = bookings.Where(b => b.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Ok(userBookings);
});

app.Run();

// Các record hợp đồng dữ liệu trao đổi qua ranh giới HTTP
record EventItem(int Id, string Name, string Location, int RemainingTickets);

record CreateBookingRequest(int EventId, string UserId, int Quantity);

record BookingResult(
    Guid Id,
    int EventId,
    string UserId,
    int Quantity,
    string Status,
    DateTimeOffset CreatedAt);