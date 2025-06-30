using Microsoft.EntityFrameworkCore;

namespace Financas.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Clientes.AnyAsync()) return;

        var cliente = new Cliente("Teste1", "23579305000", TipoDocumento.CPF);
        var cliente2 = new Cliente("Teste1", "23579305000", TipoDocumento.CPF);

        var conta = new Conta(TipoConta.Corrente, cliente);
        conta.AdicionarSaldo(1000);
        conta.GerarNumeroConta();

        var conta2 = new Conta(TipoConta.Corrente, cliente2);
        conta2.AdicionarSaldo(1000);
        conta.GerarNumeroConta();

        var transacao = new Transacao(conta.Id, 1000, TipoTransacao.VendaCreditoAVista);

        await context.Clientes.AddAsync(cliente);
        await context.Clientes.AddAsync(cliente2);
        await context.Contas.AddAsync(conta);
        await context.Contas.AddAsync(conta2);
        await context.Transacoes.AddAsync(transacao);

        await context.SaveChangesAsync();
    }
}
