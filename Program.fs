namespace AvaloniaApp

open System.IO
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Input
open Avalonia.Media.Imaging
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Wasmtime
open System

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "AvaloniaApp"
        base.Width <- 1024.0
        base.Height <- 1024.0
        this.Content <- Counter.view

        //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
        //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

        
type App() =
    inherit Application()

//    member this.Test() =
//        printfn "Start Wasi"
//        use config = (new Config()).WithDebugInfo(true)
//        use engine = new Engine(config)
////        use ``module`` = Module.FromText(
////                engine,
////                "hello",
////                "(module (func $hello (import \"\" \"hello\")) (func (export \"run\") (call $hello)))"
////            )
//        use ``module`` = Module.FromFile(engine, "fable_raytracer_bg.wasm")
//        use linker = new Linker(engine)
//        use store = new Store(engine)
//        linker.DefineWasi()
//        
////        linker.Define(
////            "",
////            "hello",
////            Function.FromCallback(store, Action(fun _ -> Console.WriteLine("Hello from F# in WASM!")))
////        )
//        let instance = linker.Instantiate(store, ``module``)
//        let render_scene = instance.GetFunction(store, "render_scene")
//        let get_buffer_offset = instance.GetFunction(store, "get_buffer_offset")
//        let get_buffer_length = instance.GetFunction(store, "get_buffer_length")
//        if not render_scene.IsNull && not get_buffer_offset.IsNull && not get_buffer_length.IsNull then
//            render_scene.Invoke(store, 0,0,1024,1024,0.0) |> ignore
//            printfn "Render finished"
//            let res1 = get_buffer_offset.Invoke(store) :?> int
//            
////            let ptr_hex = String.Format("{0:X8}", ret_ptr.ToInt64())
//            printfn $"{res1}"
//            
//            let res2 = get_buffer_length.Invoke(store) :?> int
//            printfn $"{res2}"
//            let mem = instance.GetMemory(store,"memory")
//            let sp = mem.GetSpan(store)
//            printfn $"{sp.Length}"
//            let slice = sp.Slice(res1,res2)
//            let arr = slice.ToArray()
////            printfn $"%A{arr}"
//            
//            let bitmap = Bitmap.DecodeToHeight(MemoryStream(arr),1024)
//            ()
        
    override this.Initialize() =
        this.Styles.Add (FluentTheme(baseUri = null, Mode = FluentThemeMode.Light))
//        this.Test()

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)