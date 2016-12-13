# ld37
Programming game with assembly like language

The objective of the game is to program your bot and build a level so he can get to the top!

##Instructions:

Press Q/W to move between possible block colors, press left mouse button to add a new block of the selected color, and right mouse button to delete blocks.

Press E to enter the text editor and edit the bot's code.

##Language reference:

- nop: no operation (sleeps for the tick time)
- jump: makes the bot jump (if grounded)
- turn: turns the bot around
- cmp [what] [value]: compares what to the value passed, setting the robots internal flag if the comparison is true, what can be either:
 - scan: with possible values (r, g, b, w). Compares the value of the bot's scanner to the value. (if no value passed, compares if the scanner is reading something).
 - mdir: with possible values (r, l). Compares the moving direction of the bot.
- jmp [line]: jumps to the specified line of code
- jf [line]: jumps to the specified line of code if the flag is set to true
- jnf [line]: jumps to the specified line of code if the flag is set to false
- clf: sets the value of the flag to false
Good Luck!
