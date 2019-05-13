<#
.SYNOPSIS
Starts a new Coordinator session by obtaining a new JWT token.

.PARAMETER Uri
The base URI of your Coordinator's API.
.PARAMETER Token
The API token to authenticate to the JWT provider with.

.EXAMPLE
New-CoordinatorSession -Uri "https://your-org.coordinator.rackview.eu/api/v0" -Token "your-secret-token"
#>
function New-CoordinatorSession {
    Param(
        [Parameter(Mandatory=$True)]
        [string]$Uri,
        [Parameter(Mandatory=$True)]
        [string]$Token
    )

    # Obtain our JWT
    $request = $Token | ConvertTo-Json;
    $response = Invoke-RestMethod -Method "Post" -Uri "$Uri/jwt/obtain" -Body $request -ContentType "application/json";
    return @{"Uri" = $Uri; "BearerToken" = $response};
}

<#
.SYNOPSIS
Retrieves the value at the specified SRN from the Coordinator.

.PARAMETER Session
The active Coordinator session to use.
.PARAMETER Srn
The partially or fully qualified SRN.
.PARAMETER Providers
Array of providers to use to retrieve the value.
Please note that specifying more than one provider is only supported for fully qualified SRN queries. Bulk queries are not supported.

.EXAMPLE
Get-SrnValue -Session $session -Srn "urn:srn:v0:global:test.foo"
#>
function Get-SrnValue {
    Param(
        [Parameter(Mandatory=$True)]
        [object]$Session,
        [Parameter(Mandatory=$True)]
        [string]$Srn,

        [string[]]$Providers = @("default")
    )

    $uri = $Session.Uri + "/config/$Srn";
    $request = @{"Providers" = $Providers} | ConvertTo-Json;
    $response = Invoke-RestMethod -Method "Get" -Uri $uri -Body $request -ContentType "application/json" -Headers @{"Authorization" = "Bearer " + $Session.BearerToken}
    return $response;
}

<#
.SYNOPSIS
Sets the value at the specified SRN on the Coordinator.

.PARAMETER Session
The active Coordinator session to use.
.PARAMETER Srn
The partially or fully qualified SRN.
.PARAMETER Provider
The provider to keep the value in.

.EXAMPLE
Set-SrnValue -Session $session -Srn "urn:srn:v0:global:test.foo" -Value "foo"
#>
function Set-SrnValue {
    Param(
        [Parameter(Mandatory=$True)]
        [object]$Session,
        [Parameter(Mandatory=$True)]
        [string]$Srn,
        [Parameter(Mandatory=$True)]
        [object]$Value,

        [string]$Provider = "default"
    )

    $uri = $Session.Uri + "/config/$Srn";
    $request = @{"Data" = $Value; "Provider" = $Provider} | ConvertTo-Json;
    Invoke-RestMethod -Method "Put" -Uri $uri -Body $request -ContentType "application/json" -Headers @{"Authorization" = "Bearer " + $Session.BearerToken}
}

<#
.SYNOPSIS
Removes the value at the specified SRN from the Coordinator.

.PARAMETER Session
The active Coordinator session to use.
.PARAMETER Srn
The partially or fully qualified SRN.
.PARAMETER Provider
The provider to remove the value from.

.EXAMPLE
Remove-SrnValue -Session $session -Srn "urn:srn:v0:global:test.foo"
#>
function Remove-SrnValue {
    Param(
        [Parameter(Mandatory=$True)]
        [object]$Session,
        [Parameter(Mandatory=$True)]
        [string]$Srn,

        [string]$Provider = "default"
    )

    $uri = $Session.Uri + "/config/$Srn";
    $request = @{"Provider" = $Provider} | ConvertTo-Json;
    $response = Invoke-RestMethod -Method "Delete" -Uri $uri -Body $request -ContentType "application/json" -Headers @{"Authorization" = "Bearer " + $Session.BearerToken}
    return $response;
}