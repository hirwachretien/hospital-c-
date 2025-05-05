Hospital Queue & Appointment Booking System

A web-based ASP.NET Core Web application (Razor Pages) designed to streamline hospital appointment scheduling and real-time queue management across multiple clinics. Built with SQL Server, the system supports Admin, Doctor, and Patient roles with dedicated dashboards, secure login, and intuitive user interfaces.

🌟 Features
=============

👨‍⚕️ Patient
=============

Sign up / Log in securely

View available departments and doctors

Book, cancel, or reschedule appointments

View real-time queue status and position

Access appointment history

🩺 Doctor
=============

Log in and access daily appointment list

Accept / decline / reschedule patient appointments

Set availability calendar

View patient history and add visit notes

🛠 Admin
=============

Manage users, departments, and clinics

Assign doctors to departments

Generate reports on appointments and queue data

View analytics dashboard (appointments by day, clinic, etc.)

🧱 Technologies Used
======================

ASP.NET Core Web (Razor Pages) (C#)

SQL Server + SSMS

HTML5, CSS3, Bootstrap

JavaScript, jQuery

Visual Studio 2022

🗃 Database Overview (HospitalFlowDB)
=====================================

Main Tables:
============

Users – User credentials and roles

Clinics – Multi-location support

Departments – Linked to clinics

Doctors – Tied to departments & users

Patients – Linked to users

Appointments – Booking records

Queue – Real-time queue system

![Database Overview (HospitalFlowDB)](https://github.com/user-attachments/assets/82c8ab60-ace4-4410-8a8d-5f956de67793)

Use Case Diagram Overview 

Main Use Cases:

 Book, Cancel or Reschedule Appointment. 

 Set Doctor Availability. 

 View Appointment History.

 Manage Users, Departments, Clinics (Admin). 

 Generate Reports.

 Manage Queue.

![usercase overview](https://github.com/user-attachments/assets/99c61df4-df2d-495e-ad68-633f41c69882)


🚀 Getting Started
====================

Clone the repository:

git clone https://github.com/Kvnn-rgmba/Hospital-Queue-Appointment-Booking-System.git

Open the solution in Visual Studio 2022

Update the connection string with your local SQL Server connection string

Run the SQL script from HospitalFlowDB_CreateTables.sql

Build and run the project
