$ErrorActionPreference = "Stop"

$BaseUrl = $env:COFFEEHOUSE_BASEURL
if (-not $BaseUrl) { $BaseUrl = "http://localhost:5069" }
$DevConfigPath = "src/CoffeeHouse.Web/appsettings.Development.json"
$AppConfigPath = "src/CoffeeHouse.Web/appsettings.json"
$TestImagePath = "tools/test-image.png"

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
    if ($Body) { $bodyData = $Body | ConvertTo-Json -Depth 12 }

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

function Invoke-ApiMultipart {
    param(
        [string]$Path,
        [string]$FilePath,
        [string]$Token = $null
    )

    Add-Type -AssemblyName System.Net.Http
    $url = "$BaseUrl$Path"
    $client = New-Object System.Net.Http.HttpClient
    if ($Token) {
        $client.DefaultRequestHeaders.Authorization = New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $Token)
    }

    $content = New-Object System.Net.Http.MultipartFormDataContent
    $fileStream = [System.IO.File]::OpenRead($FilePath)

    try {
        $fileContent = New-Object System.Net.Http.StreamContent($fileStream)
        $fileContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse("image/png")
        $content.Add($fileContent, "ImageFile", [System.IO.Path]::GetFileName($FilePath))

        $response = $client.PostAsync($url, $content).Result
        $raw = $response.Content.ReadAsStringAsync().Result
        $json = $null
        if ($raw) { $json = $raw | ConvertFrom-Json }
        $ok = $response.IsSuccessStatusCode

        return @{ ok = $ok; status = [int]$response.StatusCode; json = $json; raw = $raw }
    }
    catch {
        return @{ ok = $false; status = 0; json = $null; raw = ""; error = $_.Exception.Message }
    }
    finally {
        $fileStream.Dispose()
        $content.Dispose()
        $client.Dispose()
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
        [hashtable]$Resp,
        [string]$Note = ""
    )
    $ok = $Resp.ok
    if (-not $ok -and $Resp.status -ge 200 -and $Resp.status -lt 300) { $ok = $true }
    $script:Results += [pscustomobject]@{
        Step = $Name
        Ok = $ok
        Status = $Resp.status
        Note = $Note
    }
}

function Add-Skipped {
    param(
        [string]$Name,
        [string]$Note
    )
    $script:Results += [pscustomobject]@{
        Step = $Name
        Ok = $true
        Status = 0
        Note = $Note
    }
}

function New-Phone {
    return ("09" + ([DateTime]::UtcNow.Ticks.ToString().Substring(0,8)))
}

function Ensure-TestImage {
    if (Test-Path $TestImagePath) { return }
    $pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII="
    [IO.File]::WriteAllBytes($TestImagePath, [Convert]::FromBase64String($pngBase64))
}

Write-Host "Loading configuration..."
$config = Get-Config
$jwtSecret = $config.JwtSettings.SecretKey
$issuer = $config.JwtSettings.Issuer
$audience = $config.JwtSettings.Audience
$Results = @()

$nowSuffix = (Get-Date).ToString("yyyyMMddHHmmssfff")

Write-Host "Preparing tokens..."
$adminClaims = @{
    sub = "1"
    unique_name = "api-admin@coffeehouse.local"
    email = "api-admin@coffeehouse.local"
    jti = [guid]::NewGuid().ToString()
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" = "Admin"
    exp = [int][DateTimeOffset]::UtcNow.AddHours(4).ToUnixTimeSeconds()
    iss = $issuer
    aud = $audience
}
$adminToken = New-JwtToken -Secret $jwtSecret -Issuer $issuer -Audience $audience -Claims $adminClaims

Write-Host "Bootstrapping (admin bootstrap endpoint)..."
$bootstrapResp = Invoke-Api -Method POST -Path "/api/v1/admin/bootstrap" -Body @{
    roles = @("Admin","Employee","Manager","User")
} -Token $null
Add-Result -Name "POST /api/v1/admin/bootstrap" -Resp $bootstrapResp

Write-Host "Listing admin roles..."
$rolesList = Invoke-Api -Method GET -Path "/api/v1/admin/roles?page=1&pageSize=50" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/roles" -Resp $rolesList

Write-Host "Creating core admin roles..."
$roles = @("Admin", "Employee", "Manager", "User")
foreach ($role in $roles) {
    $createRole = Invoke-Api -Method POST -Path "/api/v1/admin/roles" -Body @{ name = $role; description = "$role role" } -Token $adminToken
    if (-not $createRole.ok -and $createRole.status -eq 409) { $createRole.ok = $true }
    Add-Result -Name "POST /api/v1/admin/roles ($role)" -Resp $createRole
}

$roleTemp = Invoke-Api -Method POST -Path "/api/v1/admin/roles" -Body @{ name = "TempRole $nowSuffix"; description = "Temp role" } -Token $adminToken
Add-Result -Name "POST /api/v1/admin/roles (temp)" -Resp $roleTemp
$roleTempId = $roleTemp.json.data.id

if ($roleTempId) {
    $roleTempUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/roles/$roleTempId" -Body @{ id = $roleTempId; name = "TempRole Updated $nowSuffix"; description = "Updated" } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/roles/{id}" -Resp $roleTempUpdate

    $roleTempDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/roles/$roleTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/roles/{id}" -Resp $roleTempDelete
} else {
    Add-Skipped -Name "PUT /api/v1/admin/roles/{id}" -Note "Missing temp role id"
    Add-Skipped -Name "DELETE /api/v1/admin/roles/{id}" -Note "Missing temp role id"
}

Write-Host "Cafe stores..."
$storeList = Invoke-Api -Method GET -Path "/api/v1/admin/cafe-stores?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/cafe-stores" -Resp $storeList

