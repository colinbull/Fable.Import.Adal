module ADALExample

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser
open Fable.PowerPack

module P = Fable.PowerPack.Promise

let tokenUri = "<API TOken URi - typically APP uri>"
let authContext = 
    let config = createEmpty<adal.Adal.Config>
    config.clientId <- "<Client ID>"
    config.tenant <- Some "<Tenant ID>"
    config.postLogoutRedirectUri <- Some window.location.origin
    config.cacheLocation <- Some "localstorage"
    adal.Adal.createAuthContext(config)



let withToken f =  
    let token = authContext.getCachedToken(tokenUri)
    if token = null 
    then 
        authContext.acquireToken(tokenUri, fun _ token -> f token; None)
    else f token

let getData endpoint f token = 
    promise {
        let url = sprintf "http://localhost:5000/api/%s" endpoint 
        let props = [
            Fetch.requestHeaders [
                   Fetch.Fetch_types.Authorization ("Bearer " + token) 
              ]
        ]
        let! data = Fetch.fetchAs<string[]> url props
        return f data
    } |> P.start
     

let init() =
    let loginBut = document.getElementById("login") :?> HTMLButtonElement
    let logoutBut = document.getElementById("logout") :?> HTMLButtonElement
    let getDataBut = document.getElementById("getdata") :?> HTMLButtonElement
    let getTradeBut = document.getElementById("gettrades") :?> HTMLButtonElement
    let status = document.getElementById("status") :?> HTMLDivElement
    let data = document.getElementById("data") :?> HTMLDivElement
    let trade = document.getElementById("trades") :?> HTMLDivElement

    authContext.handleWindowCallback()
    if(authContext.isCallback(window.location.hash) && (authContext.getLoginError() <> null))
    then ()
    else status.innerText <- ("Error: " + authContext.getLoginError())

    let user = authContext.getCachedUser()
    if(user <> null)
    then 
        withToken(fun token -> 
            status.innerText <- user.userName + " : " + token
            getDataBut.disabled <- false
            getTradeBut.disabled <- false 
        )
    else 
        getDataBut.disabled <- true
        getTradeBut.disabled <- true
        status.innerText <- "No user, no cry!!!"


    loginBut.addEventListener_click(fun _ -> printf "%s" "login"; authContext.login(); box())
    logoutBut.addEventListener_click(fun _ -> getDataBut.disabled <- true;authContext.logOut(); box())
    getDataBut.addEventListener_click (fun _ -> withToken <| getData "items" (fun d -> data.innerHTML <- ("<pre>" + (System.String.Join(System.Environment.NewLine, d)) + "</pre>")); box())
    getTradeBut.addEventListener_click (fun _ -> withToken <| getData "trades" (fun d -> data.innerHTML <- ("<pre>" + (System.String.Join(System.Environment.NewLine, d)) + "</pre>")); box())

init()