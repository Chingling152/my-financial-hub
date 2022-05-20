CREATE DATABASE financial_hub;
USE financial_hub;

CREATE TABLE categories(
  id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  name VARCHAR(200) NOT NULL,
  description VARCHAR(500) NULL,
  active BIT DEFAULT 1,

  update_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
  creation_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
);

CREATE TABLE accounts(
  id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  name VARCHAR(200) NOT NULL,
  description VARCHAR(500) NULL,
  active BIT DEFAULT 1,

  update_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
  creation_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
);

CREATE TABLE balances(
  id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  name VARCHAR(200) NOT NULL,
  amount MONEY NOT NULL,

  account_id UNIQUEIDENTIFIER FOREIGN KEY REFERENCES accounts(id) NOT NULL,

  update_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
  creation_time DATETIMEOFFSET DEFAULT GETUTCDATE()
);

CREATE TABLE transactions(
  id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  description VARCHAR(500) NULL,

  amount MONEY NOT NULL,
  target_date DATETIMEOFFSET NOT NULL,
  finish_date DATETIMEOFFSET NOT NULL,

  balance_id UNIQUEIDENTIFIER FOREIGN KEY REFERENCES balances(id) NOT NULL,
  category_id UNIQUEIDENTIFIER FOREIGN KEY REFERENCES categories(id) NOT NULL,

  status INT NOT NULL,
  type INT NOT NULL,
  active BIT DEFAULT 1,

  update_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
  creation_time DATETIMEOFFSET DEFAULT GETUTCDATE(),
);
