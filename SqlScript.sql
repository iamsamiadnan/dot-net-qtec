CREATE TABLE RoleModules (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- New identity column as primary key
    RoleId NVARCHAR(450) NOT NULL, -- Now can be 450 to match AspNetUserRoles.UserId
    ModuleName NVARCHAR(100) NOT NULL,
    -- Add a unique constraint on RoleId and ModuleName to ensure uniqueness of pairs
    CONSTRAINT UQ_RoleModules UNIQUE (RoleId, ModuleName),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE Modules (
    ModuleId INT PRIMARY KEY IDENTITY,
    ModuleName NVARCHAR(100) NOT NULL
);

CREATE TABLE RoleModuleAccess (
    Id INT IDENTITY PRIMARY KEY CLUSTERED,
    RoleId NVARCHAR(450) NOT NULL,
    ModuleId INT NOT NULL,
    UNIQUE NONCLUSTERED (RoleId, ModuleId), -- to enforce uniqueness
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id),
    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
);

CREATE TABLE ChartOfAccounts
(
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    AccountName NVARCHAR(100) NOT NULL,
    AccountType NVARCHAR(50) NOT NULL,       -- e.g., Asset, Liability, Expense, etc.
    ParentAccountId INT NULL,                  -- Self-referencing foreign key for hierarchy

    CONSTRAINT FK_ChartOfAccounts_Parent FOREIGN KEY (ParentAccountId)
        REFERENCES ChartOfAccounts(AccountId)
);



CREATE PROCEDURE sp_ManageChartOfAccounts
	@ACTION NVARCHAR(10),
	@ID INT = NULL,
	@NAME NVARCHAR(100) = NULL,
	@CATEGORY NVARCHAR(50) = NULL,
	@PARENTID INT = NULL
AS 
BEGIN 
	SET NOCOUNT ON;

	IF @ACTION = 'CREATE'
	BEGIN
		INSERT INTO CHARTOFACCOUNTS (NAME, CATEGORY, PARENTID) 
		VALUES (@NAME, @CATEGORY, @PARENTID);
	
		PRINT 'ACCOUNT CREATED';
	END
	
	ELSE IF @ACTION = 'UPDATE'
	BEGIN
		UPDATE CHARTOFACCOUNTS 
		SET 
			NAME = @NAME,
			CATEGORY = @CATEGORY,
			PARENTID = @PARENTID 
		WHERE ID = @ID;
	
		PRINT 'ACCOUNT UPDATED';
	END
	
	ELSE IF @ACTION = 'DELETE' 
	BEGIN 
		DELETE FROM CHARTOFACCOUNTS WHERE ID = @ID;
		
		PRINT 'ACCOUNT DELETED';
	END
	
	ELSE 
	BEGIN
		PRINT 'INVALID ACTION PARAMETER';
	END
	
END



EXEC sp_rename 'ChartOfAccounts.AccountId', 'Id', 'COLUMN';
EXEC sp_rename 'ChartOfAccounts.AccountName', 'Name', 'COLUMN';
EXEC sp_rename 'ChartOfAccounts.AType', 'Category', 'COLUMN';
EXEC sp_rename 'ChartOfAccounts.ParentAccountId', 'ParentId', 'COLUMN';


DBCC CHECKIDENT ('ChartOfAccounts', RESEED, 6);



CREATE TABLE Vouchers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Category NVARCHAR(50),
    Date DATETIME,
    Ref NVARCHAR(100)
);

CREATE TABLE VoucherEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VoucherId INT NOT NULL,
    AccountId INT NOT NULL,
    Debit DECIMAL(18,2) DEFAULT 0,
    Credit DECIMAL(18,2) DEFAULT 0,
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id),
    FOREIGN KEY (AccountId) REFERENCES ChartOfAccounts(Id)
);


CREATE TYPE VoucherEntryType AS TABLE
(
    AccountId INT,
    Debit DECIMAL(18,2),
    Credit DECIMAL(18,2)
);

CREATE PROCEDURE sp_SaveVoucher 
	@CATEGORY NVARCHAR(50),
	@DATE DATETIME, 
	@REF NVARCHAR(100),
	@ENTRIES VoucherEntryType READONLY 
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY 
		BEGIN TRANSACTION;
		INSERT INTO VOUCHERS (CATEGORY, DATE, REF) 
		VALUES (@CATEGORY, @DATE, @REF); 
		
		DECLARE @VOUCHERID INT = SCOPE_IDENTITY();
		
		INSERT INTO VOUCHERENTRIES (VOUCHERID, ACCOUNTID, DEBIT, CREDIT) 
		SELECT @VOUCHERID, ACCOUNTID, DEBIT, CREDIT 
		FROM @ENTRIES;
		
		COMMIT TRANSACTION;
	END TRY
	
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
	
		THROW;
	END CATCH
END

	


SELECT name, create_date, modify_date
FROM sys.procedures
ORDER BY name;

DROP PROCEDURE SP_SAVE_VOUCHER;