$storeCreate = Invoke-Api -Method POST -Path "/api/v1/admin/cafe-stores" -Body @{
    storeName = "CoffeeHouse Nguyen Trai $nowSuffix"
    address = "123 Nguyen Trai, Q1, HCM"
    phoneNumber = New-Phone
    email = "nguyentrai_$nowSuffix@coffeehouse.local"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/cafe-stores" -Resp $storeCreate
$storeId = $storeCreate.json.data.storeId

if ($storeId) {
    $storeGet = Invoke-Api -Method GET -Path "/api/v1/admin/cafe-stores/$storeId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/cafe-stores/{id}" -Resp $storeGet

    $storeUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/cafe-stores/$storeId" -Body @{
        storeId = $storeId
        storeName = "CoffeeHouse Nguyen Trai Updated $nowSuffix"
        address = "123 Nguyen Trai, Q1, HCM (Updated)"
        phoneNumber = New-Phone
        email = "nguyentrai_updated_$nowSuffix@coffeehouse.local"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/cafe-stores/{id}" -Resp $storeUpdate
}

$storeTemp = Invoke-Api -Method POST -Path "/api/v1/admin/cafe-stores" -Body @{
    storeName = "CoffeeHouse Temp $nowSuffix"
    address = "999 Temp Street, Q3, HCM"
    phoneNumber = New-Phone
    email = "temp_$nowSuffix@coffeehouse.local"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/cafe-stores (temp)" -Resp $storeTemp
$storeTempId = $storeTemp.json.data.storeId

Write-Host "Product categories (admin)..."
$catList = Invoke-Api -Method GET -Path "/api/v1/admin/product-categories?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/product-categories" -Resp $catList

$catCoffee = Invoke-Api -Method POST -Path "/api/v1/admin/product-categories" -Body @{
    name = "Coffee $nowSuffix"
    description = "Coffee drinks"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/product-categories (Coffee)" -Resp $catCoffee
$categoryId = $catCoffee.json.data.id

$catTea = Invoke-Api -Method POST -Path "/api/v1/admin/product-categories" -Body @{
    name = "Tea $nowSuffix"
    description = "Tea drinks"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/product-categories (Tea)" -Resp $catTea
$categoryTeaId = $catTea.json.data.id

if ($categoryId) {
    $catGet = Invoke-Api -Method GET -Path "/api/v1/admin/product-categories/$categoryId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/product-categories/{id}" -Resp $catGet

    $catUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/product-categories/$categoryId" -Body @{
        id = $categoryId
        name = "Coffee Updated $nowSuffix"
        description = "Updated coffee drinks"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/product-categories/{id}" -Resp $catUpdate
}

$catTemp = Invoke-Api -Method POST -Path "/api/v1/admin/product-categories" -Body @{
    name = "Temp Category $nowSuffix"
    description = "Temp"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/product-categories (temp)" -Resp $catTemp
$categoryTempId = $catTemp.json.data.id

Write-Host "Suppliers..."
$supplierList = Invoke-Api -Method GET -Path "/api/v1/admin/suppliers?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/suppliers" -Resp $supplierList

$supplier = Invoke-Api -Method POST -Path "/api/v1/admin/suppliers" -Body @{
    supplierName = "VietBeans Co. $nowSuffix"
    address = "456 Le Loi, Q1, HCM"
    phoneNumber = New-Phone
    stk = "0123456789"
    status = "Active"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/suppliers" -Resp $supplier
$supplierId = $supplier.json.data.supplierId

if ($supplierId) {
    $supplierGet = Invoke-Api -Method GET -Path "/api/v1/admin/suppliers/$supplierId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/suppliers/{id}" -Resp $supplierGet

    $supplierUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/suppliers/$supplierId" -Body @{
        supplierId = $supplierId
        supplierName = "VietBeans Co. Updated $nowSuffix"
        address = "456 Le Loi, Q1, HCM (Updated)"
        phoneNumber = New-Phone
        stk = "0123456789"
        status = "Active"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/suppliers/{id}" -Resp $supplierUpdate
}

$supplierTemp = Invoke-Api -Method POST -Path "/api/v1/admin/suppliers" -Body @{
    supplierName = "Temp Supplier $nowSuffix"
    address = "Temp Address"
    phoneNumber = New-Phone
    stk = "9876543210"
    status = "Active"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/suppliers (temp)" -Resp $supplierTemp
$supplierTempId = $supplierTemp.json.data.supplierId

Write-Host "Ingredients..."
$ingredientList = Invoke-Api -Method GET -Path "/api/v1/admin/ingredients?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/ingredients" -Resp $ingredientList

$ingredient1 = Invoke-Api -Method POST -Path "/api/v1/admin/ingredients" -Body @{
    ingredientName = "Arabica Bean $nowSuffix"
    quantity = 25
    unit = "kg"
    unitPrice = 180000
    minimumQuantity = 5
    expirationDate = (Get-Date).AddMonths(6).ToString("o")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/ingredients (Arabica)" -Resp $ingredient1
$ingredientId = $ingredient1.json.data.ingredientId

$ingredient2 = Invoke-Api -Method POST -Path "/api/v1/admin/ingredients" -Body @{
    ingredientName = "Condensed Milk $nowSuffix"
    quantity = 40
    unit = "can"
    unitPrice = 22000
    minimumQuantity = 10
    expirationDate = (Get-Date).AddMonths(9).ToString("o")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/ingredients (Milk)" -Resp $ingredient2
$ingredientId2 = $ingredient2.json.data.ingredientId

if ($ingredientId) {
    $ingredientGet = Invoke-Api -Method GET -Path "/api/v1/admin/ingredients/$ingredientId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/ingredients/{id}" -Resp $ingredientGet

    $ingredientUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/ingredients/$ingredientId" -Body @{
        ingredientId = $ingredientId
        ingredientName = "Arabica Bean Premium $nowSuffix"
        quantity = 30
        unit = "kg"
        unitPrice = 185000
        minimumQuantity = 6
        expirationDate = (Get-Date).AddMonths(7).ToString("o")
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/ingredients/{id}" -Resp $ingredientUpdate
}

$ingredientTemp = Invoke-Api -Method POST -Path "/api/v1/admin/ingredients" -Body @{
    ingredientName = "Temp Ingredient $nowSuffix"
    quantity = 5
    unit = "kg"
    unitPrice = 10000
    minimumQuantity = 1
    expirationDate = (Get-Date).AddMonths(1).ToString("o")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/ingredients (temp)" -Resp $ingredientTemp
$ingredientTempId = $ingredientTemp.json.data.ingredientId

Write-Host "Products (admin)..."
$productList = Invoke-Api -Method GET -Path "/api/v1/admin/products?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/products" -Resp $productList

$product1 = Invoke-Api -Method POST -Path "/api/v1/admin/products" -Body @{
    name = "Latte $nowSuffix"
    price = 45000
    description = "Latte with arabica beans"
    notes = "Best served hot"
    categoryId = $categoryId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/products (Latte)" -Resp $product1
$productId = $product1.json.data.id

$product2 = Invoke-Api -Method POST -Path "/api/v1/admin/products" -Body @{
    name = "Iced Milk Tea $nowSuffix"
    price = 39000
    description = "Refreshing milk tea"
    notes = "Less sugar"
    categoryId = $categoryTeaId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/products (MilkTea)" -Resp $product2
$productId2 = $product2.json.data.id

if ($productId) {
    $productGet = Invoke-Api -Method GET -Path "/api/v1/admin/products/$productId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/products/{id}" -Resp $productGet

    $productUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/products/$productId" -Body @{
        id = $productId
        name = "Latte Signature $nowSuffix"
        price = 47000
        description = "Signature latte with arabica beans"
        notes = "Try with extra shot"
        categoryId = $categoryId
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/products/{id}" -Resp $productUpdate
}

$productTemp = Invoke-Api -Method POST -Path "/api/v1/admin/products" -Body @{
    name = "Temp Product $nowSuffix"
    price = 10000
    description = "Temp"
    notes = "Temp"
    categoryId = $categoryId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/products (temp)" -Resp $productTemp
$productTempId = $productTemp.json.data.id

Write-Host "Uploading product image..."
if ($productId) {
    Ensure-TestImage
    $uploadResp = Invoke-ApiMultipart -Path "/api/v1/admin/products/$productId/image" -FilePath $TestImagePath -Token $adminToken
    Add-Result -Name "POST /api/v1/admin/products/{id}/image" -Resp $uploadResp
} else {
    Add-Skipped -Name "POST /api/v1/admin/products/{id}/image" -Note "Missing product id"
}

Write-Host "Employees..."
$employeeList = Invoke-Api -Method GET -Path "/api/v1/admin/employees?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/employees" -Resp $employeeList

$employeeCreate = Invoke-Api -Method POST -Path "/api/v1/admin/employees" -Body @{
    coffeeShopId = $storeId
    fullName = "Nguyen Minh Anh"
    position = "Barista"
    phoneNumber = New-Phone
    idCardNumber = "ID$nowSuffix"
    baseSalary = 6500000
    salaryCoefficient = 1.1
    address = "Quan 1, HCM"
    email = "minhanh_$nowSuffix@coffeehouse.local"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/employees" -Resp $employeeCreate
$employeeId = $employeeCreate.json.data.id

if ($employeeId) {
    $employeeGet = Invoke-Api -Method GET -Path "/api/v1/admin/employees/$employeeId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/employees/{id}" -Resp $employeeGet

    $employeeUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/employees/$employeeId" -Body @{
        id = $employeeId
        coffeeShopId = $storeId
        fullName = "Nguyen Minh Anh Updated"
        position = "Senior Barista"
        phoneNumber = New-Phone
        idCardNumber = "ID$nowSuffix"
        baseSalary = 7000000
        salaryCoefficient = 1.2
        address = "Quan 1, HCM (Updated)"
        email = "minhanh_updated_$nowSuffix@coffeehouse.local"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/employees/{id}" -Resp $employeeUpdate
}

$employeeTemp = Invoke-Api -Method POST -Path "/api/v1/admin/employees" -Body @{
    coffeeShopId = $storeId
    fullName = "Temp Employee $nowSuffix"
    position = "Cashier"
    phoneNumber = New-Phone
    idCardNumber = "TEMP$nowSuffix"
    baseSalary = 5000000
    salaryCoefficient = 1.0
    address = "Quan 2, HCM"
    email = "temp_emp_$nowSuffix@coffeehouse.local"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/employees (temp)" -Resp $employeeTemp
$employeeTempId = $employeeTemp.json.data.id

Write-Host "Customers (admin API)..."
$adminCustomerList = Invoke-Api -Method GET -Path "/api/v1/admin/customers?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/customers" -Resp $adminCustomerList

$customerAdmin = Invoke-Api -Method POST -Path "/api/v1/admin/customers" -Body @{
    name = "Le Thi Mai"
    phoneNumber = New-Phone
    address = "Quan 3, HCM"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/customers" -Resp $customerAdmin
$customerAdminId = $customerAdmin.json.data.id

if ($customerAdminId) {
    $adminCustomerGet = Invoke-Api -Method GET -Path "/api/v1/admin/customers/$customerAdminId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/customers/{id}" -Resp $adminCustomerGet

    $adminCustomerUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/customers/$customerAdminId" -Body @{
        id = $customerAdminId
        name = "Le Thi Mai Updated"
        phoneNumber = New-Phone
        address = "Quan 3, HCM (Updated)"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/customers/{id}" -Resp $adminCustomerUpdate
}

$customerTemp = Invoke-Api -Method POST -Path "/api/v1/admin/customers" -Body @{
    name = "Temp Customer $nowSuffix"
    phoneNumber = New-Phone
    address = "Quan 7, HCM"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/customers (temp)" -Resp $customerTemp
$customerTempId = $customerTemp.json.data.id

Write-Host "Purchase orders (admin)..."
$poList = Invoke-Api -Method GET -Path "/api/v1/admin/purchase-orders?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/purchase-orders" -Resp $poList

$poCreate = Invoke-Api -Method POST -Path "/api/v1/admin/purchase-orders" -Body @{
    storeId = $storeId
    employeeId = $employeeId
    supplierId = $supplierId
    orderDate = [DateTime]::UtcNow.ToString("o")
    description = "Nhap kho dau thang"
    status = "Pending"
    items = @(
        @{ ingredientId = $ingredientId; quantity = 10; unitPrice = 180000 },
        @{ ingredientId = $ingredientId2; quantity = 20; unitPrice = 22000 }
    )
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/purchase-orders" -Resp $poCreate
$purchaseOrderId = $poCreate.json.data.purchaseOrderId

Write-Host "Testing purchase order items endpoints..."
if ($purchaseOrderId) {
    $poGet = Invoke-Api -Method GET -Path "/api/v1/admin/purchase-orders/$purchaseOrderId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/purchase-orders/{id}" -Resp $poGet

    $poItems = Invoke-Api -Method GET -Path "/api/v1/admin/purchase-orders/$purchaseOrderId/items" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/purchase-orders/{id}/items" -Resp $poItems

    if ($ingredientTempId) {
        $poItemAdd = Invoke-Api -Method POST -Path "/api/v1/admin/purchase-orders/$purchaseOrderId/items" -Body @{
            ingredientId = $ingredientTempId
            quantity = 2
            unitPrice = 10000
        } -Token $adminToken
        Add-Result -Name "POST /api/v1/admin/purchase-orders/{id}/items" -Resp $poItemAdd

        $poItemUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/purchase-orders/$purchaseOrderId/items/$ingredientTempId" -Body @{
            ingredientId = $ingredientTempId
            quantity = 3
            unitPrice = 12000
        } -Token $adminToken
        Add-Result -Name "PUT /api/v1/admin/purchase-orders/{id}/items/{ingredientId}" -Resp $poItemUpdate

        $poItemDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/purchase-orders/$purchaseOrderId/items/$ingredientTempId" -Token $adminToken
        Add-Result -Name "DELETE /api/v1/admin/purchase-orders/{id}/items/{ingredientId}" -Resp $poItemDelete
    } else {
        Add-Skipped -Name "POST /api/v1/admin/purchase-orders/{id}/items" -Note "Missing ingredient temp id"
        Add-Skipped -Name "PUT /api/v1/admin/purchase-orders/{id}/items/{ingredientId}" -Note "Missing ingredient temp id"
        Add-Skipped -Name "DELETE /api/v1/admin/purchase-orders/{id}/items/{ingredientId}" -Note "Missing ingredient temp id"
    }

    $poUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/purchase-orders/$purchaseOrderId" -Body @{
        purchaseOrderId = $purchaseOrderId
        storeId = $storeId
        employeeId = $employeeId
        supplierId = $supplierId
        orderDate = [DateTime]::UtcNow.ToString("o")
        description = "Nhap kho dau thang (Updated)"
        status = "Processing"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/purchase-orders/{id}" -Resp $poUpdate
} else {
    Add-Skipped -Name "GET /api/v1/admin/purchase-orders/{id}" -Note "Missing purchase order id"
    Add-Skipped -Name "GET /api/v1/admin/purchase-orders/{id}/items" -Note "Missing purchase order id"
    Add-Skipped -Name "PUT /api/v1/admin/purchase-orders/{id}" -Note "Missing purchase order id"
}

$poTemp = Invoke-Api -Method POST -Path "/api/v1/admin/purchase-orders" -Body @{
    storeId = $storeId
    employeeId = $employeeId
    supplierId = $supplierId
    orderDate = [DateTime]::UtcNow.ToString("o")
    description = "Temp purchase order"
    status = "Pending"
    items = @(
        @{ ingredientId = $ingredientId; quantity = 1; unitPrice = 180000 }
    )
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/purchase-orders (temp)" -Resp $poTemp
$poTempId = $poTemp.json.data.purchaseOrderId

if ($poTempId) {
    $poTempDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/purchase-orders/$poTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/purchase-orders/{id}" -Resp $poTempDelete
}

Write-Host "News (admin)..."
$newsList = Invoke-Api -Method GET -Path "/api/v1/admin/news?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/news" -Resp $newsList

$newsCreate = Invoke-Api -Method POST -Path "/api/v1/admin/news" -Body @{
    title = "Khai truong chi nhanh Nguyen Trai $nowSuffix"
    content = "CoffeeHouse khai truong chi nhanh moi tai Nguyen Trai, Q1. Uu dai 20% cho tuan dau."
    status = "Published"
    publishedAt = [DateTime]::UtcNow.ToString("o")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/news" -Resp $newsCreate
$newsId = $newsCreate.json.data.articleId

if ($newsId) {
    $newsGet = Invoke-Api -Method GET -Path "/api/v1/admin/news/$newsId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/news/{id}" -Resp $newsGet

    $newsUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/news/$newsId" -Body @{
        articleId = $newsId
        title = "Khai truong chi nhanh Nguyen Trai $nowSuffix (Updated)"
        content = "Cap nhat: Uu dai 30% trong 3 ngay dau."
        status = "Published"
        publishedAt = [DateTime]::UtcNow.ToString("o")
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/news/{id}" -Resp $newsUpdate
}

$newsTemp = Invoke-Api -Method POST -Path "/api/v1/admin/news" -Body @{
    title = "Temp News $nowSuffix"
    content = "Temp"
    status = "Draft"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/news (temp)" -Resp $newsTemp
$newsTempId = $newsTemp.json.data.articleId

if ($newsTempId) {
    $newsTempDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/news/$newsTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/news/{id}" -Resp $newsTempDelete
}

Write-Host "Users (admin)..."
$adminUsersList = Invoke-Api -Method GET -Path "/api/v1/admin/users?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/users" -Resp $adminUsersList

$adminEmail = "admin_seed_$nowSuffix@example.com"
$adminUserCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
    userName = $adminEmail
    email = $adminEmail
    password = "Admin@12345"
    fullName = "Seed Admin $nowSuffix"
    roles = @("Admin")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users (Admin)" -Resp $adminUserCreate
$adminUserId = $adminUserCreate.json.data.id

$managerEmail = "manager_seed_$nowSuffix@example.com"
$managerUserCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
    userName = $managerEmail
    email = $managerEmail
    password = "Manager@12345"
    fullName = "Seed Manager $nowSuffix"
    roles = @("Manager")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users (Manager)" -Resp $managerUserCreate

$employeeEmail = "employee_seed_$nowSuffix@example.com"
$employeeUserCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
    userName = $employeeEmail
    email = $employeeEmail
    password = "Employee@12345"
    fullName = "Seed Employee $nowSuffix"
    roles = @("Employee")
    employeeId = $employeeId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users (Employee)" -Resp $employeeUserCreate

$userTempEmail = "temp_user_$nowSuffix@example.com"
$userTempCreate = Invoke-Api -Method POST -Path "/api/v1/admin/users" -Body @{
    userName = $userTempEmail
    email = $userTempEmail
    password = "Temp@12345"
    fullName = "Temp User $nowSuffix"
    roles = @("User")
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/users (temp)" -Resp $userTempCreate
$userTempId = $userTempCreate.json.data.id

if ($userTempId) {
    $userTempGet = Invoke-Api -Method GET -Path "/api/v1/admin/users/$userTempId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/users/{id}" -Resp $userTempGet

    $userTempUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/users/$userTempId" -Body @{
        id = $userTempId
        fullName = "Temp User Updated $nowSuffix"
        address = "HCM"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/users/{id}" -Resp $userTempUpdate

    $userTempRoles = Invoke-Api -Method PUT -Path "/api/v1/admin/users/$userTempId/roles" -Body @{
        userId = $userTempId
        roles = @("Manager")
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/users/{id}/roles" -Resp $userTempRoles

    $userTempDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/users/$userTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/users/{id}" -Resp $userTempDelete
}

Write-Host "Legacy roles & accounts (admin)..."
$legacyRoleList = Invoke-Api -Method GET -Path "/api/v1/admin/legacy-roles?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/legacy-roles" -Resp $legacyRoleList

$legacyRoleCreate = Invoke-Api -Method POST -Path "/api/v1/admin/legacy-roles" -Body @{
    name = "LegacyManager $nowSuffix"
    description = "Legacy role for testing"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/legacy-roles" -Resp $legacyRoleCreate
$legacyRoleId = $legacyRoleCreate.json.data.roleId

if ($legacyRoleId) {
    $legacyRoleGet = Invoke-Api -Method GET -Path "/api/v1/admin/legacy-roles/$legacyRoleId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/legacy-roles/{id}" -Resp $legacyRoleGet

    $legacyRoleUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/legacy-roles/$legacyRoleId" -Body @{
        roleId = $legacyRoleId
        name = "LegacyManager Updated $nowSuffix"
        description = "Updated legacy role"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/legacy-roles/{id}" -Resp $legacyRoleUpdate
}

$legacyRoleTemp = Invoke-Api -Method POST -Path "/api/v1/admin/legacy-roles" -Body @{
    name = "LegacyTemp $nowSuffix"
    description = "Temp legacy role"
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/legacy-roles (temp)" -Resp $legacyRoleTemp
$legacyRoleTempId = $legacyRoleTemp.json.data.roleId

$legacyAccountList = Invoke-Api -Method GET -Path "/api/v1/admin/legacy-accounts?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/legacy-accounts" -Resp $legacyAccountList

$legacyAccountCreate = Invoke-Api -Method POST -Path "/api/v1/admin/legacy-accounts" -Body @{
    username = "legacy_user_$nowSuffix"
    password = "Legacy@12345"
    roleId = $legacyRoleId
    status = "Active"
    employeeId = $employeeId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/legacy-accounts" -Resp $legacyAccountCreate
$legacyAccountId = $legacyAccountCreate.json.data.accountId

if ($legacyAccountId) {
    $legacyAccountGet = Invoke-Api -Method GET -Path "/api/v1/admin/legacy-accounts/$legacyAccountId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/legacy-accounts/{id}" -Resp $legacyAccountGet

    $legacyAccountUpdate = Invoke-Api -Method PUT -Path "/api/v1/admin/legacy-accounts/$legacyAccountId" -Body @{
        accountId = $legacyAccountId
        username = "legacy_user_updated_$nowSuffix"
        password = "Legacy@12345"
        roleId = $legacyRoleId
        status = "Active"
        employeeId = $employeeId
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/admin/legacy-accounts/{id}" -Resp $legacyAccountUpdate
}

$legacyAccountTemp = Invoke-Api -Method POST -Path "/api/v1/admin/legacy-accounts" -Body @{
    username = "legacy_temp_$nowSuffix"
    password = "Legacy@12345"
    roleId = $legacyRoleTempId
    status = "Active"
    employeeId = $employeeId
} -Token $adminToken
Add-Result -Name "POST /api/v1/admin/legacy-accounts (temp)" -Resp $legacyAccountTemp
$legacyAccountTempId = $legacyAccountTemp.json.data.accountId

if ($legacyAccountTempId) {
    $legacyAccountDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/legacy-accounts/$legacyAccountTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/legacy-accounts/{id}" -Resp $legacyAccountDelete
}

if ($legacyRoleTempId) {
    $legacyRoleDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/legacy-roles/$legacyRoleTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/legacy-roles/{id}" -Resp $legacyRoleDelete
}

Write-Host "Registering a customer via Auth API..."
$customerEmail = "customer_$nowSuffix@example.com"
$registerResp = Invoke-Api -Method POST -Path "/api/v1/auth/register" -Body @{
    username = $customerEmail
    password = "Customer@12345"
    confirmPassword = "Customer@12345"
    fullName = "Pham Thi Lan"
    phoneNumber = New-Phone
    address = "Quan 5, HCM"
} -Token $null
Add-Result -Name "POST /api/v1/auth/register" -Resp $registerResp

$loginResp = Invoke-Api -Method POST -Path "/api/v1/auth/login" -Body @{
    username = $customerEmail
    password = "Customer@12345"
} -Token $null
Add-Result -Name "POST /api/v1/auth/login" -Resp $loginResp
$customerToken = $loginResp.json.data.token

if ($customerToken) {
    $meResp = Invoke-Api -Method GET -Path "/api/v1/auth/me" -Token $customerToken
    Add-Result -Name "GET /api/v1/auth/me" -Resp $meResp

    $logoutResp = Invoke-Api -Method POST -Path "/api/v1/auth/logout" -Token $customerToken
    Add-Result -Name "POST /api/v1/auth/logout" -Resp $logoutResp
} else {
    Add-Skipped -Name "GET /api/v1/auth/me" -Note "Missing customer token"
    Add-Skipped -Name "POST /api/v1/auth/logout" -Note "Missing customer token"
}

Write-Host "Cart and checkout..."
if ($customerToken -and $productId -and $storeId) {
    $cartGet = Invoke-Api -Method GET -Path "/api/v1/cart" -Token $customerToken
    Add-Result -Name "GET /api/v1/cart" -Resp $cartGet

    $cartAdd = Invoke-Api -Method POST -Path "/api/v1/cart/items" -Body @{
        productId = $productId
        quantity = 2
    } -Token $customerToken
    Add-Result -Name "POST /api/v1/cart/items" -Resp $cartAdd

    $cartUpdate = Invoke-Api -Method PUT -Path "/api/v1/cart" -Body @{
        updates = @(
            @{ productId = $productId; quantity = 1 }
        )
    } -Token $customerToken
    Add-Result -Name "PUT /api/v1/cart" -Resp $cartUpdate

    $cartCount = Invoke-Api -Method GET -Path "/api/v1/cart/count" -Token $customerToken
    Add-Result -Name "GET /api/v1/cart/count" -Resp $cartCount

    $cartRemove = Invoke-Api -Method DELETE -Path "/api/v1/cart/items/$productId" -Token $customerToken
    Add-Result -Name "DELETE /api/v1/cart/items/{productId}" -Resp $cartRemove

    $cartAddAgain = Invoke-Api -Method POST -Path "/api/v1/cart/items" -Body @{
        productId = $productId
        quantity = 1
    } -Token $customerToken
    Add-Result -Name "POST /api/v1/cart/items (again)" -Resp $cartAddAgain

    $checkout = Invoke-Api -Method POST -Path "/api/v1/cart/checkout" -Body @{
        customerName = "Pham Thi Lan"
        phoneNumber = New-Phone
        address = "Quan 5, HCM"
        coffeeShopId = $storeId
        paymentMethod = "Cash"
    } -Token $customerToken
    Add-Result -Name "POST /api/v1/cart/checkout" -Resp $checkout
    $orderId = $checkout.json.data.orderId
} else {
    Add-Skipped -Name "Cart operations" -Note "Missing customer token, product id, or store id"
}

Write-Host "Orders (public API)..."
if ($customerToken) {
    $ordersList = Invoke-Api -Method GET -Path "/api/v1/orders?page=1&pageSize=5" -Token $customerToken
    Add-Result -Name "GET /api/v1/orders" -Resp $ordersList
} else {
    Add-Skipped -Name "GET /api/v1/orders" -Note "Missing customer token"
}

if ($customerToken -and $orderId) {
    $orderGet = Invoke-Api -Method GET -Path "/api/v1/orders/$orderId" -Token $customerToken
    Add-Result -Name "GET /api/v1/orders/{id}" -Resp $orderGet

    if ($productId2) {
        $orderItemAdd = Invoke-Api -Method POST -Path "/api/v1/orders/$orderId/items" -Body @{
            orderId = $orderId
            productId = $productId2
            quantity = 1
        } -Token $customerToken
        Add-Result -Name "POST /api/v1/orders/{id}/items" -Resp $orderItemAdd

        $orderItemRemove = Invoke-Api -Method DELETE -Path "/api/v1/orders/$orderId/items/$productId2" -Token $customerToken
        Add-Result -Name "DELETE /api/v1/orders/{id}/items/{productId}" -Resp $orderItemRemove
    }

    $orderStatusUpdate = Invoke-Api -Method PATCH -Path "/api/v1/orders/$orderId/status" -Body @{
        orderId = $orderId
        status = "Processing"
    } -Token $adminToken
    Add-Result -Name "PATCH /api/v1/orders/{id}/status" -Resp $orderStatusUpdate
}

$directOrder = $null
if ($customerToken -and $productId -and $storeId) {
    $directOrder = Invoke-Api -Method POST -Path "/api/v1/orders" -Body @{
        coffeeShopId = $storeId
        customerId = 0
        paymentMethod = "Cash"
        items = @(
            @{ productId = $productId; productName = "Latte"; quantity = 1; unitPrice = 45000 }
        )
    } -Token $customerToken
    Add-Result -Name "POST /api/v1/orders" -Resp $directOrder
}

$directOrderId = $directOrder.json.data.id
if ($customerToken -and $directOrderId) {
    $orderCancel = Invoke-Api -Method DELETE -Path "/api/v1/orders/$directOrderId" -Token $customerToken
    Add-Result -Name "DELETE /api/v1/orders/{id}" -Resp $orderCancel
}

Write-Host "Admin sales orders..."
$adminSalesList = Invoke-Api -Method GET -Path "/api/v1/admin/sales-orders?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/sales-orders" -Resp $adminSalesList

if ($orderId) {
    $adminSalesDetail = Invoke-Api -Method GET -Path "/api/v1/admin/sales-orders/$orderId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/sales-orders/{id}" -Resp $adminSalesDetail

    $adminSalesUpdate = Invoke-Api -Method PATCH -Path "/api/v1/admin/sales-orders/$orderId/status" -Body @{
        orderId = $orderId
        status = "Completed"
        employeeId = $employeeId
    } -Token $adminToken
    Add-Result -Name "PATCH /api/v1/admin/sales-orders/{id}/status" -Resp $adminSalesUpdate
} else {
    Add-Skipped -Name "GET /api/v1/admin/sales-orders/{id}" -Note "Missing order id"
    Add-Skipped -Name "PATCH /api/v1/admin/sales-orders/{id}/status" -Note "Missing order id"
}

Write-Host "Admin reports..."
$financeSummary = Invoke-Api -Method GET -Path "/api/v1/admin/reports/finance" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/reports/finance" -Resp $financeSummary

if ($storeId) {
    $financeDetail = Invoke-Api -Method GET -Path "/api/v1/admin/reports/finance/$storeId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/reports/finance/{storeId}" -Resp $financeDetail
} else {
    Add-Skipped -Name "GET /api/v1/admin/reports/finance/{storeId}" -Note "Missing store id"
}

Write-Host "Customers (public API)..."
$publicCustomersList = Invoke-Api -Method GET -Path "/api/v1/customers?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/customers" -Resp $publicCustomersList

$publicCustomer = Invoke-Api -Method POST -Path "/api/v1/customers" -Body @{
    name = "Tran Van B"
    phoneNumber = New-Phone
    address = "Quan 10, HCM"
} -Token $adminToken
Add-Result -Name "POST /api/v1/customers" -Resp $publicCustomer
$publicCustomerId = $publicCustomer.json.data.id

if ($publicCustomerId) {
    $publicCustomerGet = Invoke-Api -Method GET -Path "/api/v1/customers/$publicCustomerId" -Token $adminToken
    Add-Result -Name "GET /api/v1/customers/{id}" -Resp $publicCustomerGet

    $publicCustomerUpdate = Invoke-Api -Method PUT -Path "/api/v1/customers/$publicCustomerId" -Body @{
        id = $publicCustomerId
        name = "Tran Van B Updated"
        phoneNumber = New-Phone
        address = "Quan 10, HCM (Updated)"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/customers/{id}" -Resp $publicCustomerUpdate

    $publicCustomerDelete = Invoke-Api -Method DELETE -Path "/api/v1/customers/$publicCustomerId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/customers/{id}" -Resp $publicCustomerDelete
}

if ($customerAdminId -and $productId -and $storeId -and $employeeId) {
    $adminOrder = Invoke-Api -Method POST -Path "/api/v1/orders" -Body @{
        coffeeShopId = $storeId
        customerId = $customerAdminId
        paymentMethod = "Cash"
        employeeId = $employeeId
        items = @(
            @{ productId = $productId; productName = "Latte"; quantity = 1; unitPrice = 45000 }
        )
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/orders (admin customer)" -Resp $adminOrder

    $customerOrders = Invoke-Api -Method GET -Path "/api/v1/customers/$customerAdminId/orders?page=1&pageSize=5" -Token $adminToken
    Add-Result -Name "GET /api/v1/customers/{id}/orders" -Resp $customerOrders
}

Write-Host "Categories & products (public API)..."
$publicCategories = Invoke-Api -Method GET -Path "/api/v1/categories?page=1&pageSize=5" -Token $null
Add-Result -Name "GET /api/v1/categories" -Resp $publicCategories

if ($categoryId) {
    $publicCategory = Invoke-Api -Method GET -Path "/api/v1/categories/$categoryId" -Token $null
    Add-Result -Name "GET /api/v1/categories/{id}" -Resp $publicCategory

    $publicCategoryProducts = Invoke-Api -Method GET -Path "/api/v1/categories/$categoryId/products?page=1&pageSize=5" -Token $null
    Add-Result -Name "GET /api/v1/categories/{id}/products" -Resp $publicCategoryProducts
}

$publicCategoryCreate = Invoke-Api -Method POST -Path "/api/v1/categories" -Body @{
    name = "Public Category $nowSuffix"
    description = "Category created via public API"
} -Token $adminToken
Add-Result -Name "POST /api/v1/categories" -Resp $publicCategoryCreate
$publicCategoryId = $publicCategoryCreate.json.data.id

if ($publicCategoryId) {
    $publicCategoryUpdate = Invoke-Api -Method PUT -Path "/api/v1/categories/$publicCategoryId" -Body @{
        id = $publicCategoryId
        name = "Public Category Updated $nowSuffix"
        description = "Updated via public API"
    } -Token $adminToken
    Add-Result -Name "PUT /api/v1/categories/{id}" -Resp $publicCategoryUpdate

    $publicCategoryDelete = Invoke-Api -Method DELETE -Path "/api/v1/categories/$publicCategoryId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/categories/{id}" -Resp $publicCategoryDelete
}

$publicProducts = Invoke-Api -Method GET -Path "/api/v1/products?page=1&pageSize=5" -Token $null
Add-Result -Name "GET /api/v1/products" -Resp $publicProducts

if ($productId) {
    $publicProduct = Invoke-Api -Method GET -Path "/api/v1/products/$productId" -Token $null
    Add-Result -Name "GET /api/v1/products/{id}" -Resp $publicProduct
}

if ($categoryId) {
    $publicProductCreate = Invoke-Api -Method POST -Path "/api/v1/products" -Body @{
        name = "Public Product $nowSuffix"
        price = 25000
        description = "Product created via public API"
        notes = "Test"
        categoryId = $categoryId
    } -Token $adminToken
    Add-Result -Name "POST /api/v1/products" -Resp $publicProductCreate
    $publicProductId = $publicProductCreate.json.data.id

    if ($publicProductId) {
        $publicProductUpdate = Invoke-Api -Method PUT -Path "/api/v1/products/$publicProductId" -Body @{
            id = $publicProductId
            name = "Public Product Updated $nowSuffix"
            price = 26000
            description = "Updated via public API"
            notes = "Test updated"
            categoryId = $categoryId
        } -Token $adminToken
        Add-Result -Name "PUT /api/v1/products/{id}" -Resp $publicProductUpdate

        $publicProductDelete = Invoke-Api -Method DELETE -Path "/api/v1/products/$publicProductId" -Token $adminToken
        Add-Result -Name "DELETE /api/v1/products/{id}" -Resp $publicProductDelete
    }
} else {
    Add-Skipped -Name "POST /api/v1/products" -Note "Missing category id"
}

Write-Host "Public news..."
$publicNews = Invoke-Api -Method GET -Path "/api/v1/news?page=1&pageSize=5" -Token $null
Add-Result -Name "GET /api/v1/news" -Resp $publicNews

$publicNewsById = Invoke-Api -Method GET -Path "/api/v1/news/$newsId" -Token $null
Add-Result -Name "GET /api/v1/news/{id}" -Resp $publicNewsById

Write-Host "Admin refresh tokens..."
$refreshTokens = Invoke-Api -Method GET -Path "/api/v1/admin/refresh-tokens?page=1&pageSize=5" -Token $adminToken
Add-Result -Name "GET /api/v1/admin/refresh-tokens" -Resp $refreshTokens

$refreshId = $null
if ($refreshTokens.ok -and $refreshTokens.json -and $refreshTokens.json.data -and $refreshTokens.json.data.items -and $refreshTokens.json.data.items.Count -gt 0) {
    $refreshId = $refreshTokens.json.data.items[0].id
}

if ($refreshId) {
    $refreshGet = Invoke-Api -Method GET -Path "/api/v1/admin/refresh-tokens/$refreshId" -Token $adminToken
    Add-Result -Name "GET /api/v1/admin/refresh-tokens/{id}" -Resp $refreshGet

    $refreshRevoke = Invoke-Api -Method PATCH -Path "/api/v1/admin/refresh-tokens/$refreshId/revoke" -Token $adminToken
    Add-Result -Name "PATCH /api/v1/admin/refresh-tokens/{id}/revoke" -Resp $refreshRevoke

    $refreshDelete = Invoke-Api -Method DELETE -Path "/api/v1/admin/refresh-tokens/$refreshId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/refresh-tokens/{id}" -Resp $refreshDelete
} else {
    Add-Skipped -Name "GET /api/v1/admin/refresh-tokens/{id}" -Note "No refresh tokens found"
    Add-Skipped -Name "PATCH /api/v1/admin/refresh-tokens/{id}/revoke" -Note "No refresh tokens found"
    Add-Skipped -Name "DELETE /api/v1/admin/refresh-tokens/{id}" -Note "No refresh tokens found"
}

Write-Host "Cleanup temp records (delete endpoints)..."
if ($categoryTempId) {
    $deleteTempCategory = Invoke-Api -Method DELETE -Path "/api/v1/admin/product-categories/$categoryTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/product-categories/{id}" -Resp $deleteTempCategory
}

if ($supplierTempId) {
    $deleteTempSupplier = Invoke-Api -Method DELETE -Path "/api/v1/admin/suppliers/$supplierTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/suppliers/{id}" -Resp $deleteTempSupplier
}

if ($ingredientTempId) {
    $deleteTempIngredient = Invoke-Api -Method DELETE -Path "/api/v1/admin/ingredients/$ingredientTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/ingredients/{id}" -Resp $deleteTempIngredient
}

if ($productTempId) {
    $deleteTempProduct = Invoke-Api -Method DELETE -Path "/api/v1/admin/products/$productTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/products/{id}" -Resp $deleteTempProduct
}

if ($employeeTempId) {
    $deleteTempEmployee = Invoke-Api -Method DELETE -Path "/api/v1/admin/employees/$employeeTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/employees/{id}" -Resp $deleteTempEmployee
}

if ($customerTempId) {
    $deleteTempCustomer = Invoke-Api -Method DELETE -Path "/api/v1/admin/customers/$customerTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/customers/{id}" -Resp $deleteTempCustomer
}

if ($storeTempId) {
    $deleteTempStore = Invoke-Api -Method DELETE -Path "/api/v1/admin/cafe-stores/$storeTempId" -Token $adminToken
    Add-Result -Name "DELETE /api/v1/admin/cafe-stores/{id}" -Resp $deleteTempStore
}

Write-Host "`n=== Full API Test Summary ==="
$Results | Format-Table -AutoSize
$failed = $Results | Where-Object { -not $_.Ok }
if ($failed.Count -gt 0) {
    Write-Host "`nFailed steps:"
    $failed | Format-Table -AutoSize
}
