module CsProject
open FSharp.Data

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
    { Name = assemblyName; References = refs; Files = files; OutputFile = assemblyName }