﻿#I "../../bin/net40"
#r "Argu.dll"
#r "Argu.Tests.dll"

open System
open Argu
open Argu.Tests

type MyEnum =
    | First
    | Second
    | Third

type LogLevel =
    | Info
    | Warning
    | Error

type PushArgs =
    | All
    | Prune
    | [<AltCommandLine("-f")>] Force
    | [<AltCommandLine("-v")>] Verbose
    | [<ExactlyOnce; MainCommand>] Remote of repository_name:string * branch_name:string
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | All -> "Push all branches (i.e. refs under refs/heads/); cannot be used with other <refspec>."
            | Prune -> "Remove remote branches that don't have a local counterpart."
            | Verbose -> "Run verbosely."
            | Force -> "Usually, the command refuses to update a remote ref that is not an ancestor of the local ref used to overwrite it."
            | Remote _ -> "Specify a remote repository and branch to push to."

[<CliPrefix(CliPrefix.Dash)>]
type CleanArgs =
    | D
    | [<AltCommandLine("--force")>]F
    | X
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | D -> "Remove untracked directories in addition to untracked files"
            | F -> "If the Git configuration variable clean.requireForce is not set to false, git clean will refuse to delete files or directories unless given -f."
            | X -> "Remove only files ignored by Git. This may be useful to rebuild everything from scratch, but keep manually created files."

type GitArgs =
    | [<Unique>] Listener of address:string * number:int
    | Log_Level of LogLevel
    | [<CliPrefix(CliPrefix.None)>] Push of options:ParseResults<PushArgs>
    | [<CliPrefix(CliPrefix.None)>] Clean of suboptions:ParseResults<CleanArgs>
    | [<AltCommandLine("-E")>][<EqualsAssignment>] Environment_Variable of key:string * value:string
    | Ports of tcp_port:int list
    | Optional of num:int option
    | Options of MyEnum option
    | [<Inherit>] Silent
with 
    interface IArgParserTemplate with 
        member this.Usage =
            match this with
            | Listener _ -> "Specify a listener host/port combination."
            | Log_Level _ -> "Specify a log level for the process."
            | Push _ -> "Pushes changes to remote repo. See 'gadget push --help' for more info."
            | Clean _ -> "Cleans the local repo. See 'gadget clean --help' for more info."
            | Environment_Variable _ -> "Specifies an environment variable for the process."
            | Ports _ -> "Specifies a collection of port for the process."
            | Optional _ -> "just an optional parameter."
            | Options _ -> "enumeration of options."
            | Silent -> "just be silent."

let parser = ArgumentParser.Create<GitArgs>(programName = "gadget", helpTextMessage = "Gadget -- my awesome CLI tool", errorHandler = ProcessExiter(fun _ -> ConsoleColor.))

parser.PrintCommandLineArgumentsFlat [Options(Some MyEnum.First) ; Push(toParseResults [Remote ("origin", "master")])]

let result = parser.Parse [| "--options?second" ; "--ports" ; "1" ; "2" ; "3" ; "push" ; "origin" ; "master" |]
let cresult = result.GetResult <@ Push @>

let pparser = parser.GetSubCommandParser <@ Push @>
let cparser = parser.GetSubCommandParser <@ Clean @>

parser.PrintUsage() |> Console.WriteLine
pparser.PrintUsage() |> Console.WriteLine
cparser.PrintUsage() |> Console.WriteLine

parser.PrintCommandLineSyntax(usageStringCharacterWidth = 10) |> Console.WriteLine
parser.PrintCommandLineSyntax(usageStringCharacterWidth = 100) |> Console.WriteLine
cparser.PrintCommandLineSyntax() |> Console.WriteLine

open System
open System.Text


let text = "Sed \nut perspiciatis, unde\n omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam eaque ipsa, quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt, explicabo. Nemo enim ipsam voluptatem, quia voluptas sit, aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos, qui ratione voluptatem sequi nesciunt, neque porro quisquam est, qui dolorem ipsum, quia dolor sit amet consectetur adipisci[ng] velit, sed quia non numquam [do] eius modi tempora inci[di]dunt, ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit, qui in ea voluptate velit esse, quam nihil molestiae consequatur, vel illum, qui dolorem eum fugiat, quo voluptas nulla pariatur?"

wordwrap text 60 "\n"