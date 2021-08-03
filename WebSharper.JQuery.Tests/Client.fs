namespace WebSharper.JQuery.Tests

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =

    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let newName = Var.Create ""

        IndexTemplate.Main()
            .FadeOutDiv(
                p [attr.id "fadeP"] [text "If you click on this paragraph you'll see it just fade away."]
            )
            .HoverOnMe(
                p [attr.id "hoverOnMe"] [text "The secret ingredient is: "]
            )
            .SelectMe(
                Doc.Concat [
                    input [
                        attr.id "selectMe"
                        attr.value "Select Me!"
                    ] []
                    div [attr.id "showSelection"] []
                ]
            )
            .Doc()
        |> Doc.RunById "main"

        let animSettings = New [
            "width" => "70%"
            "opacity" => 0.4
            "marginLeft" => "0.6in"
            "fontSize" => "3em"
            "borderWidth" => "10px"
        ]

        JQuery("#focusin1").Focusin(fun _ _ -> JQuery("#fadeoutspan1").Css("display", "inline").FadeOut(1000) |> ignore) |> ignore
        JQuery("#focusin2").Focusin(fun _ _ -> JQuery("#fadeoutspan2").Css("display", "inline").FadeOut(1000) |> ignore) |> ignore
        JQuery("#fadeP").Click(fun e _ -> JQuery(e).FadeOut("slow") |> ignore) |> ignore
        JQuery("#clickIMG").Click(fun _ _ -> JQuery("#bird").Hide("slow", (fun () -> JS.Window.Alert("Animation complete"))) |> ignore) |> ignore
        JQuery("#go").Click(fun _ _ -> JQuery("#animblock").Animate(animSettings, 1500) |> ignore) |> ignore
        JQuery("#findme").Children().Css("background-color", "crimson") |> ignore
        JQuery("#hoverOnMe").Hover((fun e _ -> JQuery(e).Append("<span id='hovertext'>WebSharper :)</span>") |> ignore), (fun _ _ -> JQuery("#hovertext").Remove() |> ignore)) |> ignore
        JQuery("#selectMe").Select(fun _ _ -> JQuery("#showSelection").Text("I was selected...").Show().FadeOut(1000) |> ignore)
        // Select(fun _ _ -> JQuery("#showSelection").Text("I was selected...").Show() |> ignore)