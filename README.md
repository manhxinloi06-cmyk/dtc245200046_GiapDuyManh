# Đề tài 03 - Hệ thống đặt vé sự kiện phân tán (Event Ticket System)

Hệ thống đặt vé cho các sự kiện với kiến trúc phân tán sử dụng **.NET 10**, **ASP.NET Core Minimal API**, **RabbitMQ** để xử lý xác nhận đặt vé bất đồng bộ thông qua **Booking Worker**.

---

## 1. Kiến trúc hệ thống

```text
Client -> Ticket API -> RabbitMQ -> Booking Worker -> Database
              |                         |
              +-------- Database <------+
```

### Cấu trúc thư mục dự án

```text
EventTicketSystem/
├── src/
│   ├── Ticket.Api/           # ASP.NET Core Minimal API nhận request đặt vé & tra cứu
│   ├── Ticket.Worker/        # Background Worker lắng nghe RabbitMQ để xử lý đặt vé
│   ├── Ticket.Client/        # Console / Web Client tương tác với hệ thống
│   └── Ticket.Shared/        # (Tùy chọn) Models, DTOs, Contracts dùng chung
├── tailieu/
│   ├── HUONG-DAN-CHUNG.md    # Yêu cầu kỹ thuật chung của học phần
│   └── DE-TAI-03.md          # Mô tả chi tiết đề tài 03
├── docker-compose.yml        # Khởi chạy toàn bộ hệ thống (Database, RabbitMQ, API, Worker)
├── EventTicketSystem.slnx    # Solution file (.NET 10)
└── README.md                 # Tài liệu hướng dẫn cài đặt, build và chạy dự án
```

---

## 2. Yêu cầu môi trường (Prerequisites)

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (hỗ trợ Docker Compose)
- Git

Kiểm tra phiên bản trước khi chạy:
```powershell
dotnet --version
docker --version
docker compose version
```

---

## 3. Hướng dẫn các lệnh chạy dự án

### Cách 1: Chạy toàn bộ hệ thống bằng Docker Compose (Khuyên dùng)

Cách này sẽ tự động build image và khởi chạy tất cả các container: **Database**, **RabbitMQ**, **Ticket.Api**, và **Ticket.Worker** trong cùng một mạng Docker network.

```powershell
# 1. Build và khởi chạy toàn bộ các services ở chế độ nền (detached mode)
docker compose up -d --build

# 2. Kiểm tra trạng thái các container đang chạy
docker compose ps

# 3. Xem log thời gian thực của toàn bộ hệ thống (hoặc riêng từng service)
docker compose logs -f
docker compose logs -f ticket-api
docker compose logs -f booking-worker

# 4. Dừng hệ thống
docker compose down

# 5. Dừng hệ thống và xóa luôn dữ liệu volume (reset Database / Queue)
docker compose down -v
```

Sau khi hệ thống backend đã chạy trên Docker, bạn có thể mở một terminal mới để chạy ứng dụng **Client**:
```powershell
dotnet run --project src/Ticket.Client/Ticket.Client.csproj
```

---

### Cách 2: Chạy trực tiếp trên máy (Local Development với `dotnet` CLI)

Khi phát triển hoặc debug trên máy, bạn có thể chỉ bật **RabbitMQ** và **Database** bằng Docker, sau đó chạy các project `.NET` bằng lệnh `dotnet`:

#### Bước 1: Khởi chạy hạ tầng (RabbitMQ & Database) trên Docker
```powershell
# Chỉ bật các container hạ tầng (nếu đã cấu hình trong docker-compose.yml)
docker compose up -d rabbitmq db
```

#### Bước 2: Khôi phục thư viện và Build toàn bộ Solution
```powershell
# Restore các gói NuGet
dotnet restore EventTicketSystem.slnx

# Build toàn bộ dự án để kiểm tra lỗi biên dịch
dotnet build EventTicketSystem.slnx
```

#### Bước 3: Chạy từng Service trên các cửa sổ Terminal riêng biệt

- **Terminal 1 - Chạy Ticket API:**
  ```powershell
  dotnet run --project src/Ticket.Api/Ticket.Api.csproj
  ```
  *(Hoặc dùng `dotnet watch --project src/Ticket.Api/Ticket.Api.csproj` để tự động reload khi sửa code)*

- **Terminal 2 - Chạy Booking Worker (khi đã thêm project Worker):**
  ```powershell
  dotnet run --project src/Ticket.Worker/Ticket.Worker.csproj
  ```

- **Terminal 3 - Chạy Ticket Client:**
  ```powershell
  dotnet run --project src/Ticket.Client/Ticket.Client.csproj
  ```

---

## 4. Các lệnh quản lý Project & Package thường dùng (Dành cho quá trình phát triển)

