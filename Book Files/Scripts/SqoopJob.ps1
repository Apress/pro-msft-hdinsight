 $subscriptionName = "Your_Subscription"
 $clusterName = "democluster"
 $SqoopCommand = "export --connect `"jdbc:sqlserver://<server>.database.windows.net;username=user@server;password=password;database=sqoopdemo`" --table stock_analysis --export-dir /user/hadoopuser/example/data/StockAnalysis --input-fields-terminated-by `",`""
 $sqoop = New-AzureHDInsightSqoopJobDefinition -Command $SqoopCommand
 $SqoopJob = Start-AzureHDInsightJob -Subscription (Get-AzureSubscription -Current).SubscriptionId -Cluster $clustername -JobDefinition $sqoop
 Wait-AzureHDInsightJob -Subscription (Get-AzureSubscription -Current).SubscriptionId -Job $SqoopJob -WaitTimeoutInSeconds 3600
 Get-AzureHDInsightJobOutput -Cluster $clusterName -JobId $SqoopJob.JobId -StandardError -Subscription $subscriptionName