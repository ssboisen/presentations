#load "Utils.fsx"
open Utils

type Leadership =
    | CEO of string * Leadership list
    | CFO of string
    | Manager of string * Resource list
and Resource =
    | Teamlead of string * Resource list
    | Programmer of string
    | TechnicalEvangelist of string


let printOrganization leadership =
    let rec printResources indent = function
        | Teamlead(name, resources) -> 
            printfni indent "Guy who likes guys who likes computers: %s" name
            List.iter (printResources (indent + 1)) resources
        | TechnicalEvangelist(name)
        | Programmer(name) ->
            printfni indent "Guy who likes computers: %s" name

    let rec printLeadership indent = function
        | CEO(name, puppets) -> 
            printfni indent "CEO: %s" name
            List.iter (printLeadership (indent + 1)) puppets
        | CFO(name) ->
            printfni indent "CFO: %s" name
        | Manager(name, resources) -> 
            printfni indent "Manager: %s" name
            List.iter (printResources (indent + 1)) resources

    printLeadership 0 leadership

let d60 = CEO("Brian",
                [ Manager("Niels", 
                        [ Teamlead("Martin", 
                            [ Programmer("Simon"); 
                              Programmer("Dovydas") ] );
                          TechnicalEvangelist("Mogens")
                          ]);
                  CFO("Henrik")
                  ])

printOrganization d60