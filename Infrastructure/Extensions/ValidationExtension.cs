namespace Infrastructure.Extensions;

public static class ValidationExtension
{
    public static bool IsValidCnpj(this string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return false;

        var invalid = new[]
        {
            "00000000000000", 
            "11111111111111", 
            "22222222222222",
            "33333333333333", 
            "44444444444444", 
            "55555555555555",
            "66666666666666", 
            "77777777777777", 
            "88888888888888",
            "99999999999999"
        };

        if (invalid.Contains(cnpj))
            return false;

        int[] multiplicador1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplicador2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var tempCnpj = cnpj[..12];
        var soma = 0;

        for (var i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        var digito1 = (resto < 2 ? 0 : 11 - resto).ToString();

        tempCnpj += digito1;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        var digito2 = (resto < 2 ? 0 : 11 - resto).ToString();

        return cnpj.EndsWith(digito1 + digito2);
    }
}