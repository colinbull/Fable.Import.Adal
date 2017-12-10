// ts2fable 0.5.2
module rec adal
open System
open Fable.Core
open Fable.Import.JS

let [<Import("*","adal")>] AuthenticationContext: Adal.AuthenticationContext = jsNative
let [<Import("*","adal")>] Logging: Adal.Logging = jsNative
let [<Import("*","adal")>] adal: Adal.IExports = jsNative

module Adal =

    type [<AllowNullLiteral>] IExports =
        abstract AuthenticationContext: Adal.AuthenticationContext
        abstract Logging: Adal.Logging

    type [<AllowNullLiteral>] Config =
        abstract tenant: string option with get, set
        abstract clientId: string with get, set
        abstract redirectUri: string option with get, set
        abstract instance: string option with get, set
        abstract endpoints: obj option with get, set
        abstract popUp: bool option with get, set
        abstract localLoginUrl: string option with get, set
        abstract displayCall: (string -> obj option) option with get, set
        abstract postLogoutRedirectUri: string option with get, set
        abstract cacheLocation: string option with get, set
        abstract anonymousEndpoints: ResizeArray<string> option with get, set
        abstract expireOffsetSeconds: float option with get, set
        abstract correlationId: string option with get, set
        abstract loginResource: string option with get, set
        abstract resource: string option with get, set
        abstract extraQueryParameter: string option with get, set
        abstract navigateToLoginRequestUrl: bool option with get, set
        abstract logOutUri: string option with get, set
        abstract isAngular: bool option with get, set

    type [<AllowNullLiteral>] User =
        abstract userName: string with get, set
        abstract profile: obj option with get, set

    type [<AllowNullLiteral>] RequestInfo =
        abstract valid: bool with get, set
        abstract parameters: obj option with get, set
        abstract stateMatch: bool with get, set
        abstract stateResponse: string with get, set
        abstract requestType: string with get, set

    type [<AllowNullLiteral>] Logging =
        abstract log: (string -> unit) with get, set
        abstract level: LoggingLevel with get, set

    type [<RequireQualifiedAccess>] LoggingLevel =
        | ERROR = 0
        | WARNING = 1
        | INFO = 2
        | VERBOSE = 3

    type [<AllowNullLiteral>] AuthenticationContext =
        abstract instance: string with get, set
        abstract config: Config with get, set
        /// Gets initial Idtoken for the app backend
        /// Saves the resulting Idtoken in localStorage.
        abstract login: unit -> unit
        /// Indicates whether login is in progress now or not.
        abstract loginInProgress: unit -> bool
        /// <summary>Gets token for the specified resource from local storage cache</summary>
        /// <param name="resource">A URI that identifies the resource for which the token is valid.</param>
        abstract getCachedToken: resource: string -> string
        /// Retrieves and parse idToken from localstorage
        abstract getCachedUser: unit -> User
        abstract registerCallback: expectedState: string * resource: string * callback: (string -> string -> obj option) -> unit
        /// <summary>Acquire token from cache if not expired and available. Acquires token from iframe if expired.</summary>
        /// <param name="resource">ResourceUri identifying the target resource</param>
        /// <param name="callback"></param>
        abstract acquireToken: resource: string * callback: (string -> string -> obj option) -> unit
        /// <summary>Redirect the Browser to Azure AD Authorization endpoint</summary>
        /// <param name="urlNavigate">The authorization request url</param>
        abstract promptUser: urlNavigate: string -> unit
        /// Clear cache items.
        abstract clearCache: unit -> unit
        /// Clear cache items for a resource.
        abstract clearCacheForResource: resource: string -> unit
        /// Logout user will redirect page to logout endpoint.
        /// After logout, it will redirect to post_logout page if provided.
        abstract logOut: unit -> unit
        /// <summary>Gets a user profile</summary>
        /// <param name="callback">- The callback that handles the response.</param>
        abstract getUser: callback: (string -> User -> obj option) -> unit
        /// <summary>Checks if hash contains access token or id token or error_description</summary>
        /// <param name="hash">-  Hash passed from redirect page</param>
        abstract isCallback: hash: string -> bool
        /// Gets login error
        abstract getLoginError: unit -> string
        /// Gets requestInfo from given hash.
        abstract getRequestInfo: hash: string -> RequestInfo
        /// Saves token from hash that is received from redirect.
        abstract saveTokenFromHash: requestInfo: RequestInfo -> unit
        /// <summary>Gets resource for given endpoint if mapping is provided with config.</summary>
        /// <param name="endpoint">-  API endpoint</param>
        abstract getResourceForEndpoint: endpoint: string -> string
        /// Handles redirection after login operation.
        /// Gets access token from url and saves token to the (local/session) storage
        /// or saves error in case unsuccessful login.
        abstract handleWindowCallback: unit -> unit
        abstract log: level: float * message: string * error: obj option -> unit
        abstract error: message: string * error: obj option -> unit
        abstract warn: message: string -> unit
        abstract info: message: string -> unit
        abstract verbose: message: string -> unit


    [<Emit("new AuthenticationContext($0)")>]
    let createAuthContext (config:Config) : AuthenticationContext = jsNative
