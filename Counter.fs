﻿namespace AvaloniaApp

open System.IO
open System.Runtime.InteropServices
open Avalonia
open Avalonia.Platform

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.Media.Imaging
    open Wasmtime
    open System

    
    let getBitmapFromWasi w h =
        use engine = new Engine()
        use ``module`` = Module.FromFile(engine, "fable_raytracer_bg.wasm")
        use linker = new Linker(engine)
        use store = new Store(engine)
        linker.DefineWasi()
        let instance = linker.Instantiate(store, ``module``)
        let render_scene = instance.GetFunction(store, "render_scene")
        let get_buffer_offset = instance.GetFunction(store, "get_buffer_offset")
        let get_buffer_length = instance.GetFunction(store, "get_buffer_length")
        if not render_scene.IsNull && not get_buffer_offset.IsNull && not get_buffer_length.IsNull then
            let sw = System.Diagnostics.Stopwatch.StartNew()
            render_scene.Invoke(store, 0,0,w,h,0.0) |> ignore
            sw.Stop()
            let elapsed = sw.Elapsed.TotalMilliseconds
            Console.WriteLine("Ray tracing done:\n - rendered image size: ({0}x{1})\n - elapsed: {2} ms", w, h, elapsed)
            let offset = get_buffer_offset.Invoke(store) :?> int
            let len = get_buffer_length.Invoke(store) :?> int
            let mem = instance.GetMemory(store,"memory")
            let data = mem.GetSpan(store).Slice(offset,len).ToArray()
            let bitmap = new WriteableBitmap(PixelSize(w,h), Vector(72,72),PixelFormat.Rgba8888,AlphaFormat.Unpremul)
            use lock = bitmap.Lock()
            Marshal.Copy(data,0,IntPtr(lock.Address.ToInt64()),data.Length)
            ValueSome <| bitmap
         else
            ValueNone
    let view w h=
        Component(fun _ ->
            match getBitmapFromWasi w h with
            | ValueSome bitmap ->
                Image.create [
                    Image.source bitmap
                    Image.height h
                    Image.width w
                ]
            | ValueNone ->
                TextBlock.create [
                    TextBlock.text "Required functions not found in wasm"
                ]
        )
