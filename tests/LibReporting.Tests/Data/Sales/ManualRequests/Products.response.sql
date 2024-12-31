WITH ProductsCte AS 
		(SELECT [Products].[ProductId] AS [ProductId], [Products].[Code] AS [ProductCode], [ProductRoots].[Code] AS [RootCode]
			FROM [Dim].[Products] AS [Products] INNER JOIN [Dim].[ProductRoots] AS [ProductRoots]
				ON [Products].[ProductRootId] = [ProductRoots].[ProductRootId]
		)
	SELECT [ProductsCte].[ProductCode], [ProductsCte].[ProductCode], [ProductsCte].[RootCode]
		FROM ProductsCte