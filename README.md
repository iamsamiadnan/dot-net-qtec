Qtec - Mini Account Management System

---

## What I Did

I used the **ASP.NET Core Razor Pages** template to build this project, following the **MVVM architecture pattern**, which Razor Pages is designed to support.

---

### 1. User Roles & Permissions

- I registered users using the **Identity Framework** and assigned a default role **Viewer** during registration.
- I used **ADO.NET** to perform transactions when updating user information and roles.
- Only users with the **Admin** role can access this module.
- The Admin can:
  - Create users
  - Update user’s first name and last name
  - Change the user’s role

#### 🔧 Areas for Improvement

- Instead of hardcoding access rights, I can:

  - Store module access data in a database table.
  - Use **Authorization Policies** in Identity Framework to handle access control via middleware.
  - Update access using stored procedures.

- Add a **Delete User** feature.

---

### 2. Chart of Accounts

- Only users with the **Accountant** role can access this module.
- I implemented a **hierarchical tree** to display the chart of accounts.
- Accountants can:

  - Create an account by specifying a name and selecting a parent account.
  - If no parent is selected, the account becomes a **Parent Account**.

- Only accounts with a parent account (i.e., sub-accounts) are shown in the **Vouchers** page.
- Parent accounts are used for **grouping** and act like categories.

#### 🔧 Areas for Improvement

- Implement **Update** and **Delete** features (stored procedures for these are already in place).

---

### 3. Voucher Entry Module

- Accountants can create vouchers using a form.
- Each voucher can have **multiple debit and credit entries**.
- I used **SQL Transactions** and **Table-Valued Parameters (TVP)** to handle bulk inserts efficiently and safely.

---

## Final Notes

I will also share the **SQL scripts** and .bak file used in the project.

I request you to review my **codebase** to see the actual implementation.
Please note: I might have missed mentioning some programming details or techniques I applied during development.
