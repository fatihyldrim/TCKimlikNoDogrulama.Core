using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace TCKimlikNoDogrulama.Core
{
    public class Dogrulama
    {      
        /// <summary>
        /// Nüfus ve Vatandaşlık İşleri Genel Müdürlüğüne TC Kimlik Numarası, Ad, Soyad ve Doğum Yılı göndererek kişinin Türkiye Cumhuriyeti kayıtlarında olup olmadığını doğrular.
        /// KisiVarMi() fonksiyonu ile birlikte kullanılır.
        /// </summary>
        /// <param name="TCKimlikNo"></param>
        /// <param name="Adi"></param>
        /// <param name="Soyadi"></param>
        /// <param name="DogumYili"></param>
        public Dogrulama(long TCKimlikNo, string Adi, string Soyadi, int DogumYili)
        {
            _TCKimlikNo = TCKimlikNo;
            _Adi = Adi.Trim().ToUpper();
            _Soyadi = Soyadi.Trim().ToUpper();
            _DogumYili = DogumYili;
        }

        /// <summary>
        /// TC Kimlik Numarasını TC Kimlik No Algoritmasına göre kontrol eder. 
        /// TCAlgoritmasi() fonksiyonu ile birlikte kullanılır.
        /// </summary>
        /// <param name="TCKimlikNo"></param>
        public Dogrulama(long TCKimlikNo)
        {
            _TCKimlikNo = TCKimlikNo;
        }

        /// <summary>
        /// T.C kimlik numarası 11 haneli ve sayısal olmalıdır.
        ///T.C kimlik numarası 0 ile başlayamaz.
        ///T.C kimlik numarasının 11 haneli ve sayısal değerde olduğu kontrol edilir.İlk 9 rakam arasındaki formül 10.cu rakamı, ilk 10 rakam arasındaki formülasyon ise 11.ci rakamı oluşturur.İlk rakam 0 olamaz.
        /// 1,3,5,7 ve 9.cu hanelerin toplamının 7 ile çarpımından 2,4,6, ve 8. haneler çıkartıldığında geriye kalan sayının 10’a göre modu 10. haneyi verir. (çıkarma işleminden elde edilen sonucun 10’a bölümünden kalan) 1,2,3,4,5,6,7,8,9 ve 10. sayıların toplamının 10’a göre modu (10’a bölümünden kalan) 11. rakamı sağlar.
        /// </summary>
        /// <returns>bool</returns>
        public bool TCAlgoritmasi()
        {
            char[] arr = _TCKimlikNo.ToString().ToCharArray();
            int sumEven = 0, sumOdd = 0, sumFirst10 = 0, i = 0;
            bool sonuc = true;

            if (_TCKimlikNo.ToString().Length != 11)
            {
                sonuc = false;
            }
            if (arr[0] == '0')
            {
                sonuc = false;
            }

            while (i <= 8)
            {
                int temp = int.Parse(arr[i].ToString());
                sumFirst10 += temp;
                if (i % 2 == 1) sumEven += temp;
                else sumOdd += temp;
                i++;
            }
            sumFirst10 += int.Parse(arr[9].ToString());

            if (!(((sumEven * 9) + (sumOdd * 7)) % 10 == int.Parse(arr[9].ToString()) && (sumFirst10 % 10 == int.Parse(arr[10].ToString()))))
            {
                sonuc = false;
            }

            return sonuc;
        }


        /// <summary>
        /// https://tckimlik.nvi.gov.tr da kişi kontrolü.
        /// </summary>
        /// <returns></returns>
        public bool KisiVarMi()
        {
            InputKontrolleri();
            string requestUrl = "https://tckimlik.nvi.gov.tr/Service/KPSPublic.asmx";
            byte[] bytes = Encoding.UTF8.GetBytes(RequestXml());
            int num = bytes.Length;
            HttpWebRequest request = BuildRequest(requestUrl, num);
            return GetResponse(request, bytes);
        }



        private bool TCDogruMu()
        {
            char[] arr = _TCKimlikNo.ToString().ToCharArray();
            int sumEven = 0, sumOdd = 0, sumFirst10 = 0, i = 0;
            bool sonuc = true;

            if (_TCKimlikNo.ToString().Length != 11)
            {
                sonuc = false;
            }
            if (arr[0] == '0')
            {
                sonuc = false;
            }

            while (i <= 8)
            {
                int temp = int.Parse(arr[i].ToString());
                sumFirst10 += temp;
                if (i % 2 == 1) sumEven += temp;
                else sumOdd += temp;
                i++;
            }
            sumFirst10 += int.Parse(arr[9].ToString());

            if (!(((sumEven * 9) + (sumOdd * 7)) % 10 == int.Parse(arr[9].ToString()) && (sumFirst10 % 10 == int.Parse(arr[10].ToString()))))
            {
                sonuc = false;
            }

            return sonuc;
        }

        private bool DogumYiliDogruMu()
        {
            return _DogumYili.ToString().Length == 4;
        }

        private bool AdiDogruMu()
        {
            return !string.IsNullOrEmpty(_Adi);
        }

        private bool SoyAdiDogruMu()
        {
            return !string.IsNullOrEmpty(_Soyadi);
        }

        private void InputKontrolleri()
        {
            if (!TCDogruMu())
            {
                throw new Exception("TC Kimlik Numarası hatalı.");
            }
            if (!AdiDogruMu())
            {
                throw new Exception("Adi hatalı.");
            }
            if (!SoyAdiDogruMu())
            {
                throw new Exception("Soyadi hatalı.");
            }
            if (!DogumYiliDogruMu())
            {
                throw new Exception("Doğum Yılı hatalı.");
            }
        }

        private string RequestXml()
        {
            string str = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            str += "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">";
            str += "<soap12:Body>";
            str += "<TCKimlikNoDogrula xmlns=\"http://tckimlik.nvi.gov.tr/WS\">";
            object obj = str;
            str = obj + "<TCKimlikNo>" + _TCKimlikNo + "</TCKimlikNo>";
            str = str + "<Ad>" + _Adi + "</Ad>";
            str = str + "<Soyad>" + _Soyadi + "</Soyad>";
            object obj2 = str;
            str = obj2 + "<DogumYili>" + _DogumYili + "</DogumYili>";
            str += "</TCKimlikNoDogrula>";
            str += "</soap12:Body>";
            return str + "</soap12:Envelope>";
        }

        private HttpWebRequest BuildRequest(string requestUrl, long contentLength)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.ContentType = "application/soap+xml; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = contentLength;
            return httpWebRequest;
        }

        private bool GetResponse(HttpWebRequest request, byte[] requestBytesArray)
        {
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(requestBytesArray, 0, (int)request.ContentLength);
            }
            try
            {
                string text;
                using (WebResponse webResponse = request.GetResponse())
                {
                    if (webResponse == null)
                    {
                        return false;
                    }
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        text = streamReader.ReadToEnd().Trim();
                    }
                }
                XDocument xDocument = XDocument.Parse(text);
                string value = xDocument.Descendants().SingleOrDefault((XElement x) => x.Name.LocalName == "TCKimlikNoDogrulaResult").Value;
                return bool.Parse(value);
            }
            catch
            {
                return false;
            }
        }

        private long _TCKimlikNo
        {
            get;
            set;
        }

        private string _Adi
        {
            get;
            set;
        }

        private string _Soyadi
        {
            get;
            set;
        }

        private int _DogumYili
        {
            get;
            set;
        }
    }
}