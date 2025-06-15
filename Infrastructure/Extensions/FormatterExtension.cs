namespace Infrastructure.Extensions;

public static class FormatterExtension
{
    public static string RemoveNonNumericCharacters(this string str) => new (str.Where(char.IsDigit).ToArray());

    public static string FormatToCnpj(this string cnpj) => Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
}