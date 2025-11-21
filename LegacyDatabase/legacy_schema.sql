-- ============================================
-- LEGACY CORP INC. - EMPLOYEE DATABASE (2005)
-- WARNING: This is intentionally BAD design!
-- ============================================

-- Employee Master Table (The Nightmare)
CREATE TABLE EMPLOYEE_MASTER_TBL (
    EMP_NUM VARCHAR(50) PRIMARY KEY,           -- Should be INT, but stored as string
    FULL_NM VARCHAR(200),                      -- Should be split into first/last
    DEPT_CD VARCHAR(10),                       -- Cryptic department codes
    HIRE_DT VARCHAR(20),                       -- Dates stored as strings! "01/15/2003"
    SAL_AMT VARCHAR(20),                       -- Money stored as string! "$45,000.00"
    STATUS_FLG VARCHAR(1),                     -- 'A' = Active, 'I' = Inactive, 'T' = Terminated
    MGR_EMP_NUM VARCHAR(50),                   -- Manager's employee number
    EMAIL_ADDR VARCHAR(100),
    PHONE_NBR VARCHAR(30),                     -- Inconsistent formats
    CREATED_BY VARCHAR(50),
    CREATED_DT VARCHAR(20),
    MODIFIED_BY VARCHAR(50),
    MODIFIED_DT VARCHAR(20)
);

-- Department Lookup Table (Also messy)
CREATE TABLE DEPT_LKP_TBL (
    DEPT_CD VARCHAR(10) PRIMARY KEY,
    DEPT_NM VARCHAR(100),
    DEPT_MGR_EMP VARCHAR(50),
    BUDGET_AMT VARCHAR(20),                    -- Again, money as string
    ACTIVE_YN VARCHAR(1)                       -- 'Y' or 'N'
);

-- Project Assignment Table (No proper relationships!)
CREATE TABLE PROJ_ASSIGN_TBL (
    ASSIGN_ID VARCHAR(50) PRIMARY KEY,
    EMP_NUM VARCHAR(50),                       -- No foreign key constraint!
    PROJ_CD VARCHAR(20),
    START_DT VARCHAR(20),                      -- String dates again
    END_DT VARCHAR(20),
    HOURS_PER_WK VARCHAR(10),                  -- Numbers as strings
    BILL_RATE VARCHAR(20),                     -- "$150.00/hr" - mixed format
    NOTES TEXT
);
