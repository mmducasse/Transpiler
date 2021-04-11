mod Core.List

use Core.Compare

count =
	| Empty -> 0
	| Node _ xs -> 1 + (count xs)

xs (@) n = get n xs

get n =
	| Empty -> None
	| Node x xs -> match n <-> 0
		| Equal -> Some x
		| Less  -> Undefined
		| More  -> get (n - 1) xs

set n y =
	| Empty -> Empty
	| Node x xs -> match n <-> 0
		| Equal -> Node y xs
		| Less  -> Undefined
		| More  -> Node x (set (n - 1) y xs)

contains y =
	| Empty -> False
	| Node x xs -> if (y == x)
		then True
		else (contains y xs)

newList n x = match n <-> 0
	| Equal -> Empty
	| Less  -> Undefined
	| More  -> Node x (newList (n - 1) x)

map f =
	| Empty     -> Empty
	| Node x xs -> Node (f x) (map f xs)

filter f =
	| Empty     -> Empty
	| Node x xs -> if f x
		then Node x rest
		else rest
		rest = filter f xs

fold f i =
	| Empty     -> i
	| Node x xs -> fold f (f i x) xs

reduce f =
	| Node x xs -> fold f x xs
	| Empty     -> Undefined

foreach f =
	| Empty -> ()
	| Node x xs -> {
		f x
		foreach f xs

a (..) b = match a <-> b
	| Equal -> [a]
	| Less  -> Node a ((a + 1)..b)
	| More  -> Node a ((a - 1)..b)

x (++) y = match x
	| Empty -> y
	| Node z xs -> Node z (xs ++ y)
