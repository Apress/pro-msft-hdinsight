$subscriptionName = "Your_Subscription"
$clustername = "democluster"
Select-AzureSubscription -SubscriptionName $subscriptionName
Use-AzureHDInsightCluster $clusterName -Subscription (Get-AzureSubscription -Current).SubscriptionId

$querystring  = "load data inpath 'wasb://democlustercontainer@democluster.blob.core.windows.net/hadoopuser/StockData/tableApple.csv' into table AAPLStockData partition(exchange ='NASDAQ');"
Invoke-Hive –Query $querystring
