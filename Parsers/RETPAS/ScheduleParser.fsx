module RETPAS_parser
open System
printfn "Please enter the path to the RET.PAS file you wish to scan:"
let fileLocation = Console.ReadLine()

let file = new IO.StreamReader(fileLocation)
let readLine (x: IO.StreamReader) = x.ReadLine()

//Ignore the first two lines
readLine file |> ignore
readLine file |> ignore

let StringToList str = List.map (fun x -> x.ToString()) (List.ofSeq str)
let rec LsToStr ls =
    match ls with
    | h :: t -> h + (LsToStr t)
    | _ -> " "

type halte =
    | BeginHalte of (string * int)
    | EindHalte of (string * int)
    | TussenHalte of (string * int)
    | WachtHalte of (string * int * int)

let SplitHalte (str: string) x =
    match List.ofArray (str.Split(',')), x with
    | h :: t, ">" -> BeginHalte (h, t.Head |> int)
    | h :: t, "." -> TussenHalte (h, t.Head |> int)
    | h :: t, "<" -> EindHalte (h, t.Head |> int)
    | h :: t, "+" -> WachtHalte (h, t.Head |> int, t.Tail.Head |> int)
    | _, _ -> failwith "Incorrect format thrown into SplitHalte"


let rec SplitLijnRit (str: string) =
    let x = List.ofArray (str.Split(','))
    (x.[0], x.[1], x.[2], x.[3], x.[4])

type rit =
    {
        lijnnummer  :   int
        richting    :   int
        ritnummer   :   int
        haltes      :   halte list
        starthalte  :   halte
        eindhalte   :   halte
        dagcode     :   int
        typecode    :   string
    } with
    static member Zero =
        {
            lijnnummer = 0;
            richting = 0;
            ritnummer = 0;
            haltes = [];
            starthalte = BeginHalte("", 0)
            eindhalte = EindHalte("", 0)
            dagcode = 0;
            typecode = "";
        }

type tokens =
    | Lijnrit of string * string * string * string * string
    | Dagcode of string
    | Starthalte of halte
    | Tussenhalte of halte
    | Eindhalte of halte

let CastLine (str: string) =
    let strLst = List.map (fun x -> x.ToString()) (List.ofSeq str)
    match strLst with
    // Add splitting here
    | h :: t when h = "#" -> Lijnrit (SplitLijnRit (LsToStr t))
    | h :: t when h = ">" -> Starthalte (SplitHalte (LsToStr t) h)
    | h :: t when h = "." -> Tussenhalte (SplitHalte (LsToStr t) h)
    | h :: t when h = "<" -> Eindhalte (SplitHalte (LsToStr t) h)
    | h :: t when h = "-" -> Dagcode (LsToStr t)
    | h :: t when h = "+" -> Tussenhalte (SplitHalte (LsToStr t) h)
    | _ -> failwith ("Passed wrong line to CastLine: " + str)

let rec GenerateRit file (acc: rit) =
    let l = readLine file
    if l <> null then
        match CastLine l with
        | Eindhalte halte                                       -> Some { acc with eindhalte = halte; haltes = List.rev (halte :: acc.haltes) }
        | Starthalte halte                                      -> GenerateRit file { acc with starthalte = halte; haltes = [halte] }
        | Tussenhalte halte                                     -> GenerateRit file { acc with haltes = (halte :: acc.haltes) }
        | Dagcode code                                          -> GenerateRit file { acc with dagcode = Int32.Parse code }
        | Lijnrit (code, _, lijnnummer, richting, ritnummer)    -> GenerateRit file { acc with typecode = code; lijnnummer = Int32.Parse lijnnummer; richting = Int32.Parse richting; ritnummer = Int32.Parse ritnummer; }
    else
        None

let rec eraseLine acc x =
    eraseLine "\b \b" (x-1)

let GenerateRitten file =
    let rec looper (acc: rit list) (z: int) =
        printfn "At number: %A" z
        match (GenerateRit file rit.Zero) with
        | Some x -> looper (x :: acc) (z+1)
        | None -> List.rev acc
    looper [] 0

let ritten = fun () -> List.filter (fun x -> x.typecode = "M") (GenerateRitten file)
