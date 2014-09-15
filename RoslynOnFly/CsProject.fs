module CsProject
open FSharp.Data

type ProjectXml = XmlProvider<"projectExample2.xml">
type Path = string

type Reference =
    | RefFile of Path
    | RefProject of Path

type ProjectFile = 
    | CsFile of Path
    | EmbeddedRes of Path

type Project = {name: string; references: seq<Reference>; files: seq<ProjectFile>}


let getProject (filePath: Path) =
    let proj = ProjectXml.Load filePath

    let refs = proj.ItemGroups |> Seq.collect (fun p->seq{
        for reference in p.ProjectReferences -> RefProject reference.Include
        for reference in p.References -> RefFile reference.Include
    })
    
    let files = proj.ItemGroups |> Seq.collect (fun p->seq{
        for file in p.Compiles -> CsFile(file.Include)
        for file in p.EmbeddedResources -> EmbeddedRes(file.Include)
    })

    let assemblyName=
        match (proj.PropertyGroups |> Seq.find (fun x-> x.AssemblyName.IsSome)).AssemblyName with
        | Some n -> n
        | None -> filePath
    {name = assemblyName; references = refs; files = files}