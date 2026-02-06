# MovieTicketBookingSystem
# 🎬 Movie Ticket Booking System

A web-based *Movie Ticket Booking System* developed using *ASP.NET Core MVC*.  
This application allows users to browse movies, book tickets online, and make payments, while administrators manage movies and monitor bookings through a secure admin panel.

---

## 📌 Project Overview

The Movie Ticket Booking System simulates a real-world online cinema booking platform.  
It simplifies the ticket booking process by offering an intuitive interface, real-time ticket availability, and role-based access for users and administrators.

---

## 👥 User Roles

### 👤 User
- Register & Login
- View *Now Showing* and *Upcoming Movies*
- Book movie tickets
- Make dummy payments
- View booking history
- Cancel bookings

### 👨‍💼 Admin
- Add, edit, and delete movies
- Manage ticket availability
- View all user bookings

---

## 🚀 Key Features

- Role-based authentication (*Admin & User*)
- Real-time ticket updates using *SignalR*
- Dummy payment module
- Secure and smooth booking workflow
- Clean and responsive UI using *Bootstrap*
- SQL Server database integration

---

## 🛠 Technologies Used

- *ASP.NET Core MVC*
- *C#*
- *SQL Server*
- *Dapper ORM*
- *ASP.NET Identity*
- *SignalR*
- *Bootstrap 5*
- *JavaScript*

---


## 🔁 Booking Workflow

```text
Login
  ↓
Browse Movies
  ↓
Select Show Date & Tickets
  ↓
Confirm Booking
  ↓
Payment
  ↓
Booking Confirmation
