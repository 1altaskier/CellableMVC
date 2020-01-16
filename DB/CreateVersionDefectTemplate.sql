SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE CreateVersionDefectTemplate 
	-- Add the parameters for the stored procedure here
	@NewVersionID int,
	@TemplateVersionID int = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [Cellable].[dbo].[PossibleDefects] 
		([DefectName], [DefectCost], [VersionId], [DefectGroupId], [GroupImage])
	SELECT [DefectName], [DefectCost], @NewVersionID, [DefectGroupId], [GroupImage]
	FROM [Cellable].[dbo].[PossibleDefects]
	WHERE VersionId = @TemplateVersionID;
END
GO