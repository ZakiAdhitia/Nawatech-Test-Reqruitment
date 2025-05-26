## Cara Menjalankan Website PROJECT 1

1. **Clone repository**
git clone https://github.com/ZakiAdhitia/Nawatech-Test-Reqruitment.git
cd Project1

2. **Instal semua package lokal yang dibutuhkan**

dotnet restore

3. **instal dotnet-ef untuk membuat database**

dotnet tool install --global dotnet-ef

4. **buat file.env dan konfigurasikan**

cek .env.example untuk pengisian file .env

5. **buat table dengan command berikut**
<!-- pastikan sudah setup database di .env -->
dotnet ef database update

6. **jalankan web**

dotnet run
<!-- jika ingin agar otomatis reload -->
dotnet watch run
open browser: http://localhost:5026

## Cara Menjalankan PROJECT 2

1. **Clone repository (jika belum clone)**

git clone https://github.com/ZakiAdhitia/Nawatech-Test-Reqruitment.git
cd Project2

2. **jalankan web**
dotnet run

