module Compile
open CsProject
open System.IO
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp

//Temporary evil...
let dotNetPath = """C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\"""

let getSyntaxTree (file : FilePath) = File.ReadAllText file |>  CSharpSyntaxTree.ParseText

let compileProject (project : Project) =
    let sourceCode = project.Files |> Seq.choose (function CsFile file -> Some(getSyntaxTree file) | _ -> None)
    let references = project.References |> Seq.choose (function RefFile file -> Some(MetadataFileReference(dotNetPath + file + ".dll") :> MetadataReference) | _ -> None) |> Seq.toList
    let references = [(MetadataFileReference(dotNetPath + "mscorlib.dll") :> MetadataReference)] @ references
    let getOutputKind = function Dll -> OutputKind.NetModule | WinExe -> OutputKind.WindowsApplication

    let cmp = CSharpCompilation.Create (project.Name, sourceCode, references, CSharpCompilationOptions(getOutputKind project.OutputType))
    cmp.Emit project.OutputFile
