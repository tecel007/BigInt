# BigInt (High performance BigInteger)

Faster than dotnet's inbuilt BigInteger

## Performance - Arithmetic on byte[134217728]

- a + b (+ve + +ve) 48 mS => 24 mS (2.0 x faster)
- a + b (-ve + +ve) 45 mS => 23 mS (2.0 x faster)
- a + b (+ve + -ve) 36 mS => 24 mS (1.5 x faster)
- a + b (-ve + -ve) 35 mS => 26 mS (1.4 x faster)
- a - b (+ve - +ve) 35 mS => 25 mS (1.4 x faster)
- a - b (-ve - +ve) 35 mS => 24 mS (1.5 x faster)
- a - b (+ve - -ve) 35 mS => 22 mS (1.6 x faster)
- a - b (-ve - -ve) 35 mS => 22 mS (1.6 x faster)

## Performance - Multiplication on byte[1048576, 2097152, ...]

Multiplication MB (horizontal) vs mS (vertical)

![alt text](https://github.com/tecel007/BigInt/blob/main/performance.png?raw=true)

## Performance - Squaring on byte[1048576, 2097152, ...]

- a ^ 2 => 745 mS => 754 mS (1 x faster)
- a ^ 2 => 2153 mS => 1178 mS (1.8 x faster)
- a ^ 2 => 6474 mS => 2992 mS (2.2 x faster)
- a ^ 2 => 19408 mS => 9239 mS (2.1 x faster)
- a ^ 2 => 122066 mS => 39351 mS (3.1 x faster)
- a ^ 2 => 237512 mS => 66343 mS (3.6 x faster)

## Performance - Multiplication on byte[1048576, 2097152, ...]

- a * b => 1107 mS => 818 mS (1.4 x faster)
- a * b => 3118 mS => 1769 mS (1.8 x faster)
- a * b => 9400 mS => 4535 mS (2.1 x faster)
- a * b => 28636 mS => 15920 mS (1.8 x faster)
- a * b => 130839 mS => 65256 mS (2 x faster)
- a * b => 271468 mS => 116419 mS (2.3 x faster)

## Performance - Fibonacci(1073741824)

- BigInt      => 440 seconds
- BigInteger  => 1521 seconds

## Performance - Largest fibonacci number in 1 second

- BigInt      => Fibonacci(18000000)
- BigInteger  => Fibonacci(13000000)

## Size limits

- BigInt       1 Gb.
- BigInteger 128 Mb

## Implementation

- 64-bit, 2's compliment arithmetic and buffers
- Multi-threaded ToomCook3 multiplication
- Pinned arrays

## Conclusion

- BigInt is simpler, faster and better aligned with computer architecture.
- BigInt currently implements add, sub, mul, div, rem, shift, not, and, or, neg
