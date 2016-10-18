$subscriptionName = "Your_Subscription"
$storageAccountName = "democluster"
$containerName = "democlustercontainer"
$clustername = "democluster"

$querystring = "create external table stock_analysis111 (stock_symbol string,stock_Date string,stock_price_open double,stock_price_high  double,stock_price_low   double,stock_price_close double,stock_volume int,stock_price_adj_close double) partitioned by (exchange string) row format delimited fields terminated by ',' LOCATION 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData';"  
$HiveJobDefinition = New-AzureHDInsightHiveJobDefinition -Query $querystring
$HiveJob = Start-AzureHDInsightJob -Subscription $subscriptionname -Cluster $clustername -JobDefinition $HiveJobDefinition 

$HiveJob | Wait-AzureHDInsightJob -Subscription $subscriptionname -WaitTimeoutInSeconds 3600 

Get-AzureHDInsightJobOutput -Cluster $clustername -Subscription $subscriptionname -JobId $HiveJob.JobId -StandardError 