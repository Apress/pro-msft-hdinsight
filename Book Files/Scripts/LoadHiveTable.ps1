$subscriptionName = "Your_Subscription"
$storageAccountName = "democluster"
$containerName = "democlustercontainer"
$clustername = "democluster"

#$querystring = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData/tableFacebook.csv' into table stock_analysis partition(exchange ='NASDAQ');" 
#$querystring = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData/tableApple.csv' into table stock_analysis partition(exchange ='NASDAQ');" 
#$querystring = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData/tableGoogle.csv' into table stock_analysis partition(exchange ='NASDAQ');" 
#querystring = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData/tableIBM.csv' into table stock_analysis1 partition(exchange ='NYSE');" 
#$querystring = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/debarchan/StockData/tableOracle.csv' into table stock_analysis partition(exchange ='NYSE');" 

#$querystring = "show tables"

$HiveJobDefinition = New-AzureHDInsightHiveJobDefinition -Query $querystring
$HiveJob = Start-AzureHDInsightJob -Subscription $subscriptionname -Cluster $clustername -JobDefinition $HiveJobDefinition 

$HiveJob | Wait-AzureHDInsightJob -Subscription $subscriptionname -WaitTimeoutInSeconds 3600
Get-AzureHDInsightJobOutput -Cluster $clustername -Subscription $subscriptionname -JobId $HiveJob.JobId –StandardError