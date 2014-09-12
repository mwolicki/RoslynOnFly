namespace RoslynOnFly

module CsProject =

    open FSharp.Data

    type ProjectXml = XmlProvider<"projectExample2.xml">


    type Reference =
        | RefFile of path: string
        | RefProject of path: string

    type ProjectFile = 
        | CsFile of path: string
        | EmbeddedRes of path: string

    type Project = {name: string; references: seq<Reference>; files: seq<ProjectFile>}


    let getProject (filePath: string) =
        let proj = (ProjectXml.Load filePath)

        let refs = proj.ItemGroups |> Seq.collect (fun p->seq{
            for i in p.ProjectReferences -> RefProject(i.Include)
            for i in p.References -> RefFile(i.Include)
        })
    
        let files = proj.ItemGroups |> Seq.collect (fun p->seq{
            for i in p.Compiles -> CsFile(i.Include)
            for i in p.EmbeddedResources -> EmbeddedRes(i.Include)
        })

        let n=(proj.PropertyGroups |> Seq.find (fun x-> x.AssemblyName.IsSome )).AssemblyName.Value
        {name = n; references = refs; files = files}