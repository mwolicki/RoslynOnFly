open CsProject
open Compile

[<EntryPoint>]
let main argv = 
    let compiledProject = getProject argv.[0] |> compileProject
    printfn "%A" compiledProject.Success
    0
