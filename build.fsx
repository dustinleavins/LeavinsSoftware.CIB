#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO
open System
open System.Text.RegularExpressions
open Microsoft.Win32
open System.Diagnostics

// Default target
Target "Default" (fun _ ->
    let nsis = (fun args ->
        // Lookup registry
        let cmd = ((Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\NSIS", "", null)).ToString()) + "\\makensis.exe"
        
        let pInfo = new ProcessStartInfo(cmd, args);
        pInfo.UseShellExecute <- false
        pInfo.RedirectStandardError <- true
        pInfo.RedirectStandardOutput <- true
        trace ((Process.Start (pInfo)).StandardOutput.ReadToEnd())
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
    
    CleanDir buildDir
    
    !! "LeavinsSoftware.Collection.Program/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
      
    nsis ("/DProgramVersion=" + versionNumber + " Installer.nsi")
    
    !! "Bootstrapper.proj"
      |> MSBuildRelease "." "BuildBootstrapper"
      |> Log "AppBuild-Output: "
      
    nsis ("/DProgramVersion=" + versionNumber + " \"Wrapper Installer.nsi\"")
    
    CleanDir buildDir
    File.Delete ("main.exe")
    File.Delete ("setup.exe")
)

// start build
RunTargetOrDefault "Default"