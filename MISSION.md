# 🎯 MISSION: Legacy Data Modernization Service

## The Situation

**Legacy Corp Inc.** has been running their employee management system since 2005. The database is a disaster:

### Problems with the Legacy Database:

1. **Wrong Data Types**
   - Employee numbers stored as VARCHAR instead of INT
   - Dates stored as strings in multiple formats ("01/15/2003", "2008-06-10", "3/22/2005")
   - Money stored as strings with inconsistent formatting ("$95,000.00", "85000", "$0.00")
   - Numbers stored as strings ("40", "35", "Full-time")

2. **Poor Naming Conventions**
   - Cryptic abbreviations (EMP_NUM, DEPT_CD, SAL_AMT)
   - FULL_NM instead of separate FirstName/LastName
   - Inconsistent column suffixes (_TBL, _FLG, _DT, _AMT)

3. **Data Quality Issues**
   - Inconsistent date formats in the same column
   - Missing data (empty strings, NULL values)
   - Invalid/corrupt data ("Invalid Date", "N/A")
   - Orphaned records (project assignments for non-existent employees)
   - Duplicate/contradictory information

4. **No Data Integrity**
   - No foreign key constraints
   - No referential integrity
   - Status flags instead of proper booleans ('A', 'I', 'T')
   - No validation

---

## Your Mission

Build a **C# ASP.NET Core API** that:

### Phase 1: Foundation ✅
- [x] Create proper C# entity models with correct data types
- [x] Set up Entity Framework DbContext
- [x] Configure SQLite database with proper schema
- [x] Create database migrations

### Phase 2: Modern API 🎯
- [x] Build REST API controllers with CRUD operations
- [x] Implement proper endpoints (GET, POST, PUT, DELETE)
- [x] Return clean JSON responses
- [x] Handle errors gracefully

### Phase 3: Legacy Integration 🔥
- [ ] Read data from the legacy database
- [ ] Transform messy legacy data into clean modern format
- [ ] Handle data inconsistencies and errors
- [ ] Validate and clean data before storing

### Phase 4: Business Logic 💼
- [ ] Calculate derived fields (e.g., years of service)
- [ ] Implement business rules (e.g., salary ranges, active status)
- [ ] Create data validation rules
- [ ] Handle edge cases

---

## Your Modern Database Design

Design a **clean, normalized** database with proper:

### Employee Table (Your Design!)
Think about:
- Proper data types (int, decimal, DateTime, bool)
- Separate first/last names
- Proper foreign keys
- What fields are required vs optional?
- How to handle status (IsActive boolean instead of 'A'/'I'/'T')?

### Department Table (Your Design!)
Consider:
- How to link to employees
- Budget as proper decimal
- Active status as boolean
- Manager relationship

### ProjectAssignment Table (Your Design!)
Think about:
- Foreign keys to Employee and Project
- Proper date handling
- Billable rate as decimal
- Hours as int/decimal

---

## API Endpoints You Need to Build

```
GET    /api/employees              - List all active employees
GET    /api/employees/{id}         - Get specific employee
POST   /api/employees              - Create new employee
PUT    /api/employees/{id}         - Update employee
DELETE /api/employees/{id}         - Deactivate employee (soft delete)

GET    /api/employees/{id}/projects - Get employee's projects
GET    /api/departments             - List all departments
GET    /api/departments/{code}/employees - Get all employees in department
```

---

## Business Rules to Implement

1. **Employee Creation**
   - Email must be unique
   - Employee code auto-generated (format: EMP-XXXX)
   - Salary must be positive
   - Hire date cannot be in the future
   - Department must exist

2. **Status Management**
   - Active employees can be assigned to projects
   - Inactive employees cannot get new assignments
   - Terminated employees should not appear in active lists

3. **Department Rules**
   - Department manager must be an employee in that department
   - Cannot delete department with active employees
   - Budget must be positive or zero

4. **Project Assignments**
   - Employee must be active
   - Start date must be before or equal to end date
   - Hours per week between 0-40
   - Billable rate must be positive

---

## Data Transformation Challenges

You'll need to handle:

### String to DateTime Conversion
```
"01/15/2003"   → DateTime
"2008-06-10"   → DateTime
"3/22/2005"    → DateTime
"Invalid Date" → null (handle gracefully)
```

### String to Decimal (Money)
```
"$95,000.00"   → 95000.00
"85000"        → 85000.00
"72,000.00"    → 72000.00
"$0.00"        → 0.00
"N/A"          → null or default
```

### Name Parsing
```
"Smith, John A."        → First: "John", Last: "Smith"
"JOHNSON,SARAH"         → First: "Sarah", Last: "Johnson"
"Williams, Robert"      → First: "Robert", Last: "Williams"
"Brown,Emily Jane"      → First: "Emily", Last: "Brown"
```

### Status Conversion
```
'A' → IsActive = true, IsTerminated = false
'I' → IsActive = false, IsTerminated = false
'T' → IsActive = false, IsTerminated = true
```

---

## Success Criteria

Your API should:

✅ Return clean, consistent JSON  
✅ Have proper error handling (404, 400, 500)  
✅ Use correct HTTP status codes  
✅ Validate input data  
✅ Handle legacy data edge cases  
✅ Have well-designed database schema  
✅ Use async/await properly  
✅ Follow REST conventions  

---

## Getting Started

### Step 1: Design Your Models
Create C# classes for:
- Employee (with proper types)
- Department
- ProjectAssignment

### Step 2: Create DbContext
Set up Entity Framework with:
- DbSets for each entity
- Configure relationships
- Set up indexes and constraints

### Step 3: Build Controllers
Create API controllers with:
- CRUD operations
- Proper async methods
- Error handling

### Step 4: Data Migration (Later)
Build a service to:
- Read from legacy database
- Transform data
- Validate and import

---

## Resources You Have

📁 **LegacyDatabase/legacy_schema.sql** - The terrible database schema  
📁 **LegacyDatabase/legacy_sample_data.sql** - Messy sample data  
📦 **Packages Installed:**
- Microsoft.EntityFrameworkCore (9.0.0)
- Microsoft.EntityFrameworkCore.Sqlite (9.0.0)
- Microsoft.EntityFrameworkCore.Tools (9.0.0)

---

## Questions to Guide You

**Before You Code:**
1. What properties should an Employee have in modern C#?
2. What data types should I use for salary, dates, booleans?
3. How should I model relationships between Employee, Department, and Projects?
4. What validation should happen at the model level vs controller level?

**When You Get Stuck:**
- Ask about specific C# concepts (attributes, async/await, etc.)
- Ask about Entity Framework patterns
- Ask about API best practices
- Show me your code and ask for feedback

---

## Ready?

Start with **Phase 1: Foundation**

Create your first model class in `Models/` folder and ask questions as you go!

Remember: **I guide, you code!** 🚀
