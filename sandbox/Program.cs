// ERROR: all errors
int[] values = [
        1, 2, 3, 4
];

#pragma warning disable CEK001  // Collection expressions are disallowed

// ERROR: 4 or more elements
values = [1, 2, 3, 4];

// ERROR: only an element but more than 12 chars
values = [123_456_789];

// ERROR: multiline expression is not allowed
values = [1,
2];

// OK: only 3 elements and expression is less than or equal to 12 chars
values = [1, 2, 3];

Console.WriteLine(values.Length);
