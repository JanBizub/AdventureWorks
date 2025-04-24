-- Get the names of all product categories from the DimProductCategory table in alphabetical order.
select EnglishProductCategoryName
from DimProductCategory
order by EnglishProductCategoryName asc

-- List all currencies (CurrencyName) from the DimCurrency table where the currency code (CurrencyAlternateKey) starts 
-- with the letter 'U'.

select CurrencyName
from DimCurrency
where CurrencyAlternateKey like 'U%'

-- Show the first 10 rows from DimCustomer, displaying:
-- CustomerKey
-- FirstName
-- LastName
-- EmailAddress

select top 10 customerkey, firstname, lastname, emailaddress
from DimCustomer

-- Now, group and aggregate.
-- From the FactInternetSales table, show total SalesAmount for each SalesTerritoryKey. Column output should be:
-- SalesTerritoryKey
-- TotalSalesAmount
-- Sort by TotalSalesAmount descending — biggest territories first.

select SalesTerritoryKey, SUM(SalesAmount) as TotalSalesAmount
from FactInternetSales
group by SalesTerritoryKey
order by TotalSalesAmount desc

-- Get a list of products along with their subcategories and categories.
-- Tables:
-- DimProduct → has ProductSubcategoryKey
-- DimProductSubcategory → join on ProductSubcategoryKey, has ProductCategoryKey
-- DimProductCategory → join on ProductCategoryKey
-- 
-- Your output should be:
-- EnglishProductName
-- EnglishProductSubcategoryName
-- EnglishProductCategoryName
-- Sort alphabetically by EnglishProductName.
-- Let’s see how you handle a multi-level join.

select EnglishProductName, EnglishProductSubcategoryName, EnglishProductCategoryName
from DimProduct
         join dbo.DimProductSubcategory
              on DimProduct.ProductSubcategoryKey = DimProductSubcategory.ProductSubcategoryKey
         join dbo.DimProductCategory DPC on DimProductSubcategory.ProductCategoryKey = DPC.ProductCategoryKey
order by EnglishProductName

-- Get all customers, even if they’ve never made a purchase.
-- Join DimCustomer with FactInternetSales, using CustomerKey. Return:
-- CustomerKey
-- FirstName
-- LastName
-- SalesOrderNumber (nullable if no sale)
-- Sort by CustomerKey.

select DimCustomer.CustomerKey, FirstName, LastName, SalesOrderNumber
from DimCustomer
         left join FactInternetSales
         on DimCustomer.CustomerKey = FactInternetSales.CustomerKey
order by CustomerKey

-- For each product:
-- Join DimProduct + FactInternetSales
-- Show EnglishProductName and TotalSalesAmount
-- Only include products where total sales > 1,000,000
-- Sort TotalSalesAmount descending
-- Let’s see your aggregate filtering game.

select EnglishProductName, SUM(FactInternetSales.SalesAmount) as TotalSalesAmount
from DimProduct
         join FactInternetSales
              on DimProduct.ProductKey = FactInternetSales.ProductKey
group by EnglishProductName
having SUM(FactInternetSales.SalesAmount) > 1000000
order by TotalSalesAmount desc
    
-- List all FactInternetSales orders placed in 2022. Return:
-- SalesOrderNumber
-- OrderDate
-- SalesAmount
-- You're filtering on OrderDate (not the surrogate key).
select SalesOrderNumber, OrderDate, SalesAmount
from FactInternetSales
-- where YEAR(OrderDate) = 2022
where OrderDate >= '2022-01-01' and OrderDate < '2023-01-01'

