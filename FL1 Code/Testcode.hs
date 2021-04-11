mod Testcode

use Core.List
use Core.IO

-- Conditional expression
sign n = if n == 0
	then 0
	else if n > 0
		then 1
		else -1


-- Pattern matching expression
length =
	| Empty -> 0
	| Node _ xs -> 1 + (length xs)


-- Imperative closure expression
greet = () -> {
	let name = prompt "Whats your name? "
	putstrln ("Hello " ++ name ++ "!!!")




