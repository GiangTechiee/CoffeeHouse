$ErrorActionPreference = "Stop"

$BaseUrl = "http://localhost:5069"
$DevConfigPath = "src/CoffeeHouse.Web/appsettings.Development.json"
$AppConfigPath = "src/CoffeeHouse.Web/appsettings.json"

function ConvertTo-Base64Url([byte[]]$bytes) {
    $base64 = [Convert]::ToBase64String($bytes)
    $base64 = $base64.TrimEnd("=").Replace("+", "-").Replace("/", "_")
    return $base64
}

function New-JwtToken {
    param(
        [string]$Secret,
        [string]$Issuer,
        [string]$Audience,
        [hashtable]$Claims
    )

    $header = @{ alg = "HS256"; typ = "JWT" } | ConvertTo-Json -Compress
    $payload = $Claims | ConvertTo-Json -Compress

    $headerB64 = ConvertTo-Base64Url([Text.Encoding]::UTF8.GetBytes($header))
    $payloadB64 = ConvertTo-Base64Url([Text.Encoding]::UTF8.GetBytes($payload))
    $unsigned = "$headerB64.$payloadB64"

    $hmac = New-Object System.Security.Cryptography.HMACSHA256
    $hmac.Key = [Text.Encoding]::UTF8.GetBytes($Secret)
    $sig = ConvertTo-Base64Url($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($unsigned)))

    return "$unsigned.$sig"
}

function Decode-JwtPayload([string]$Token) {
    $parts = $Token.Split(".")
    if ($parts.Length -lt 2) { return $null }
    $payload = $parts[1].Replace("-", "+").Replace("_", "/")
    switch ($payload.Length % 4) {
        2 { $payload += "==" }
        3 { $payload += "=" }
    }
    $json = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload))
    return $json | ConvertFrom-Json
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [string]$Token = $null
    )

    $url = "$BaseUrl$Path"
    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    $bodyJson = $null
    if ($Body) { $bodyJson = $Body | ConvertTo-Json -Depth 8 }

    try {
        $resp = Invoke-WebRequest -Uri $url -Method $Method -Headers $headers -ContentType "application/json" -Body $bodyJson -UseBasicParsing
        $json = $null
        if ($resp.Content) { $json = $resp.Content | ConvertFrom-Json }
        return @{ ok = $true; status = $resp.StatusCode; json = $json; raw = $resp.Content }
    } catch {
        $status = 0
        $raw = ""
        if ($_.Exception.Response) {
            $status = [int]$_.Exception.Response.StatusCode
            $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $raw = $sr.ReadToEnd()
        }
        $json = $null
        if ($raw) {
            try { $json = $raw | ConvertFrom-Json } catch { $json = $null }
        }
        return @{ ok = $false; status = $status; json = $json; raw = $raw; error = $_.Exception.Message }
    }
}

function Add-Result {
    param(
        [string]$Name,
        [bool]$Ok,
        [int]$Status,
        [string]$Note = ""
    )
    $script:Results += [pscustomobject]@{
        Step = $Name
        Ok = $Ok
        Status = $Status
        Note = $Note
    }
}

function Get-Config {
    if (Test-Path $DevConfigPath) {
        return (Get-Content $DevConfigPath | ConvertFrom-Json)
    }
    if (Test-Path $AppConfigPath) {
        return (Get-Content $AppConfigPath | ConvertFrom-Json)
    }
    throw "Config not found: $DevConfigPath or $AppConfigPath"
}

function Get-StoreId([string]$ConnectionString) {
    $npgsqlPath = "tools/CoffeeHouse.Seeder/bin/Debug/net8.0/Npgsql.dll"
    if (Test-Path $npgsqlPath) {
        try {
            Add-Type -Path $npgsqlPath | Out-Null
            $conn = New-Object Npgsql.NpgsqlConnection($ConnectionString)
            $conn.Open()
            $cmd = $conn.CreateCommand()
            $cmd.CommandText = "SELECT ""StoreId"" FROM ""CafeStores"" ORDER BY ""StoreId"" LIMIT 1"
            $storeId = [int]$cmd.ExecuteScalar()
            $conn.Close()
            if ($storeId -gt 0) { return $storeId }
        } catch {
            return 1
        }
    }
    return 1
}

Write-Host "Loading configuration..."
$config = Get-Config
$jwtSecret = $config.JwtSettings.SecretKey
$issuer = $config.JwtSettings.Issuer
$audience = $config.JwtSettings.Audience
$connStr = $config.ConnectionStrings.CoffeeHouseDb

$Results = @()

