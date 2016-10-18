using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HadoopClient
{
    public class Constants
    {
        public static Uri azureClusterUri = new Uri("https://democluster.azurehdinsight.net:443");
        public static string clusterName = "democluster";
        public static string thumbprint = "Your_Certificate_Thumbprint";
        public static Guid subscriptionId = new Guid("Your_Subscription_Id");
        public static string clusterUser = "Your_Cluster_User";
        public static string hadoopUser = "hdp";
        public static string clusterPassword = "Your_Password";
        public static string storageAccount = "democluster";
        public static string storageAccountKey = "Your_Storage_Key";
        public static string container = "democlustercontainer";
        public static string wasbPath = "wasb://democlustercontainer@democluster.blob.core.windows.net";
    }
}
