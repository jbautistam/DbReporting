WITH
FilteredTransferDataCte AS 
(
SELECT TransferReports.OriginPointOfSaleId, TransferReports.DestinationPointOfSaleId,
						   TransferReports.ProductId,
						   
						   TransferReports.TransferType, TransferReports.ActualStockOriginStore,
						   TransferReports.IdealStockOriginStore, TransferReports.ActualStockDestinationStore,
						   TransferReports.IdealStockDestinationStore, TransferReports.CostPVM, 
						   TransferReports.TotalTransfersOut, TransferReports.DaysInStore,
						   TransferReports.Sales, TransferReports.Margin
					FROM Fact.TransferReports
					
					
		            
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
SELECT 
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
					 GROUP BY TransferType
)
SELECT 
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