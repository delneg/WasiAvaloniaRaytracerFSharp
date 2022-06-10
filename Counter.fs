namespace AvaloniaApp

open System.IO
open System.Runtime.InteropServices
open Avalonia
open Avalonia.Platform
open SkiaSharp

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.Media.Imaging
    open Wasmtime
    open System

    
    let getBitmapFromWasi () =
        use config = (new Config()).WithDebugInfo(true)
        use engine = new Engine(config)
        use ``module`` = Module.FromFile(engine, "fable_raytracer_bg.wasm")
        use linker = new Linker(engine)
        use store = new Store(engine)
        linker.DefineWasi()
        let instance = linker.Instantiate(store, ``module``)
        let render_scene = instance.GetFunction(store, "render_scene")
        let get_buffer_offset = instance.GetFunction(store, "get_buffer_offset")
        let get_buffer_length = instance.GetFunction(store, "get_buffer_length")
        if not render_scene.IsNull && not get_buffer_offset.IsNull && not get_buffer_length.IsNull then
            render_scene.Invoke(store, 0,0,1024,1024,0.0) |> ignore
            printfn "Render finished"
            let res1 = get_buffer_offset.Invoke(store) :?> int
            
//            let ptr_hex = String.Format("{0:X8}", ret_ptr.ToInt64())
            printfn $"{res1}"
            
            let res2 = get_buffer_length.Invoke(store) :?> int
            printfn $"{res2}"
            let mem = instance.GetMemory(store,"memory")
            let sp = mem.GetSpan(store)
            printfn $"{sp.Length}"
            let slice = sp.Slice(res1,res2)
            let arr = slice.ToArray()
            
            
            use stream = new MemoryStream(slice.ToArray())
//            printfn $"%A{stream.}"
            
            let bitmap = new WriteableBitmap(PixelSize(1024,1024), Vector(72,72),PixelFormat.Rgba8888,AlphaFormat.Unpremul)
            use lock = bitmap.Lock()
            Marshal.Copy(arr,0,IntPtr(lock.Address.ToInt64()),arr.Length)

            Some <| bitmap
         else
            None
    let view =
        Component(fun ctx ->
            let state = ctx.useState 0
            let bitmap = getBitmapFromWasi()
            printfn $"bitmap {bitmap}"
            DockPanel.create [
                DockPanel.verticalAlignment VerticalAlignment.Stretch
                DockPanel.horizontalAlignment HorizontalAlignment.Stretch
                DockPanel.children [
                    Image.create [
                        Image.source bitmap.Value
                        Image.height 1024.0
                    ]
//                    Button.create [
//                        Button.width 64
//                        Button.horizontalAlignment HorizontalAlignment.Center
//                        Button.horizontalContentAlignment HorizontalAlignment.Center
//                        Button.content "Reset"
//                        Button.onClick (fun _ -> state.Set 0)
//                        Button.dock Dock.Bottom
//                    ]
//                    Button.create [
//                        Button.width 64
//                        Button.horizontalAlignment HorizontalAlignment.Center
//                        Button.horizontalContentAlignment HorizontalAlignment.Center
//                        Button.content "-"
//                        Button.onClick (fun _ -> state.Current - 1 |> state.Set)
//                        Button.dock Dock.Bottom
//                    ]
//                    Button.create [
//                        Button.width 64
//                        Button.horizontalAlignment HorizontalAlignment.Center
//                        Button.horizontalContentAlignment HorizontalAlignment.Center
//                        Button.content "+"
//                        Button.onClick (fun _ -> state.Current + 1 |> state.Set)
//                        Button.dock Dock.Bottom
//                    ]
//                    TextBlock.create [
//                        TextBlock.dock Dock.Top
//                        TextBlock.fontSize 48.0
//                        TextBlock.horizontalAlignment HorizontalAlignment.Center
//                        TextBlock.text (string state.Current)
//                    ]
                ]
            ]
            Image.create [
                Image.source bitmap.Value
                Image.height 1024.0
            ]
        )
