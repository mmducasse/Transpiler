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

function MkInt(si) {
	//return new BigInt(si)
	return parseInt(si)
}

function MkReal(sr) {
	//return BigDecimal(sr)
	return parseFloat(sr)
}

const primEq = a => b => a == b
const primNeq = a => b => a != b

const primAdd = a => b => a + b
const primSub = a => b => a - b
const primMul = a => b => a * b
const primDiv = a => b => a / b

const primLt = a => b => a < b
const primLte = a => b => a <= b
const primGt = a => b => a > b
const primGte = a => b => a >= b

////////////////// END OF CORE.JS //////////////////


