////////////////// START OF CORE.JS //////////////////


// Gets the ith element from a composite data object.
function Get(i, x)
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
		for (var i = 0; i < x.length; i++)
		{
			if (!Match(x[i], pattern[i]))
			{
				return false;
			}
		}

		return true;
	}

	return x == pattern;
}

function FormatResult(data) {
	if (Match(data, ["Empty"])) {
		return "[]"
	}
	else if (Match(data, ["Node", null, null])) {
		var a = Get(0, data)
		if (typeof a == 'string') {
			return "\"" + FormatString(data) + "\""
		}
		else {
			return "[" + FormatList(data) + "]"
		}
	}
	else if (Array.isArray(data)) {
		if (data[0] == '') {
			return "(" + FormatTuple(data) + ")"
		}
		else {
			return "(" + FormatData(data) + ")"
		}
	}
	else {
		return String(data)
	}
}

// The object's type is List.
function FormatList(data) {
	var a = Get(0, data)
	var b = Get(1, data)
	s = FormatResult(a)
	if (Match(b, ["Empty"])) {
		return s
	}
	else {
		return s + ", " + FormatList(b)
	}
}

// The object's type is String.
function FormatString(data) {
	var a = Get(0, data)
	var b = Get(1, data)
	s = a
	if (Match(b, ["Empty"])) {
		return s
	}
	else {
		return s + FormatString(b)
	}
}

// The object is some ADT.
function FormatData(data) {
	var s = data[0]
	for (var i = 1; i < data.length; i++) {
		s += " " + FormatResult(data[i])
	}
	return s
}

// The object is a tuple.
function FormatTuple(data) {
	if (data.length < 2) { return '' }
	var s = data[1]
	for (var i = 2; i < data.length; i++) {
		s += ", " + FormatResult(data[i])
	}
	return s
}

function PrintResult(output) {
	s = FormatResult(output)
	console.log(s)
}

function Putchar(c) {
	process.stdout.write(c);
	return ['']
}

let prompt = require('prompt-sync')({ sigint: true });
function Prompt(msg) {
	msg = ListCharToString(msg)
	let s = prompt(msg) ?? '';
	return StringToListChar(s)
}

function StringToListChar(s) {
	var str = ['Empty']
	for (var i = s.length - 1; i >= 0; i--) {
		str = ['Node', s[i], str];
	}
	return str
}

function ListCharToString(list) {
	var str = ''
	while (!Match(list, ['Empty'])) {
		let ch = Get(0, list)
		list = Get(1, list)
		str += ch
	}
	return str
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

const intAdd = a => b => a + b
const intSub = a => b => a - b
const intMul = a => b => a * b
const intDiv = a => b => (a / b >> 0)
const intMod = a => b => a % b

const realAdd = a => b => a + b
const realSub = a => b => a - b
const realMul = a => b => a * b
const realDiv = a => b => a / b

const primLt = a => b => MkBool(a < b)
const primLte = a => b => MkBool(a <= b)
const primGt = a => b => MkBool(a > b)
const primGte = a => b => MkBool(a >= b)

const primShow = a => StringToListChar(a.toString())

const RawEq = a => b => MkBool(Match(a, b))

function ParseInt(s) {
	let str = ListCharToString(s)
	let i = parseInt(str)
	if (isNaN(i)) {
		return ['None']
	}
	else {
		return ['Some', i]
	}
}

function ParseReal(s) {
	let str = ListCharToString(s)
	let f = parseFloat(str)
	if (isNaN(f)) {
		return ['None']
	}
	else {
		return ['Some', f]
	}
}

////////////////// END OF CORE.JS //////////////////


