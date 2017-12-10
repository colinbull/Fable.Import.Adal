module Server.App

open System
open System.IO
open System.Collections.Generic
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.JwtBearer
open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.Razor.HttpHandlers
open Giraffe.Razor.Middleware
open Server.Models
open Giraffe.Tasks
// ---------------------------------
// Web app
// ---------------------------------

let accessDenied = setStatusCode 401 >=> text "Access Denied"

let itemHandler = 
    (fun (next:HttpFunc) (ctx:HttpContext) -> 
        task { 
            let claims =
                ctx.User.Claims 
                |> Seq.map (fun x -> x.Type + " : " + x.Value)
            return! json claims next ctx 
        }
    )

let webApp =
    choose [
        requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme) 
        >=>
            GET >=>
                choose [
                    route "/api/items" >=> itemHandler
                    route "/api/trades" >=> requiresRole "Admin" accessDenied >=> json ["MSTF,40,500"] 
                ]
            setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureApp (app : IApplicationBuilder) =
    
    app.UseCors(configureCors)
       .UseGiraffeErrorHandler(errorHandler)
       .UseAuthentication()
       .UseStaticFiles()
       .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    let sp  = services.BuildServiceProvider()
    let env = sp.GetService<IHostingEnvironment>()
    let viewsFolderPath = Path.Combine(env.ContentRootPath, "Views")
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
    |> fun x -> x.AddJwtBearer(fun o -> 
        o.Audience <- "<Audience>"
        o.Authority <- "<Authority>"
        o.Events <- new JwtBearerEvents(OnAuthenticationFailed = (fun x -> Threading.Tasks.Task.Factory.StartNew(fun () -> printfn "%A" x.Exception.Message)))
    )
    |> ignore
    services.AddRazorEngine viewsFolderPath |> ignore
    services.AddCors() |> ignore
    

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Trace
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main argv =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0