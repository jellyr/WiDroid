﻿module NetworkingCommon

open System
open System.IO
open System.Net
open System.Net.NetworkInformation
open System.Net.Sockets


let connectionIsStillActive (client:TcpClient) =
    let ipProperties = IPGlobalProperties.GetIPGlobalProperties ()
    let allTcpConnections = ipProperties.GetActiveTcpConnections ()
    let relevantTcpConnections = Array.filter (fun (connectionInfo:TcpConnectionInformation) -> 
        (connectionInfo.LocalEndPoint = (client.Client.LocalEndPoint :?> IPEndPoint)) && (connectionInfo.RemoteEndPoint = (client.Client.RemoteEndPoint :?> IPEndPoint))) allTcpConnections
        
    try
        let stateOfConnection = (Array.get relevantTcpConnections 0).State
        match stateOfConnection with
        | TcpState.Established ->
            true
        | _ ->
            false
    with
    | :? System.IndexOutOfRangeException as ex ->
        false

let readStreamToFile (stream:NetworkStream) outputPath =
    let rec readStreamToFileLoop (fileStream:FileStream) buffer =
        try
            let bytesRead = stream.Read(buffer, 0, buffer.Length)
            match bytesRead > 0 with
            | true ->
                fileStream.Write(buffer, 0, bytesRead)
                readStreamToFileLoop fileStream buffer
            | false ->
                ()
        with
        | :? IOException as ex ->
            let socketException =
                match ex.InnerException with
                | :? SocketException as scktEx ->
                    scktEx
                | _ ->  // The inner exception is not the expected socket exception, raise the original exception
                    raise ex    
            match socketException.ErrorCode with
            | 10060 ->  // Timeout Exception, nothing more to read
                ()
            | _ ->  // Unexpected error code, raise the original exception    
                raise ex      
        
    let buffer = Array.zeroCreate 1024
    let fileStream = File.Open(outputPath, FileMode.Create)
    try
        readStreamToFileLoop fileStream buffer
    finally
        fileStream.Close ()