CREATE DATABASE BookStoreASM5;
GO

USE BookStoreASM5;
GO

CREATE TABLE ChuDe (
    MaChuDe INT IDENTITY(1,1) PRIMARY KEY,
    TenChuDe NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Sach (
    MaSach INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    TacGia NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX),
    Gia DECIMAL(18,2) NOT NULL,
    HinhAnh NVARCHAR(255),
    NgayCapNhat DATE,
    MaChuDe INT NOT NULL,

    CONSTRAINT FK_Sach_ChuDe 
    FOREIGN KEY (MaChuDe) REFERENCES ChuDe(MaChuDe)
);
GO

INSERT INTO ChuDe (TenChuDe)
VALUES 
(N'Cuộc sống'),
(N'Lập trình'),
(N'Sức khỏe'),
(N'Thiếu nhi');

INSERT INTO Sach (TenSach, TacGia, MoTa, Gia, HinhAnh, NgayCapNhat, MaChuDe)
VALUES
(N'Đắc Nhân Tâm', N'Dale Carnegie', N'Sách kỹ năng sống', 85000, N'dacnhantam.jpg', '2024-01-01', 1),
(N'C# Cơ Bản', N'Nguyễn Văn A', N'Sách học lập trình C#', 120000, N'csharp.jpg', '2024-02-01', 2),
(N'Java Căn Bản', N'Trần Văn B', N'Sách học Java', 130000, N'java.jpg', '2024-03-01', 2);