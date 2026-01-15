# Stack vs Heap

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                    STACK vs HEAP MEMORY IN C#
                    Complete Visual Guide
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚠️ IMPORTANT CLARIFICATION: WHERE DO STACK AND HEAP LIVE?
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

BOTH Stack and Heap live in RAM (Random Access Memory)!

❌ COMMON MISCONCEPTION:
   "Stack is in RAM, Heap is in ROM" ← WRONG!

✅ CORRECT ANSWER:
   Both Stack AND Heap are regions of RAM allocated to your program.
   
   ROM (Read-Only Memory) is used for:
   • BIOS/Firmware
   • Compiled program code (instructions)
   • NOT for runtime data storage

   RAM (Random Access Memory) contains:
   • Stack (part of RAM)
   • Heap (different part of RAM)
   • Both are volatile (lost when program ends)

VISUAL:
┌──────────────────────────────────────────┐
│          COMPUTER RAM (Main Memory)      │
├──────────────────────────────────────────┤
│  Operating System                        │
├──────────────────────────────────────────┤
│  Your Program's Memory:                  │
│    ┌──────────┐                          │
│    │  STACK   │ ← Small, fast region     │
│    ├──────────┤                          │
│    │          │                          │
│    │  HEAP    │ ← Large, flexible region │
│    │          │                          │
│    └──────────┘                          │
├──────────────────────────────────────────┤
│  Other Programs...                       │
└──────────────────────────────────────────┘
      ↑
   ALL IN RAM!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

STACK vs HEAP: THE DIFFERENCES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

STACK:
  • Location: RAM (specific region)
  • Fast, automatic memory (like a stack of plates)
  • Stores: local variables (value types), method parameters, return addresses
  • Size: Small (~1MB per thread)
  • Lifetime: Exists only while method is running
  • Cleanup: Automatic and IMMEDIATE when method exits
  • Access: LIFO (Last In, First Out)

HEAP:
  • Location: RAM (different region)
  • Larger, flexible memory (like a warehouse)
  • Stores: Objects (instances of classes), arrays, strings
  • Size: Large (can be GBs)
  • Lifetime: Until Garbage Collector cleans it up
  • Cleanup: Automatic but DELAYED (non-deterministic)
  • Access: Random access via references

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                        VISUAL REPRESENTATION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

       STACK                           HEAP
   (Small region of RAM)        (Large region of RAM)
   
┌──────────────┐              ┌─────────────────────┐
│              │              │                     │
│ Method Frame │              │  Student Object     │
│   x: 10      │              │  Name: "Alice"      │
│   y: 20      │              │  Grade: 85          │
│   s: [ref]───┼─────────────→│                     │
│              │              │                     │
└──────────────┘              │  Another Object     │
                              │  ...                │
                              └─────────────────────┘

  Fast access                   Slower but flexible
  LIFO (Last In First Out)      Random access
  Small size                    Large size
  Immediate cleanup             GC cleanup (delayed)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
