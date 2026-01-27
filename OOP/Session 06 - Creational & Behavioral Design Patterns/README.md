# Class 5 – Creational & Behavioral Design Patterns

## Why Do We Need Design Patterns?

Picture this: You're building a car. Do you invent the wheel from scratch? No! You use the wheel design that's been proven to work for centuries.

Design patterns are the same idea for code. They're proven solutions to common problems that developers face again and again. Instead of reinventing the wheel every time, we learn from those who came before us.

## The Three Types of Design Patterns

There are 23 classic design patterns divided into three categories:

- **Creational** – How to create objects
- **Structural** – How to organize classes and objects
- **Behavioral** – How objects communicate and behave

**But here's the truth:** You don't need all 23!
In most real-world projects, you'll use maybe **5-7 patterns regularly**. Some you'll never touch. Today, we'll focus on the **most practical ones** that you'll actually use in your career.

---

## Singleton - A Creational Design Pattern

Requirement:

40 students will be given 40 whiteboard. Everyone will write in his own whiteboard.
But everyone should see same content.

Examples 1_1 to 1_4 demonstrate achieving singleton.

**Advanced Task**: Try Simulating race condition when thread-safety is not used in singleton by running parallel threads.

## Factory - Another Creational Design Pattern

Factory design pattern helps in making payment without knowing the payment type.

Factory helped achieve OCP of SOLID.

## Strategy - Behavioral Design Pattern

Strategy design pattern helps in changing an algorithm’s behavior at runtime without modifying the client code.

## Abstract Factory – Creational Design Pattern

Abstract Factory design pattern helps in creating families of related objects without knowing their concrete classes.

It merges two factory patterns into one.

- Say there are two types of payment: bkash, card
- Say there are two types of receipt: paper, email
- Bkash must use paper
- Card must use email

In such case, you need abstract factory!

**Note**: These might be hard for students, but don't get afraid! Not required for entry level that much.
