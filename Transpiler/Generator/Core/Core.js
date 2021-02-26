////////////////// START OF CORE.JS //////////////////


// Gets the ith element from a composite data object.
function Get(x, i)
{
	return x[i + 1]
}

// Returns true if the 'x' matches the pattern.
function Match(x, pattern)
{
	// Wildcard pattern, return true.
	if (pattern == null)
	{
		return true;
	}

	// Type tags are being compared.
	if (typeof x == 'string' &&
		typeof pattern == 'string')
	{
		return x == pattern
	}

	// Objects are being compared.
	if (Array.isArray(x) &&
		Array.isArray(pattern) &&
		x.length == pattern.length)
	{
		for (i = 0; i < x.length; i++)
		{
			if (!Match(x[i], pattern[i]))
			{
				return false;
			}
		}

		return true;
	}

	return false;
}

function PrintResult(output) {
	if (Match(output, ["Empty"])) {

	}
	else if (Match(output, ["Node", null, null])) {
		var a = Get(output, 0)
		var b = Get(output, 1)
		if (typeof a == 'string') {
			process.stdout.write(a)
		}
		else {
			console.log(a)
		}
		PrintResult(b)
	}
	else {
		console.log(output)
	}
}

let fs = require('fs')
function Getchar() {
	let buffer = Buffer.alloc(1)
	fs.readSync(0, buffer, 0, 1)
	return buffer.toString('utf8')
}

function Putchar(c) {
	console.log(c)
	return ['']
}

function Undefined() {
	throw 'Undefined!'
}

function MkInt(si) {
	//return new BigInt(si)
	return parseInt(si)
}

function MkReal(sr) {
	//return BigDecimal(sr)
	return parseFloat(sr)
}

function MkBool(b) {
	if (b) {
		return ['True']
	} else {
		return ['False']
	}
}

const primEq = a => b => MkBool(a == b)
const primNeq = a => b => MkBool(a != b)

const primAdd = a => b => a + b
const primSub = a => b => a - b
const primMul = a => b => a * b
const primDiv = a => b => a / b

const primLt = a => b => MkBool(a < b)
const primLte = a => b => MkBool(a <= b)
const primGt = a => b => MkBool(a > b)
const primGte = a => b => MkBool(a >= b)

////////////////// END OF CORE.JS //////////////////


