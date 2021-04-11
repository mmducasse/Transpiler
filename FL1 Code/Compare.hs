mod Core.Compare

Comparison =
	| Less
	| Equal
	| More

a (<->) b = if a == b
	then Equal
	else if a < b
		then Less
		else More