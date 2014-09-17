module CsProject
open FSharp.Data
open System.IO

type ProjectXml = XmlProvider<"projectExample2.xml">
type FilePath = string

type Reference =
    | RefFile of FilePath
    | RefProject of FilePath

type ProjectFile = 
    | CsFile of FilePath
    | EmbeddedRes of FilePath

type Project = {
        Name: string;
        References: seq<Reference>;
        Files: seq<ProjectFile>;
        OutputFile: FilePath
    }

let getProject (filePath: FilePath) =
    let getPath filePath:FilePath = Path.GetFullPath filePath
    let getPath2 filePath filename = getPath (filePath + "/../" + filename)
    let proj = ProjectXml.Load filePath

    let refs = proj.ItemGroups |> Seq.collect (fun p->seq{
        for reference in p.ProjectReferences -> RefProject (getPath2 filePath reference.Include)
        for reference in p.References -> RefFile reference.Include
    })
    
    let files = proj.ItemGroups |> Seq.collect (fun p->seq{
        for file in p.Compiles -> CsFile(getPath2 filePath file.Include)
        for file in p.EmbeddedResources -> EmbeddedRes(getPath2 filePath file.Include)
    })

    let assemblyName=
        match (proj.PropertyGroups |> Seq.map (fun x->x.AssemblyName) |> Seq.tryFind (fun x-> x.IsSome)) with
        | Some n when n.IsSome -> n.Value
        | _ -> Path.GetFileName filePath

    let getExt (outputType) = 
        match outputType with
        | Some (Some "WinExe") -> ".exe"
        | _ -> ".dll"

    let getOutputFile outputType = assemblyName + getExt outputType

    let outputFile= proj.PropertyGroups |> Seq.map (fun x->x.OutputType) |> Seq.tryFind (fun x-> x.IsSome) |> getOutputFile
    

    { Name = assemblyName; References = refs; Files = files; OutputFile = outputFile }

