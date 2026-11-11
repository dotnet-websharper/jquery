namespace WebSharper.JQuery.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =

    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // Shared, observable sample state.
    let status = Var.Create "Ready — jQuery is bundled from npm by the binding."
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"
    let name = Var.Create ""
    let clickCount = Var.Create 0
    let hoverCount = Var.Create 0
    let dataResult = Var.Create "No data loaded yet."
    let utilityResult = Var.Create ""
    let deferredResult = Var.Create ""

    let greeting =
        name.View.Map(fun n ->
            if n = "" then "Type your name to see it echoed back through jQuery."
            else "Hello, " + n + "!")

    // A plain jQuery plugin, registered on the imported instance's prototype.
    // The plugin body closes over the box selector so it does not depend on the
    // JavaScript `this` (which the WebSharper inline compiler would rebind).
    [<Inline "$0.extend({ highlight: function () { return $1().css('background', 'gold').css('border-color', '#d4a017'); } })">]
    let private registerHighlight (fn: obj) (box: unit -> JQuery) : obj =
        X<obj>

    [<Inline "$0.highlight()">]
    let private callHighlight (jq: JQuery) : JQuery =
        X<JQuery>

    [<Inline "typeof $0.highlight === 'function'">]
    let private pluginAvailable (fn: obj) : bool =
        X<bool>

    let theBox () = JQuery.Of "#box"

    let installPlugin () =
        registerHighlight JQuery.Fn theBox |> ignore

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try
            JS.Window.LocalStorage.SetItem("ws-jquery-theme", theme.Value)
        with _ ->
            ()
        applyTheme ()

    let highlight () =
        callHighlight (theBox()) |> ignore
        status.Value <- "Registered .highlight() on jQuery.fn and called it through JQuery.Of(\"#box\")."

    let fadeOut () =
        theBox().FadeOut(600, fun () ->
            status.Value <- "JQuery.Of(\"#box\").FadeOut(600) finished.") |> ignore

    let reset () =
        theBox()
            .Css("background", "")
            .Css("border-color", "")
            .Show()
        |> ignore
        status.Value <- "Reset the box."

    let increment () =
        clickCount.Value <- clickCount.Value + 1
        JQuery("#counter")
            .Text((string clickCount.Value))
            .Attr("data-count", (string clickCount.Value))
            .Data("clicks", box clickCount.Value)
        |> ignore

    let loadData () =
        JQuery.Ajax(
            AjaxSettings(
                Url = "data.json",
                DataType = DataType.Json,
                Type = RequestType.GET,
                Success = fun data _ _ ->
                    let items = As<string[]> (data?items)
                    dataResult.Value <-
                        "Loaded " + string items.Length + " items: " + String.concat ", " items
            )
        )
        |> ignore

    let runUtilities () =
        let numbers : obj[] = [| box 1; box 2; box 3; box 4; box 5; box 6 |]
        let odds = JQuery.Grep(numbers, (fun v _ -> As<int> v % 2 = 1), false)
        let index = JQuery.InArray(box 4, numbers)
        utilityResult.Value <-
            "JQuery.Grep kept " + String.concat ", " (Array.map string odds)
            + "; JQuery.InArray(4) = " + string index

    let runDeferred () =
        let d = JQuery.Deferred()
        d.Done(fun () -> deferredResult.Value <- "Deferred resolved via .Done().") |> ignore
        d.Resolve(box 42) |> ignore
        deferredResult.Value <- deferredResult.Value + " State=" + As<string> (d.State())

    [<SPAEntryPoint>]
    let Main () =
        // Restore the stored theme before the first render.
        try
            let stored = JS.Window.LocalStorage.GetItem("ws-jquery-theme")
            if As<string> stored ===. "dark" then theme.Value <- "dark"
        with _ ->
            ()
        applyTheme ()

        // Register the plugin on the imported jQuery instance, then prove it is
        // reachable through the binding.
        installPlugin()

        let boxEl = theBox()

        boxEl
            .Mouseenter(fun _ _ -> hoverCount.Value <- hoverCount.Value + 1)
            .Mouseleave(fun _ _ -> ())
        |> ignore

        IndexTemplate()
            .Highlight(fun _ -> highlight ())
            .FadeOut(fun _ -> fadeOut ())
            .Reset(fun _ -> reset ())
            .Increment(fun _ -> increment ())
            .LoadData(fun _ -> loadData ())
            .RunUtilities(fun _ -> runUtilities ())
            .RunDeferred(fun _ -> runDeferred ())
            .ToggleTheme(fun _ -> toggleTheme ())
            .Status(status.View)
            .ThemeLabel(themeLabel.View)
            .Name(name)
            .Greeting(greeting)
            .ClickCount(clickCount.View.Map string)
            .HoverCount(hoverCount.View.Map string)
            .DataResult(dataResult.View)
            .UtilityResult(utilityResult.View)
            .DeferredResult(deferredResult.View)
            .Bind()

        status.Value <-
            if pluginAvailable JQuery.Fn then
                "Ready — the .highlight() plugin is registered on the npm jQuery instance."
            else
                "The .highlight() plugin was not registered."
