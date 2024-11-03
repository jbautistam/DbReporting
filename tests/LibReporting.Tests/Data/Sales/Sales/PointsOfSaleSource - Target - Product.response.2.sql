WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
PointsOfSaleSourceCte AS 
(
SELECT [PointOfSaleSourcePointsOfSale].[Id] AS [PointOfSaleSourcePointOfSaleId], [PointOfSaleSourcePointsOfSale].[Name] AS [PointOfSaleSourcePointOfSale], 
		[PointOfSaleSourcePointsOfSale].[ErpCode] AS [PointOfSaleSourceErpCode], [PointOfSaleSourcePointsOfSale].[ImageUrl] AS [PointOfSaleSourceImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleSourcePointsOfSale]


),
PointsOfSaleTargetCte AS 
(
SELECT [PointOfSaleTargetPointsOfSale].[Id] AS [PointOfSaleTargetPointOfSaleId], [PointOfSaleTargetPointsOfSale].[Name] AS [PointOfSaleTargetPointOfSale], 
		[PointOfSaleTargetPointsOfSale].[ErpCode] AS [PointOfSaleTargetErpCode], [PointOfSaleTargetPointsOfSale].[ImageUrl] AS [PointOfSaleTargetImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleTargetPointsOfSale]


),
FilteredTransferDataCte AS 
(
SELECT TransferReports.OriginPointOfSaleId, TransferReports.DestinationPointOfSaleId,
						   TransferReports.ProductId,
						   [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl], [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 
						   TransferReports.TransferType, TransferReports.ActualStockOriginStore,
						   TransferReports.IdealStockOriginStore, TransferReports.ActualStockDestinationStore,
						   TransferReports.IdealStockDestinationStore, TransferReports.CostPVM, 
						   TransferReports.TotalTransfersOut, TransferReports.DaysInStore,
						   TransferReports.Sales, TransferReports.Margin
					FROM Fact.TransferReports
					 INNER JOIN  PointsOfSaleSourceCte
															ON 
 [TransferReports].[OriginPointOfSaleId] = [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSaleId]
					 INNER JOIN  PointsOfSaleTargetCte
															ON 
 [TransferReports].[DestinationPointOfSaleId] = [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSaleId]
		             INNER JOIN  ProductsCte
															ON 
 [TransferReports].[ProductId] = [ProductsCte].[ProductId]
),
StockOriginCte AS 
(
SELECT SUM(ActualStockOriginStore) AS TotalActualStockOriginStore,
							   SUM(IdealStockOriginStore) AS TotalIdealStockOriginStore
							FROM (SELECT OriginPointOfSaleId, ProductId, ActualStockOriginStore, IdealStockOriginStore
									FROM FilteredTransferDataCte
									GROUP BY OriginPointOfSaleId, ProductId, ActualStockOriginStore, IdealStockOriginStore
								) AS Grouped
),
StockDestinationCte AS 
(
SELECT SUM(ActualStockDestinationStore) AS TotalActualStockDestinationStore,
						   SUM(IdealStockDestinationStore) AS TotalIdealStockDestinationStore
						FROM (SELECT DestinationPointOfSaleId, ProductId, 
									 ActualStockDestinationStore, IdealStockDestinationStore
								FROM FilteredTransferDataCte
								GROUP BY DestinationPointOfSaleId, ProductId, 
										 ActualStockDestinationStore, IdealStockDestinationStore
							 ) AS Grouped
),
GroupedCte AS 
(
SELECT [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], [ProductCode], [ProductDescription], [UrlImage], 
						   TransferType,
						   MAX(ActualStockOriginStore) AS ActualStockOriginStore, 
 MAX(IdealStockOriginStore) AS IdealStockOriginStore, 
 MAX(ActualStockDestinationStore) AS ActualStockDestinationStore, 
 MAX(IdealStockDestinationStore) AS IdealStockDestinationStore, 
 SUM(CostPVM) AS CostPVM, 
 SUM(TotalTransfersOut) AS TotalTransfersOut, 
 MIN(DaysInStore) AS DaysInStore, 
 SUM(Sales) AS Sales, 
 SUM(Margin) AS Margin, 
 IsNull((SUM(Margin) * 100) / NullIf(SUM(Sales), 0), 0) AS MarginPercentage
                    FROM FilteredTransferDataCte
					 GROUP BY [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], [ProductCode], [ProductDescription], [UrlImage], TransferType
)
SELECT [GroupedCte].[PointOfSaleSourcePointOfSale], [GroupedCte].[PointOfSaleSourceErpCode], [GroupedCte].[PointOfSaleSourceImageUrl], [GroupedCte].[PointOfSaleTargetPointOfSale], [GroupedCte].[PointOfSaleTargetErpCode], [GroupedCte].[PointOfSaleTargetImageUrl], [GroupedCte].[ProductCode], [GroupedCte].[ProductDescription], [GroupedCte].[UrlImage], 
						   GroupedCte.TransferType,
						   GroupedCte.ActualStockOriginStore, 
 GroupedCte.IdealStockOriginStore, 
 GroupedCte.ActualStockDestinationStore, 
 GroupedCte.IdealStockDestinationStore, 
 GroupedCte.CostPVM, 
 GroupedCte.TotalTransfersOut, 
 GroupedCte.DaysInStore, 
 GroupedCte.Sales, 
 GroupedCte.Margin, 
 GroupedCte.MarginPercentage
                    FROM GroupedCte
					
					ORDER BY GroupedCte.TransferType
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY