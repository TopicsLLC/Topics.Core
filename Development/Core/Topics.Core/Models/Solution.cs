using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topics.Core.Models2
{





    [XmlRoot(ElementName = "subsystems")]
    public class Subsystems
    {
        [XmlElement(ElementName = "subsystem")]
        public List<string> Subsystem { get; set; }
    }

    //[XmlRoot(ElementName = "buildsystem")]
    //public class Buildsystem
    //{
    //    [XmlElement(ElementName = "ccnetport")]
    //    public string Ccnetport { get; set; }
    //    [XmlElement(ElementName = "ccnethost")]
    //    public string Ccnethost { get; set; }
    //    [XmlElement(ElementName = "ccnetdashboard")]
    //    public string Ccnetdashboard { get; set; }
    //}

    [XmlRoot(ElementName = "server")]
    public class Server : INotifyPropertyChanged, ILazyUpdater
    {

        public Server()
        {
            NodeId = "1";
        }

        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "DeploymentProjectName")]
        public string DeploymentProjectName { get; set; }
        [XmlElement(ElementName = "host")]
        public string Host { get; set; }
        [XmlElement(ElementName = "port")]
        public int DeploymentServerPort { get; set; }
        [XmlElement(ElementName = "path")]
        public string Path { get; set; }

        //[XmlElement(ElementName = "ClientSetupBootstrapperProject")]
        //public ClientSetupBootstrapperProject ClientSetupBootstrapperProject { get; set; }

        //[XmlElement(ElementName = "ClientSetupDeployments")]
        //public ClientSetupDeployments ClientSetupDeployments { get; set; }

        //[XmlElement(ElementName = "PackageServerDeployments")]
        //public PackageServerDeployments PackageServerDeployments { get; set; }

        //[XmlElement(ElementName = "ServiceDeployments")]
        //public ServiceDeployments ServiceDeployments { get; set; }

        //[XmlElement(ElementName = "WebSiteDeployments")]
        //public WebSiteDeployments WebSiteDeployments { get; set; }


        [XmlElement(ElementName = "ServerProject")]
        public string ServerProject { get; set; }

        [XmlElement(ElementName = "ProjectName")]
        public string ProjectName { get; set; }
        [XmlElement(ElementName = "SolutionPath")]
        public string SolutionPath { get; set; }
        [XmlElement(ElementName = "ServerApplication")]
        public string ServerApplication { get; set; }
        [XmlElement(ElementName = "NodeId")]
        public string NodeId { get; set; }
        [XmlElement(ElementName = "ProjectFolder")]
        public string ProjectFolder { get; set; }
        [XmlElement(ElementName = "ProjectDeploymentConfig")]
        public string ProjectDeploymentConfig { get; set; }
        [XmlElement(ElementName = "ServerType")]
        public string ServerType { get; set; }

        [XmlElement(ElementName = "ReleaseDirectory")]
        public string ReleaseDirectory { get; set; }
        [XmlElement(ElementName = "ApplicationSolutionFolder")]
        public string ApplicationSolutionFolder { get; set; }

        [XmlElement(ElementName = "DeploymentDirectoryLocal")]
        public string DeploymentDirectoryLocal { get; set; }
        [XmlElement(ElementName = "DeploymentShareName")]
        public string DeploymentShareName { get; set; }
        [XmlElement(ElementName = "DeploymentShareLocal")]
        public string DeploymentShareLocal { get; set; }

        [XmlElement(ElementName = "DeploymentDirectory")]
        public string DeploymentDirectory { get; set; }
        [XmlElement(ElementName = "DeploymentShareDrive")]
        public string DeploymentShareDrive { get; set; }
        [XmlElement(ElementName = "DeploymentShareUser")]
        public string DeploymentShareUser { get; set; }
        [XmlElement(ElementName = "DeploymentSharePassword")]
        public string DeploymentSharePassword { get; set; }
        [XmlElement(ElementName = "DeploymentRunAsUser")]
        public string DeploymentRunAsUser { get; set; }
        [XmlElement(ElementName = "DeploymentRunAsPassword")]
        public string DeploymentRunAsPassword { get; set; }
        [XmlElement(ElementName = "DeploymentRunAsDomain")]
        public string DeploymentRunAsDomain { get; set; }

        [XmlElement(ElementName = "WorkingDirectory")]
        public string WorkingDirectory { get; set; }

        [XmlElement(ElementName = "SourceControlType")]
        public string SourceControlType { get; set; }
        [XmlElement(ElementName = "IntervalTrigger")]
        public string IntervalTrigger { get; set; }
        [XmlElement(ElementName = "ModificationDelaySeconds")]
        public string ModificationDelaySeconds { get; set; }

        [XmlElement(ElementName = "SourceControlExecutable")]
        public string SourceControlExecutable { get; set; }
        [XmlElement(ElementName = "NantExecutable")]
        public string NantExecutable { get; set; }

        [XmlElement(ElementName = "BaseDreictory")]
        public string BaseDreictory { get; set; }
        [XmlElement(ElementName = "BuildFile")]
        public string BuildFile { get; set; }
        [XmlElement(ElementName = "BuildTimeout")]
        public string BuildTimeout { get; set; }

        [XmlElement(ElementName = "DeploymentScriptPath")]
        public string DeploymentScriptPath { get; set; }
        [XmlElement(ElementName = "DeploymentConfigPath")]
        public string DeploymentConfigPath { get; set; }
        [XmlElement(ElementName = "PowershellPath")]
        public string PowershellPath { get; set; }

        [XmlElement(ElementName = "DeploymentShare")]
        public string DeploymentShare { get; set; }

        [XmlElement(ElementName = "DefaultHostLocalDrive")]
        public string DefaultHostLocalDrive { get; set; }

        [XmlElement(ElementName = "ServiceAssembly")]
        public string ServiceAssembly { get; set; }


        //ClickOnce
        public string OriginalEntryPoint { get; set; }
        public string EntryPoint { get; set; }
        public string DevelopmentDirectory { get; set; }
        public string OriginalClickOnceManifest { get; set; }
        public string OriginalClickOnceApplicationFile { get; set; }
        public string OriginalClickOnceApplicationName { get; set; }
        public string ClickOnceManifest { get; set; }
        public string ClickOnceApplicationFile { get; set; }
        public string ClickOnceApplicationName { get; set; }
        public string ClickOnceKeyFile { get; set; }
        public string ClickOncePublisher { get; set; }
        public string ProviderUrl { get; set; }
        public string BootStrapper { get; set; }
        public string CertificatePassword { get; set; }
        public string ProjectInstaller { get; set; }
        public string OriginalProjectInstaller { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    [XmlRoot(ElementName = "servers")]
    public class Servers
    {
        [XmlElement(ElementName = "server")]
        public BindingList<Server> Server { get; set; }
    }

    [XmlRoot(ElementName = "environment")]
    public class Environment
    {
        [XmlElement(ElementName = "environmentId")]
        public string EnvironmentId { get; set; }
        [XmlElement(ElementName = "environmentName")]
        public string EnvironmentName { get; set; }
        [XmlElement(ElementName = "SourceControlBranch")]
        public string SourceControlBranch { get; set; }
        [XmlElement(ElementName = "SourceControlPassword")]
        public string SourceControlPassword { get; set; }
        [XmlElement(ElementName = "SourceControlUserName")]
        public string SourceControlUserName { get; set; }

        [XmlElement(ElementName = "WorkingDirectoryDrive")]
        public string WorkingDirectoryDrive { get; set; }
        [XmlElement(ElementName = "WorkingDirectory")]
        public string WorkingDirectory { get; set; }




        [XmlElement(ElementName = "servers")]
        public Servers Servers { get; set; }

        [XmlElement(ElementName = "active")]
        public bool Active { get; set; }
        [XmlElement(ElementName = "hostName")]
        public string HostName { get; set; }
        [XmlElement(ElementName = "shareDrive")]
        public string ShareDrive { get; set; }
        [XmlElement(ElementName = "shareUser")]
        public string ShareUser { get; set; }
        [XmlElement(ElementName = "sharePassword")]
        public string SharePassword { get; set; }

        [XmlElement(ElementName = "ccnetport")]
        public int Ccnetport { get; set; }
        [XmlElement(ElementName = "ccnethost")]
        public string Ccnethost { get; set; }
        [XmlElement(ElementName = "ccnetdashboard")]
        public int Ccnetdashboard { get; set; }

        [XmlElement(ElementName = "MonitoredBuildAll")]
        public bool MonitoredBuildAll { get; set; }
        [XmlElement(ElementName = "UnmonitoredBuildAll")]
        public bool UnmonitoredBuildAll { get; set; }
        [XmlElement(ElementName = "IndividualBuilds")]
        public bool IndividualBuilds { get; set; }
        [XmlElement(ElementName = "SourceControlType")]
        public string SourceControlType { get; set; }
        [XmlElement(ElementName = "IntervalTrigger")]
        public string IntervalTrigger { get; set; }
        [XmlElement(ElementName = "ModificationDelaySeconds")]
        public string ModificationDelaySeconds { get; set; }
        [XmlElement(ElementName = "SourceControlExecutable")]
        public string SourceControlExecutable { get; set; }
        [XmlElement(ElementName = "NantExecutable")]
        public string NantExecutable { get; set; }

        [XmlElement(ElementName = "SDLCDirectory")]
        public string SDLCDirectory { get; set; }
        [XmlElement(ElementName = "BuildFile")]
        public string BuildFile { get;  set; }
        [XmlElement(ElementName = "BuildTimeout")]
        public string BuildTimeout { get;  set; }


        [XmlElement(ElementName = "DeployScriptPath")]
        public string DeploymentScriptPath { get; set; }
        [XmlElement(ElementName = "DeployConfigPath")]
        public string DeploymentConfigPath { get; set; }
        [XmlElement(ElementName = "PowershellPath")]
        public string PowershellPath { get; set; }
        public string BuildServerHost { get; set; }
        public string BuildServerDomain { get; set; }
        public string BuildServerPassword { get; set; }
        public string BuildServerSvnPath { get; set; }
        public string BuildServerUserName { get; set; }
    }

    [XmlRoot(ElementName = "environments")]
    public class Environments
    {
        public Environments()
        {
            Environment = new ObservableCollection<Environment>();
        }
        [XmlElement(ElementName = "environment")]
        public ObservableCollection<Environment> Environment { get; set; }
    }

    [XmlRoot(ElementName = "solution")]
    public class TopicsSolution
    {
        public TopicsSolution()
        {
            Environments = new Environments();
            SolutionProjects = new SolutionProjects();
        }

        [XmlElement(ElementName = "organizationname")]
        public string OrganizationName { get; set; }
        [XmlElement(ElementName = "organizationurl")]
        public string OrganizationURL { get; set; }
        [XmlElement(ElementName = "solutionname")]
        public string SolutionName { get; set; }
        [XmlElement(ElementName = "solutiondescription")]
        public string SolutionDescription { get; set; }
        [XmlElement(ElementName = "solutiondirectory")]
        public string SolutionDirectory { get; set; }
        [XmlElement(ElementName = "solutionfile")]
        public string SolutionFile { get; set; }
        [XmlElement(ElementName = "buildfile")]
        public string Buildfile { get; set; }
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "svnrepo")]
        public string Svnrepo { get; set; }
        [XmlElement(ElementName = "nextPort")]
        public int NextPort { get; set; }

        [XmlElement(ElementName = "subsystems")]
        public Subsystems Subsystems { get; set; }
        //[XmlElement(ElementName = "buildsystem")]
        //public Buildsystem Buildsystem { get; set; }
        [XmlElement(ElementName = "environments")]
        public Environments Environments { get; set; }
        [XmlElement(ElementName = "solutionprojects")]
        public SolutionProjects SolutionProjects { get; set; }
        [XmlElement(ElementName = "sdlcsource")]
        public SolutionProjects SDLCSource { get; set; }



        public string SourceTemplateFolder { get; set; }
        public string SourceDevelopmentFolder { get; set; }
        public string TemplateApplicationName { get; set; }
        public string TopicsCoreVersion { get; set; }
        public string SDKDir { get; set; }
    }

    [XmlRoot(ElementName = "solutionprojects")]
    public class SolutionProjects
    {
        public SolutionProjects()
        {
            SolutionProject = new ObservableCollection<SolutionProject>();
        }
        [XmlElement(ElementName = "solutionproject")]
        public ObservableCollection<SolutionProject> SolutionProject { get; set; }
    }


    [XmlRoot(ElementName = "solutionproject")]
    public class SolutionProject
    {
        [XmlElement(ElementName = "TemplateProjectName")]
        public string TemplateProjectName { get; set; }
        [XmlElement(ElementName = "templateprojecttype")]
        public string TemplateProjectType { get; set; }
        [XmlElement(ElementName = "TargetProjectName")]
        public string TargetProjectName { get; set; }
        [XmlElement(ElementName = "targetProjectPath")]
        public string TargetProjectPath { get; set; }
        [XmlElement(ElementName = "deploy")]
        public bool Deployed { get; set; }
        [XmlElement(ElementName = "applicationName")]
        public string ApplicationName { get; set; }
        [XmlElement(ElementName = "applicationsolutionfolder")]
        public string ApplicationSolutionFolder { get; set; }
        [XmlElement(ElementName = "devPort")]
        public int DevPort { get; set; }
        [XmlElement(ElementName = "sslPort")]
        public int SSLPort { get; set; }
        [XmlElement(ElementName = "deployable")]
        public bool Deployable { get; set; }


        [XmlElement(ElementName = "deploymentsuffix")]
        public string DeploymentSuffix { get; set; }
        [XmlElement(ElementName = "deploymenttype")]
        public string DeploymentType { get; set; }

        [XmlElement(ElementName = "ProjectFolder")]
        public string ProjectFolder { get; set; }
        [XmlElement(ElementName = "ProjectName")]
        public string ProjectName { get; set; }
    }
}
