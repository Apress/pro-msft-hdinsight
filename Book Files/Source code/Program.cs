using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.Management.HDInsight;
//using Newtonsoft.Json.Linq;
using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop.Client;
//For Stream IO
using System.IO;
//For Ambari Monitoring Client
using Microsoft.Hadoop.WebClient.AmbariClient;
using Microsoft.Hadoop.WebClient.AmbariClient.Contracts;
//For Regex
using System.Text.RegularExpressions;
//For thread
using System.Threading;
//For Blob Storage
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace HadoopClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ListClusters();
            //CreateCluster();
            //DeleteCluster();                        
            //DoCustomMapReduce();
            //DoMapReduce();
            //DoHiveOperations();            
            TestMethod();
            //MonitorCluster();
            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        public static void TestMethod()
        {
            Stream stream = new MemoryStream();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=" + Constants.storageAccount + ";AccountKey=" + Constants.storageAccountKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(Constants.container);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("01Tweets");

            //blob.

            //blob.DownloadToStream(stream);
            //stream.Position = 0;
            //StreamReader reader = new StreamReader(stream);
            //Console.Write("Testing..\n");
            //Console.WriteLine(reader.ReadToEnd());
        }
        
        //List existing HDI clusters
        public static void ListClusters()
        {
            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.Thumbprint == Constants.thumbprint);            
            var creds = new HDInsightCertificateCredential(Constants.subscriptionId, cert);
            var client = HDInsightClient.Connect(creds);
            var clusters = client.ListClusters();
            Console.WriteLine("The list of clusters and their details are");
            foreach (var item in clusters)
            {
                Console.WriteLine("Cluster: {0}, Nodes: {1}, State: {2}, Version: {3}", item.Name, item.ClusterSizeInNodes, item.State, item.Version);
            }
        }
        //Create a new HDI cluster
        public static void CreateCluster()
        {
            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.Thumbprint == Constants.thumbprint);
            var creds = new HDInsightCertificateCredential(Constants.subscriptionId, cert);
            var client = HDInsightClient.Connect(creds);

            //Cluster information
           var clusterInfo = new ClusterCreateParameters()
            {
                Name = "AutomatedHDICluster",
                Location = "West Europe",
                DefaultStorageAccountName = Constants.storageAccount,
                DefaultStorageAccountKey = Constants.storageAccountKey,
                DefaultStorageContainer = Constants.container,
                UserName = Constants.clusterUser,
                Password = Constants.clusterPassword,
                ClusterSizeInNodes = 2,
                Version="2.1"
            };
           Console.Write("Creating cluster...");
           var clusterDetails = client.CreateCluster(clusterInfo);
           Console.Write("Done\n");
           ListClusters();
        }
 
        //Delete an existing HDI cluster
        public static void DeleteCluster()
        {
            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.Thumbprint == Constants.thumbprint);
            var creds = new HDInsightCertificateCredential(Constants.subscriptionId, cert);
            var client = HDInsightClient.Connect(creds);
            Console.Write("Deleting cluster...");
            client.DeleteCluster("AutomatedHDICluster");
            Console.Write("Done\n");
            ListClusters();
        }

        //Run Custom Map Reduce
        public static void DoCustomMapReduce()
        {
            Console.WriteLine("Starting MapReduce job. Remote login to your Name Node and check progress from JobTracker portal with the returned JobID...");
            IHadoop hadoop = Hadoop.Connect(Constants.azureClusterUri, Constants.clusterUser,
                            Constants.hadoopUser, Constants.clusterPassword, Constants.storageAccount,
                            Constants.storageAccountKey, Constants.container, true);
            //IHadoop hadoop = Hadoop.Connect();
            var output = hadoop.MapReduceJob.ExecuteJob<SquareRootJob>();
        }
        //Run Sample Map Reduce Job
        public static void DoMapReduce()
        {
            // Define the MapReduce job
            MapReduceJobCreateParameters mrJobDefinition = new MapReduceJobCreateParameters()
            {
                JarFile = "wasb:///example/jars/hadoop-examples.jar",
                ClassName = "wordcount"
            };
            mrJobDefinition.Arguments.Add("wasb:///example/data/gutenberg/davinci.txt");
            mrJobDefinition.Arguments.Add("wasb:///example/data/WordCountOutput");

            //Get certificate
            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.Thumbprint == Constants.thumbprint);
            var creds = new JobSubmissionCertificateCredential(Constants.subscriptionId, cert, Constants.clusterName);

            // Create a hadoop client to connect to HDInsight
            var jobClient = JobSubmissionClientFactory.Connect(creds);

            // Run the MapReduce job
            JobCreationResults mrJobResults = jobClient.CreateMapReduceJob(mrJobDefinition);
            Console.Write("Executing WordCount MapReduce Job.");

            // Wait for the job to complete
            WaitForJobCompletion(mrJobResults, jobClient);

            // Print the MapReduce job output
            Stream stream = new MemoryStream();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=" + Constants.storageAccount + ";AccountKey=" + Constants.storageAccountKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(Constants.container);
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference("example/data/WordCountOutput/part-r-00000");
            blockBlob.DownloadToStream(stream);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            Console.Write("Done..Word counts are:\n");
            Console.WriteLine(reader.ReadToEnd());
        }
        
        //Run Hive Job
        public static void DoHiveOperations()
        {
            HiveJobCreateParameters hiveJobDefinition = new HiveJobCreateParameters()
            {
                JobName = "Show tables job",
                StatusFolder = "/TableListFolder",
                Query = "show tables;"
            };

            var store = new X509Store();
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Cast<X509Certificate2>().First(item => item.Thumbprint == Constants.thumbprint);
            var creds = new JobSubmissionCertificateCredential(Constants.subscriptionId, cert, Constants.clusterName);
            var jobClient = JobSubmissionClientFactory.Connect(creds);
            JobCreationResults jobResults = jobClient.CreateHiveJob(hiveJobDefinition);
            Console.Write("Executing Hive Job.");
            // Wait for the job to complete
            WaitForJobCompletion(jobResults, jobClient);
            // Print the Hive job output
            System.IO.Stream stream = jobClient.GetJobOutput(jobResults.JobId);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            Console.Write("Done..List of Tables are:\n");
            Console.WriteLine(reader.ReadToEnd());            
        }
        //Monitor cluster Map Reduce statistics
        public static void MonitorCluster()
        {
            var client = new AmbariClient(Constants.azureClusterUri, Constants.clusterUser, Constants.clusterPassword);
            IList<ClusterInfo> clusterInfos = client.GetClusters();
            ClusterInfo clusterInfo = clusterInfos[0];
            Console.WriteLine("Cluster Href: {0}", clusterInfo.Href);

            Regex clusterNameRegEx = new Regex(@"(\w+)\.*");
            var clusterName = clusterNameRegEx.Match(Constants.azureClusterUri.Authority).Groups[1].Value;
            HostComponentMetric hostComponentMetric = client.GetHostComponentMetric(clusterName + ".azurehdinsight.net");
            Console.WriteLine("Cluster Map Redeuce Metrics:");
            Console.WriteLine("\tMaps Completed: \t{0}", hostComponentMetric.MapsCompleted);
            Console.WriteLine("\tMaps Failed: \t{0}", hostComponentMetric.MapsFailed);
            Console.WriteLine("\tMaps Killed: \t{0}", hostComponentMetric.MapsKilled);
            Console.WriteLine("\tMaps Launched: \t{0}", hostComponentMetric.MapsLaunched);
            Console.WriteLine("\tMaps Running: \t{0}", hostComponentMetric.MapsRunning);
            Console.WriteLine("\tMaps Waiting: \t{0}", hostComponentMetric.MapsWaiting);
        }

        ///Helper Function to Wait while job execution
        private static void WaitForJobCompletion(JobCreationResults jobResults, IJobSubmissionClient client)
        {
            JobDetails jobInProgress = client.GetJob(jobResults.JobId);
            while (jobInProgress.StatusCode != JobStatusCode.Completed && jobInProgress.StatusCode != JobStatusCode.Failed)
            {
                jobInProgress = client.GetJob(jobInProgress.JobId);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Console.Write(".");
            }
        }

    }
}

    