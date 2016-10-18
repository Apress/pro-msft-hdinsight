$creds = Get-Credential
$cluster = "http://localhost:50111"
$inputPath = "wasb://democlustercontainer@democluster.blob.core.windows.net/example/data/gutenberg/davinci.txt"
$outputFolder = "wasb://democlustercontainer@democluster.blob.core.windows.net/example/data/WordCountOutputEmulatorPS"
$jar = "wasb://democlustercontainer@democluster.blob.core.windows.net/hadoop-examples.jar"
$className = "wordcount"
$hdinsightJob = New-AzureHDInsightMapReduceJobDefinition -JarFile $jar -ClassName $className  -Arguments $inputPath, $outputPath

# Submit the MapReduce job
$wordCountJob = Start-AzureHDInsightJob -Cluster $cluster -JobDefinition $hdinsightJob -Credential $creds
# Wait for the job to complete
Wait-AzureHDInsightJob -Job $wordCountJob -WaitTimeoutInSeconds 3600 -Credential $creds




#hadoop fs -ls wasb://democlustercontainer@democluster.blob.core.windows.net/
#hadoop jar "hadoop-examples.jar" "wordcount" "/example/data/gutenberg/davinci.txt" "/example/data/WordCountOutputEmulator"