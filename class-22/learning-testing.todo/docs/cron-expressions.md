# Cron Expressions

Cron expressions define a schedule for recurring jobs. In this project, they are used with **Hangfire** to control when the overdue-todo archiving job runs.

---

## Table of Contents

- [Format](#format)
- [Special Characters Deep Dive](#special-characters-deep-dive)
- [Field-by-Field Examples](#field-by-field-examples)
- [Practical Scheduling Scenarios](#practical-scheduling-scenarios)
- [The 6-Field Format (Hangfire / Quartz)](#the-6-field-format-hangfire--quartz)
- [Day-of-Week and Day-of-Month Interaction](#day-of-week-and-day-of-month-interaction)
- [Hangfire's Cron Helper Class](#hangfires-cron-helper-class)
- [Timezones and UTC](#timezones-and-utc)
- [Usage in This Project](#usage-in-this-project)
- [Debugging Cron Expressions](#debugging-cron-expressions)
- [Common Mistakes and Gotchas](#common-mistakes-and-gotchas)
- [Tools and References](#tools-and-references)

---

## Format

A cron expression consists of **5 or 6 fields** separated by spaces.

### 5-field (standard Unix cron)

```
 ┌───────────── minute  (0-59)
 │ ┌──────────── hour    (0-23)
 │ │ ┌────────── day of month (1-31)
 │ │ │ ┌──────── month  (1-12)
 │ │ │ │ ┌────── day of week  (0-6, 0=Sunday, 7=Sunday)
 │ │ │ │ │
 * * * * *
```

### 6-field (Hangfire / Quartz)

Hangfire uses the **6-field** format — the first field is **seconds**:

```
 ┌───────────── second  (0-59)
 │ ┌──────────── minute  (0-59)
 │ │ ┌──────────── hour    (0-23)
 │ │ │ ┌────────── day of month (1-31)
 │ │ │ │ ┌──────── month  (1-12)
 │ │ │ │ │ ┌────── day of week  (0-6, 0=Sunday, 7=Sunday)
 │ │ │ │ │ │
 * * * * * *
```

Example mappings:

| Goal                        | 5-field         | 6-field (Hangfire) |
|-----------------------------|-----------------|--------------------|
| Every minute                | `* * * * *`     | `* * * * * *`      |
| Every hour at minute 0      | `0 * * * *`     | `0 0 * * * *`      |
| Daily at midnight            | `0 0 * * *`     | `0 0 0 * * *`      |
| Every Sunday at midnight     | `0 0 * * 0`     | `0 0 0 * * 0`      |

When using Hangfire's `Cron` helper class (e.g., `Cron.Minutely()`), the seconds field is set to `0` automatically — the expression becomes `0 * * * * *`.

---

## Special Characters Deep Dive

| Char | Name    | Meaning                                  |
|------|---------|------------------------------------------|
| `*`  | Asterisk | Every possible value in the field       |
| `,`  | Comma   | List of values                          |
| `-`  | Hyphen  | Range of values                         |
| `/`  | Slash   | Step (every N values)                   |
| `?`  | Question| No specific value (Hangfire/Quartz only)|
| `L`  | L       | Last (day of week / day of month)       |
| `#`  | Hash    | Nth occurrence of a weekday             |
| `W`  | W       | Nearest weekday                         |

### `*` — Asterisk (Every)

Matches **all** valid values in the field.

```
* * * * * *     — every second of every minute of every hour, every day
0 * * * * *     — every minute (at second 0)
0 0 * * * *     — every hour (at minute 0, second 0)
0 0 0 * * *     — every day at midnight
```

### `,` — Comma (List)

Specify **multiple specific values**.

```
0,30 * * * * *       — at seconds 0 and 30 (every 30 seconds)
0 0,15,30,45 * * * * — at minutes 0, 15, 30, 45 (every 15 minutes)
0 0 9,12,17 * * *    — at 9 AM, noon, and 5 PM daily
0 0 0 * * 1,3,5      — at midnight on Monday, Wednesday, Friday
0 0 0 1,15 * *       — at midnight on the 1st and 15th of every month
```

### `-` — Hyphen (Range)

Define an **inclusive range**.

```
0 0 9-17 * * *      — every hour from 9 AM to 5 PM (inclusive)
0 0 * * 1-5 *       — every day Monday through Friday
0 0 0 1-7 * *       — first 7 days of every month
0 0 0 * 6-8 *       — June through August (months 6, 7, 8)
0 30 8 * * 0-4      — at 8:30 AM, Sunday through Thursday
```

### `/` — Slash (Step)

**Skip** N values. The value before the slash is the starting point.

```
*/5 * * * * *        — every 5 seconds
0 */5 * * * *        — every 5 minutes (at second 0)
0 0 */2 * * *        — every 2 hours
0 0 0 */3 * *        — every 3 days
0 0 0 * */3 *        — every 3 months
0 0 0 * * */2        — every 2 days (but see [Day-of-week note below])

0 0 8-18/2 * * *     — every 2 hours between 8 AM and 6 PM
0 0 0 1-15/3 * *     — days 1, 4, 7, 10, 13 of every month
30 */3 * * * *       — at second 30 of every 3rd minute
```

**Step starting from a value:**

```
0 5/10 * * * *       — at minute 5, 15, 25, 35, 45, 55
0 30/15 * * * *      — at minute 30, 45 (not 0 — starts from 30)
0 2,4,6/10 * * * *   — at minutes 2, 4, 6, 16, 26, 36, 46, 56
```

### `?` — Question Mark (No Specific Value)

Used in **day-of-month** or **day-of-week** to mean "I do not care." You cannot use `?` and `*` in the same field — both mean "any," but `?` explicitly says "leave this field unspecified." It is useful when you specify the other day field.

```
0 0 0 1 * ?        — every 1st of the month (day of week = don't care)
0 0 0 ? * 1        — every Monday (day of month = don't care)
0 0 0 ? * 1-5      — every weekday (day of month = don't care)
```

### `L` — Last

For **day of month**: the last day of the month.
For **day of week**: the last occurrence of that weekday in the month.

```
0 0 0 L * *          — last day of every month, at midnight
0 0 0 L * ?          — last day of every month (day of week = ?)
0 0 0 * * 5L         — last Friday of every month
0 0 0 * * 1L         — last Monday of every month
```

### `#` — Nth Occurrence

Specify the **Nth** weekday of the month. Format: `weekday#N`.

```
0 0 0 * * 1#1        — first Monday of every month
0 0 0 * * 5#3        — third Friday of every month
0 0 0 * * 1#-1       — same as 1L (last Monday)
```

### `W` — Nearest Weekday

For **day of month**: the nearest weekday to that day.

```
0 0 0 15W * *       — nearest weekday to the 15th
                     (if 15th is Saturday → runs Friday 14th;
                      if 15th is Sunday → runs Monday 16th)
0 0 0 1W * *        — nearest weekday to the 1st
                     (never crosses into previous month — if 1st is Sat/Sun, runs Monday 2nd)
```

---

## Field-by-Field Examples

### Seconds (0-59)

```
*/10 * * * * *       — every 10 seconds
0 * * * * *          — at second 0 (every minute)
0,15,30,45 * * * * * — every 15 seconds
0-10 * * * * *       — first 10 seconds of every minute
```

### Minutes (0-59)

```
0 */5 * * * *        — every 5 minutes
0 0,30 * * * *       — every half hour
0 45 * * * *         — at minute 45 of every hour (10:45, 11:45, ...)
0 0-10 * * * *       — first 10 minutes of every hour
0 0-55/5 * * * *     — every 5 minutes, starting at minute 0
0 15,45 * * * *      — at quarter past and quarter to each hour
0 5,10,15,20,25,30 * * * * — scattered specific minutes
```

### Hours (0-23)

```
0 0 */3 * * *        — every 3 hours
0 0 0 * * *          — midnight (12:00 AM)
0 0 12 * * *         — noon (12:00 PM)
0 0 8-17 * * *       — every hour from 8 AM to 5 PM
0 0 6,18 * * *       — 6 AM and 6 PM
0 0 0,12 * * *       — midnight and noon
0 30 5 * * *         — 5:30 AM
0 45 23 * * *        — 11:45 PM
0 0 0,6,12,18 * * *  — every 6 hours (midnight, 6 AM, noon, 6 PM)
```

### Day of Month (1-31)

```
0 0 0 1 * *          — 1st of every month
0 0 0 15 * *         — 15th of every month
0 0 0 1,15 * *       — 1st and 15th of every month
0 0 0 1-7 * *        — first 7 days of every month
0 0 0 25-L * *       — 25th through last day of every month
0 0 0 */5 * *        — every 5th day: 1, 6, 11, 16, 21, 26, 31
0 0 0 L * *          — last day of every month
```

### Month (1-12)

```
0 0 0 1 1 *          — January 1st
0 0 0 1 6 *          — June 1st
0 0 0 1 1,6 *        — January 1st and June 1st
0 0 0 1 3-6 *        — 1st of March, April, May, June
0 0 0 * */3 *        — every 3 months: Jan, Apr, Jul, Oct
0 0 0 * 1-3,10-12 *  — first quarter and last quarter (Jan-Mar, Oct-Dec)
0 0 0 1 1,4,7,10 *   — 1st of each quarter
```

### Day of Week (0-7, 0=Sunday, 7=Sunday)

```
0 0 0 * * 0          — every Sunday
0 0 0 * * 1          — every Monday
0 0 0 * * 1-5        — every weekday (Mon-Fri)
0 0 0 * * 0,6        — every weekend (Sat-Sun)
0 0 0 * * 1,3,5      — Mon, Wed, Fri
0 0 0 * * 2,4        — Tue, Thu
0 0 0 * * 1#1        — first Monday of each month
0 0 0 * * 5L         — last Friday of each month
```

---

## Practical Scheduling Scenarios

### Report Generation

```
0 0 6 * * *          — daily sales report at 6 AM
0 0 8 * * 1          — weekly summary every Monday at 8 AM
0 0 0 1 * *          — monthly report on the 1st at midnight
0 0 0 1 1 *          — yearly report on Jan 1st
0 30 7 * * 1-5       — weekday morning digest at 7:30 AM
```

### Cleanup / Maintenance

```
0 0 3 * * 0          — weekly DB cleanup every Sunday at 3 AM
0 0 2 * * *          — daily log rotation at 2 AM
0 0 4 1 * *          — monthly archive on the 1st at 4 AM
0 0 5 */7 * *        — every 7 days at 5 AM
0 0 1 * * 6          — every Saturday at 1 AM for full backup
```

### Business Process Automation

```
0 0 9-17/2 * * 1-5   — every 2 hours during work hours on weekdays
0 0 8,12,16 * * 1-5  — at 8 AM, noon, and 4 PM on weekdays
0 30 16 * * 5        — every Friday at 4:30 PM (end-of-week processing)
0 0 7 * * 1          — Monday morning queue processing at 7 AM
0 */15 9-18 * * 1-5  — every 15 minutes during business hours
```

### Notification / Reminder

```
0 0 9 * * 1          — Monday morning reminder at 9 AM
0 0 18 * * 5         — Friday evening wrap-up at 6 PM
0 0 8,12,17 * * *    — 3 reminders daily (morning, noon, evening)
0 0 0 * * 5          — Friday midnight "end of week" notification
```

### Data Synchronization

```
0 */10 * * * *       — sync every 10 minutes (high frequency)
0 0 */4 * * *        — sync every 4 hours
0 0 0,12 * * *       — sync twice a day (midnight and noon)
0 0 0 */2 * *        — sync every 2 days
```

### Health Check / Heartbeat

```
*/30 * * * * *       — heartbeat every 30 seconds
0 * * * * *          — health check every minute (at second 0)
0 0 */6 * * *        — deep health check every 6 hours
0 0 0 * * *          — daily health summary at midnight
```

### User-Facing Scheduled Features

```
0 0 6 * * 1-5        — daily email blast at 6 AM on weekdays
0 0 10 * * 1         — weekly newsletter every Monday at 10 AM
0 0 0 1 * *          — monthly invoice generation on 1st
0 0 15 * * 3         — Wednesday 3 PM — mid-week promotion push
0 0 9 * * 1,3,5      — mon, wed, fri at 9 AM — task reminders
```

---

## The 6-Field Format (Hangfire / Quartz)

Hangfire uses the Quartz-style 6-field cron format (with seconds).

### 5-field vs 6-field equivalence

| Schedule              | 5-field Unix      | 6-field Hangfire  | Hangfire Helper     |
|-----------------------|-------------------|-------------------|---------------------|
| Every second          | N/A (no second)   | `* * * * * *`     | —                   |
| Every 5 seconds       | N/A               | `*/5 * * * * *`   | —                   |
| Every minute          | `* * * * *`       | `0 * * * * *`     | `Cron.Minutely()`   |
| Every 5 minutes       | `*/5 * * * *`     | `0 */5 * * * *`   | —                   |
| Every hour            | `0 * * * *`       | `0 0 * * * *`     | `Cron.Hourly()`     |
| Every 2 hours         | `0 */2 * * *`     | `0 0 */2 * * *`   | —                   |
| Daily at midnight     | `0 0 * * *`       | `0 0 0 * * *`     | `Cron.Daily()`      |
| Weekly (Sun midnight) | `0 0 * * 0`       | `0 0 0 * * 0`     | `Cron.Weekly()`     |
| Monthly (1st midnight)| `0 0 1 * *`       | `0 0 0 1 * *`     | `Cron.Monthly()`    |
| Yearly (Jan 1)        | `0 0 1 1 *`       | `0 0 0 1 1 *`     | `Cron.Yearly()`     |

### When to use the 6th field (seconds)

Most of the time you want `0` in the seconds field — you rarely need second-level precision for recurring jobs. But there are cases:

```
0 0 0 * * *            — daily at midnight (seconds = 0)
*/30 * * * * *         — every 30 seconds (useful for high-frequency polling)
0,10,20,30,40,50 * * * * * — every 10 seconds
5 * * * * *            — at second 5 of every minute (stagger to avoid thundering herd)
```

**Thundering herd problem**: If multiple jobs start at `0 * * * * *` (second 0), they all trigger simultaneously. Stagger them:

```
0 * * * * *            — job A: at second 0
15 * * * * *           — job B: at second 15
30 * * * * *           — job C: at second 30
```

### Using seconds field with Hangfire's Cron class

Hangfire's `Cron` helpers set seconds to `0`. For second-level schedules, pass a raw string:

```csharp
// Using helper (seconds = 0)
RecurringJob.AddOrUpdate("job-id", () => Method(), Cron.Minutely());
// → "0 * * * * *"

// Raw string for custom seconds
RecurringJob.AddOrUpdate("job-id", () => Method(), "*/30 * * * * *");
// → every 30 seconds

RecurringJob.AddOrUpdate("job-id", () => Method(), "0 */5 * * * *");
// → every 5 minutes at second 0
```

---

## Day-of-Week and Day-of-Month Interaction

When **both** day-of-month and day-of-week are specified (neither is `*` or `?`), the job runs when **either** condition matches (OR logic).

| Expression                      | Meaning                                                  |
|---------------------------------|----------------------------------------------------------|
| `0 0 0 1 * 1`                  | Runs on the 1st **or** every Monday (whichever comes first) |
| `0 0 0 15 * 5`                 | Runs on the 15th **or** every Friday                     |
| `0 0 0 1 1 3`                  | Runs on Jan 1st **or** every Wednesday                   |

If you want **both** conditions to be met (AND logic), you usually cannot — this is a known cron limitation. Some workarounds:

- Use `?` in day-of-month and `#` in day-of-week: `0 0 0 ? * 5#1` (first Friday — AND implied)
- Use application-level filtering: run the job daily and check the date inside the method

### Examples with both fields

```
0 0 0 15 * 5            — 15th of month OR every Friday
0 0 0 1,15 * 1,3,5      — 1st/15th OR Mon/Wed/Fri
0 0 0 1-7 * 0           — first 7 days OR every Sunday
```

---

## Hangfire's Cron Helper Class

Hangfire provides built-in helpers for common schedules. These generate the 6-field expression with `0` in the seconds field.

| Method                 | Expression        | Actual Schedule                              |
|------------------------|-------------------|----------------------------------------------|
| `Cron.Minutely()`      | `0 * * * * *`     | Every minute at second 0                     |
| `Cron.Hourly()`        | `0 0 * * * *`     | Every hour at minute 0                       |
| `Cron.Hourly(15)`      | `0 15 * * * *`    | Every hour at minute 15                      |
| `Cron.Daily()`         | `0 0 0 * * *`     | Every day at midnight                        |
| `Cron.Daily(9)`        | `0 0 9 * * *`     | Every day at 9 AM                            |
| `Cron.Daily(14, 30)`   | `0 30 14 * * *`   | Every day at 2:30 PM                         |
| `Cron.Weekly()`        | `0 0 0 * * 0`     | Every Sunday at midnight                     |
| `Cron.Weekly(DayOfWeek.Monday)` | `0 0 0 * * 1`    | Every Monday at midnight                     |
| `Cron.Weekly(DayOfWeek.Friday, 17)` | `0 0 17 * * 5`  | Every Friday at 5 PM                         |
| `Cron.Monthly()`       | `0 0 0 1 * *`     | 1st of every month at midnight               |
| `Cron.Monthly(15)`     | `0 0 0 15 * *`    | 15th of every month at midnight              |
| `Cron.Monthly(15, 8, 30)` | `0 30 8 15 * *` | 15th of every month at 8:30 AM               |
| `Cron.Yearly()`        | `0 0 0 1 1 *`     | January 1st at midnight                      |
| `Cron.Yearly(6)`       | `0 0 0 1 6 *`     | June 1st at midnight                         |
| `Cron.Yearly(12, 25, 9, 0)` | `0 0 9 25 12 *` | December 25th at 9 AM                        |
| `Cron.Never()`         | —                 | Never runs (useful for disabling)            |

### When to use helpers vs raw strings

```csharp
// ✅ Helpers for simple schedules (clear intent)
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Daily(3));                          // daily at 3 AM


RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Minutely());                        // every minute for testing

// ✅ Raw strings for complex schedules
RecurringJob.AddOrUpdate<Job>(
    "complex-schedule",
    job => job.Run(),
    "0 0 8,12,17 * * 1-5");                 // Mon-Fri at 8 AM, noon, 5 PM

RecurringJob.AddOrUpdate<Job>(
    "every-2-hours",
    job => job.Run(),
    "0 0 */2 * * *");                       // every 2 hours

RecurringJob.AddOrUpdate<Job>(
    "30-seconds",
    job => job.Run(),
    "*/30 * * * * *");                      // every 30 seconds
```

---

## Timezones and UTC

Hangfire cron expressions use **UTC** by default.

### Local timezone example

If you are in `Asia/Dhaka` (UTC+6) and want the job to run at **9 AM local time**:

```csharp
var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");

RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Daily(9),      // 9 AM UTC = 3 PM Dhaka
    timeZone);          // ← Schedule is evaluated in the given timezone
```

Now the job runs at **9 AM Bangladesh time**, which is **3 AM UTC** in standard time.

### Common timezone IDs

| Timezone                  | ID (Linux)                    | ID (Windows)                 |
|---------------------------|-------------------------------|------------------------------|
| UTC                       | `UTC`                         | `UTC`                        |
| US Eastern                | `America/New_York`            | `Eastern Standard Time`      |
| US Pacific                | `America/Los_Angeles`         | `Pacific Standard Time`      |
| India                     | `Asia/Kolkata`                | `India Standard Time`        |
| Bangladesh                | `Asia/Dhaka`                  | `Bangladesh Standard Time`   |
| UK                        | `Europe/London`               | `GMT Standard Time`          |
| Central Europe            | `Europe/Berlin`               | `Central European Standard Time`|
| Japan                     | `Asia/Tokyo`                  | `Tokyo Standard Time`        |
| Australia Eastern         | `Australia/Sydney`            | `AUS Eastern Standard Time`  |

### Daylight saving time (DST) caveats

```
DST transitions can cause:
- A job to be skipped (spring forward: 2 AM → 3 AM, no 2:30 AM)
- A job to run twice (fall back: 1 AM → 1 AM, 1:30 AM happens twice)
```

**Best practice**: Run scheduled jobs in UTC and convert to local time only for display purposes. If you must use local time, test during DST transitions.

---

## Usage in This Project

### Current: every minute (for development/testing)

```csharp
// BackgroundServices/OverDueTodoArchieveJob.cs

RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Minutely);
```

### Production-ready alternatives

```csharp
// Option 1: Every hour at minute 0
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Hourly);

// Option 2: Daily at 3 AM (UTC)
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Daily(3));

// Option 3: Daily at 3 AM Bangladesh time (Asia/Dhaka)
var dhaka = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Daily(3),
    dhaka);

// Option 4: Every 6 hours (4 times daily)
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    "0 0 */6 * * *");

// Option 5: Every Monday at 2 AM (weekly maintenance)
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Weekly(DayOfWeek.Monday, 2));
```

### Running multiple jobs on different schedules

```csharp
// Archive overdue todos — daily at 3 AM
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Daily(3));

// Send reminder emails — weekday mornings at 8 AM
RecurringJob.AddOrUpdate<EmailReminderJob>(
    "send-reminders",
    job => job.SendReminders(),
    "0 0 8 * * 1-5");

// Cleanup temporary data — every Sunday at midnight
RecurringJob.AddOrUpdate<CleanupJob>(
    "weekly-cleanup",
    job => job.Cleanup(),
    Cron.Weekly());

// Generate daily summary — every evening at 6 PM
RecurringJob.AddOrUpdate<SummaryJob>(
    "daily-summary",
    job => job.Generate(),
    "0 0 18 * * *");
```

---

## Debugging Cron Expressions

### Print the expression

```csharp
var expr = Cron.Daily(14, 30);
Console.WriteLine(expr);  // "0 30 14 * * *"

var raw = "0 0 8,12,17 * * 1-5";
Console.WriteLine(raw);   // "0 0 8,12,17 * * 1-5"
```

### Next occurrences

Hangfire does not have a built-in "next N occurrences" method, but you can use Quartz's `CronExpression`:

```csharp
using Quartz;

var cron = new CronExpression("0 0 8,12,17 * * 1-5");
var next = cron.GetNextValidTimeAfter(DateTimeOffset.UtcNow);
Console.WriteLine($"Next run: {next}");
```

### Hangfire dashboard

The Hangfire dashboard at `/hangfire` (locally: `http://localhost:5182/hangfire`) shows:
- **Recurring Jobs** tab — lists all registered jobs with their cron expressions
- **Next execution** column — shows the calculated next run time
- Manual trigger button — run a job immediately regardless of schedule
- History of past executions with timings and status

### Logging

The `OverDueTodoArchieveJob` logs start/completion/errors with timestamps:

```
info: BackgroundServices.OverDueTodoArchieveJob[0]
      Starting overdue todo archiving job at 06/23/2026 14:30:00 +00:00
info: BackgroundServices.OverDueTodoArchieveJob[0]
      Completed overdue todo archiving job at 06/23/2026 14:30:00 +00:00.
      Archived 5 todos.
```

---

## Common Mistakes and Gotchas

### 1. Off-by-one on minutes

```
❌ "0 */1 * * * *"   ← runs every minute (same as "* * * * * *" with seconds=0)
✅ "*/5 * * * * *"   ← every 5 seconds
✅ "0 */5 * * * *"   ← every 5 minutes
```

### 2. Confusing 5-field and 6-field

```
❌ Cron.Minutely() → "0 * * * * *" (6 fields)
   Passing this to a 5-field parser will fail

✅ Always use Hangfire for 6-field, Unix crontab for 5-field
```

### 3. Day-of-week 0 vs 7

```
Both 0 and 7 mean Sunday in most cron implementations.
But to be safe, use 0.
```

### 4. Forgetting seconds when using raw strings

```
❌ "*/5 * * * *"     ← missing seconds field → Hangfire will parse incorrectly
✅ "0 */5 * * * *"   ← correct 6-field format
```

### 5. Day-of-month 31 on short months

```
❌ "0 0 0 31 * *"   ← fails on Feb, Apr, Jun, Sep, Nov
✅ "0 0 0 L * *"    ← last day of month (safe)
```

### 6. Mixing * with ? incorrectly

```
❌ "0 0 0 * * *"    ← day-of-month = *, day-of-week = * → both "any" → runs daily
❌ "0 0 0 15 * *"   ← 15th of month OR any day-of-week → runs daily + 15th (daily!)
✅ "0 0 0 15 * ?"   ← 15th of month only
```

### 7. Thundering herd at second 0

```
Multiple jobs all scheduled at "0 * * * * *" (second 0) will all fire simultaneously.

Fix: stagger the seconds:
  Job A: "0 * * * * *"   (second 0)
  Job B: "15 * * * * *"  (second 15)
  Job C: "30 * * * * *"  (second 30)
```

### 8. The `Cron.Daily()` default

```
Cron.Daily()       → midnight UTC
Cron.Daily(9)      → 9 AM UTC (not 9 AM local!)

If your users are in Bangladesh, Cron.Daily(9) runs at 9 AM UTC = 3 PM Bangladesh time.
```

### 9. DST and Cron

```
In timezones with DST:
- A job at 2:30 AM might be skipped (spring forward)
- A job at 1:30 AM might run twice (fall back)

Prefers UTC schedules if possible, or test during DST transitions.
```

### 10. Jobs running when you don't expect

```
"0 0 0 15 * 5"
This runs on the 15th of the month AND every Friday.
If the 15th is a Tuesday and it's also Friday, it runs on both days.

If you want "15th of month AND it's a Friday" (AND logic):
→ Use "0 0 0 15 * ?" and check day-of-week in code.
```

---

## Cron Expression Cheat Sheet

| Desire                        | 5-field           | 6-field (Hangfire)    |
|-------------------------------|-------------------|-----------------------|
| Every second                  | —                 | `* * * * * *`        |
| Every 5 seconds               | —                 | `*/5 * * * * *`      |
| Every 30 seconds              | —                 | `*/30 * * * * *`     |
| Every minute                  | `* * * * *`       | `0 * * * * *`        |
| Every 5 minutes               | `*/5 * * * *`     | `0 */5 * * * *`      |
| Every 15 minutes              | `*/15 * * * *`    | `0 */15 * * * *`     |
| Every 30 minutes              | `*/30 * * * *`    | `0 */30 * * * *`     |
| Every hour                    | `0 * * * *`       | `0 0 * * * *`        |
| Every 2 hours                 | `0 */2 * * *`     | `0 0 */2 * * *`      |
| Every 6 hours                 | `0 */6 * * *`     | `0 0 */6 * * *`      |
| 9 AM daily                    | `0 9 * * *`       | `0 0 9 * * *`        |
| 9 AM and 5 PM daily           | `0 9,17 * * *`    | `0 0 9,17 * * *`     |
| Midnight daily                | `0 0 * * *`       | `0 0 0 * * *`        |
| 8:30 AM daily                 | `30 8 * * *`      | `0 30 8 * * *`       |
| Noon Mon-Fri                  | `0 12 * * 1-5`    | `0 0 12 * * 1-5`     |
| Midnight 1st of month         | `0 0 1 * *`       | `0 0 0 1 * *`        |
| Midnight 1st of quarter       | `0 0 1 1,4,7,10 *`| `0 0 0 1 1,4,7,10 *`|
| Every Sunday midnight         | `0 0 * * 0`       | `0 0 0 * * 0`        |
| Weekdays at 6 AM              | `0 6 * * 1-5`     | `0 0 6 * * 1-5`      |
| First Monday of month         | —                 | `0 0 0 ? * 1#1`      |
| Last day of month midnight    | `0 0 28-31 * *`(❌)| `0 0 0 L * *`        |
| Last Friday of month          | —                 | `0 0 0 * * 5L`       |
| Every 10 seconds              | —                 | `*/10 * * * * *`     |
| Every 3 hours between 9-18    | `0 9-18/3 * * *`  | `0 0 9-18/3 * * *`   |

---

## Tools and References

- **[Crontab Guru](https://crontab.guru/)** — interactive 5-field cron editor (does not support seconds)
- **[Freeformatter Cron Generator](https://www.freeformatter.com/cron-expression-generator-quartz.html)** — Quartz-style (6-field, Hangfire-compatible)
- **[Cron Expression Descriptor](https://bradymholt.github.io/cron-expression-descriptor/)** — translates expressions to English
- **[Hangfire Documentation — Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)** — official Hangfire docs
- **[Quartz Cron Trigger Tutorial](https://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/crontrigger.html)** — Quartz cron reference (matches Hangfire format)
