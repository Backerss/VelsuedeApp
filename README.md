# Velsuede — ระบบจัดการข้อมูลพนักงาน

> **ส่งมอบโดย:** นาย อาสาฬ รอดนวน  
> **ตำแหน่งที่สมัคร:** IT Staff  
> **บริษัท:** VEL SUEDE (THAILAND) COMPANY LIMITED

---

## 📋 ภาพรวมโปรแกรม

**VelsuedeApp** เป็นระบบจัดการข้อมูลพนักงาน (HR Management System) พัฒนาด้วย **C# Windows Forms** บน **.NET 10** เชื่อมต่อกับฐานข้อมูล **MySQL** รองรับการเพิ่ม / แก้ไข / ลบ / ค้นหาข้อมูลพนักงานผ่านหน้าจอที่ออกแบบด้วย Dark UI สมัยใหม่

---

## 🛠️ ความต้องการของระบบ (System Requirements)

| รายการ | รายละเอียด |
|---|---|
| ระบบปฏิบัติการ | Windows 10 / 11 (64-bit) |
| .NET Runtime | .NET 10 SDK หรือ Runtime |
| ฐานข้อมูล | MySQL Server 8.x หรือ MariaDB 10.x |
| IDE (สำหรับนักพัฒนา) | Visual Studio 2022 / VS Code |
| NuGet Package | `MySql.Data` v9.6.0 |

---

## 📦 ขั้นตอนการติดตั้ง

### ขั้นที่ 1 — ติดตั้ง .NET SDK

