namespace WebSharper.JQuery.RouterExtensions

open WebSharper
open WebSharper.JQuery
open WebSharper.JavaScript
open WebSharper.Sitelets

module RouterExtensions =

    type Router with
        static member AjaxWith (settings: AjaxSettings) (router: Router<'A>) endpoint =
            let settings = if As settings then settings else AjaxSettings()
            match Router.Write router endpoint with
            | Some path ->
                if settings.DataType ===. JS.Undefined then
                    settings.DataType <- DataType.Text
                settings.Type <-
                    match path.Method with
                    | Some m -> As m
                    | None -> RequestType.POST
                match path.Body.Value with
                | null ->
                    if not (Map.isEmpty path.FormData) then
                        let fd = FormData()
                        path.FormData |> Map.iter (fun k v -> fd.Append(k, v))
                        settings.ContentType <- Union1Of2 false
                        settings.Data <- fd
                        settings.ProcessData <- false
                | b ->
                    settings.ContentType <- Union2Of2 "application/json"
                    settings.Data <- b
                    settings.ProcessData <- false
                Async.FromContinuations (fun (ok, err, cc) ->
                    settings.Success <- fun res _ _ -> ok (As<string> res) 
                    settings.Error <- fun _ _ msg -> err (exn msg)
                    // todo: cancellation
                    let url = path.ToLink()
                    settings.Url <-
                        if As settings.Url then
                            settings.Url.TrimEnd('/') + url
                        else
                            url
                    JQuery.Ajax(settings) |> ignore
                )
            | _ -> 
                failwith "Failed to map endpoint to request" 

        static member Ajax router endpoint =
            Router.AjaxWith (AjaxSettings()) router endpoint