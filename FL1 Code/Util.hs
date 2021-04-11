mod Core.Util

x (.) f = f x

f (>>) g = x -> g (f x)