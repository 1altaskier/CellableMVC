SELECT [AnswerId]
      ,[Answer]
      ,[PossibleDefectId]
      ,[UserPhoneId]
  FROM [dbo].[UserAnswers] 

  delete from Orders where UserPhoneId = 47
  delete from UserAnswers where UserPhoneId = 47

  SELECT [UserPhoneId]
      ,[UserId]
      ,[PhoneId]
      ,[CarrierId]
      ,[VersionId]
      ,[CreateDate]
  FROM [dbo].[UserPhone]

  delete from UserPhone where VersionId = 14

  SELECT [VersionCapacityId]
      ,[VersionId]
      ,[StorageCapacityId]
      ,[Value]
  FROM [dbo].[VersionCapacity]

    delete from [VersionCapacity] where VersionId = 45

  SELECT TOP (1000) [PossibleDefectId]
      ,[DefectName]
      ,[DefectCost]
      ,[VersionId]
      ,[DefectGroupId]
      ,[GroupImage]
  FROM [dbo].[PossibleDefects]

    delete from [PossibleDefects] where VersionId = 45


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

      delete from [PhoneVersion] where VersionId = 45