1. เปิดเว็บไซต์ [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
2. เลือกดาวน์โหลด **.NET 10 SDK** (Windows x64)
3. รันตัวติดตั้งและทำตามขั้นตอน
4. ยืนยันการติดตั้งโดยเปิด **Command Prompt** แล้วพิมพ์:

```bash
dotnet --version
```

ควรได้ผลลัพธ์เป็นเลขเวอร์ชัน เช่น `10.0.x`

---

### ขั้นที่ 2 — ติดตั้ง MySQL Server

1. ดาวน์โหลด **MySQL Community Server** จาก [https://dev.mysql.com/downloads/mysql/](https://dev.mysql.com/downloads/mysql/)
2. รันตัวติดตั้ง (MySQL Installer)
3. เลือก **Developer Default** หรือ **Server Only**
4. ตั้งค่า root password ตามต้องการ  
   > ⚠️ หากใช้ password ต้องแก้ไขค่า `Pwd=` ใน `Form1.cs` บรรทัดที่ 22 ให้ตรงกัน
5. ให้ MySQL ทำงานที่ Port **3306** (ค่าเริ่มต้น)
6. ยืนยันการทำงานของ MySQL:

```bash
mysql -u root -p
```

---

### ขั้นที่ 3 — โคลนหรือคัดลอกโปรเจกต์

วิธีที่ 1 — ใช้ Git:
```bash
git clone https://github.com/Backerss/VelsuedeApp
cd VelsuedeApp
```

วิธีที่ 2 — คัดลอกโฟลเดอร์โปรเจกต์ `VelsuedeApp` ไปวางในตำแหน่งที่ต้องการ

---

### ขั้นที่ 4 — Restore NuGet Packages

เปิด **Command Prompt** หรือ **PowerShell** ที่โฟลเดอร์โปรเจกต์ แล้วรัน:

```bash
dotnet restore
```

คำสั่งนี้จะดาวน์โหลด `MySql.Data` v9.6.0 โดยอัตโนมัติ

---

### ขั้นที่ 5 — ตั้งค่าการเชื่อมต่อฐานข้อมูล

เปิดไฟล์ `Form1.cs` และตรวจสอบบรรทัดที่ 22:

```csharp
private readonly string connStr =
    "Server=localhost;Port=3306;Database=Velsuede;Uid=root;Pwd=;CharSet=utf8;";
```

แก้ไขตามการตั้งค่า MySQL ของเครื่องท่าน:

| ค่า | คำอธิบาย | ค่าเริ่มต้น |
|---|---|---|
| `Server` | IP หรือชื่อเครื่อง MySQL | `localhost` |
| `Port` | Port ของ MySQL | `3306` |
| `Uid` | ชื่อผู้ใช้ MySQL | `root` |
| `Pwd` | รหัสผ่าน MySQL | *(ว่าง)* |

> 💡 **หมายเหตุ:** ไม่ต้องสร้างฐานข้อมูลหรือตารางเอง โปรแกรมจะสร้าง `Velsuede` database และตาราง `employees` ให้อัตโนมัติเมื่อเปิดครั้งแรก

---

### ขั้นที่ 6 — Build และรันโปรแกรม

**วิธีที่ 1 — ผ่าน Command Line:**

```bash
# Build โปรเจกต์
dotnet build

# รันโปรแกรม
dotnet run
```

**วิธีที่ 2 — ผ่าน Visual Studio 2022:**

1. เปิดไฟล์ `VelsuedeApp.csproj` ด้วย Visual Studio 2022
2. กด **F5** หรือคลิกปุ่ม ▶ **Start** เพื่อ Build และรัน

---

## 🚀 การใช้งานโปรแกรม

เมื่อเปิดโปรแกรมจะพบหน้าจอหลัก **ระบบจัดการข้อมูลพนักงาน** ประกอบด้วยฟังก์ชันดังนี้:

| ปุ่ม | ฟังก์ชัน |
|---|---|
| 🔍 **ค้นหา** | ค้นหาพนักงานจาก รหัส / ชื่อ / นามสกุล / ตำแหน่ง / แผนก / เบอร์โทร |
| ➕ **เพิ่ม** | เปิดหน้าต่างกรอกข้อมูลพนักงานใหม่ |
| ✏ **แก้ไข** | แก้ไขข้อมูลพนักงานที่เลือกในตาราง |
| 🗑 **ลบ** | ลบข้อมูลพนักงานที่เลือก (มีการยืนยันก่อนลบ) |
| ✖ **รีเซ็ต** | ล้างช่องค้นหาและโหลดข้อมูลทั้งหมดใหม่ |

> 💡 ดับเบิลคลิกที่แถวในตารางเพื่อเปิดหน้าแก้ไขข้อมูลพนักงานได้โดยตรง

---

## 🗄️ โครงสร้างฐานข้อมูล

**Database:** `Velsuede`  
**Table:** `employees`

| คอลัมน์ | ชนิดข้อมูล | คำอธิบาย |
|---|---|---|
| `emp_id` | VARCHAR(20) | รหัสพนักงาน (Primary Key) |
| `first_name` | VARCHAR(100) | ชื่อ |
| `last_name` | VARCHAR(100) | นามสกุล |
| `position` | VARCHAR(100) | ตำแหน่งงาน |
| `department` | VARCHAR(100) | แผนก |
| `phone` | VARCHAR(20) | เบอร์โทรศัพท์ |
| `status` | TINYINT(1) | สถานะ (1 = กำลังทำงาน, 0 = ลาออก) |

---

## 📁 โครงสร้างโปรเจกต์

```
VelsuedeApp/
├── Form1.cs               # หน้าจอหลัก + EmployeeDialog + DatabaseInitializer
├── Form1.Designer.cs      # Designer stub
├── Program.cs             # Entry point
├── VelsuedeApp.csproj     # Project file (.NET 10, Windows Forms)
└── README.md              # ไฟล์นี้
```

---

## ❗ การแก้ไขปัญหาเบื้องต้น

| ปัญหา | แนวทางแก้ไข |
|---|---|
| เชื่อมต่อ MySQL ไม่ได้ | ตรวจสอบว่า MySQL Service กำลังทำงาน และ Port 3306 ไม่ถูกบล็อก |
| รหัสผ่านไม่ถูกต้อง | แก้ค่า `Pwd=` ใน `Form1.cs` บรรทัดที่ 22 ให้ตรงกับรหัสผ่าน root ของท่าน |
| `dotnet` ไม่พบคำสั่ง | ติดตั้ง .NET 10 SDK และ Restart Command Prompt |
| ตัวอักษรภาษาไทยแสดงผลผิด | ตรวจสอบว่า MySQL ใช้ Charset `utf8mb4` และ collation `utf8mb4_unicode_ci` |

---

*README นี้จัดทำโดย **นาย อาสาฬ รอดนวน** เพื่อประกอบการสมัครงานตำแหน่ง IT Staff*  
*บริษัท VEL SUEDE (THAILAND) COMPANY LIMITED*
