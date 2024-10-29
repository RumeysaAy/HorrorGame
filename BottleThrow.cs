using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleThrow : MonoBehaviour
{
    public float rotationSpeed = 0.5f; // dönüş hızı
    public float throwPower = 40f; // fırlatma gücü (şişeyi fırlatmak için uygulanan güç)
    public GameObject bottleObj; // fırlatılacak şişe nesnesi
    public GameObject fireBottleObj; // fırlatılacak molotof kokteyli nesnesi
    public Transform throwPoint; // atış noktası (şişenin fırlatılacağı nokta)

    LineRenderer line;
    public int linePoints = 75; // çizgi noktaları sayısı
    // nokta sayısı arttıkça eğri o kadar düzgün olur fakat kare hızı yavaşlar
    public float pointDistance = 0.03f; // bu çizgi noktalarının her biri arasındaki mesafe
    public LayerMask collideLayer; // çizginin çarpışacağı nesnenin katmanı
    public Material mBlue, mRed; // şişe için mavi, molotof kokteyli için kırmızı

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // eğer envanter menüsü açık değilse ve şişe(7) veya molotof kokteyli(8) seçiliyse
        if (SaveScript.inventoryOpen == false && SaveScript.weaponID > 6)
        {
            // fırlatılacak olan şişenin rotasyonunun hesaplanması

            float HorizontalRotation = Input.GetAxis("Mouse X") * 2;
            float VerticalRotation = Input.GetAxis("Mouse Y") * 2;
            // çizgiyi kamerayla aynı hizada tutacak. Bu yapılmazsa oluşturulan çizgi dünyada kalacak ve etrafta hareket etmeyecek.
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, HorizontalRotation * rotationSpeed, VerticalRotation * rotationSpeed));

            // Farenin yukarı veya aşağı hareket ettirilmesine bağlı olarak mesafenin ayarlanması
            // atış mesafesinin kontrol edilmesi

            // fareyi ileriye doğru hareket ettirdiğimde
            if (Input.GetAxis("Mouse Y") > 0)
            {
                if (throwPower < 70)
                {
                    throwPower += 6 * Time.deltaTime; // atış gücü artar
                }
            }

            // fareyi geriye doğru hareket ettirdiğimde
            if (Input.GetAxis("Mouse Y") < 0)
            {
                if (throwPower > 20)
                {
                    throwPower -= 12 * Time.deltaTime; // atış gücü azalır
                }
            }

            // Karakterin sağ kolunun arkasındaki boş oyun nesnesinden doğrudan ileri doğru bir çizgi çekilecek. 
            // Bu çizgi throwPower’a bağlı olarak büyüyebilir veya küçülebilir.

            line.positionCount = linePoints; // çizgi noktaları sayısı
            List<Vector3> points = new List<Vector3>(); // çizgi noktalarının konumu
            Vector3 startPos = throwPoint.position; // çizginin başlangıç noktası
                                                    // fareyi nasıl hareket ettirdiğime bağlı olarak “throwPower” artar veya azalır.
                                                    // başlangıç hızı = başlangıç noktasının ileri yönü * throwPower
            Vector3 startVelocity = throwPoint.forward * throwPower; // başlangıç hızı

            // Çizgi yalnızca farenin sağ tuşuna bastığım sürece oluşturulsun
            if (Input.GetMouseButton(1))
            {
                if (SaveScript.weaponID == 7)
                {
                    // eğer şişe ise çizgi mavi renkli olsun
                    line.material = mBlue;
                }

                if (SaveScript.weaponID == 8)
                {
                    // eğer molotof kokteyli ise çizgi kırmızı renkli olsun
                    line.material = mRed;
                }

                // toplam "linePoints" nokta var ve her iki noktanın arasında "pointDistance" birim fark var
                for (float i = 0; i < linePoints; i += pointDistance)
                {
                    Vector3 newPoint = startPos + i * startVelocity;
                    newPoint.y = startPos.y + startVelocity.y + i + Physics.gravity.y / 2f * i * i;
                    points.Add(newPoint); // oluşturulan eğrinin noktaları

                    // oluşan eğri eğer bir nesneye çarptıysa çizimi durdurmalıyım
                    // o noktadaki küre, yarıçapı, belirlenen katmandaki nesnelerden birisine çarptıysa
                    // Ne zaman vurulan bir şey olsa, bu bir diziye eklenecek. Yani eğer uzunluk sıfırdan büyükse bir nesneyle çarpıştığını gösterir.
                    if (Physics.OverlapSphere(newPoint, 0.01f, collideLayer).Length > 0)
                    {
                        // eğrideki konum sayısı, nokta sayısına eşitlenir.
                        line.positionCount = points.Count;
                        break;
                    }
                }
                // eğri oluşturulacak ve görüntülenecek
                line.SetPositions(points.ToArray());
            }
            // sağ fare düğmemi bıraktığımda
            if (Input.GetMouseButtonUp(1))
            {
                // çizim dursun
                line.positionCount = 0; // toplam konum sayısı sıfırlanır
            }

            // true ise boş şişe atılır
            if (WeaponManager.emptyBottleThrow == true)
            {
                // her şey bir animasyon tarafından yönlendiriliyor.

                WeaponManager.emptyBottleThrow = false; // yeni şişe için false

                // atılacak olan şişe, fırlatma başlangıç noktasında oluşturulsun
                GameObject createBottle = Instantiate(bottleObj, throwPoint.position, throwPoint.rotation);
                // şişeye kuvvet uygulamak için rigidbody'ye ulaşmam lazım
                createBottle.GetComponentInChildren<Rigidbody>().velocity = throwPoint.transform.forward * throwPower;

                // Boş şişeyi attığımda, toplam topladığım boş şişe miktarını bir eksiltmeliyim.
                SaveScript.weaponAmts[7]--;

                // savescript.cs’de toplam topladığım boş şişe miktarını güncellemem gerekiyor
                SaveScript.change = true; // bu yüzden true'ya eşitledim
            }

            // true ise molotof kokteyli atılır
            if (WeaponManager.fireBottleThrow == true)
            {
                // her şey bir animasyon tarafından yönlendiriliyor.

                WeaponManager.fireBottleThrow = false; // yeni bir molotof kokteyli için false

                // atılacak olan molotof kokteyli, fırlatma başlangıç noktasında oluşturulsun
                GameObject createBottle = Instantiate(fireBottleObj, throwPoint.position, throwPoint.rotation);
                // molotof kokteyline kuvvet uygulamak için rigidbody'ye ulaşmam lazım
                createBottle.GetComponentInChildren<Rigidbody>().velocity = throwPoint.transform.forward * throwPower;

                // molotof kokteyli attığımda, toplam topladığım şişe miktarını bir eksiltmeliyim.
                // molotof kokteyli şişe ve kumaştan oluşur
                SaveScript.weaponAmts[7]--;

                // kumaşın azaltılması
                SaveScript.itemAmts[3]--;

                // savescript.cs’de toplam topladığım molotof kokteyli miktarını güncellemem gerekiyor
                SaveScript.change = true; // bu yüzden true'ya eşitledim
            }
        }
    }
}
