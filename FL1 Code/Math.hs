mod Core.Math

use Core.List

pi = 3.14159265358979323846

factorial n = if n <= 1
	then 1
	else reduce (*) (1..n)

fibonacci n = if n <= 1
	then 1
	else (fibonacci (n - 1)) + (fibonacci (n - 2))


