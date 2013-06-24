# Copyright (c) 2013 Dustin Leavins
# See the file 'LICENSE.txt' for copying permission.

require 'fileutils'
require 'win32/registry'

def version_number
    version_number = nil
    File.open('SolutionInfo.cs', 'r') do |f|
        f.each do |line|
            if /AssemblyVersion\("(\d+(?:\.\d+){1,3})"\)/ =~ line
                version_number = $1 # matching text
                break
            end
        end
    end

    return version_number
end

def ms_build(*args)
    path = "#{ENV['SystemRoot']}\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe"
    system path, *args

end

def nsis(*args)
    nsis_path = ''

    Win32::Registry::HKEY_LOCAL_MACHINE.open('SOFTWARE\NSIS') do |reg|
        nsis_path = reg['']
    end

    system "#{nsis_path}\\makensis.exe", *args
end

def xcopy(*args)
    system 'xcopy', *args
end

task :default => [:nothing]

task :nothing do
end

task :build_installer do
    version = version_number()
    fail if version.nil?
    if !ms_build 'LeavinsSoftware.Collection.sln', '/p:Configuration=Release' 
        fail
    end

    xcopy '/E', 'LeavinsSoftware.Collection.Program\bin\x86\Release', "Files\\#{version}\\" 

    nsis "/DProgramVersion=#{version}", 'Installer.nsi' 

    unless File.exists? 'main.exe'
        fail
    end

    ms_build 'Bootstrapper.proj' 

    unless File.exists? 'setup.exe'
        fail
    end

    nsis "/DProgramVersion=#{version}", 'Wrapper Installer.nsi' 

    unless File.exists? "CIB #{version}.exe"
        fail
    end


    # Removing temporary files
    FileUtils.rm_rf ["Files\\#{version}\\", 'main.exe', 'setup.exe']
end

