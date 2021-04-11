mod Core.Bool

use Core.List

inst Eq Bool =
	a (==) b = match a, b
		| True, True   -> True
		| False, False -> True
		| _            -> False
	a (!=) b = not (a == b)

not c = if c then False else True

a (and) b = match a, b
	| True, True -> True
	| _          -> False

a (or) b = match a, b
	| False, False -> False
	| _            -> True

any bs = fold (or) False bs

all bs = match bs
	| Empty -> False
	| _     -> reduce (and) bs