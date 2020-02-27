DECLARE @VersionID int;
SELECT @VersionID = 71;

SELECT [AnswerId]
      ,[Answer]
      ,[PossibleDefectId]
      ,[UserPhoneId]
  FROM [dbo].[UserAnswers] 

  delete from Orders where UserPhoneId = 8
  delete from UserAnswers where UserPhoneId = 8

  SELECT [UserPhoneId]
      ,[UserId]
      ,[PhoneId]
      ,[CarrierId]
      ,[VersionId]
      ,[CreateDate]
  FROM [dbo].[UserPhone]

  delete from UserPhone where VersionId = 9

  SELECT [VersionCapacityId]
      ,[VersionId]
      ,[StorageCapacityId]
      ,[Value]
  FROM [dbo].[VersionCapacity]

    delete from [VersionCapacity] where VersionId = @VersionID

  SELECT TOP (1000) [PossibleDefectId]
      ,[DefectName]
      ,[DefectCost]
      ,[VersionId]
      ,[DefectGroupId]
      ,[GroupImage]
  FROM [dbo].[PossibleDefects]

    delete from [PossibleDefects] where VersionId = @VersionID

  SELECT TOP (1000) [VersionId]
      ,[Version]
      ,[BaseCost]
      ,[ImageName]
      ,[PhoneId]
      ,[StorageCapacityId]
      ,[Views]
      ,[Purchases]
      ,[Active]
  FROM [dbo].[PhoneVersion]

      delete from [PhoneVersion] where VersionId = @VersionID
