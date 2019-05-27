# TCKimlikNoDogrulama.Core

*.Net Core 2 için nuget paket projesidir.* 

TC Kimlik Numarası için Algoritma doğrulaması yapılabilir. 
Nüfus ve Vatandaşlık İşleri Genel Müdürlüğünden kişi doğrulaması yapabilir. 

# Kullanımı

1 - Nüfus ve Vatandaşlık İşleri Genel Müdürlüğüne (Mernis) TC Kimlik Numarası, Ad, Soyad ve Doğum Yılı göndererek kişinin Türkiye Cumhuriyeti kayıtlarında olup olmadığını doğrular. 

```csharp      
bool cevap = new TCKimlikNoDogrulamaCORE(01234567890, "Adı", "Soyadı", 1900).KisiVarMi();
```


2 - TC Kimlik Numarasını TC Kimlik No Algoritmasına göre kontrol eder.

```csharp      
bool cevap = new TCKimlikNoDogrulamaCORE(01234567890).TCAlgoritmasi();
```
