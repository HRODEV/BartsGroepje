module Coroutines

// Coroutine and coroutinestep
type Coroutine<'a, 's> = 's -> CoroutineStep<'a, 's>
    and CoroutineStep<'a, 's> =
    |   Done of 'a * 's
    |   Wait of Coroutine<'a, 's> * 's

// Bind
let rec (>>) p k =
    fun s ->
        match p s with
        | Done(a, s') -> k a s'
        | Wait(leftOver, s') -> Wait((leftOver >> k), s')

// Computation expression builder
type CoroutineBuilder() =
    member this.Return(x) = fun s -> Done(x, s)
    member this.Bind(p, k) = p >> k
    member this.ReturnFrom c = c
    member this.Zero () = fun s -> Done((), s)

let co = CoroutineBuilder()

// Get the state inside the computation expression
let getState = fun s -> Done(s, s)

// Yield
let yield_ = fun s -> Wait((fun s -> Done((), s)), s)

// Repeats the given coroutine
let rec repeat_ c =
    co {
        do! c
        return! repeat_ c
    }

// Recursive looping coroutine. Keeps going until end of computation
let rec costep coroutine state =
  match coroutine state with
  | Done(_, newState) ->  (fun s -> Done((), s)), newState
  | Wait(c', s') -> costep c' s'

// Single step coroutine. Continues until it finds a yield, then returns the yielded function.
let singlestep coroutine state =
  match coroutine state with
      | Done(_, newState) ->  (fun s -> Done((), s)), newState
      | Wait(c', s') -> c', s'