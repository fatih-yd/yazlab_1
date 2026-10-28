using UnityEngine;
using UnityEngine.AI;

// Tozcu'nun beyni. Hangi durumda oldugunu tutar ve durumlar arasi gecisi yonetir.
[RequireComponent(typeof(NavMeshAgent))]
public class TozcuKontrolcu : MonoBehaviour
{
    [SerializeField] private PowerPanel hedefPanel;
    [SerializeField] private float engelMesafesi = 1.5f;
    [SerializeField] private float yenidenPlanlamaBeklemesi = 0.5f;

    private ITozcuDurumu mevcutDurum;

    public NavMeshAgent Agent { get; private set; }
    public EngelSensoru Sensor { get; private set; }
    public PowerPanel HedefPanel => hedefPanel;
    public float YenidenPlanlamaBeklemesi => yenidenPlanlamaBeklemesi;

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
        if (hedefPanel == null)
        {
            Debug.LogWarning("Tozcu icin hedef panel atanmamis.");
            return;
        }

        DurumDegistir(new YolaCikDurumu());
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
