using System;
using System.Diagnostics;
using System.IO;
using Serilog;

namespace Hydra.Deploy
{
    public static class Build
    {
        public const string MsBuildPath = "C:\\Program Files (x86)\\MSBuild\\12.0\\bin\\msbuild.exe";

        public static void CleanAndBuild( string project, ILogger log )
        {
            CleanSolution( project, log );
            BuildSolution( project, log );
        }

        private static void CleanSolution( string project, ILogger log )
        {
            if( !File.Exists( MsBuildPath ) )
            {
                throw new Exception( string.Format( "Invalid MsBuildPath {0}", MsBuildPath ) );
            }

            log.Information( "Cleaning Solution..." );
            var buildProcess = new Process();

            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.RedirectStandardOutput = true;
            buildProcess.StartInfo.FileName = MsBuildPath;
            buildProcess.StartInfo.Arguments = project + " /t:Clean /p:VisualStudioVersion=12.0";

            buildProcess.OutputDataReceived += ( sender, args ) => { log.Information( args.Data ); };
            buildProcess.ErrorDataReceived += ( sender, args ) => { log.Error( args.Data ); };

            buildProcess.Start();
            buildProcess.BeginOutputReadLine();
            buildProcess.WaitForExit();

            if( buildProcess.ExitCode != 0 )
            {
                var msg = "clean failed";
                log.Error( msg );
                throw new Exception( msg );
            }
        }

        private static void BuildSolution( string solution, ILogger log )
        {
            if( !File.Exists( MsBuildPath ) )
            {
                throw new Exception( string.Format( "Invalid MsBuildPath {0}", MsBuildPath ) );
            }

            log.Information( "Building Solution..." );
            var buildProcess = new Process();

            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.RedirectStandardOutput = true;
            buildProcess.StartInfo.FileName = MsBuildPath;
            buildProcess.StartInfo.Arguments = solution + " /t:Build ";

            buildProcess.OutputDataReceived += ( sender, args ) => { log.Information( args.Data ); };
            buildProcess.ErrorDataReceived += ( sender, args ) => { log.Error( args.Data ); };

            buildProcess.Start();
            buildProcess.BeginOutputReadLine();
            buildProcess.WaitForExit();

            if( buildProcess.ExitCode != 0 )
            {
                var msg = "build failed";
                log.Error( msg );
                throw new Exception( msg );
            }
        }
    }
}
