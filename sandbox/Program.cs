using System.Diagnostics.CodeAnalysis;
[assembly: SuppressMessage("", "IDE0059")]

// ERROR: all errors
int[] values = [
        1, 2, 3, 4
];


#pragma warning disable CEK001  // Collection expressions are disallowed
#pragma warning disable CEK005  // Collection expressions with elements are disallowed

// ERROR: 4 or more elements
values = [1, 2, 3, 4];

// ERROR: only an element but more than 12 chars
values = [123_456_789];

// ERROR: multiline expression is not allowed
values = [1,
2];


#pragma warning restore
#pragma warning disable CEK001  // Collection expressions are disallowed
#pragma warning disable CEK003  // Long collection expression text is disallowed
#pragma warning disable CEK005  // Collection expressions with elements are disallowed

// OK: Disable CEK001/003 to allow expression which is 3 or less elements in single line.
values = [1234, 56789, .. values];

// OK: only 3 elements and expression is less than or equal to 12 chars
values = [1, 2, 3];


#pragma warning restore
#pragma warning disable CEK001  // Collection expressions are disallowed
#pragma warning disable CEK002  // Collection expressions with more than 3 elements are disallowed
#pragma warning disable CEK003  // Long collection expression text is disallowed
#pragma warning disable CEK004  // Multiline collection expressions are disallowed

// ERROR: only `[]` is allowed
values = [1];
values = [];


Console.WriteLine(values.Length);
