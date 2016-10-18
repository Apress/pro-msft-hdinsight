$subscription = "Your_Subscription"   
$cluster = "democluster"
$storageAccountName = "democluster"
$Container = "democlustercontainer"
$storageAccountKey = Get-AzureStorageKey $storageAccountName | %{ $_.Primary }
$storageContext = New-AzureStorageContext –StorageAccountName $storageAccountName –StorageAccountKey $storageAccountKey
$inputPath = "wasb:///example/data/gutenberg/davinci.txt" 
$outputPath = "wasb:///example/data/WordCountOutputPS"
$jarFile = "wasb:///example/jars/hadoop-examples.jar"
$class = "wordcount"
$secpasswd = ConvertTo-SecureString "password" -AsPlainText -Force
$myCreds = New-Object System.Management.Automation.PSCredential ("user", $secpasswd)

# Define the word count MapReduce job
$mapReduceJobDefinition = New-AzureHDInsightMapReduceJobDefinition -JarFile $jarFile -ClassName $class  -Arguments $inputPath, $outputPath

# Submit the MapReduce job
Select-AzureSubscription $subscription
$wordCountJob = Start-AzureHDInsightJob -Cluster $cluster -JobDefinition $mapReduceJobDefinition -Credential $myCreds

# Wait for the job to complete
Wait-AzureHDInsightJob -Job $wordCountJob -WaitTimeoutInSeconds 3600 -Credential $myCreds

# Get the job standard error output
Get-AzureHDInsightJobOutput -Cluster $cluster -JobId $wordCountJob.JobId -StandardError -Subscription $subscription

# Get the blob content
Get-AzureStorageBlobContent -Container $Container -Blob example/data/WordCountOutputPS/part-r-00000 -Context $storageContext -Force

# List the content of the output file
cat ./example/data/WordCountOutputPS/part-r-00000 | findstr "human"