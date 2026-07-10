namespace Navyblue.Samples.Application.Authentication;

/// <summary>
///     Abstraction for issuing an access token for an authenticated user.
///     Defined in the application layer so command handlers stay decoupled from
///     any specific web/JWT stack; implemented in the web host using Navyblue JWT.
/// </summary>
public interface ITokenIssuer
{
    /// <summary>
    ///     Issues a signed token for the given user.
    /// </summary>
    /// <param name="userId">Authenticated user id.</param>
    /// <param name="userName">Display name added as a claim.</param>
    /// <param name="extraClaims">Optional additional claim type/value pairs.</param>
    /// <returns>The compact, serialized token string.</returns>
    string Issue(long userId, string userName, IEnumerable<KeyValuePair<string, string>>? extraClaims = null);
}
