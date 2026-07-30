// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ToolArgumentValidator.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ToolArgumentValidator.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Globalization;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Validates and normalizes tool arguments against the tool's declared parameter specs.
///     Tools that declare no parameters are passed through unchanged, which keeps adoption incremental.
/// </summary>
public sealed class SpecToolArgumentValidator(NavyblueAiOptions? options = null) : IToolArgumentValidator
{
    private readonly bool _rejectUnknown = options?.RejectUnknownToolArguments ?? true;

    #region IToolArgumentValidator Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyDictionary<string, object?>>> ValidateAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tool);
        ArgumentNullException.ThrowIfNull(arguments);

        IReadOnlyList<ToolParameterSpec>? specs = tool.Parameters;
        if (specs is null || specs.Count == 0)
        {
            return ValueTask.FromResult(Result.Success(arguments));
        }

        Dictionary<string, object?> normalized = new(StringComparer.OrdinalIgnoreCase);

        if (this._rejectUnknown)
        {
            foreach (string name in arguments.Keys)
            {
                if (!specs.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    return ValueTask.FromResult(Fail($"Unknown argument '{name}' for tool '{tool.Name}'."));
                }
            }
        }

        foreach (ToolParameterSpec spec in specs)
        {
            bool present = TryGetValue(arguments, spec.Name, out object? raw);
            bool empty = !present || raw is null || (raw is string s && string.IsNullOrWhiteSpace(s));

            if (empty)
            {
                if (spec.Required)
                {
                    return ValueTask.FromResult(Fail($"Required argument '{spec.Name}' is missing for tool '{tool.Name}'."));
                }

                if (present)
                {
                    normalized[spec.Name] = raw;
                }

                continue;
            }

            Result<object?> converted = Convert(spec, raw!, tool.Name);
            if (converted.Failed)
            {
                return ValueTask.FromResult(Result.Failure<IReadOnlyDictionary<string, object?>>(converted.Error));
            }

            normalized[spec.Name] = converted.Value;
        }

        return ValueTask.FromResult(Result.Success<IReadOnlyDictionary<string, object?>>(normalized));
    }

    #endregion

    private static bool TryGetValue(IReadOnlyDictionary<string, object?> arguments, string name, out object? value)
    {
        if (arguments.TryGetValue(name, out value))
        {
            return true;
        }

        foreach (KeyValuePair<string, object?> pair in arguments)
        {
            if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                value = pair.Value;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static Result<object?> Convert(ToolParameterSpec spec, object raw, string toolName)
    {
        string text = raw as string ?? System.Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;

        if (spec.AllowedValues is { Count: > 0 } allowed
            && !allowed.Any(v => string.Equals(v, text, StringComparison.OrdinalIgnoreCase)))
        {
            return FailValue($"Argument '{spec.Name}' must be one of: {string.Join(", ", allowed)}.");
        }

        switch (spec.Type)
        {
            case ToolParameterType.String:
                if (spec.MaxLength is { } maxLength && text.Length > maxLength)
                {
                    return FailValue($"Argument '{spec.Name}' exceeds max length {maxLength}.");
                }

                return Result.Success<object?>(text);

            case ToolParameterType.Integer:
                if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integer))
                {
                    return FailValue($"Argument '{spec.Name}' must be an integer.");
                }

                return CheckRange(spec, integer) ?? Result.Success<object?>(integer);

            case ToolParameterType.Number:
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                {
                    return FailValue($"Argument '{spec.Name}' must be a number.");
                }

                return CheckRange(spec, number) ?? Result.Success<object?>(number);

            case ToolParameterType.Boolean:
                if (raw is bool boolean)
                {
                    return Result.Success<object?>(boolean);
                }

                if (!bool.TryParse(text, out bool parsedBoolean))
                {
                    return FailValue($"Argument '{spec.Name}' must be a boolean.");
                }

                return Result.Success<object?>(parsedBoolean);

            case ToolParameterType.DateTime:
                if (raw is DateTimeOffset dto)
                {
                    return Result.Success<object?>(dto);
                }

                if (!DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTimeOffset parsedDate))
                {
                    return FailValue($"Argument '{spec.Name}' must be a date/time value.");
                }

                return Result.Success<object?>(parsedDate);

            default:
                // Array / Object are passed through; the tool owns their shape.
                return Result.Success<object?>(raw);
        }

        static Result<object?>? CheckRange(ToolParameterSpec spec, double value)
        {
            if (spec.Minimum is { } min && value < min)
            {
                return FailValue($"Argument '{spec.Name}' must be >= {min}.");
            }

            if (spec.Maximum is { } max && value > max)
            {
                return FailValue($"Argument '{spec.Name}' must be <= {max}.");
            }

            return null;
        }
    }

    private static Result<IReadOnlyDictionary<string, object?>> Fail(string message)
        => Result.Failure<IReadOnlyDictionary<string, object?>>(new Error(AiErrorCodes.ToolArgumentInvalid, message));

    private static Result<object?> FailValue(string message)
        => Result.Failure<object?>(new Error(AiErrorCodes.ToolArgumentInvalid, message));
}

/// <summary>
///     Validator that accepts arguments unchanged. Only for tools whose contract is enforced elsewhere.
/// </summary>
public sealed class PassThroughToolArgumentValidator : IToolArgumentValidator
{
    #region IToolArgumentValidator Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyDictionary<string, object?>>> ValidateAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success(arguments));

    #endregion
}