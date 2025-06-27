using System.Text.RegularExpressions;

public class ValidadorDocumento
{
    public static bool EhValido(string documento, TipoDocumento tipo)
    {
        documento = OnlyNumbers(documento);

        return tipo switch
        {
            TipoDocumento.CPF => EhCPFValido(documento),
            TipoDocumento.CNPJ => EhCNPJValido(documento),
            _ => false
        };
    }

    private static bool EhCPFValido(string cpf)
    {
        if (cpf.Length != 11 || Regex.IsMatch(cpf, @"^(.)\1{10}$"))
            return false;

        var tempCpf = cpf[..9];
        var sum = 0;

        for (var i = 0; i < 9; i++)
            sum += (10 - i) * (tempCpf[i] - '0');

        var firstDigit = sum % 11 < 2 ? 0 : 11 - sum % 11;
        tempCpf += firstDigit;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (11 - i) * (tempCpf[i] - '0');

        var secondDigit = sum % 11 < 2 ? 0 : 11 - sum % 11;

        return cpf.EndsWith($"{firstDigit}{secondDigit}");
    }

    private static bool EhCNPJValido(string cnpj)
    {
        if (cnpj.Length != 14 || Regex.IsMatch(cnpj, @"^(.)\1{13}$"))
            return false;

        int[] mult1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = new int[] { 6 }.Concat(mult1).ToArray();

        var tempCnpj = cnpj[..12];
        var sum = tempCnpj.Select((c, i) => (c - '0') * mult1[i]).Sum();

        var firstDigit = sum % 11 < 2 ? 0 : 11 - sum % 11;
        tempCnpj += firstDigit;

        sum = tempCnpj.Select((c, i) => (c - '0') * mult2[i]).Sum();
        var secondDigit = sum % 11 < 2 ? 0 : 11 - sum % 11;

        return cnpj.EndsWith($"{firstDigit}{secondDigit}");
    }

    private static string OnlyNumbers(string input) =>
        Regex.Replace(input, "[^0-9]", "");
}