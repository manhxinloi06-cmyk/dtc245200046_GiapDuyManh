# Đề tài 03 - Hệ thống đặt vé sự kiện phân tán

## 1. Mô tả đề tài

Xây dựng hệ thống đặt vé cho các sự kiện. Người dùng có thể xem sự kiện, đặt vé và theo dõi trạng thái xử lý. Việc xác nhận vé được thực hiện bất đồng bộ để mô phỏng quá trình kiểm tra và xử lý phía sau.

Kiến trúc tối thiểu của hệ thống:

```text
Client -> Ticket API -> RabbitMQ -> Booking Worker -> Database
              |                         |
              +-------- Database <------+
```

## 2. Chức năng chính

1. Tra cứu danh sách sự kiện và số vé còn lại.
2. Gửi yêu cầu đặt vé.
3. Tạo yêu cầu đặt vé với trạng thái `Pending`.
4. Đưa yêu cầu xác nhận vé vào RabbitMQ.
5. Worker kiểm tra số lượng vé và xác nhận hoặc từ chối yêu cầu.
6. Cập nhật số vé còn lại khi đặt vé thành công.
7. Tra cứu trạng thái yêu cầu đặt vé.
8. Tra cứu danh sách vé của một người dùng.

## 3. API tối thiểu

```http
GET /events
GET /events/{id}
POST /bookings
GET /bookings/{id}
GET /users/{userId}/bookings
```

Dữ liệu tối thiểu gồm sự kiện, số lượng vé, người đặt, yêu cầu đặt vé và trạng thái xử lý.