Nếu cần khởi tạo thêm các project còn thiếu trong kiến trúc (như `Ticket.Worker` hoặc `Ticket.Shared`), chạy các lệnh sau tại thư mục gốc `EventTicketSystem`:

```powershell
# Tạo project Worker Service cho Booking Worker
dotnet new worker -n Ticket.Worker -o src/Ticket.Worker
dotnet sln EventTicketSystem.slnx add src/Ticket.Worker/Ticket.Worker.csproj

# Tạo project Class Library dùng chung (Models, Events, DTOs)
dotnet new classlib -n Ticket.Shared -o src/Ticket.Shared
dotnet sln EventTicketSystem.slnx add src/Ticket.Shared/Ticket.Shared.csproj

# Thêm tham chiếu (Reference) tới Ticket.Shared
dotnet add src/Ticket.Api/Ticket.Api.csproj reference src/Ticket.Shared/Ticket.Shared.csproj
dotnet add src/Ticket.Worker/Ticket.Worker.csproj reference src/Ticket.Shared/Ticket.Shared.csproj
dotnet add src/Ticket.Client/Ticket.Client.csproj reference src/Ticket.Shared/Ticket.Shared.csproj
```

---

## 5. Danh sách API tối thiểu (`Ticket.Api`)

| Phương thức | Endpoint | Mô tả |
| :--- | :--- | :--- |
| `GET` | `/events` | Tra cứu danh sách sự kiện và số vé còn lại |
| `GET` | `/events/{id}` | Xem chi tiết một sự kiện theo `id` |
| `POST` | `/bookings` | Gửi yêu cầu đặt vé (tạo bản ghi trạng thái `Pending` & đẩy vào RabbitMQ) |
| `GET` | `/bookings/{id}` | Tra cứu trạng thái xử lý của yêu cầu đặt vé |
| `GET` | `/users/{userId}/bookings` | Tra cứu danh sách vé đã đặt của một người dùng |

### Ví dụ kiểm thử nhanh API bằng `curl` (PowerShell / Bash)

```powershell
# 1. Lấy danh sách sự kiện
curl http://localhost:5000/events

# 2. Chi tiết sự kiện có id = 1
curl http://localhost:5000/events/1

# 3. Gửi yêu cầu đặt vé
curl -X POST http://localhost:5000/bookings `
  -H "Content-Type: application/json" `
  -d '{"eventId": 1, "userId": "user-01", "quantity": 2}'

# 4. Kiểm tra trạng thái đặt vé
curl http://localhost:5000/bookings/1

# 5. Xem danh sách vé của user-01
curl http://localhost:5000/users/user-01/bookings
```

---

## 6. Hướng dẫn đưa Project lên GitHub (Git Workflow)

### Bước 1: Tạo Repository mới trên GitHub
1. Truy cập [https://github.com/new](https://github.com/new).
2. Đặt tên Repository (ví dụ: `EventTicketSystem`).
3. **Lưu ý:** Không tích chọn *Add a README file* hay *Add .gitignore* (vì trong project đã có sẵn).
4. Nhấn **Create repository** và copy đường dẫn HTTPS/SSH của repo (ví dụ: `https://github.com/<username>/EventTicketSystem.git`).

### Bước 2: Khởi tạo Git và Push code lần đầu lên GitHub
Mở terminal tại thư mục gốc `c:\Users\Admin\Desktop\UngDungPhanTan\EventTicketSystem` và chạy lần lượt các lệnh:

```powershell
# 1. Tạo file .gitignore chuẩn cho .NET (để bỏ qua thư mục bin/, obj/, .vs/)
dotnet new gitignore

# 2. Khởi tạo Git repository tại thư mục dự án
git init

# 3. Đổi tên nhánh mặc định thành main
git branch -M main

# 4. Đưa toàn bộ mã nguồn vào Staging
git add .

# 5. Tạo commit đầu tiên
git commit -m "Khởi tạo cấu trúc dự án và tài liệu hướng dẫn"

# 6. Liên kết với Repository trên GitHub (thay <URL_GITHUB> bằng link repo của bạn)
git remote add origin https://github.com/<username>/EventTicketSystem.git

# 7. Đẩy code lên nhánh main trên GitHub
git push -u origin main
```

### Bước 3: Cập nhật code lên GitHub trong quá trình làm bài tập lớn
Theo yêu cầu học phần, repository cần có lịch sử commit thể hiện quá trình thực hiện. Mỗi khi hoàn thành một phần chức năng, chạy:

```powershell
# Kiểm tra các file đã thay đổi
git status

# Thêm tất cả các file thay đổi
git add .

# Commit kèm mô tả tiếng Việt rõ ràng về công việc vừa làm
git commit -m "Xây dựng các API đặt vé và kết nối RabbitMQ"

# Đẩy commit mới lên GitHub
git push
```

