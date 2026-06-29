namespace Navyblue.BaseLibrary.Validation;

public static class ValidationRules { public static bool IsEmail(string? value) => !string.IsNullOrWhiteSpace(value) && value.Contains('@') && value.Contains('.'); public static bool IsMobileChina(string? value) => !string.IsNullOrWhiteSpace(value) && value.Length == 11 && value.All(char.IsDigit) && value[0] == '1'; }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)] public sealed class RequiredGuidAttribute : Attribute;
