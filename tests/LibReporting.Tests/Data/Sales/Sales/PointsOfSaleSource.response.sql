WITH
PointsOfSaleSourceCte AS 
(
SELECT [PointOfSaleSourcePointsOfSale].[Id] AS [PointOfSaleSourcePointOfSaleId], [PointOfSaleSourcePointsOfSale].[Name] AS [PointOfSaleSourcePointOfSale], 
		[PointOfSaleSourcePointsOfSale].[ErpCode] AS [PointOfSaleSourceErpCode], [PointOfSaleSourcePointsOfSale].[ImageUrl] AS [PointOfSaleSourceImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleSourcePointsOfSale]


),
FilteredTransferDataCte AS 
(
SELECT TransferReports.OriginPointOfSaleId, TransferReports.DestinationPointOfSaleId,
						   TransferReports.ProductId,
						   [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl], 
						   TransferReports.TransferType, TransferReports.ActualStockOriginStore,
						   TransferReports.IdealStockOriginStore, TransferReports.ActualStockDestinationStore,
						   TransferReports.IdealStockDestinationStore, TransferReports.CostPVM, 
						   TransferReports.TotalTransfersOut, TransferReports.DaysInStore,
						   TransferReports.Sales, TransferReports.Margin
					FROM Fact.TransferReports
					 INNER JOIN  PointsOfSaleSourceCte
															ON 
 [TransferReports].[OriginPointOfSaleId] = [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSaleId]
					
		            
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
SELECT [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], 
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
					 GROUP BY [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], TransferType
)
SELECT [GroupedCte].[PointOfSaleSourcePointOfSale], [GroupedCte].[PointOfSaleSourceErpCode], [GroupedCte].[PointOfSaleSourceImageUrl], 
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
 GroupedCte.MarginPercentage,
 COUNT(*) OVER () AS TotalCount, 
 StockOriginCte.TotalActualStockOriginStore, 
 StockOriginCte.TotalIdealStockOriginStore, 
 StockDestinationCte.TotalActualStockDestinationStore, 
 StockDestinationCte.TotalIdealStockDestinationStore, 
 SUM(GroupedCte.CostPVM) OVER() AS TotalCostPVM, 
 SUM(GroupedCte.TotalTransfersOut) OVER () AS TotalTotalTransfersOut, 
 SUM(GroupedCte.DaysInStore) OVER() AS TotalDaysInStore, 
 SUM(GroupedCte.Sales) OVER() AS TotalSales, 
 SUM(GroupedCte.Margin) OVER() AS TotalMargin, 
 SUM(GroupedCte.MarginPercentage) OVER() AS TotalMarginPercentage
                    FROM GroupedCte
					CROSS JOIN StockOriginCte
 CROSS JOIN StockDestinationCte
					ORDER BY GroupedCte.TransferType
					