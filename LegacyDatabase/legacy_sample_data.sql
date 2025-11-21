-- ============================================
-- SAMPLE DATA - The Messy Reality
-- ============================================

INSERT INTO DEPT_LKP_TBL (DEPT_CD, DEPT_NM, DEPT_MGR_EMP, BUDGET_AMT, ACTIVE_YN) VALUES
('IT', 'Information Technology', '1001', '$500,000.00', 'Y'),
('HR', 'Human Resources', '1005', '$250,000', 'Y'),          -- Inconsistent format
('FIN', 'Finance', '1010', '350000.00', 'Y'),                 -- No $ sign
('MKT', 'Marketing', '1015', '$400,000.00', 'Y'),
('OPS', 'Operations', '1020', 'N/A', 'N'),                    -- Inactive dept with bad data
('SALES', 'Sales Department', '', '$600,000.00', 'Y');        -- Empty manager

INSERT INTO EMPLOYEE_MASTER_TBL (EMP_NUM, FULL_NM, DEPT_CD, HIRE_DT, SAL_AMT, STATUS_FLG, MGR_EMP_NUM, EMAIL_ADDR, PHONE_NBR, CREATED_BY, CREATED_DT) VALUES
('1001', 'Smith, John A.', 'IT', '01/15/2003', '$95,000.00', 'A', NULL, 'john.smith@legacycorp.com', '555-0101', 'SYSTEM', '01/15/2003'),
('1002', 'JOHNSON,SARAH', 'IT', '03/22/2005', '85000', 'A', '1001', 'sarah.johnson@legacycorp.com', '(555) 0102', 'ADMIN', '3/22/2005'),  -- Inconsistent formats
('1003', 'Williams, Robert', 'IT', '2008-06-10', '$78,500.00', 'A', '1001', 'bob.williams@legacycorp.com', '555.0103', 'SYSTEM', '06/10/2008'),  -- Different date format!
('1004', 'Brown,Emily Jane', 'HR', '12/01/2010', '72,000.00', 'A', '1005', 'emily.brown@legacycorp.com', '5550104', 'ADMIN', '12/01/2010'),  -- No $
('1005', 'Davis, Michael T.', 'HR', '05/15/2002', '$88,000.00', 'A', NULL, 'michael.davis@legacycorp.com', '555-0105', 'SYSTEM', '05/15/2002'),
('1006', 'Miller,Jennifer', 'FIN', '09/30/2007', '$92,500.00', 'I', '1010', 'jennifer.miller@legacycorp.com', '', 'SYSTEM', '9/30/2007'),  -- Inactive, no phone
('1007', 'Wilson, James Robert', 'IT', '11/20/2011', '81000.00', 'A', '1001', 'james.wilson@legacycorp.com', '555-0107', 'ADMIN', '11/20/2011'),
('1008', 'Moore,Patricia', 'MKT', '02/14/2006', '$0.00', 'T', '1015', 'patricia.moore@legacycorp.com', '555-0108', 'SYSTEM', '02/14/2006'),  -- Terminated, $0 salary
('1009', 'Taylor,Christopher', 'SALES', '07/01/2009', '$105,000.00', 'A', '1020', 'chris.taylor@legacycorp.com', '555-0109', 'ADMIN', '7/1/2009'),
('1010', 'Anderson, Linda Sue', 'FIN', '04/10/2004', '$98,000.00', 'A', NULL, 'linda.anderson@legacycorp.com', '555-0110', 'SYSTEM', '04/10/2004'),
('EMP-1011', 'Thomas,David', 'IT', 'Invalid Date', '75,000', 'A', '1001', 'david.thomas@legacycorp.com', '555-0111', 'TEMP', 'N/A'),  -- Bad data!
('1012', 'Jackson, Mary', 'HR', '08/25/2012', '$68,500.00', 'A', '1005', 'mary.jackson@legacycorp.com', '555-0112', 'SYSTEM', '08/25/2012'),
('1013', '', 'IT', '01/01/2000', '$0.00', 'T', '1001', '', '', 'UNKNOWN', ''),  -- Minimal/corrupt data
('1015', 'Martinez, Carlos', 'MKT', '10/05/2008', '$91,000.00', 'A', NULL, 'carlos.martinez@legacycorp.com', '555-0115', 'SYSTEM', '10/05/2008'),
('1020', 'Garcia,Lisa Marie', 'OPS', '06/18/2005', '$87,000.00', 'I', NULL, 'lisa.garcia@legacycorp.com', '555-0120', 'ADMIN', '6/18/2005');

INSERT INTO PROJ_ASSIGN_TBL (ASSIGN_ID, EMP_NUM, PROJ_CD, START_DT, END_DT, HOURS_PER_WK, BILL_RATE, NOTES) VALUES
('A001', '1001', 'PROJ-IT-001', '01/01/2023', '12/31/2023', '40', '$150.00/hr', 'Lead architect'),
('A002', '1002', 'PROJ-IT-001', '01/01/2023', 'Ongoing', '35', '125', 'Senior developer'),  -- Mixed formats
('A003', '1003', 'PROJ-IT-002', '2023-03-15', '2023-09-30', '40', '$120.00/hr', 'Database specialist'),
('A004', '1007', 'PROJ-IT-001', '06/01/2023', '', '20', '110', 'Part-time developer'),  -- No end date
('A005', '1009', 'PROJ-SALES-001', '01/01/2023', '06/30/2023', 'Full-time', '$200/hour', 'Sales lead'),  -- Inconsistent
('A006', 'INVALID_EMP', 'PROJ-IT-999', '01/01/2020', '01/01/2021', '40', '100', 'Employee no longer exists');  -- Orphaned record!
