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

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [string]$Token = $null,
        [string]$ContentType = "application/json"
    )

    $url = "$BaseUrl$Path"
    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    $bodyData = $null
    if ($Body) { $bodyData = $Body | ConvertTo-Json -Depth 8 }

    try {
        $resp = Invoke-WebRequest -Uri $url -Method $Method -Headers $headers -ContentType $ContentType -Body $bodyData -UseBasicParsing
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

function Get-Config {
    if (Test-Path $DevConfigPath) {
        return (Get-Content $DevConfigPath | ConvertFrom-Json)
    }
    if (Test-Path $AppConfigPath) {
        return (Get-Content $AppConfigPath | ConvertFrom-Json)
    }
    throw "Config not found: $DevConfigPath or $AppConfigPath"
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

Write-Host "Loading configuration..."
$config = Get-Config
$jwtSecret = $config.JwtSettings.SecretKey
$issuer = $config.JwtSettings.Issuer
$audience = $config.JwtSettings.Audience

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

Write-Host "Creating roles..."
$roles = @("Admin", "Employee", "Manager", "User")
foreach ($role in $roles) {
    $createRole = Invoke-Api -Method POST -Path "/api/v1/admin/roles" -Body @{ name = $role; description = "$role role" } -Token $adminToken
    $roleOk = $createRole.ok -or $createRole.status -eq 409
    Add-Result -Name "POST /api/v1/admin/roles ($role)" -Ok $roleOk -Status $createRole.status
}

Write-Host "Creating cafe store if needed..."
$stores = Invoke-Api -Method GET -Path "/api/v1/admin/cafe-stores?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/cafe-stores" -Ok $stores.ok -Status $stores.status
$storeId = $null
if ($stores.ok -and $stores.json.data.items.Count -gt 0) {
    $storeId = $stores.json.data.items[0].storeId
}
else {
    $storeCreate = Invoke-Api -Method POST -Path "/api/v1/admin/cafe-stores" -Body @{
        storeName = "CoffeeHouse Nguyen Trai"
        address = "123 Nguyen Trai, Q1, HCM"
        phoneNumber = "0908123456"
        email = "nguyentrai@coffeehouse.local"
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/cafe-stores" -Ok $storeCreate.ok -Status $storeCreate.status
    if ($storeCreate.ok) { $storeId = $storeCreate.json.data.storeId }
}

Write-Host "Creating product category if needed..."
$categories = Invoke-Api -Method GET -Path "/api/v1/admin/product-categories?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/product-categories" -Ok $categories.ok -Status $categories.status
$categoryId = $null
if ($categories.ok -and $categories.json.data.items.Count -gt 0) {
    $categoryId = $categories.json.data.items[0].id
}
else {
    $categoryCreate = Invoke-Api -Method POST -Path "/api/v1/admin/product-categories" -Body @{
        name = "Coffee"
        description = "Coffee drinks"
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/product-categories" -Ok $categoryCreate.ok -Status $categoryCreate.status
    if ($categoryCreate.ok) { $categoryId = $categoryCreate.json.data.id }
}

Write-Host "Creating supplier if needed..."
$suppliers = Invoke-Api -Method GET -Path "/api/v1/admin/suppliers?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/suppliers" -Ok $suppliers.ok -Status $suppliers.status
$supplierId = $null
if ($suppliers.ok -and $suppliers.json.data.items.Count -gt 0) {
    $supplierId = $suppliers.json.data.items[0].supplierId
}
else {
    $supplierCreate = Invoke-Api -Method POST -Path "/api/v1/admin/suppliers" -Body @{
        supplierName = "VietBeans Co."
        address = "456 Le Loi, Q1, HCM"
        phoneNumber = "0909001122"
        stk = "0123456789"
        status = "Active"
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/suppliers" -Ok $supplierCreate.ok -Status $supplierCreate.status
    if ($supplierCreate.ok) { $supplierId = $supplierCreate.json.data.supplierId }
}

Write-Host "Creating customer if needed..."
$customers = Invoke-Api -Method GET -Path "/api/v1/admin/customers?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/customers" -Ok $customers.ok -Status $customers.status
$customerId = $null
if ($customers.ok -and $customers.json.data.items.Count -gt 0) {
    $customerId = $customers.json.data.items[0].id
}
else {
    $customerCreate = Invoke-Api -Method POST -Path "/api/v1/admin/customers" -Body @{
        name = "Le Thi Mai"
        phoneNumber = "0909007788"
        address = "Quan 3, HCM"
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/customers" -Ok $customerCreate.ok -Status $customerCreate.status
    if ($customerCreate.ok) { $customerId = $customerCreate.json.data.id }
}

Write-Host "Creating ingredient if needed..."
$ingredients = Invoke-Api -Method GET -Path "/api/v1/admin/ingredients?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/ingredients" -Ok $ingredients.ok -Status $ingredients.status
$ingredientId = $null
if ($ingredients.ok -and $ingredients.json.data.items.Count -gt 0) {
    $ingredientId = $ingredients.json.data.items[0].ingredientId
}
else {
    $ingredientCreate = Invoke-Api -Method POST -Path "/api/v1/admin/ingredients" -Body @{
        ingredientName = "Arabica Bean"
        quantity = 25
        unit = "kg"
        unitPrice = 180000
        minimumQuantity = 5
        expirationDate = (Get-Date).AddMonths(6).ToString("o")
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/ingredients" -Ok $ingredientCreate.ok -Status $ingredientCreate.status
    if ($ingredientCreate.ok) { $ingredientId = $ingredientCreate.json.data.ingredientId }
}

Write-Host "Creating employee..."
$employeePhone = "09" + ([DateTime]::UtcNow.Ticks.ToString().Substring(0,8))
$employeeIdCard = "ID" + ([DateTime]::UtcNow.Ticks.ToString().Substring(0,8))
$employeeCreate = Invoke-Api -Method POST -Path "/api/v1/admin/employees" -Body @{
    coffeeShopId = $storeId
    fullName = "Nguyen Minh Anh"
    position = "Barista"
    phoneNumber = $employeePhone
    idCardNumber = $employeeIdCard
    baseSalary = 6500000
    salaryCoefficient = 1.1
    address = "Quan 1, HCM"
    email = "minhanh.barista@coffeehouse.local"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/employees" -Ok $employeeCreate.ok -Status $employeeCreate.status
$employeeId = $null
if ($employeeCreate.ok) { $employeeId = $employeeCreate.json.data.id }

Write-Host "Creating purchase order..."
if ($storeId -and $supplierId -and $ingredientId -and $employeeId) {
    $poCreate = Invoke-Api -Method POST -Path "/api/v1/admin/purchase-orders" -Body @{
        storeId = $storeId
        employeeId = $employeeId
        supplierId = $supplierId
        orderDate = [DateTime]::UtcNow.ToString("o")
        description = "Nhap kho dau thang"
        items = @(
            @{ ingredientId = $ingredientId; quantity = 10; unitPrice = 180000 }
        )
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/purchase-orders" -Ok $poCreate.ok -Status $poCreate.status
}

Write-Host "Creating news article..."
$newsCreate = Invoke-Api -Method POST -Path "/api/v1/admin/news" -Body @{
    title = "Khai truong chi nhanh Nguyen Trai"
    content = "CoffeeHouse chinh thuc khai truong chi nhanh moi tai Nguyen Trai, Q1. Nhieu uu dai hap dan trong tuan dau."
    status = "Published"
    publishedAt = [DateTime]::UtcNow.ToString("o")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/news" -Ok $newsCreate.ok -Status $newsCreate.status

Write-Host "Creating test admin user..."
$userEmail = "admin+seed_" + [DateTime]::UtcNow.Ticks + "@example.com"
$userCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{ userName = $userEmail; email = $userEmail; password = "Admin@12345"; fullName = "Seed Admin"; roles = @("Admin") } -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users" -Ok $userCreate.ok -Status $userCreate.status

Write-Host "Creating manager user..."
$managerEmail = "manager+seed_" + [DateTime]::UtcNow.Ticks + "@example.com"
$managerCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
    userName = $managerEmail
    email = $managerEmail
    password = "Manager@12345"
    fullName = "Seed Manager"
    roles = @("Manager")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users (Manager)" -Ok $managerCreate.ok -Status $managerCreate.status

Write-Host "Creating employee user..."
if ($employeeId) {
    $employeeEmail = "employee+seed_" + [DateTime]::UtcNow.Ticks + "@example.com"
    $employeeUserCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
        userName = $employeeEmail
        email = $employeeEmail
        password = "Employee@12345"
        fullName = "Seed Employee User"
        roles = @("Employee")
        employeeId = $employeeId
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/users (Employee)" -Ok $employeeUserCreate.ok -Status $employeeUserCreate.status
}

Write-Host "Finance summary..."
$finance = Invoke-Api -Method GET -Path "/api/v1/admin/reports/finance" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/reports/finance" -Ok $finance.ok -Status $finance.status

Write-Host "Sales orders list..."
$salesOrders = Invoke-Api -Method GET -Path "/api/v1/admin/sales-orders?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/sales-orders" -Ok $salesOrders.ok -Status $salesOrders.status

Write-Host "\n=== Admin API Test Summary ==="
$Results | Format-Table -AutoSize
$failed = $Results | Where-Object { -not $_.Ok }
if ($failed.Count -gt 0) {
    Write-Host "\nFailed steps:"
    $failed | Format-Table -AutoSize
}
