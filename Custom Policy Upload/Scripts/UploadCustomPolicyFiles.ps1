Function UploadPolicy
{
param(
    [string][Parameter(mandatory=$true)]$EU, 
    [string][Parameter(mandatory=$true)]$aP,
    [string][Parameter(mandatory=$true)]$Subscription ,
    [string][Parameter(mandatory=$true)]$AzureSubscriptionTenantId,
    [string][Parameter(mandatory=$true)]$b2cPolicyFolder,
    [bool][Parameter(mandatory=$false)]$overwriteIfExists=$true
    )
try
{
    Add-Type -AssemblyName System.Web
    $eP = ConvertTo-SecureString -String $aP -AsPlainText -Force
    $creds = New-Object Management.Automation.PSCredential ($EU,$eP)
    Login-AzureRmAccount -Credential $creds
    $context = Set-AzureRmContext -Tenant $AzureSubscriptionTenantId 
    $TenantId = $context.Tenant.TenantId
    $token = $context.TokenCache.ReadItems() | Where-Object { $_.Resource -ilike "*/management.core.windows.net/*" -and $_.AccessToken -ne $null -and $AzureSubscriptionTenantId -ieq $_.Authority.Split('/')[3] } | sort -Property ExpiresOn -Descending | select -First 1
    $strAccessToken = $token.AccessToken
    $folder=Get-ChildItem -Name $b2cPolicyFolder -File
    foreach ($file in $folder)
    {
        $b2cFilePolicy="$b2cPolicyFolder"+"\$file"
        if (Test-Path $b2cFilePolicy)
        {
            Write-Output "Uploading policy ($($b2cFilePolicy.Split('`\')[-1]))"

            $strPolicy = (Get-Content -Path $b2cFilePolicy) -join "`n"
            $strBody = "<string xmlns=`"http://schemas.microsoft.com/2003/10/Serialization/`">$([System.Web.HttpUtility]::HtmlEncode($strPolicy))</string>"
            $htHeaders = @{ "Authorization" = "Bearer $strAccessToken" }
            $response = $null           
            $response = Invoke-WebRequest -Uri "https://main.b2cadmin.ext.azure.com/api/trustframework?tenantId=$AzureSubscriptionTenantId&overwriteIfExists=$overwriteIfExists" -Method POST -Body $strBody -ContentType "application/xml" -Headers $htHeaders -UseBasicParsing -ErrorAction SilentlyContinue
            if ($response.StatusCode -ge 200 -and $response.StatusCode -le 299)
            {
                Write-Output "Uploaded policy ($($b2cFilePolicy.Split('`\')[-1])) successfully "
            }
            else
            {
                Write-Output "Failed to upload policy ($($b2cFilePolicy.Split('`\')[-1]))"
            }
        }
        else
        {
            Write-Error "Cannot find file: $($b2cFilePolicy.Split('`\')[-1])"
        }
    }
}
catch
{
    Write-Output $_
}
}

