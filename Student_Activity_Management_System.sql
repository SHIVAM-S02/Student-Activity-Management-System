CREATE DATABASE Student_Activity_Management_System;


CREATE TABLE Users
(
    User_ID INT PRIMARY KEY IDENTITY(1,1),
    First_Name VARCHAR(255),
    Last_Name VARCHAR(255),
    Gender VARCHAR(10),
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role_ID INT,
    Batch_ID INT,
    CONSTRAINT FK_Users_Role FOREIGN KEY (Role_ID) REFERENCES Roles(Role_ID),
    CONSTRAINT FK_Users_Batch FOREIGN KEY (Batch_ID) REFERENCES Batches(Batch_ID)
);

CREATE TABLE Roles
(
    Role_ID INT PRIMARY KEY IDENTITY(1,1),
    Role_Name VARCHAR(50) NOT NULL
);

INSERT INTO Roles (Role_Name) VALUES ('Admin');
INSERT INTO Roles (Role_Name) VALUES ('Teacher');
INSERT INTO Roles (Role_Name) VALUES ('Student');

-- Insert values into the Users table
INSERT INTO Users (First_Name, Last_Name, Gender, Email, Password, Role_ID, Batch_ID)
VALUES
    ('John', 'Doe', 'Male', 'john.doe@example.com', 'password123', 1, 1),  -- User with Role 'Admin' and Batch 'Batch A'
    ('Jane', 'Smith', 'Female', 'jane.smith@example.com', 'securepass', 2, 2),  -- User with Role 'Teacher' and Batch 'Batch B'
    ('Alice', 'Johnson', 'Female', 'alice.johnson@example.com', 'mypassword', 3, 3);  -- User with Role 'Student' and Batch 'Batch C'

select * from Users;
select * from Roles;

CREATE TABLE Batches
(
    Batch_ID INT PRIMARY KEY IDENTITY(1,1),
    Batch_Name VARCHAR(50) NOT NULL,
);

INSERT INTO Batches (Batch_Name)
VALUES
    ('2024'),
    ('2025'),
    ('2026');

TRUNCATE TABLE BATCHES;

SELECT * FROM BATCHES;


CREATE PROCEDURE sp_UserSignUp
    @First_Name VARCHAR(255),
    @Last_Name VARCHAR(255),
    @Gender VARCHAR(10),
    @Email NVARCHAR(255),
    @Password NVARCHAR(255),
    @Role_Name VARCHAR(50),
    @Batch_Name VARCHAR(50)
AS
BEGIN
    DECLARE @Role_ID INT;
    DECLARE @Batch_ID INT;

    -- Get the Role_ID based on the Role_Name
    SELECT @Role_ID = Role_ID FROM Roles WHERE Role_Name = @Role_Name;

    -- Get the Batch_ID based on the Batch_Name
    SELECT @Batch_ID = Batch_ID FROM Batches WHERE Batch_Name = @Batch_Name;

    IF @Role_ID IS NOT NULL AND @Batch_ID IS NOT NULL
    BEGIN
        INSERT INTO Users (First_Name, Last_Name, Gender, Email, Password, Role_ID, Batch_ID)
        VALUES (@First_Name, @Last_Name, @Gender, @Email, @Password, @Role_ID, @Batch_ID);
    END

END;
 

CREATE PROCEDURE [dbo].[UserLogin]
    @Email VARCHAR(25),
    @Password VARCHAR(15)
AS
BEGIN
    DECLARE @UserID INT;
    DECLARE @RoleName VARCHAR(25);
   
    -- Check if the provided email and password match a user in the Users table
    SELECT @UserID = User_ID, @RoleName = r.Role_Name
    FROM Users u
    INNER JOIN Roles r ON u.Role_ID = r.Role_ID
    WHERE u.Email = @Email AND u.Password = @Password;

    -- If a matching user is found, return their details
    IF @UserID IS NOT NULL
    BEGIN
        SELECT @UserID AS User_ID, @RoleName AS Role_Name;
    END
    ELSE
    BEGIN
        -- If no matching user is found, throw an error
        THROW 50000, 'Invalid email or password.', 1;
    END
END;



CREATE PROCEDURE [dbo].[CreateAdmin]
    @p_FirstName VARCHAR(25),
    @p_LastName VARCHAR(25),
    @p_Gender VARCHAR(25),
    @p_Email VARCHAR(25),
    @p_Password VARCHAR(15)
AS
BEGIN
    DECLARE @p_RoleID INT;

    -- Find the Role_ID for the "Admin" role
    SELECT @p_RoleID = Role_ID FROM Roles WHERE Role_Name = 'Admin';

    -- Insert the new admin into the Users table
    INSERT INTO Users (First_Name, Last_Name, Gender, Email, Password, Role_ID)
    VALUES (@p_FirstName, @p_LastName, @p_Gender, @p_Email, @p_Password, @p_RoleID);
