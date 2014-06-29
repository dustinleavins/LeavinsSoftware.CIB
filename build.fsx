#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO
open System
open System.Text.RegularExpressions
open Microsoft.Win32
open System.Diagnostics

RestorePackages()

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
  trace (proc.StandardOutput.ReadToEnd())
    
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
let versionNumber = (Seq.nth 1 (matchCast (pattern.Match (Seq.nth 0 matches)).Groups)).Value
let buildDir = "./Files/" + versionNumber

Target "Clean" (fun _ ->
    CleanDir buildDir
    File.Delete ("main.exe")
    File.Delete ("setup.exe")
)

Target "BuildProduction" (fun _ ->
    !! "LeavinsSoftware.Collection.Program/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "Build: "
)

Target "Default" (fun _ ->
    nsis ("/DProgramVersion=" + versionNumber + " Installer.nsi")
    
    !! "Bootstrapper.proj"
      |> MSBuildRelease "." "BuildBootstrapper"
      |> Log "Build: "
      
    nsis ("/DProgramVersion=" + versionNumber + " \"Wrapper Installer.nsi\"")
    
    CleanDir buildDir
    File.Delete ("main.exe")
    File.Delete ("setup.exe")
)

"Clean"
  ==> "BuildProduction"
  ==> "Default"

// start build
RunTargetOrDefault "Default"