Write-Host "Generating admin token..."
$adminClaims = @{
    sub = "1"
    unique_name = "api-admin@coffeehouse.local"
    email = "api-admin@coffeehouse.local"
    jti = [guid]::NewGuid().ToString()
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" = "Admin"
    exp = [int][DateTimeOffset]::UtcNow.AddHours(2).ToUnixTimeSeconds()
    iss = $issuer
    aud = $audience
}
$adminToken = New-JwtToken -Secret $jwtSecret -Issuer $issuer -Audience $audience -Claims $adminClaims

Write-Host "Health check..."
$health = Invoke-Api -Method GET -Path "/health"
Add-Result -Name "GET /health" -Ok $health.ok -Status $health.status

Write-Host "Ensuring cafe store..."
$storeId = $null
$storeList = Invoke-Api -Method GET -Path "/api/v1/admin/cafe-stores?page=1&pageSize=1" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/cafe-stores" -Ok $storeList.ok -Status $storeList.status
if ($storeList.ok -and $storeList.json.data.items.Count -gt 0) {
    $storeId = $storeList.json.data.items[0].storeId
}
else {
    $storeCreate = Invoke-Api -Method POST -Path "/api/v1/admin/cafe-stores" -Body @{
        storeName = "CoffeeHouse Nguyen Hue"
        address = "123 Nguyen Hue, Q1, HCM"
        phoneNumber = "0908123999"
        email = "nguyenhue@coffeehouse.local"
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/cafe-stores" -Ok $storeCreate.ok -Status $storeCreate.status
    if ($storeCreate.ok -and $storeCreate.json.data) {
        $storeId = $storeCreate.json.data.storeId
    }
}

Write-Host "Loading categories..."
$categoriesResp = Invoke-Api -Method GET -Path "/api/v1/categories?page=1&pageSize=100"
Add-Result -Name "GET /api/v1/categories" -Ok $categoriesResp.ok -Status $categoriesResp.status
$categoryMap = @{}
if ($categoriesResp.ok -and $categoriesResp.json.data) {
    foreach ($c in $categoriesResp.json.data.items) {
        $categoryMap[$c.name] = $c.id
    }
}

Write-Host "Seeding categories..."
$desiredCategories = @(
    @{ name = "Coffee"; description = "Signature espresso-based drinks" },
    @{ name = "Tea"; description = "Vietnamese and fruit teas" },
    @{ name = "Smoothie"; description = "Fresh fruit smoothies" },
    @{ name = "Cake"; description = "Fresh bakery pastries" }
)
foreach ($cat in $desiredCategories) {
    if (-not $categoryMap.ContainsKey($cat.name)) {
        $create = Invoke-Api -Method POST -Path "/api/v1/categories" -Body $cat -Token $adminToken
        Add-Result -Name "POST /api/v1/categories ($($cat.name))" -Ok $create.ok -Status $create.status
        if ($create.ok -and $create.json.data) {
            $categoryMap[$cat.name] = $create.json.data.id
        }
    }
}

Write-Host "Loading products..."
$productsResp = Invoke-Api -Method GET -Path "/api/v1/products?page=1&pageSize=100"
Add-Result -Name "GET /api/v1/products" -Ok $productsResp.ok -Status $productsResp.status
$productMap = @{}
if ($productsResp.ok -and $productsResp.json.data) {
    foreach ($p in $productsResp.json.data.items) {
        $productMap[$p.name] = $p.id
    }
}

Write-Host "Seeding products..."
$desiredProducts = @(
    @{ name = "Espresso"; price = 30000; category = "Coffee"; desc = "Single shot espresso" },
    @{ name = "Cappuccino"; price = 45000; category = "Coffee"; desc = "Espresso with steamed milk and foam" },
    @{ name = "Peach Tea"; price = 35000; category = "Tea"; desc = "Refreshing peach tea" },
    @{ name = "Cheese Cake"; price = 55000; category = "Cake"; desc = "Creamy cheese cake slice" }
)
foreach ($prod in $desiredProducts) {
    if (-not $productMap.ContainsKey($prod.name)) {
        if ($categoryMap.ContainsKey($prod.category)) {
            $body = @{
                name = $prod.name
                price = $prod.price
                description = $prod.desc
                categoryId = $categoryMap[$prod.category]
            }
            $create = Invoke-Api -Method POST -Path "/api/v1/products" -Body $body -Token $adminToken
            Add-Result -Name "POST /api/v1/products ($($prod.name))" -Ok $create.ok -Status $create.status
            if ($create.ok -and $create.json.data) {
                $productMap[$prod.name] = $create.json.data.id
            }
        }
    }
}

Write-Host "Testing category read endpoints..."
if ($categoryMap.Count -gt 0) {
    $firstCatId = $categoryMap.Values | Select-Object -First 1
    $catGet = Invoke-Api -Method GET -Path "/api/v1/categories/$firstCatId"
    Add-Result -Name "GET /api/v1/categories/{id}" -Ok $catGet.ok -Status $catGet.status
    $catProducts = Invoke-Api -Method GET -Path "/api/v1/categories/$firstCatId/products"
    Add-Result -Name "GET /api/v1/categories/{id}/products" -Ok $catProducts.ok -Status $catProducts.status
}

Write-Host "Testing product read endpoints..."
if ($productMap.Count -gt 0) {
    $firstProdId = $productMap.Values | Select-Object -First 1
    $prodGet = Invoke-Api -Method GET -Path "/api/v1/products/$firstProdId"
    Add-Result -Name "GET /api/v1/products/{id}" -Ok $prodGet.ok -Status $prodGet.status
}

Write-Host "Testing category update/delete..."
$tempCatName = "ApiTestCat-" + [DateTime]::UtcNow.ToString("yyyyMMddHHmmss")
$tempCat = Invoke-Api -Method POST -Path "/api/v1/categories" -Body @{ name = $tempCatName; description = "Temp" } -Token $adminToken
Add-Result -Name "POST /api/v1/categories (temp)" -Ok $tempCat.ok -Status $tempCat.status
if ($tempCat.ok) {
    $tempCatId = $tempCat.json.data.id
    $updateCat = Invoke-Api -Method PUT -Path "/api/v1/categories/$tempCatId" -Body @{ id = $tempCatId; name = "$tempCatName-Updated"; description = "Temp Updated" } -Token $adminToken
    Add-Result -Name "PUT /api/v1/categories/{id}" -Ok $updateCat.ok -Status $updateCat.status
    $deleteCat = Invoke-Api -Method DELETE -Path "/api/v1/categories/$tempCatId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/categories/{id}" -Ok $deleteCat.ok -Status $deleteCat.status
}

Write-Host "Testing product update/delete..."
$tempProdName = "ApiTestProd-" + [DateTime]::UtcNow.ToString("yyyyMMddHHmmss")
$tempCategoryId = $categoryMap["Coffee"]
if ($tempCategoryId) {
    $tempProd = Invoke-Api -Method POST -Path "/api/v1/products" -Body @{
        name = $tempProdName
        price = 15000
        description = "Temp"
        categoryId = $tempCategoryId
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/products (temp)" -Ok $tempProd.ok -Status $tempProd.status
    if ($tempProd.ok) {
        $tempProdId = $tempProd.json.data.id
        $updateProd = Invoke-Api -Method PUT -Path "/api/v1/products/$tempProdId" -Body @{
            id = $tempProdId
            name = "$tempProdName-Updated"
            price = 18000
            description = "Temp Updated"
            categoryId = $tempCategoryId
        } -Token $adminToken
        Add-Result -Name "PUT /api/v1/products/{id}" -Ok $updateProd.ok -Status $updateProd.status
        $deleteProd = Invoke-Api -Method DELETE -Path "/api/v1/products/$tempProdId" -Token $adminToken
        Add-Result -Name "DELETE /api/v1/products/{id}" -Ok $deleteProd.ok -Status $deleteProd.status
    }
}

Write-Host "Registering test customer..."
$custEmail = "customer_" + [DateTime]::UtcNow.Ticks + "@example.com"
$register = Invoke-Api -Method POST -Path "/api/v1/auth/register" -Body @{
    username = $custEmail
    email = $custEmail
    password = "Customer@123"
    confirmPassword = "Customer@123"
    fullName = "Pham Minh Tam"
    phoneNumber = "0123456789"
    address = "27 Le Duan, Q1, HCM"
}
Add-Result -Name "POST /api/v1/auth/register" -Ok $register.ok -Status $register.status

$login = Invoke-Api -Method POST -Path "/api/v1/auth/login" -Body @{ username = $custEmail; password = "Customer@123" }
Add-Result -Name "POST /api/v1/auth/login" -Ok $login.ok -Status $login.status
$customerToken = $null
$customerId = 0
if ($login.ok -and $login.json.data) {
    $customerToken = $login.json.data.token
    $payload = Decode-JwtPayload $customerToken
    if ($payload -and $payload.CustomerId) { $customerId = [int]$payload.CustomerId }
}

if ($customerToken) {
    $me = Invoke-Api -Method GET -Path "/api/v1/auth/me" -Token $customerToken
    Add-Result -Name "GET /api/v1/auth/me" -Ok $me.ok -Status $me.status
}

Write-Host "Testing customers API..."
$custList = Invoke-Api -Method GET -Path "/api/v1/customers?page=1&pageSize=10" -Token $adminToken
Add-Result -Name "GET /api/v1/customers" -Ok $custList.ok -Status $custList.status

$adminCust = Invoke-Api -Method POST -Path "/api/v1/customers" -Body @{
    name = "Admin Created Customer"
    phoneNumber = "0900000001"
    address = "Admin Address"
} -Token $adminToken
Add-Result -Name "POST /api/v1/customers" -Ok $adminCust.ok -Status $adminCust.status

if ($adminCust.ok) {
    $adminCustId = $adminCust.json.data.id
    $getCust = Invoke-Api -Method GET -Path "/api/v1/customers/$adminCustId" -Token $adminToken
    Add-Result -Name "GET /api/v1/customers/{id}" -Ok $getCust.ok -Status $getCust.status
    $updateCust = Invoke-Api -Method PUT -Path "/api/v1/customers/$adminCustId" -Body @{
        id = $adminCustId
        name = "Admin Created Customer Updated"
        phoneNumber = "0900000002"
        address = "Admin Address 2"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/customers/{id}" -Ok $updateCust.ok -Status $updateCust.status
    $custOrders = Invoke-Api -Method GET -Path "/api/v1/customers/$adminCustId/orders?page=1&pageSize=5" -Token $adminToken
    Add-Result -Name "GET /api/v1/customers/{id}/orders" -Ok $custOrders.ok -Status $custOrders.status
    $deleteCust = Invoke-Api -Method DELETE -Path "/api/v1/customers/$adminCustId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/customers/{id}" -Ok $deleteCust.ok -Status $deleteCust.status
}

Write-Host "Testing orders API..."
if ($customerToken -and $productMap.Count -gt 0 -and $storeId) {
    $espressoId = $productMap["Espresso"]
    $cappuccinoId = $productMap["Cappuccino"]
    if (-not $espressoId) { $espressoId = $productMap.Values | Select-Object -First 1 }
    if (-not $cappuccinoId) { $cappuccinoId = $espressoId }

    $orderCreate = Invoke-Api -Method POST -Path "/api/v1/orders" -Body @{
        coffeeShopId = $storeId
        customerId = $customerId
        paymentMethod = "Cash"
        items = @(
            @{ productId = $espressoId; productName = "Espresso"; quantity = 2; unitPrice = 20000 },
            @{ productId = $cappuccinoId; productName = "Cappuccino"; quantity = 1; unitPrice = 35000 }
        )
    } -Token $customerToken

    if ($orderCreate -and $orderCreate.ok) {
        Add-Result -Name "POST /api/v1/orders" -Ok $orderCreate.ok -Status $orderCreate.status

        $orderId = $orderCreate.json.data.id
        $orderGet = Invoke-Api -Method GET -Path "/api/v1/orders/$orderId" -Token $customerToken
        Add-Result -Name "GET /api/v1/orders/{id}" -Ok $orderGet.ok -Status $orderGet.status

        $orderList = Invoke-Api -Method GET -Path "/api/v1/orders?page=1&pageSize=5" -Token $adminToken
        Add-Result -Name "GET /api/v1/orders" -Ok $orderList.ok -Status $orderList.status

        $orderStatus = Invoke-Api -Method PATCH -Path "/api/v1/orders/$orderId/status" -Body @{
            orderId = $orderId
            status = "Confirmed"
        } -Token $adminToken
        Add-Result -Name "PATCH /api/v1/orders/{id}/status" -Ok $orderStatus.ok -Status $orderStatus.status

        $orderCancel = Invoke-Api -Method DELETE -Path "/api/v1/orders/$orderId" -Token $adminToken
        Add-Result -Name "DELETE /api/v1/orders/{id}" -Ok $orderCancel.ok -Status $orderCancel.status
    }
    else {
        Add-Result -Name "POST /api/v1/orders" -Ok $false -Status 500 -Note "Order creation failed"
    }
}

Write-Host "Testing auth/logout..."
$logout = Invoke-Api -Method POST -Path "/api/v1/auth/logout"
Add-Result -Name "POST /api/v1/auth/logout" -Ok $logout.ok -Status $logout.status

Write-Host ""
Write-Host "=== API Test Summary ==="
$Results | Format-Table -AutoSize
$failed = $Results | Where-Object { -not $_.Ok }
if ($failed.Count -gt 0) {
    Write-Host ""
    Write-Host "Failed steps:"
    $failed | Format-Table -AutoSize
}
