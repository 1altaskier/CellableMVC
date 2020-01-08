SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE GetOrderDetails
	@UserId int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT OrderID
      ,Amount
	  ,ph.Brand + ' ' + pv.Version AS Phone
	  ,os.StatusType
      ,p.PromoCode
	  ,p.PromoName
	  ,p.Discount
	  ,pt.PaymentType
      ,PaymentUserName
	  ,o.CreateDate
FROM dbo.Orders o
	LEFT JOIN UserPhone up ON up.UserId = o.UserId
	LEFT JOIN PhoneVersion pv on pv.VersionId = up.VersionId
	LEFT JOIN OrderStatus os on os.OrderStatusId = o.OrderStatusId
	LEFT JOIN PaymentTypes pt on pt.PaymentTypeId = o.PaymentTypeId
	LEFT JOIN Promos p ON p.PromoId = o.PromoId
	LEFT JOIN Phones ph ON ph.PhoneId = pv.PhoneId
WHERE o.UserId = @UserId
	AND up.UserPhoneId = o.UserPhoneId

END
GO
