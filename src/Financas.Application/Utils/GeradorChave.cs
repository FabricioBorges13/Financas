using System.Security.Cryptography;
using System.Text;

public static class GeradorChave
{
    public static string GerarChaveIdempotencia(TipoTransacao tipoTransacao, Guid contaOrigemId, Guid? transacaoId = null, Guid? contaDestinoId = null, decimal? valor = null)
    {
        var rawKey = $"Transacao:{tipoTransacao}{contaOrigemId}:{transacaoId}:{contaDestinoId}:{valor}";
        return $"idemp:{Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)))}";
    }

    public static string GerarChaveLock(Guid contaId)
    {
        var rawKey = $"Lock:{contaId}";
        return $"lock:{Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)))}";
    }
}