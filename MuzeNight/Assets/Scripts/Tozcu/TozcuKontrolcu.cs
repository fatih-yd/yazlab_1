using UnityEngine;
using UnityEngine.AI;

// Tozcu'nun beyni. Hangi durumda oldugunu tutar ve durumlar arasi gecisi yonetir.
[RequireComponent(typeof(NavMeshAgent))]
public class TozcuKontrolcu : MonoBehaviour
{
    [SerializeField] private PowerPanel[] paneller;

    private PowerPanel hedefPanel;
    [SerializeField] private float engelMesafesi = 1.5f;
    [SerializeField] private float yenidenPlanlamaBeklemesi = 0.5f;

    private ITozcuDurumu mevcutDurum;

    public NavMeshAgent Agent { get; private set; }
    public EngelSensoru Sensor { get; private set; }
    public PowerPanel HedefPanel => hedefPanel;
    public float YenidenPlanlamaBeklemesi => yenidenPlanlamaBeklemesi;

    // Engel algilandiktan sonra kisa bir sure engel kontrolu durur, boylece duvar yanindaki dongu olusmaz
    [SerializeField] private float engelSogumaSuresi = 1.5f;
    private float sonEngelZamani = -100f;

    public bool EngelSogumaBitti => Time.time - sonEngelZamani >= engelSogumaSuresi;

    public void EngelIsaretle()
    {
        sonEngelZamani = Time.time;
    }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Sensor = new EngelSensoru(transform, engelMesafesi);
    }

    private void Start()
    {
        DurumDegistir(new BekleDurumu());
    }

    private void Update()
    {
        mevcutDurum?.Update(this);
    }

    public void DurumDegistir(ITozcuDurumu yeniDurum)
    {
        mevcutDurum?.Exit(this);
        mevcutDurum = yeniDurum;
        mevcutDurum.Enter(this);
    }

    // Oyuncunun "sigorta paneline git" komutu.
    public void PaneleGitKomutu()
    {
        PowerPanel siradaki = SiradakiPaneli();
        if (siradaki == null)
        {
            Debug.Log("Tozcu: enerjilendirilecek panel kalmadi.");
            return;
        }

        hedefPanel = siradaki;
        Debug.Log("Tozcu hedef: " + hedefPanel.name);
        DurumDegistir(new YolaCikDurumu());
    }

    // Sirayla: henuz elektrigi acilmamis ilk panel
    private PowerPanel SiradakiPaneli()
    {
        if (paneller == null)
        {
            return null;
        }

        foreach (PowerPanel panel in paneller)
        {
            if (panel != null && !panel.ElektrikAcikMi)
            {
                return panel;
            }
        }

        return null;
    }

    // Hedefin erisim noktasina olan mesafe (hedef yoksa sonsuz)
    public float HedefeUzaklik
    {
        get
        {
            if (hedefPanel == null)
            {
                return float.PositiveInfinity;
            }

            return Vector3.Distance(transform.position, hedefPanel.ErisimNoktasi.position);
        }
    }

    // Ajan hedefe ulasti mi?
    public bool PaneleVardiMi()
    {
        return Agent.hasPath && !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.2f;
    }

    // Unity'de Tozcu'nun ayar menusunden tetiklenebilen test komutu.
    [ContextMenu("Test: Panele git")]
    private void TestPaneleGit()
    {
        PaneleGitKomutu();
    }
}
