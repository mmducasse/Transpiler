# Transpiler

A transpiler for a custom Haskell-like programming language called "Functional Language 1" that I created as a hobby project during Covid lockdowns.

FL1 is a functional, strict evaluating, statically typed language with Hindley-Milner type inference, parametric polymorphism, and type classes.

The transpiler parses and analyzes FL1 source code, then outputs javascript and executes it with NodeJS. It also features a REPL interface where you can evaluate FL1 expressions interactively.

A video demo: https://youtu.be/_fhOd8WIVeI


# Example FL1 Code

Navigate to the "FL1 Code" directory for more code examples.

```haskell
-- Conditional expression syntax
sign n = if n == 0
	then 0
	else if n > 0
		then 1
		else -1


-- Pattern matching syntax
length =
	| Empty -> 0
	| Node _ xs -> 1 + (length xs)


-- Imperative closure syntax
greet = () -> {
	let name = prompt "Whats your name? "
	putstrln ("Hello " ++ name ++ "!!!")
```
