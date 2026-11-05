namespace WebSharper.JQuery.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =

    // The page defines this plugin on the global jQuery instance (see
    // index.html). It is reachable through JQuery.Of only when the binding and
    // the plugin share the same instance -- which is the fix for issue #14.
    [<Inline "$j.highlight()">]
    let private highlight (j: JQuery) : JQuery =
        X<JQuery>

    [<Inline "typeof jQuery.fn.highlight === 'function'">]
    let private pluginAvailable () : bool =
        X<bool>

    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let status = Var.Create ""

        let box () = JQuery.Of "#box"

        IndexTemplate()
            .Highlight(fun _ ->
                highlight (box ()) |> ignore
                status.Value <- "called the page's .highlight() plugin through JQuery.Of")
            .FadeOut(fun _ ->
                box().FadeOut(600) |> ignore
                status.Value <- "called JQuery.Of(\"#box\").FadeOut() from the binding")
            .Reset(fun _ ->
                box().Css("background", "").Css("border-color", "") |> ignore
                box().Show() |> ignore
                status.Value <- "reset")
            .Status(status.View)
            .Bind()

        status.Value <-
            if pluginAvailable () then
                "ready - the global plugin is visible to the binding"
            else
                "the .highlight() plugin was not found"
