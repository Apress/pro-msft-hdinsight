$subscriptionName = "Your_Subscription"
$storageAccountName = "democluster"
$containerName = "democlustercontainer"
#This path may vary depending on where you place the source .csv files.
$fileName ="D:\output.txt"
$blobName = "01Tweets"
# Get the storage account key
Select-AzureSubscription $subscriptionName
$storageaccountkey = get-azurestoragekey $storageAccountName | %{$_.Primary}
# Create the storage context object
$destContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageaccountkey
# Copy the file from local workstation to the Blob container 
Set-AzureStorageBlobContent -File $fileName -Container $containerName -Blob $blobName -context $destContext 