END;




CREATE PROCEDURE [dbo].[GetAllAdmins]
As
BEGIN
    SELECT * FROM Users WHERE Role_ID = 1;
END;


CREATE PROCEDURE [dbo].[CreateStudent]
    @p_FirstName NVARCHAR(255),
    @p_LastName NVARCHAR(255),
    @p_Gender NVARCHAR(50),
    @p_Email NVARCHAR(255),
    @p_Password NVARCHAR(255),
    @p_Batch_ID INT
AS
BEGIN
    INSERT INTO users (First_Name, Last_Name, Gender, Email, Password, Batch_ID)
    VALUES (@p_FirstName, @p_LastName, @p_Gender, @p_Email, @p_Password, @p_Batch_ID);
END

CREATE PROCEDURE [dbo].[CreateTeacher]
    @p_FirstName VARCHAR(25),
    @p_LastName VARCHAR(25),
    @p_Gender VARCHAR(25),
    @p_Email VARCHAR(25),
    @p_Password VARCHAR(15)
AS
BEGIN
    DECLARE @p_RoleID INT;

    -- Find the Role_ID for the "Admin" role
    SELECT @p_RoleID = Role_ID FROM Roles WHERE Role_Name = 'Teacher';

    -- Insert the new admin into the Users table
    INSERT INTO Users (First_Name, Last_Name, Gender, Email, Password, Role_ID)
    VALUES (@p_FirstName, @p_LastName, @p_Gender, @p_Email, @p_Password, @p_RoleID);
END;

CREATE PROCEDURE [dbo].[DeleteStudent]
    @p_UserID INT
AS
BEGIN
    DELETE FROM users
    WHERE User_ID = @p_UserID;
END

CREATE PROCEDURE [dbo].[EditAdmin]
    @p_UserID INT,
    @p_FirstName VARCHAR(25),
    @p_LastName VARCHAR(25),
    @p_Gender VARCHAR(25),
    @p_Email VARCHAR(25),
    @p_Password VARCHAR(15)
AS
BEGIN
    -- Update the admin's information in the Users table
    UPDATE Users
    SET
        First_Name = @p_FirstName,
        Last_Name = @p_LastName,
        Gender = @p_Gender,
        Email = @p_Email,
        Password = @p_Password
    WHERE
        User_ID = @p_UserID;
END;


CREATE PROCEDURE [dbo].[EditStudent]
    @p_UserID INT,
    @p_FirstName NVARCHAR(255),
    @p_LastName NVARCHAR(255),
    @p_Gender NVARCHAR(50),
    @p_Email NVARCHAR(255),
    @p_Password NVARCHAR(255),
    @p_Batch_ID INT
AS
BEGIN
    UPDATE users
    SET
        First_Name = @p_FirstName,
        Last_Name = @p_LastName,
        Gender = @p_Gender,
        Email = @p_Email,
        Password = @p_Password,
        Batch_ID = @p_Batch_ID
    WHERE User_ID = @p_UserID;
END



CREATE PROCEDURE [dbo].[EditTeacher]
    @p_UserID INT,
    @p_FirstName VARCHAR(25),
    @p_LastName VARCHAR(25),
    @p_Gender VARCHAR(25),
    @p_Email VARCHAR(25),
    @p_Password VARCHAR(15)
AS
BEGIN
    -- Update the admin's information in the Users table
    UPDATE Users
    SET
        First_Name = @p_FirstName,
        Last_Name = @p_LastName,
        Gender = @p_Gender,
        Email = @p_Email,
        Password = @p_Password
    WHERE
        User_ID = @p_UserID;
END;

CREATE PROCEDURE [dbo].[GetAllTeachers]
As
BEGIN
    SELECT * FROM Users WHERE Role_ID = 2;
END;


CREATE PROCEDURE [dbo].[GetUserByBatch_ID]
AS
BEGIN
    SELECT *
    FROM Users
    WHERE Batch_ID = 2;
END;

drop procedure [GetUserByBatch_ID];

CREATE PROCEDURE [dbo].[GetUserByBatch_IDs]
AS
BEGIN
    SELECT *
    FROM Users
    WHERE Batch_ID = 3;
END;



CREATE PROCEDURE [dbo].[GetUserByBatchID]
AS
BEGIN
    SELECT *
    FROM Users
    WHERE Batch_ID = 1;
END;

drop procedure [GetUserByBatchID];

select * from Users
where Batch_ID=1;


CREATE PROCEDURE GetStudentProfile
    @User_ID INT
AS
BEGIN
    SELECT *
    FROM Users
    WHERE User_ID = @User_ID;
END;


***********************************

select * from users;

select * from Users Where Batch_ID=3;