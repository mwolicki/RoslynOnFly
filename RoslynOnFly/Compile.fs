module Compile
open CsProject
open System.IO
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp

//Temporary evil...
let dotNetPath = """C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\"""

let getSyntaxTree (file : FilePath) = File.ReadAllText file |>  CSharpSyntaxTree.ParseText

let compileProject (project : Project) =
    let sourceCode = 
        project.Files |> Seq.collect (fun file ->
            [
                match file with
                | CsFile file -> yield getSyntaxTree file
                | _ -> ()
            ])
    
    let references = 
        (project.References |> Seq.collect (fun reference ->
            [
                match reference with
                | RefFile file -> yield MetadataFileReference(dotNetPath + file + ".dll") :> MetadataReference
                | _ -> ()
            ])) |> Seq.toList
    let references = [(MetadataFileReference(dotNetPath + "mscorlib.dll") :> MetadataReference)] @ references

    let cmp = CSharpCompilation.Create (project.Name, sourceCode, references, CSharpCompilationOptions(OutputKind.NetModule))
    cmp.Emit project.OutputFile
