WITH
ProductsCte AS 
(
SELECT [Products].[ProductId] AS [ProductId], [Products].[Code] AS [ProductCode]
	FROM [Dim].[Products] AS [Products]


),
CalendarCte AS 
(
SELECT [Calendar].[CalendarId] AS [CalendarId]
	FROM [Dim].[Calendar] AS [Calendar]


 WHERE [Date] BETWEEN @StartDate AND @EndDate

)
SELECT [ProductsCte].[ProductCode], 
	                		SUM(Sales.Quantity) AS Quantity, SUM(Sales.SellingPrice) AS SellingPrice, 
	                		SUM(Sales.PurchasePrice) AS PurchasePrice, SUM(Sales.Taxes) AS Taxes
	                	FROM Fact.Sales
	                	
	                	 INNER JOIN  ProductsCte
															ON 
 [Sales].[ProductId] = [ProductsCte].[ProductId]
	                	
						 GROUP BY [ProductsCte].[ProductCode]