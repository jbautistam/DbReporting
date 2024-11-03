WITH
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
						   [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl], 
						   TransferReports.TransferType, TransferReports.ActualStockOriginStore,
						   TransferReports.IdealStockOriginStore, TransferReports.ActualStockDestinationStore,
						   TransferReports.IdealStockDestinationStore, TransferReports.CostPVM, 
						   TransferReports.TotalTransfersOut, TransferReports.DaysInStore,
						   TransferReports.Sales, TransferReports.Margin
					FROM Fact.TransferReports
					
					 INNER JOIN  PointsOfSaleTargetCte
															ON 
 [TransferReports].[DestinationPointOfSaleId] = [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSaleId]
		            
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
SELECT [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], 
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
					 GROUP BY [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], TransferType
)
SELECT [GroupedCte].[PointOfSaleTargetPointOfSale], [GroupedCte].[PointOfSaleTargetErpCode], [GroupedCte].[PointOfSaleTargetImageUrl], 
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
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY