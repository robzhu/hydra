using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hydra.Deploy;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using RzAspects;
using RzWpf;
using Serilog;

namespace Hydra.CommandCenter
{
    public class DeploymentPageViewModel : ViewModelBase
    {
        const string PackageDirectory = "D:\\hydra_packages\\";

        public HostCollectionViewModel HostCollection { get; private set; }
        public ObservableCollectionEx<ProjectViewModel> Projects { get; private set; }
        public ObservableCollectionEx<PackageViewModel> Packages { get; private set; }

        public DelegatedCommand AddProjectCommand { get; private set; }
        public DelegatedCommand RemoveProjectCommand { get; private set; }
        public DelegatedCommand PackageProjectCommand { get; private set; }

        public DelegatedCommand DeletePackageCommand { get; private set; }
        public DelegatedCommand DeployPackageCommand { get; private set; }

        public DelegatedCommand DeployAndRunCommand { get; private set; }

        public string PropertySelectedProject { get { return "SelectedProject"; } }
        private ProjectViewModel _SelectedProject;
        public ProjectViewModel SelectedProject
        {
            get{ return _SelectedProject; }
            set{ SetProperty( PropertySelectedProject, ref _SelectedProject, value ); }
        }

        public string PropertySelectedPackage { get { return "SelectedPackage"; } }
        private PackageViewModel _SelectedPackage;
        public PackageViewModel SelectedPackage
        {
            get { return _SelectedPackage; }
            set { SetProperty( PropertySelectedPackage, ref _SelectedPackage, value ); }
        }

        public string PropertyExecutingDeployment { get { return "ExecutingDeployment"; } }
        private bool _ExecutingDeployment = false;
        public bool ExecutingDeployment
        {
            get { return _ExecutingDeployment; }
            set { SetProperty( PropertyExecutingDeployment, ref _ExecutingDeployment, value ); }
        }

        private ILogger Log { get { return App.Log; } }
       
        public DeploymentPageViewModel( HostCollectionViewModel hostCollection )
        {
            HostCollection = hostCollection;
            Projects = new ObservableCollectionEx<ProjectViewModel>();
            Packages = new ObservableCollectionEx<PackageViewModel>();

            Projects.ItemAdded += Projects_ItemAdded;
            Projects.ItemRemoved += Projects_ItemRemoved;
            Packages.ItemAdded += Packages_ItemAdded;
            Packages.ItemRemoved += Packages_ItemRemoved;

            AddProjectCommand = new DelegatedCommand( OnExecuteAddProject );
            RemoveProjectCommand = new DelegatedCommand(
                () => { Projects.Remove( SelectedProject ); },
                () => { return SelectedProject != null; } );

            PackageProjectCommand = new DelegatedCommand( 
                OnExecutePackageProject,
                () => { return SelectedProject != null; } );

            DeletePackageCommand = new DelegatedCommand(
                OnExecuteDeletePackage,
                () => { return SelectedPackage != null; } );

            DeployPackageCommand = new DelegatedCommand( OnExecuteDeployPackage, CanExecuteDeploy );
            DeployAndRunCommand = new DelegatedCommand( OnExecuteDeployAndRun, CanExecuteDeploy );
        }

        private void Projects_ItemAdded( ProjectViewModel newProject )
        {
            SelectedProject = newProject;
        }

        private void Projects_ItemRemoved( ProjectViewModel obj )
        {
            if( Projects.Count >= 1 ) SelectedProject = Projects[ 0 ];
        }

        private void Packages_ItemAdded( PackageViewModel newPackage )
        {
            SelectedPackage = newPackage;
        }

        private void Packages_ItemRemoved( PackageViewModel obj )
        {
            if( Packages.Count >= 1 ) SelectedPackage = Packages[ 0 ];
        }

        private bool CanExecuteDeploy()
        {
            return ( !ExecutingDeployment &&
                     ( SelectedPackage != null ) && 
                     ( HostCollection.SelectedHost != null ) );
        }

        private void OnExecutePackageProject()
        {
            Task.Run( () => 
            {
                var projectPath = SelectedProject.ProjectFilePath;

                Log.Information( "cleaning and building: {0}", projectPath );
                Build.CleanAndBuild( projectPath, Log );

                Log.Information( "creating package for: {0}", projectPath );
                var package = Package.Create( projectPath, Log );

                var packageFileInfo = new FileInfo( package );
                var destinationFileName = PackageDirectory + packageFileInfo.Name;
                if( File.Exists( destinationFileName ) )
                {
                    File.Delete( destinationFileName );
                    var matchingPackage = Packages.First( p => p.Location == destinationFileName );
                    if( matchingPackage != null )
                    {
                        UIThread.Marshall( () =>
                        {
                            Packages.Remove( matchingPackage );
                        } );
                    }
                }
                File.Copy( package, destinationFileName );

                UIThread.Marshall( () =>
                {
                    Packages.Add( new PackageViewModel()
                    {
                        Location = destinationFileName,
                    } );
                } );
            } );
        }

        private void OnExecuteDeletePackage()
        {
            try
            {
                File.Delete( SelectedPackage.Location );
            }
            catch { }
            Packages.Remove( SelectedPackage );
        }

        private async Task<Deployment> DeployAsync( string package, string targetHost )
        {
            var deployment = new Deployment( package, targetHost, Log );
            await deployment.DeployAsync();
            return deployment;
        }

        private void OnExecuteDeployPackage()
        {
            var package = SelectedPackage.Location;
            var targetHost = HostCollection.SelectedHost.Model.Name;
            Task.Run( () =>
            {
                try
                {
                    DeployAsync( package, targetHost ).Wait();
                }
                catch( Exception ex )
                {
                    Navigation.ShowMessageAsync( "deployment failed", ex.Message ).Wait();
                }
            } );
        }

        private void OnExecuteDeployAndRun()
        {
            var package = SelectedPackage.Location;
            var targetHost = HostCollection.SelectedHost.Model.Name;
            Task.Run( () =>
            {
                try
                {
                    var deployment = DeployAsync( package, targetHost ).Result;
                    deployment.LaunchAppAsync( null ).Wait();
                }
                catch( Exception ex )
                {
                    Navigation.ShowMessageAsync( "deployment failed", ex.Message ).Wait();
                }
            } );
        }

        private void OnExecuteAddProject()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();

            if( result == CommonFileDialogResult.Ok )
            {
                var file = dialog.FileName;

                try
                {
                    Projects.Add( ProjectViewModel.Create( file ) );
                    if( Projects.Count == 1 )
                    {
                        SelectedProject = Projects[ 0 ];
                    }
                }
                catch( ArgumentException ex )
                {
                    Navigation.ShowMessageAsync( "error:", ex.Message );
                }
            }
        }
    }
}
