﻿module Coroutines

type Coroutine<'a, 's> = 's -> CoroutineStep<'a, 's>
    and CoroutineStep<'a, 's> = 
    |   Done of 'a * 's
    |   Wait of Coroutine<'a, 's> * 's

let rec (>>) p k =
    fun s ->
        match p s with
        | Done(a, s') -> k a s'
        | Wait(leftOver, s') -> Wait((leftOver >> k), s')

type CoroutineBuilder() = 
    member this.Return(x) = (fun s -> Done(x, s))
    member this.Bind(p, k) = p >> k

let co = CoroutineBuilder()

let getState = fun s -> Done(s, s)
let yield_ = fun s -> Wait((fun s -> Done((), s)), s)

let rec costep coroutine state =
  match coroutine state with
  | Done(_, newState) ->  (fun s -> Done((), s)), newState
  | Wait(c', s') -> costep c' s'