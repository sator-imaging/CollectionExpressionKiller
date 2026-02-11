// all 3 errors
int[] values = [11, 22, 33, 44];

#pragma warning disable CEK001  // Collection expressions are disallowed

// 4 or more elements but less than 12 chars
values = [1, 2, 3, 4];

// only an element but more than 12 chars
values = [0001234567890];

// OK: only 3 elements and less than 12 chars
values = [1, 2, 3];

Console.WriteLine(values.Length);
