using System.Threading.Tasks;

// Terminalin cevap aldigi kaynak. Gelistirmede sahte, son surumde Ollama tabanli olabilir.
public interface ICevapKaynagi
{
    Task<ArgusCevap> CevapAlAsync(string oyuncuMetni);
}
