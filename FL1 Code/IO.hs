mod Core.IO

putstrln =
	| Empty     -> {
		putchar '\n'
	| Node x xs -> {
		putchar x
		putstrln xs

putstr =
	| Empty     -> ()
	| Node x xs -> {
		putchar x
		putstr xs

@exit

getstr = {
	c = getchar()
	if c == '\n'
		then Empty
		else Node c (getstr)