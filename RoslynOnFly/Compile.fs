module Compile
open CsProject
open System.IO
open Microsoft.CodeAnalysis.CSharp

let getSyntaxTree (file : FilePath) =
    File.ReadAllText file |>  CSharpSyntaxTree.ParseText

let compileProject (project : Project) =
    let sourceCode = 
        project.Files |> Seq.collect (fun file ->
            [
                match file with
                | CsFile file -> yield getSyntaxTree file
                | _ -> ()
            ])
    let cmp = CSharpCompilation.Create (project.Name, sourceCode)
    cmp.Emit project.OutputFile
