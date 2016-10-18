$subid = "Your_Subscription_ID"
$subName = "Your_Subscription"
$clusterName = "democluster"
$0 = '$0';
$QueryString =  "LOGS = LOAD 'wasb:///example/data/sample.log';" +
                "LEVELS = foreach LOGS generate REGEX_EXTRACT($0, '(TRACE|DEBUG|INFO|WARN|ERROR|FATAL)', 1)  as LOGLEVEL;" +
                "FILTEREDLEVELS = FILTER LEVELS by LOGLEVEL is not null;" +
                "GROUPEDLEVELS = GROUP FILTEREDLEVELS by LOGLEVEL;" +
                "FREQUENCIES = foreach GROUPEDLEVELS generate group as LOGLEVEL, COUNT(FILTEREDLEVELS.LOGLEVEL) as COUNT;" +
                "RESULT = order FREQUENCIES by COUNT desc;" +
                "DUMP RESULT;" 


$pigJobDefinition = New-AzureHDInsightPigJobDefinition -Query $QueryString -StatusFolder "/PigJobs/PigJobStatus"
#Submit the Pig Job to the cluster
$pigJob = Start-AzureHDInsightJob -Subscription $subid -Cluster $clusterName -JobDefinition $pigJobDefinition
#Wait for the job to complete  - 
$pigJob | Wait-AzureHDInsightJob -Subscription $subid -WaitTimeoutInSeconds 3600

