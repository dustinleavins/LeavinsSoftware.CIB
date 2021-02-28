#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

// TODO - https://fake.build/fake-gettingstarted.html
open System.IO
open System
open System.Text.RegularExpressions
open Microsoft.Win32
open System.Diagnostics
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.Core.TargetOperators

exception NsisError of string

let nsis = (fun args ->
  // Lookup registry
  let registry = Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\NSIS", "", null);
        
  if registry = null then raise (NsisError("NSIS is not installed")) 
        
  let cmd = ((registry).ToString()) + "\\makensis.exe";
        
  let pInfo = new ProcessStartInfo(cmd, args);
  pInfo.UseShellExecute <- false
  pInfo.RedirectStandardError <- true
  pInfo.RedirectStandardOutput <- true
        
  let proc = Process.Start (pInfo)
  Trace.log(proc.StandardOutput.ReadToEnd())
    
  if proc.ExitCode <> 0 then raise (NsisError("Did not build installer"))
)

let pattern = new Regex ("""AssemblyVersion\("(\d+(?:\.\d+){1,3})"\)""")
let readLines (filePath:string) = seq {
    use sr = new StreamReader(filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine ()
}
    
let hasVersionInfo (line:string) =
    pattern.IsMatch(line)
    
let matches = Seq.filter hasVersionInfo (readLines ("SolutionInfo.cs"))
let matchCast (captures) : seq<Capture> = Seq.cast captures
let versionNumber = (Seq.item 1 (matchCast (pattern.Match (Seq.item 0 matches)).Groups)).Value
let buildDir = "./Files/" + versionNumber


Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
    File.Delete ("main.exe")
    File.Delete ("setup.exe")
)

Target.create "BuildProduction" (fun _ ->
    !! "LeavinsSoftware.Collection.Program/*.csproj"
      |> MSBuild.runRelease id buildDir "Build"
      |> Trace.logItems "Build: "
)

Target.create "Default" (fun _ ->
    nsis ("/DProgramVersion=" + versionNumber + " Installer.nsi")
    
    !! "Bootstrapper.proj"
      |> MSBuild.runRelease id "." "BuildBootstrapper"
      |> Trace.logItems "Build: "
      
    nsis ("/DProgramVersion=" + versionNumber + " \"Wrapper Installer.nsi\"")
    
    Shell.cleanDir buildDir
    File.Delete ("main.exe")
    File.Delete ("setup.exe")
)

"Clean"
  ==> "BuildProduction"
  ==> "Default"

// start build
Target.runOrDefault "Default"