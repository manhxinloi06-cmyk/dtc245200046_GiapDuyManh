# Hướng dẫn chung cho bài tập lớn

Tài liệu này áp dụng cho tất cả các đề tài bài tập lớn của học phần **Phát triển ứng dụng phân tán**. Mỗi đề tài riêng chỉ quy định mô tả bài toán, các chức năng chính và API tối thiểu; các yêu cầu dưới đây được áp dụng chung.

## 1. Yêu cầu kỹ thuật chung

- Ngôn ngữ: C#.
- Nền tảng: .NET 10.
- API: ASP.NET Core Minimal API.
- Client: Console App hoặc Web App.
- Sử dụng các công nghệ giao tiếp và middleware theo yêu cầu cụ thể của từng đề tài, chẳng hạn REST, SignalR, gRPC, RabbitMQ hoặc các công nghệ tương đương được chấp nhận.
- Cơ sở dữ liệu: sinh viên tự lựa chọn phù hợp với bài toán.
- Sử dụng Git để quản lý mã nguồn.
- Các service phải được tổ chức và chạy độc lập theo kiến trúc của đề tài.
- Mỗi service .NET phải có Dockerfile riêng.
- Có `docker-compose.yml` để khởi chạy toàn bộ hệ thống.
- Các container giao tiếp với nhau qua Docker network và service name.
- Các cấu hình phụ thuộc môi trường như connection string, địa chỉ message broker, URL service và port phải được tách khỏi mã nguồn.
- Có logging đủ để theo dõi luồng xử lý qua các thành phần chính của hệ thống.
- Hệ thống phải có khả năng tiếp tục hoạt động hoặc phục hồi hợp lý khi một thành phần tạm thời dừng hoạt động, phù hợp với cơ chế giao tiếp được sử dụng trong đề tài.

## 2. Sản phẩm cần nộp và hình thức trình bày

Sinh viên phải nộp đầy đủ các sản phẩm sau:

- **Quyển báo cáo bài tập lớn**, trình bày bài toán, phân tích yêu cầu, kiến trúc hệ thống, thiết kế các thành phần, các cơ chế giao tiếp phân tán đã sử dụng, cách triển khai, kết quả thực hiện và đánh giá hệ thống.
- **Mã nguồn đầy đủ của hệ thống phải được upload lên GitHub**, bao gồm toàn bộ project, file cấu hình, Dockerfile, `docker-compose.yml` và các thành phần cần thiết để có thể triển khai lại hệ thống. Repository phải có lịch sử commit thể hiện quá trình thực hiện bài tập lớn.
- Tài liệu hướng dẫn cài đặt, build và chạy hệ thống; có thể đặt trong `README.md` của repository.
- Sơ đồ kiến trúc và các hình minh họa cần thiết có thể được tích hợp trực tiếp trong quyển báo cáo.

Khi tham gia thi, sinh viên mang theo **máy tính cá nhân** có đầy đủ mã nguồn, môi trường và các thành phần cần thiết để trình bày bài tập lớn. Mã nguồn trên máy phải tương ứng với phiên bản đã upload lên GitHub. Sinh viên phải có khả năng trực tiếp khởi chạy hệ thống, minh họa các chức năng chính và giải thích cách các thành phần trong hệ thống giao tiếp với nhau.

## 3. Yêu cầu mở rộng

Phần này **không bắt buộc**. Các nội dung mở rộng chỉ được xem xét sau khi sinh viên đã hoàn thành đầy đủ các yêu cầu bắt buộc của đề tài và hệ thống hoạt động đúng theo yêu cầu chính.

Tùy đặc điểm đề tài, sinh viên có thể bổ sung một hoặc nhiều nội dung như:

- Retry và dead-letter queue.
- Idempotent consumer.
- Correlation ID.
- Health check.
- Structured logging.
- Authentication/authorization.
- Thêm service hoặc subscriber phục vụ thống kê, audit hoặc các chức năng mở rộng phù hợp với bài toán.
