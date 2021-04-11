mod Program

use Core.Bool
use Core.List
use Core.IO
use Core.Util

-- Player data type
Player =
	| PlayerX
	| PlayerO
	| NoPlayer

inst Show Player =
	show =
		| PlayerX  -> "X"
		| PlayerO  -> "O"
		| NoPlayer -> "."

opponent =
	| PlayerX -> PlayerO
	| PlayerO -> PlayerX
	| _       -> Undefined

-- Move data type
Move =
	col :: Int
	row :: Int

-- Board logic
newBoard = newList 9 NoPlayer
fullBoard = newList 9 PlayerX

getBoard board move = (get position board).unsafe
	position = (move.row * 3) + (move.col)

setBoard board player move = set position player board
	position = (move.row * 3) + (move.col)

isEmpty board move = rawEq (getBoard board move) NoPlayer

isFull board = not (any (map (i -> rawEq (board@i.unsafe) NoPlayer) (0..8)))

printBoard board = {
	let prCell i = (board@i).unsafe.show.putstr
	foreach prCell (0..2)
	putstrln ""
	foreach prCell (3..5)
	putstrln ""
	foreach prCell (6..8)
	putstrln ""
	putstrln ""

winLines = [[0,1,2], [3,4,5], [6,7,8], [0,3,6], [1,4,7], [2,5,8], [0,4,8], [2,4,6]]

-- Game logic
playGame = () -> {
	playTurn PlayerX newBoard

playTurn player board = {
	putstrln ((show player) ++ "s turn.")
	let move = getMove board
	let nextBoard = setBoard board player move
	printBoard nextBoard
	if not (didPlayerWin player nextBoard)
		then if not (isFull nextBoard)
			then playTurn (player.opponent) nextBoard
			else {
				putstrln "Its a draw!"
		else {
			putstrln ((show player) ++ " wins!!!") 

didPlayerWin player board = {
	let line xs = all (map (i -> rawEq (board@i.unsafe) player) xs)
	any (map line winLines)

-- User input
getMove board = match tryGetMove()
	| Some move -> if isEmpty board move
		then move
		else {
			putstrln "Choose an empty cell."
			getMove board
	| None      -> {
		putstrln "Invalid move."
		getMove board

tryGetMove = () -> {
	let move = prompt "Enter your move (0 - 8): "
	match parseInt move
		| None -> None
		| Some i -> if (i >= 0) and (i < 9)
			then Some (Move (i mod 3) (i / 3))
			else None

unsafe =
	| Some x -> x
	| None   -> Undefined


