
### LAST UPDATED: 29th January 2017

# Domoticz wifi bridge v6 Code

[https://github.com/domoticz/domoticz](https://github.com/domoticz/domoticz)
The sample code for wifi bridge v6 is here:
[https://github.com/domoticz/domoticz/blob/master/hardware/Limitless.cpp](https://github.com/domoticz/domoticz/blob/master/hardware/Limitless.cpp)

# LimitlessLED Wifi Bridge v6.0 VB.NET Sample code

## VB.NET


    Imports System.Net
    Imports System.Net.Sockets
    Imports System.Text.Encoding

    Private LimitlessLEDWifiBridgeSessionID1 As Byte = 0
    Private LimitlessLEDWifiBridgeSessionID2 As Byte = 0
    Private SequenceNumber As Byte = 0

    'Usage:  SendLightCommand(txtV6cmd01.Text, txtIP2.Text, txtCommandUDPPort.Text, txtZone.Text, "Waiting for response...", 500, "", 10, True)

    Private Sub SendLightCommand(ByVal v6CommandString As String, ByVal sendIPAddress As String, ByVal sPortNumber As String, ByVal sZone As String, ByVal WaitText As String, ByVal ReceiveTimeout As Integer, ByVal ValidResponse As String, ByVal Retries As Integer, ByVal WaitForResponse As Boolean)

            Dim iPortNum As Integer = CInt(sPortNumber)

            If udpAdmin Is Nothing Then
                udpAdmin = New UdpClient(iPortNum)
                udpAdmin.EnableBroadcast = True
                udpAdmin.DontFragment = True
            Else
                Throw New Exception("udpAdmin was already initialised")
            End If

            Dim sendEndPoint = New IPEndPoint(IPAddress.Parse(sendIPAddress), iPortNum)
            Dim listenEndPoint = New IPEndPoint(IPAddress.Parse(sendIPAddress), iPortNum)
            Dim StartSessionCommandString As String = "20 00 00 00 16 02 62 3A D5 ED A3 01 AE 08 2D 46 61 41 A7 F6 DC AF D3 E6 00 00 1E"
            Dim b As Byte() = StringToBytes(StartSessionCommandString)
            udpAdmin.Send(b, b.Length, sendEndPoint)
            udpAdmin.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000)

            Try
                  Do While iFoundWifiBridges < iLookForNumberOfWifiBridges


                    receiveBytes = udpAdmin.Receive(listenEndPoint) 'wait here until a UDP packet has been received. TIMEout if no response received.

                    If receiveBytes.Length > 0 AndAlso receiveBytes(0) = (b(0) + 8) AndAlso receiveBytes.Length = (receiveBytes(4) + 5) Then
                        'if the first byte sent on the command is 0x10 then expect response of 0x18 (+8)
                        'if the first byte sent on the command is 0x20 then expect response of 0x28 (+8)

                        UpdateCommandsResponseMessage("Received: " &#038; ByteArrayToString(receiveBytes)) 'ByteArrayToHexString(receiveBytes))
                       ' UpdateCommandsResponseMessage("Received bytes " &#038; CUInt(receiveBytes(4)))

                        If Not chkFreezeSessionID.Checked Then
                            LimitlessLEDWifiBridgeSessionID1 = receiveBytes(19) '20th response byte
                        End If
                        UpdateCommandsResponseMessage("LimitlessLEDWifiBridgeSessionID1 is " &#038; ByteToHexString(LimitlessLEDWifiBridgeSessionID1))

                        If Not chkFreezeSessionID.Checked Then
                            LimitlessLEDWifiBridgeSessionID2 = receiveBytes(20) '21st response byte
                        End If
                        UpdateCommandsResponseMessage("LimitlessLEDWifiBridgeSessionID2 is " &#038; ByteToHexString(LimitlessLEDWifiBridgeSessionID2))

                        'could be broadcast, so we don't know how many bridges might respond
                        UpdateCommandsResponseMessage("IP Address is " &#038; listenEndPoint.Address.ToString())

                        'The mac address is returned from the wifi bridge.
                        Dim macAddressFormatted As String = ByteToHexString(receiveBytes(8)) &#038; ":" &#038; ByteToHexString(receiveBytes(9)) &#038; ":" &#038; ByteToHexString(receiveBytes(10)) &#038; ":" &#038; ByteToHexString(receiveBytes(11)) &#038; ":" &#038; ByteToHexString(receiveBytes(12)) &#038; ":" &#038; ByteToHexString(receiveBytes(13))
                        Dim macAddressRaw As String = macAddressFormatted.Replace(":", "")
                        UpdateCommandsResponseMessage("MAC Address is " &#038; macAddressFormatted)


SendCommand:

                        '80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 01(zone) 00 3F(chksum) response: (88 00 00 00 03 00 SN 00) 
                        Dim sendFullcommand As String = "80 00 00 00 04 05 06 00 08 00 " &#038; v6CommandString &#038; " 19 00 21"
                        Dim cmd As Byte() = StringToBytes(sendFullcommand)
                        cmd(4) = cmd.Length - 5 'set packet length (excluding 5 packet header bytes)
                        cmd(5) = LimitlessLEDWifiBridgeSessionID1 'session byte 1
                        cmd(6) = LimitlessLEDWifiBridgeSessionID2 'session byte 2

                        If Not chkFreezeSequenceNumber.Checked Then
                            If SequenceNumber = 255 Then
                                SequenceNumber = 0
                            Else
                                SequenceNumber = SequenceNumber + 1
                            End If
                        End If

                        cmd(8) = SequenceNumber
                        cmd(19) = CByte(sZone)
                        'Calculate Checksum:
                        Dim iChecksum As Integer
                        Dim bChecksum As Byte
                        For i As Integer = 10 To 20 '11 bytes
                            iChecksum += cmd(i)
                        Next
                        bChecksum = CByte(iChecksum And &#038;HFF)
                        cmd(21) = bChecksum

                        UpdateCommandsResponseMessage("Sequence Number is " &#038; ByteToHexString(SequenceNumber))
                        UpdateCommandsResponseMessage("Checksum is " &#038; ByteToHexString(bChecksum))

                        udpAdmin.Send(cmd, cmd.Length, sendEndPoint)
                        'udpAdmin.Send(cmd, cmd.Length, broadcastEP)

                        UpdateCommandsResponseMessage("Sent: " &#038; ByteArrayToString(cmd))

                        Do While iFoundWifiBridges < iLookForNumberOfWifiBridges
                            receiveBytes = udpAdmin.Receive(listenEndPoint) 'wait here until a UDP packet has been received. TIMEout if no response received.

                            If receiveBytes.Length > 0 AndAlso receiveBytes(0) = (cmd(0) + 8) AndAlso receiveBytes.Length = (receiveBytes(4) + 5) Then 'check header byte 0x88 and length byte 0x03 is correct (including the 5 packet header bytes that aren't counted)
                                UpdateCommandsResponseMessage("Command SUCCESSFUL.")
                                UpdateCommandsResponseMessage("Received: " &#038; ByteArrayToString(receiveBytes))
                                'response should be: 88 00 00 00 03 00 SN 00

                                Exit Do
                            End If
                        Loop

                        Exit Do
                    End If
                Loop

         Catch ex As System.Net.Sockets.SocketException

            Dim excp = ex.SocketErrorCode

            If excp = SocketError.TimedOut Then
                'TODO: retry if there is a timeout, chances are it will work the second time on UDP
                UpdateCommandsResponseMessage("Timeout waiting for Wifi Bridge Response.")
            Else
                UpdateCommandsResponseMessage(ex.Message)
            End If

        Catch ex As Exception

            UpdateCommandsResponseMessage(ex.Message)
        Finally

            Try
                udpAdmin.Client.Shutdown(SocketShutdown.Receive)
            Catch ex As Exception
                'UpdateStatusMessage("udpAdmin.Client.Shutdown(SocketShutdown.Receive) exception." &#038; ex.Message)
            End Try
            Try
                udpAdmin.Client.Close()
            Catch ex As Exception
                'UpdateStatusMessage("udpAdmin.Client.Close() exception." &#038; ex.Message)
            End Try
            Try
                udpAdmin.Close()
            Catch ex As Exception
                'UpdateStatusMessage("udpAdmin.Close() exception." &#038; ex.Message)
            End Try
            Try
                udpAdmin = Nothing
            Catch ex As Exception
                ' UpdateStatusMessage("udpAdmin = Nothing exception." &#038; ex.Message)
            End Try

        End Try

    End Sub

    Public Shared Function StringToBytes(ByVal hex As String) As Byte()
        hex = hex.Replace(" ", "")

        If hex.Length Mod 2 = 1 Then
            Throw New Exception("The hex string cannot have an odd number of digits")
        End If

        Dim arr As Byte() = New Byte((hex.Length >> 1) - 1) {}

        For i As Integer = 0 To (hex.Length >> 1) - 1
            arr(i) = CByte((GetHexVal(hex(i << 1)) << 4) + (GetHexVal(hex((i << 1) + 1))))
        Next

        Return arr
    End Function

    Public Shared Function GetHexVal(ByVal hex As Char) As Integer
        Dim val As Integer = AscW(hex)
        'For uppercase A-F letters:
        'Return val - (If(val < 58, 48, 55))
        'For lowercase a-f letters:
        'Return val - (If(val < 58, 48, 87))
        'Or the two combined, but a bit slower:
        Return val - (If(val < 58, 48, (If(val < 97, 55, 87))))
    End Function

    Private Sub UpdateStatusMessage(ByVal message As String)
        txtLog.Text &#038;= message.Replace(vbCr, "\r").Replace(vbLf, "\n") &#038; vbCrLf
        '\r = Cr = decimal13 = 0x0D = Carriage Return  
        '\n = Lf = decimal10 = 0x0A = Linefeed
        txtLog.SelectionStart = txtLog.Text.Length
        txtLog.ScrollToCaret()
        Application.DoEvents()
    End Sub

# LimitlessLED Wifi Bridge v6.0 Sample code

        'use strict';
        var dgram = require('dgram');

        var debug=false;

        var DEFAULT_HOST ;
        var DEFAULT_PORT ;
        var DEFAULT_REPEATS=3 ;

        var PREAMPLE = [0x80,0x00,0x00,0x00,0x11]

        var FILLER = 0x00

        var CMDS={
        ON: [0x31,0,0,0x08,0x04,0x01,0,0,0],
        OFF: [0x31,0,0,0x08,0x04,0x02,0,0,0],
        NIGHT: [0x31,0,0,0x08,0x04,0x05,0,0,0],
        WHITEON: [0x31,0,0,0x08,0x05,0x64,0,0,0],
        REG: [0x33,0,0,0,0,0,0,0,0,0],
        BON: [0x31,0x00,0x00,0x00,0x03,0x03,0x00,0x00,0x00],
        BOFF:[0x31,0x00,0x00,0x00,0x03,0x04,0x00,0x00,0x00],
        BWHITE:[0x31 ,0x00 ,0x00 ,0x00 ,0x03 ,0x05 ,0x00 ,0x00 ,0x00],
        };
        var socket;

        var zoneCtlRGBWFactory=function(zoneID){
        var color=0x7A;
            var brightness=0x32;
        var saturation=0x32;
        var colorTemp=0x4B;
        var zone=zoneID;
        if(zone > 4 || zone <0 ) console.log("invalid zone");

        return {
            on: function(){
            return [0x31,0,0,0x08,0x04,0x01,0,0,0,zoneID];
            },
            off: function(){
            return [0x31,0,0,0x08,0x04,0x02,0,0,0,zoneID];
            },
            nightMode: function() {
            return [0x31,0,0,0x08,0x04,0x05,0,0,0,zoneID];
            },
            whiteMode:function() {
            return [0x31,0,0,0x08,0x05,0x64,0,0,0,zoneID];
            },
            saturationUp:function(){
            saturation=Math.min(saturation+5,0x64);
            return [0x31,0x00,0x00,0x08,0x02,saturation,0x00,0x00,0x00,zoneID]
            },
            saturationDown:function(){
            saturation=Math.max(saturation-5,0x00);
            return [0x31,0x00,0x00,0x08,0x02,saturation,0x00,0x00,0x00,zoneID]
            },
            saturationSet:function(b){
            saturation=Math.max(b,0x00);
            saturation=Math.min(b,0x64);
            return [0x31,0x00,0x00,0x08,0x02,saturation,0x00,0x00,0x00,zoneID]
            },
        brightnessUp:function(){
            brightness=Math.min(brightness+5,0x64);
            return [0x31,0x00,0x00,0x08,0x03,brightness,0x00,0x00,0x00,zoneID]
            },
            brightnessDown:function(){
            brightness=Math.max(brightness-5,0x00);
            return [0x31,0x00,0x00,0x08,0x03,brightness,0x00,0x00,0x00,zoneID]
            },
            brightnessSet:function(b){
            brightness=Math.max(b,0x00);
            brightness=Math.min(b,0x64);
            return [0x31,0x00,0x00,0x08,0x03,brightness,0x00,0x00,0x00,zoneID]
            },
            colorUp:function(){
            color=Math.min(color+5,0xFF);
            return [0x31,0x00,0x00,0x08,0x01,color,color,color,color,zoneID]
            },
            colorDown:function(){
            color=Math.max(color-5,0x00);
            return [0x31,0x00,0x00,0x08,0x01,color,color,color,color,zoneID]
            },
            colorSet:function(c){
            color=c;
            return [0x31,0x00,0x00,0x08,0x01,color,color,color,color,zoneID]
            },
            colorRGB:function(rgb){
            return rgbHandler(rgb,this);
            },
            colorTempUp:function(){
            colorTemp=Math.min(colorTemp+5,0x64);
            return [0x31,0x00,0x00,0x08,0x05,colorTemp,0,0,0,zoneID]
            },
            colorTempDown:function(){
            colorTemp=Math.max(colorTemp-5,0x00);
            return [0x31,0x00,0x00,0x08,0x05,colorTemp,0,0,0,zoneID]
            },
            colorTempSet:function(c){
            colorTemp=c;
            return [0x31,0x00,0x00,0x08,0x05,colorTemp,0,0,0,zoneID]
            },
            mode:function(mode){
            return [0x31,0x00,0x00,0x08,0x06,mode,0,0,0,zoneID]
            },
            modeSpeedUp:function(){
            return [0x31,0,0,0x08,0x04,0x03,0,0,0,zoneID]
            },
            modeSpeedDown:function(){
            return [0x31,0,0,0x08,0x04,0x04,0,0,0,zoneID]
            },
            link:function(){
            return [0x3D,0,0,0x08,0,0,0,0,0,zoneID]
            },
            unlink:function(){
            return [0x3E,0,0,0x08,0,0,0,0,0,zoneID]
            },
            command(fnName,arg){
            if (this[fnName]) {
                var cmds=this[fnName](arg);
                if (Array.isArray(cmds) &#038;&#038; Array.isArray(cmds[0])){
                cmds.forEach(function(elem){
        sendCmd(elem)})
                } else {
                sendCmd(cmds);//single cmd`
                }
            }
            }
        }
        }

        var zoneCtlRGBWWFactory=function(zoneID){
        var color=0x7A;
            var brightness=0x32;
        //var saturation=0x32;
        //var colorTemp=0x4B;
        var zone=zoneID;
        if(zone > 4 || zone <0 ) console.log("invalid zone");

        return {
            on: function(){
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x01, 0x00, 0x00, 0x00, zoneID];
            },
            off: function(){
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x02, 0x00, 0x00, 0x00, zoneID];
            },
            nightMode: function() {
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x06, 0x00, 0x00, 0x00, zoneID];
            },
            whiteMode:function() {
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x05, 0x00, 0x00, 0x00, zoneID];
            },
            brightnessUp:function(){
            brightness=Math.min(brightness+5,0x64);
            return [0x31, 0x00, 0x00, 0x07, 0x02, brightness, 0x00, 0x00, 0x00, zoneID]
            },
            brightnessDown:function(){
            brightness=Math.max(brightness-5,0x00);
            return [0x31, 0x00, 0x00, 0x07, 0x02, brightness, 0x00, 0x00, 0x00, zoneID]
            },
            brightnessSet:function(b){
            brightness=Math.max(b,0x00);
            brightness=Math.min(b,0xFF);
            return [0x31, 0x00, 0x00, 0x07, 0x02, brightness, 0x00, 0x00, 0x00, zoneID]
            },
            colorUp:function(){
            color=Math.min(color+5,0xFF);
            return [0x31, 0x00, 0x00, 0x07, 0x01, color, color, color, color, zoneID]
            },
            colorDown:function(){
            color=Math.max(color-5,0x00);
            return [0x31, 0x00, 0x00, 0x07, 0x01, color, color, color, color, zoneID]
            },
            colorSet:function(c){
            color=c;
            return [0x31, 0x00, 0x00, 0x07, 0x01, color, color, color, color, zoneID]
            },
            colorRGB:function(rgb){
            return rgbHandler(rgb,this);
            },
            mode:function(mode){
            return [0x31, 0x00, 0x00, 0x07, 0x04, mode, 0x00, 0x00, 0x00, zoneID]
            },
            modeSpeedUp:function(){
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x03, 0x00, 0x00, 0x00, zoneID]
            },
            modeSpeedDown:function(){
            return [0x31, 0x00, 0x00, 0x07, 0x03, 0x04, 0x00, 0x00, 0x00, zoneID]
            },
            link:function(){
            console.log("link not captured");
            return [0x3D,0,0,0x07,0,0,0,0,0,zoneID]
            },
            unlink:function(){
            console.log("link not captured");
            return [0x3E,0,0,0x07,0,0,0,0,0,zoneID]
            },
            command(fnName,arg){
            if (this[fnName]) {
                var cmds=this[fnName](arg);
                if (Array.isArray(cmds) &#038;&#038; Array.isArray(cmds[0])){
                cmds.forEach(function(elem){sendCmd(elem)})
                } else {
                sendCmd(cmds);//single cmd`
                }
            }
            }
        }
        }

        var baseCtlFactory=function(){
            var color=0x7A;
            var brightness=0x32;
        var zoneID=0x01;
            return {
                on:function(){return [0x31,0x00,0x00,0x00,0x03,0x03,0x00,0x00,0x00,zoneID]},
                off:function(){return [0x31,0x00,0x00,0x00,0x03,0x04,0x00,0x00,0x00,zoneID]},
                whiteMode:function(){return [0x31 ,0x00 ,0x00 ,0x00 ,0x03 ,0x05 ,0x00 ,0x00 ,0x00,zoneID]},
                brightnessUp:function(){
                    brightness=Math.min(brightness+5,0x64);
                    return [0x31,0x00,0x00,0x00,0x02,brightness,0x00,0x00,0x00,zoneID]
                },
                brightnessDown:function(){
                    brightness=Math.max(brightness-5,0x00);
                    return [0x31,0x00,0x00,0x00,0x02,brightness,0x00,0x00,0x00,zoneID]
                },
                brightnessSet:function(b){
                    brightness=Math.max(b,0x00);
                    brightness=Math.min(b,0xFF);
                    return [0x31,0x00,0x00,0x00,0x02,brightness,0x00,0x00,0x00,zoneID]
                },
                colorUp:function(){
                    color=Math.min(color+5,0xFF);
                    return [0x31,0x00,0x00,0x00,0x01,color,color,color,color,zoneID]
                },
                colorDown:function(){
                    color=Math.max(color-5,0x00);
                    return [0x31,0x00,0x00,0x00,0x01,color,color,color,color,zoneID]
                },
                colorSet:function(c){
                    color=c;
                    return [0x31,0x00,0x00,0x00,0x01,color,color,color,color,zoneID]
                },
            colorRGB:function(rgb){
            return rgbHandler(rgb,this);
            },
                mode:function(mode){
                    return [0x31,0x00,0x00,0x00,0x04,mode,0,0,0,zoneID]
                },
            command(fnName,arg){
            if (this[fnName]) {
                var cmds=this[fnName](arg);
                if (Array.isArray(cmds) &#038;&#038; Array.isArray(cmds[0])){
                cmds.forEach(function(elem){sendCmd(elem)})
                } else {
                sendCmd(cmds);//single cmd`
                }
            }
            }
            }

        };

        var bridgeID;
        var bridgeID2;
        var seqNum=0x02;

        var sendCmd = function(CMD,repeats){
            if (typeof repeats === 'undefined') { repeats = DEFAULT_REPEATS};

            var out=[];
            //console.log("#"+WB.toString('hex')+"-"+CMD.toString("hex"));
            out=out.concat(PREAMPLE,bridgeID,bridgeID2,0x00,seqNum,FILLER,CMD)
            var chkSum=calcCheckSum(out);
            out =	out.concat(chkSum);
            //console.log(JSON.stringify(out));
            if (debug) console.log("#"+out.toString('hex'));
            out=new Buffer(out);
            seqNum=(seqNum+1)%256;
            if(debug) console.log("Sending: " + out.toString('hex'));
            for (var i=0;i<repeats>< b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
            }

            h /= 6;
        }
        return [ Math.floor(h*0xFF), Math.floor(s*100), Math.floor(v*100) ];
        }



        var buildFrame = function(WB,WB2,CMD,ZONE){
            var out=[];
            //console.log("#"+WB.toString('hex')+"-"+CMD.toString("hex"));
            if (debug) console.log("WB: "+ WB.toString("hex")+ " "+WB2.toString("hex") +" CMD: "+CMD.toString("hex"))
            out=out.concat(PREAMPLE,WB,WB2,0x00,seqNum,FILLER,CMD,ZONE)
            var chkSum=calcCheckSum(out);
            out =	out.concat(chkSum);
            if (debug) console.log("out: "+ (new Buffer(out)).toString('hex'))
            //console.log(JSON.stringify(out));
            //console.log("#"+out.toString('hex'));
            seqNum=(seqNum+1)%256;
            return new Buffer(out);
        }

        var sendFrame=function(payload){
            if(debug) console.log("Sending: " + payload.toString('hex'));
            socket.send(payload,0,payload.length,DEFAULT_PORT,DEFAULT_HOST,function(){});
        }

        var sendKeepAlive=function(){
            var out=new Buffer([0xD0,0x00,0x00,0x00,0x02,bridgeID,bridgeID2]);
            sendFrame(out);
        }

        var calcCheckSum=function(aFrame){

        var add=function(a,b){
            return a+b;
        };

        var sub = aFrame.slice(Math.max(aFrame.length - 11, 0)) ;
        var val=sub.reduce(add,0)
        var val1=Math.floor(val / 0xff)
        var val2=val % 0xff
        return [val1, val2]

        }

        var _func={};

        _func['2800000011']=function(msg){
        //response to initiate
            var unknown1=msg.slice(5,7);
            var mac=msg.slice(7,13);
            var fixed=msg.slice(13,15);
            var unknown2=msg.slice(15,19);
            var counter=msg.slice(19,20);
            var counter2=msg.slice(20,21);
            var padding=msg.slice(20);
        if (debug) { 
            console.log("0:" +msg.toString('hex'));
            console.log("1:" +unknown1.toString('hex'));
            console.log("2:" +mac.toString('hex'));
            console.log("2a:" +fixed.toString('hex'));
            console.log("3:" +unknown2.toString('hex'));
            console.log("4:" +counter.toString('hex'));
            console.log("5:" +padding.toString('hex'));
        }
            bridgeID=new Uint8Array(counter);
            bridgeID2=new Uint8Array(counter2);
        //80:00:00:00:11:c1:01:00:0b:00:33:00:00:00:00:00:00:00:00:00:00:33

        //complete initiation
            if (debug) console.log("BridgeID: "+ bridgeID.toString("hex"))
            if (debug) console.log("BridgeID2: "+ bridgeID2.toString("hex"))
            var nFrame=buildFrame(bridgeID,bridgeID2,[0x33,0,0,0,0,0,0,0,0,0],0x00);
            sendFrame(nFrame);

        //start keepalive
            setInterval(sendKeepAlive,10000);
        };

        _func['8800000003']=function(msg){
            //ERROR - Confirmation?
            var code= msg.slice(0,5);
            var unknown1 = msg.slice(5,8);
            if (debug) console.log("0:" +msg.toString('hex'));
            if (debug) console.log("1:" +unknown1.toString('hex'));
            if (debug) console.log("ID:"+bridgeID.toString('hex'));
        }

        _func['d800000007']=function(){
            //keepalive response
        }
        var socket=dgram.createSocket({type:'udp4',reuseAddr:true});

        socket.on("message", (msg, rinfo) => {
        //console.log('Received %d bytes from %s:%d\n',
        //            msg.length, rinfo.address, rinfo.port);
        var hmsg=msg.toString('hex');
        var resp=(msg.toString('hex').substring(0,10));
        if (_func[resp]) {
            _func[resp](msg)
        } else {
            console.log("Unknown code");
            console.log(hmsg);
        }
        });

        var initiate=function(host,port){
            //socket.send(payload,0,payload.length,DEFAULT_PORT,DEFAULT_HOST,function(a,b){});
            DEFAULT_HOST=host;
            DEFAULT_PORT=port;
        //socket.bind();
        //socket.bind(DEFAULT_PORT);
        var payload=new Buffer([0x20,0x00,0x00,0x00,0x16,0x02,0x62,0x3a,0xd5,0xed,0xa3,0x01,0xae,0x08,0x2d,0x46,0x61,0x41,0xa7,0xf6,0xdc,0xaf,0xfe,0xf7,0x00,0x00,0x1e]);
        if (debug) console.log(payload.toString('hex'));
            sendFrame(payload);
        };




            exports.initiate= initiate;
            exports.baseCtlFactory= baseCtlFactory;
            exports.zoneCtlRGBWFactory= zoneCtlRGBWFactory;
            exports.zoneCtlRGBWWFactory= zoneCtlRGBWWFactory;
            exports.sendCmd= sendCmd;
            </repeats>

# LimitlessLED Wifi Bridge v6.0
## Section 1. Manual Web Browser Wifi Bridge v6 Setup.

0. Connect Phone to Milight AccessPoint
1. Use Phone Web Browser to http://10.10.100.254
2. Username: admin
3. Password: admin
4. Work Mode: change to STA mode, press Save
5. STA Setting: Scan for your Home Wifi Router
6. Encryption Method: WPA2PSK
7. Encryption Algorithm: AES
8. Password:  enter your home wifi router password  here.
9. Obtain an IP address automatically:  Enable
10. Restart: Click Ok
11. Now put your phone back on your home wifi network, and open the app, it will see the wifi bridge, assuming you entered your wifi password correctly a few steps back.

##Section 2. Searching for LimitlessLED v6 Bridge/s on the LAN.

1. We have a Windows App for this [http://www.limitlessled.com/download/LimitlessLEDv6.zip">called LimitlessLED Admin tool v5/v6)
2. If you are building your own Program, use the following...
3. UDP.IP = "255.255.255.255"; // IP.Broadcast
4. UDP.PORT = 48899;
5. UDP.SendBytes(UTF8.GetBytes("HF-A11ASSISTHREAD")); // this is the broadcast string for the v6 Bridge (for v5 bridges use the string "Link_Wi-Fi")
6. receiveBytes = UDP.Receive();
7. response = UTF8.GetString(receiveBytes); //each wifi bridge responds with one response at time. so call receive again until 1 second is up. 
8. // returns a string containing: IP address of the wifi bridge, the unique MAC address, and the name(which is always the same for v6, and the name is empty for v5 bridges) there is always two commas present regardless of v5 or v6 wifi bridge.
9. //  10.1.1.27,ACCF23F57AD4,HF-LPB100
10. //  10.1.1.31,ACCF23F57D80,HF-LPB100

## Section 3. Wifi Bridge v6 Protocol - All values are in Hex

all values below are in hex, 0xFF  means 255 in decimal aka FF in hex, aka 1 byte of FF, aka 8 bits of 11111111.

    UDP.IP = "255.255.255.255"; (or direct Wifi Bridge IP Address)
    UDP.PORT = 5987; //(decimal integer 5987)
    UDP.SEND (hex bytes, see below)

SN = Sequential Byte
Sequential byte just helps with keeping commands in the correct order, and it helps to ignore duplicate packets already received. increment this byte for each new command by 1.

    WB1 = LimitlessLED Wifi Bridge Session ID1
    WB2 = LimitlessLED Wifi Bridge Session ID2
    to get the WB1 and WB2 send this command
    UDP.SEND hex bytes: 20 00 00 00 16 02 62 3A D5 ED A3 01 AE 08 2D 46 61 41 A7 F6 DC AF (D3 E6) 00 00 1E  <-- Send this to the ip address of the wifi bridge v6
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (05 00)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 1)
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (17 01)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 2)
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (F2 00)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 3)
    WB1 = LimitlessLED Wifi Bridge Session ID1  = response[19] (20th byte of response above)
    WB2 = LimitlessLED Wifi Bridge Session ID2  = response[20] (21st byte of response above)

<b>RGBW/WW/CW Commands</b>

    UDP Hex Send Format: 80 00 00 00 11 {WifiBridgeSessionID1} {WifiBridgeSessionID2} 00 {SequenceNumber} 00 {COMMAND} {ZONE NUMBER} 00 {Checksum}
    UDP Hex Response: 88 00 00 00 03 00 {SequenceNumber} 00

<b>List of {COMMAND}s:</b>
format of {command} 9 byte packet = 0x31 {PasswordByte1 default 00} {PasswordByte2 default 00} {remoteStyle 08 for RGBW/WW/CW or 00 for bridge lamp} {LightCommandByte1} {LightCommandByte2} 0x00 0x00 0x00 {Zone1-4 0=All} 0x00 {Checksum}

    31 00 00 08 04 01 00 00 00 = Light ON
    31 00 00 08 04 02 00 00 00 = Light OFF
    31 00 00 08 04 05 00 00 00 = Night Light ON
    31 00 00 08 05 64 00 00 00 = White Light ON (Color RGB OFF)
    31 00 00 08 01 BA BA BA BA = Set Color to Blue (0xBA) (0xFF = Red, D9 = Lavender, BA = Blue, 85 = Aqua, 7A = Green, 54 = Lime, 3B = Yellow, 1E = Orange)
    31 00 00 08 02 SS 00 00 00 = Saturation (SS hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)
    31 00 00 08 03 BN 00 00 00 = BrightNess (BN hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)
    31 00 00 08 05 KV 00 00 00 = Kelvin (KV hex values 0x00 to 0x64 : examples: 00 = 2700K (Warm White), 19 = 3650K, 32 = 4600K, 4B, = 5550K, 64 = 6500K (Cool White))
    31 00 00 08 06 MO 00 00 00 = Mode Number (MO hex values 0x01 to 0x09 : examples: 01 = Mode1, 02 = Mode2, 03 = Mode3 .. 09 = Mode9)
    31 00 00 08 04 04 00 00 00 = Mode Speed Decrease--
    31 00 00 08 04 03 00 00 00 = Mode Speed Increase++
    3D 00 00 08 00 00 00 00 00 = Link (Sync Bulb within 3 seconds of lightbulb socket power on)
    3E 00 00 08 00 00 00 00 00 = UnLink (Clear Bulb within 3 seconds of lightbulb socket power on)

Wifi Bridge iBox LED Lamp {COMMAND}s (Zone Number = 0x01)

    31 00 00 00 03 03 00 00 00 = Wifi Bridge Lamp ON
    31 00 00 00 03 04 00 00 00 = Wifi Bridge Lamp OFF
    31 00 00 00 04 MO 00 00 00 = Mode Number (MO hex values 0x01 to 0x09 : examples: 01 = Mode1, 02 = Mode2, 03 = Mode3 .. 09 = Mode9)
    31 00 00 00 03 01 00 00 00 = Mode Speed Decrease--
    31 00 00 00 03 02 00 00 00 = Mode Speed Increase++
    31 00 00 00 01 BA BA BA BA = Set Color to Blue (BA) (FF = Red, D9 = Lavender, BA = Blue, 85 = Aqua, 7A = Green, 54 = Lime, 3B = Yellow, 1E = Orange)
    31 00 00 00 03 05 00 00 00 = Set Color to White (is ignored when Lamp is OFF, it does NOT turn the Lamp ON)
    31 00 00 00 02 BN 00 00 00 = BrightNess (BN hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)

<b>Valid List for {ZONE NUMBER}</b>

    0x00 All
    0x01 Zone1
    0x02 Zone2
    0x03 Zone3
    0x04 Zone4

<b>Checksum:</b>
RGBW/WW/CW Checksum Byte Calculation is the sum of the 11 bytes before end of the UDP packet. The checksum is then added to the end of the UDP message.
take the 9 bytes of the command, and 1 byte of the zone, and add the 0 = the checksum = (checksum &#038; 0xFF)
e.g. SUM((31 00 00 08 04 01 00 00 00)(command) 01(zone) 00) = 3F(chksum)

<b>EXAMPLES</b>

    LimitlessLED Wifi Bridge Light ON   80 00 00 00 11 WB 00 00 SN 00 (31 00 00 00 03 03 00 00 00)(cmd) 01(zone) 00 38(chksum)  UDP response: (88 00 00 00 03 00 SN 00)
    LimitlessLED Wifi Bridge Light OFF  80 00 00 00 11 WB 00 00 SN 00 (31 00 00 00 03 04 00 00 00)(cmd) 01(zone) 00 39(chksum)  UDP response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 01(zone) 00 3F(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone1 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 01(zone) 00 40(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 02(zone) 00 40(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone2 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 02(zone) 00 41(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 03(zone) 00 41(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone3 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 03(zone) 00 42(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 04(zone) 00 42(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone4 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 04(zone) 00 43(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW ZoneALL ON 80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 00(zone) 00 3E(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW ZoneALLOFF 80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 00(zone) 00 3F(chksum)  response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 01(zone) 00 46(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 02(zone) 00 47(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 03(zone) 00 48(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 04(zone) 00 49(chksum)  response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 01(zone) 00 47(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 02(zone) 00 48(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 03(zone) 00 49(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 04(zone) 00 4A(chksum)  response: (88 00 00 00 03 00 SN 00)

<b>Keep Alive Messages</b>

    KEEP ALIVES (Every 5 seconds) Wifi Bridge 1: D0 00 00 00 02 (WB) 00  (response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    KEEP ALIVES (Every 5 seconds) Wifi Bridge 1: D0 00 00 00 02 1D 00  (response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    KEEP ALIVES (Every 5 seconds) Wifi Bridge 2: D0 00 00 00 02 7C 00  (response: D8 00 00 00 07 (AC CF 23 F5 7D 80) 01)

<b>Click Search for Devices: </b>

    UDP.Send (255.255.255.255:5987) Bytes: 10 00 00 00 24 02 ee 3e 02 39 38 35 62 31 35 37 62 66 36 66 63 34 33 33 36 38 61 36 33 34 36 37 65 61 33 62 31 39 64 30 64

<b>GET ALL WIFI BRIDGE CLOUD KEYS on LAN using UDP</b>
   
    UDP.IP = "255.255.255.255"
    UDP.port = 5987
    UDP.SEND hex bytes: 10 00 00 00 0A 02 D3 E6 01 (AC CF 23 F5 7A D4)(MAC address)
    UDP.Response: 18 00 00 00 40 02 (AC CF 23 F5 7A D4)(mac) 00 20 (985b157bf6fc43368a63467ea3b19d0d)(ASCII Tokenkey) 01 00 01 17 63 00 00 05 00 09 (xlink_dev)(ASCII) 07 5B CD 15
    UDP.SEND hex bytes: 20 00 00 00 16 02 62 3A D5 ED A3 01 AE 08 2D 46 61 41 A7 F6 DC AF D3 E6 00 00 1E
    UDP.Response: 28 00 00 00 11(LENGTH) 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 05 00 00
    UDP.SEND hex bytes: D0 00 00 00 02 05(WB1) 00(WB2)
    UDP.Response: response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    example2: 10 00 00 00 0A 02 FE E7 01 (AC CF 23 F5 7A D4)(MAC address)
    example3: 10 00 00 00 0A 02 FE 51 01 (AC CF 23 F5 7D 80)(MAC address)

## Section 4. Smart Link Setup Services
* ** Warning only have one wifi bridge powered up at a time when setup using smart link (In case you have more than one in your home) **
* Smart-link is all about getting your brand new wifi bridge onto your home wifi router network.
* SmartLink saves the user time during the setup phase, from having to switch to the bridge in the wifi settings to set it up. Instead it is done using a sync button underneath the Wifi Bridge v6 and an app.
* If you want to build smart-link into your own app.
* SMART LINK UDP port 49999  Default Password "8888"
* UDP.IP = "10.1.1.255"
* UDP.PORT = 49999
* UDP.Send Whilst Waiting:  05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05

## Section 5. Cloud Remote Internet access Services
* If you want to build an app that controls the lights over the internet, more information will be made available here.
* [http://www.xlink.cn/](http://www.xlink.cn/)
* So that enterprises have the power of things. Cloud-to-cloud platform, easily and securely connected devices, rapid development of Internet of things applications,

In the Internet of Things data extraction value.
* Default Cloud Server: Amazon Web Services Cloud ec2-52-63-118-215.ap-southeast-2.compute.amazonaws.com [52.63.118.215] cm2.xlink.cn
* Default Cloud Port: 23778
* Cloud Server Login TCP.Send: 10 00 00 00 1A 02 4B 4B 59 DB 00 10 32 32 30 66 61 32 61 66 66 65 36 63 61 32 30 30 00 00 3C

* Cloud TCP.Read Login Response: 18 00 00 00 02 00 00
* Cloud Keep Alive: TCP.Send: D0 00 00 00 00
* Cloud Keep Alive: TCP.Read: D8 00 00 00 00
* Cloud TCP sent: A0 00 00 00 07 69 F0 3D F3 00 0B 00
* Cloud response: A8 00 00 00 13 69 F0 3D F3 00 0B 00 07 00 09 x l i n k _ d e v
* Cloud TCP sent:
* Cloud response: 6{"device_id":1777352179,"type":"offline","operator":0}

#  LimitlessLED v5.0 OpenSource API
SmartThings Integration 2016: [https://github.com/peeepers/MiThings](https://github.com/peeepers/MiThings)

Siri HomeKit 2016: [https://github.com/nfarina/homebridge](https://github.com/nfarina/homebridge)

Home Assistant.io 2016: [https://home-assistant.io/components/light.limitlessled/](https://home-assistant.io/components/light.limitlessled/)

March 2016: Milight/LimitlessLED can now be controlled with Amazon Echo via Home Remote for Mac  [https://youtu.be/cSPBfL62XPU](https://youtu.be/cSPBfL62XPU)

September 2015: LimitlessLED Bridge v5.0 released. new [http://www.limitlessled.com/download/LimitlessLEDv5.zip">windows admin tool available here). Set Static IP address...  *NEW* Set Cloud server... *NEW* View Web Admin in Browser

September 2015: nodeJS homekit server for windows7/8/8.1/10. now with Siri voice support for limitlessled. https://github.com/nfarina/homebridge download myTouchHome from appstore, or elgato Eve.

September 2015: Now control LimitlessLED using Amazon Echo (entire room control from voice)   https://www.youtube.com/watch?v=X-SfZXhr-Uo

[Limitlessled_and_amazon_echo_skills_sdk](https://www.reddit.com/r/amazonecho/comments/3fv1yf/limitlessled_and_amazon_echo_skills_sdk/)

August 2015: Raspberry PI 2: NodeJS opensource github code: https://github.com/oeuillot/node-milight

August 2015: Raspberry PI 2: NodeJS opensource github code: https://github.com/mwittig/node-milight-promise

August 2015: Raspberry PI 2: npm install homebridge   https://github.com/nfarina/homebridge

July 2015: Raspberry PI code for Dual White led control... scroll to the end of this page to see more info.
http://servernetworktech.com/2014/09/limitlessled-wifi-bridge-4-0-conversion-raspberry-pi/

July 2015: SPI direct bulb communication protocol documentation added. There are 3 stages to the wifi bridge   wifi->via UART->microchip mcu->via SPI->2.4Ghz PL1176 RF transceiver.

You can use a PL1176 (or compatible LT9800) 2.4Ghz transceiver and control individual bulbs directly after you have synced your bulb to the remoteID. see the commandset at the end of this page.

April 2015: Atef has created a cool plugin for the popular eventghost program, here is the link for the plugin http://www.eventghost.net/forum/viewtopic.php?f=9&#038;t=6893

April 2015: Set a Fixed IP address using our latest LimitlessLED Wifi Bridge v4.0 Tool!!

[http://www.limitlessled.com/download/LimitlessLEDv4.zip"](http://www.limitlessled.com/download/LimitlessLEDv4.zip)

February 2015: Gary Riches has made an awesome Pebble smart watch app for LimitlessLED called Home Remote [https://apps.getpebble.com/applications/5457576cac5df3c10f000036](https://apps.getpebble.com/applications/5457576cac5df3c10f000036)

Home Remote is also available on the app store [https://itunes.apple.com/us/app/home-remote-home-automation/id926193671?mt=8](https://itunes.apple.com/us/app/home-remote-home-automation/id926193671?mt=8), and it places the commands on the notification screen for fast easy access to the lights. Siri and Apple watch support also included. It works with iphone, it supports both RGBW and Dual White bulbs and led strips.
February 2015: LimitlessLED has released another official app v1.7 with faster load times (less than 1 second), displays wifi bridge IP address, supports wifi bridge v4 with up to 1000 wifi bridges, and faster 50millisecond wifi bridge UDP discovery. [https://itunes.apple.com/us/app/limitlessled/id594759938?mt=8](https://itunes.apple.com/us/app/limitlessled/id594759938?mt=8)
you can still access the old app here if you have the old wifi bridge v1 or v2.  [https://itunes.apple.com/us/app/wifi-controller-2/id589762495?mt=8](https://itunes.apple.com/us/app/wifi-controller-2/id589762495?mt=8)

February 2015: Trevor Hart from New Zealand has just updated his Free Android app and it now allows you to control LimitlessLed bulbs lights at sunrise/sunset!

URL remains the same; [https://play.google.com/store/apps/details?id=com.tchart.scheduled](https://play.google.com/store/apps/details?id=com.tchart.scheduled)

February 2015: Epocapp has written an app in C#/C++ to control your lights. It also has an 'Ambilight' type mode as well as support for Dell's 'AlienfFX' protocol for those games that support it. He made it for fun during the Christmas vacation and thought he would get it out there, to see if people like it or not. [http://epocapp.bitbucket.org/milight/](http://epocapp.bitbucket.org/milight/)

February 2015: [http://danvy.tv/" target="_blank">Alex Danvy) from France just made a cool Library and Universal Windows App [https://github.com/danvy/miled/tree/master/src" target="_blank](https://github.com/danvy/miled/tree/master/src) Control your LimitlessLED lights with C# Windows8 Windows7 Windows8.1  Universal Windows App  Windows Phone

January 2015: Dominik Schulz and Simon Carl just made an awesome Android App for LimitlessLED, supports multiple wifi bridges and alarms. [https://play.google.com/store/apps/details?id=de.leethaxxs.rgbcontroller" target="_blank](https://play.google.com/store/apps/details?id=de.leethaxxs.rgbcontroller)

January 2015: Andrea Ghensi just finished writing a cool node-RED flow to control your lights (RGBW only) via MQTT messages, so they can be integrated in an Internet of Things environment. you can find the flow here: [http://flows.nodered.org/flow/b2cb3bdc5a81ac881d4b](http://flows.nodered.org/flow/b2cb3bdc5a81ac881d4b)

January 2015: Olli implemented a better Python library for controlling RGBW lights.

Available at [https://github.com/ojarva/python-ledcontroller](https://github.com/ojarva/python-ledcontroller) and [https://pypi.python.org/pypi/ledcontroller](https://pypi.python.org/pypi/ledcontroller)

January 2015: Muscat's Android app [https://play.google.com/store/apps/details?id=org.muscat.android.alight">download aLight org.muscat.android.alight here)

December 2014: *NEW* Android app released... [http://www.limitlessled.com/download/Milight2.0.apk">apk download here).. or [https://play.google.com/store/apps/details?id=com.lierda.wifi">googleplay download here).  [https://play.google.com/store/apps/details?id=com.cdy.client.remoteLed">old app download here).

November 2014:  how to send Nightmode to RGBW lights. send a second Off command 100 ms later. the second off command is a long press.. and it is command |= 0x80.  for example 46 XOR 80 = C6 > So send 460055 Wait 100ms Then send C60055. that is hex commands 46 00 55 and C6 00 55.

October 2014: Android Beta App: [https://play.google.com/store/apps/details?id=org.muscat.android.alight">aLight Lighting Control BETA)

October 2014: Eliot Stocker new [https://play.google.com/store/apps/details?id=tv.piratemedia.lightcontroler" target="_blank">Android LightController App) and [https://github.com/eliotstocker/Light-Controller" target="_blank">github opensource code) - Android Widgets and Music modes.

October 2014: LimitlessLED Complete PHP API [https://github.com/yasharrashedi/LimitlessLED" target="_blank">github opensource)

October 2014: Gary Riches new iPhone app 1.1 with RGB+W support is out now: [https://appsto.re/gb/hGjn3.i" target="_blank](https://appsto.re/gb/hGjn3.i) Video: [http://youtu.be/ILkjyjzKiYU" target="_blank](http://youtu.be/ILkjyjzKiYU)

October 2014: New LimitlessLED wifi bridge v4.0 [#limitleslleddiscovery">device discovery sourecode).. vb.net and C++/C#

October 2014: Updated link to Apple iPhone code for UDP sockets  [https://github.com/robbiehanson/CocoaAsyncSocket/tree/master/Source/GCD">GCDAsyncUdpSocket.m)

September 2014: Free Android App Dual White Lightbulb Scheduler/Timer [https://play.google.com/store/apps/details?id=com.tchart.scheduled](https://play.google.com/store/apps/details?id=com.tchart.scheduled)

The app allows you to schedule your lights based on times. It also allows you to control your lights via text message (I use this in conjunction with IFTTT ie turn light on when I arrive home).

September 2014: linux commandline app. [http://iqjar.com/download/jar/milight/milight_binaries.zip">milight_commandline_binaries.zip) [http://iqjar.com/download/jar/milight/milight_sources.zip">milight_commandline_sourcecode.zip)

September 2014: more python sourecode on github

September 2014: NinjaBlocks plugin, just change the port to 8899 in the driver and it works. [https://github.com/theojulienne/ninja-limitlessLED](https://github.com/theojulienne/ninja-limitlessLED)

September 2014: Updated UDP broadcast discovery for Wifi Bridges on the LAN.

September 2014: Updated how to access AT commandset on port 48899 to command wifi bridge to scan for wifi routers.

August 2014:  IFTTT [https://ifttt.com/recipes/193847-turn-off-lights-iwy-light-milight-limitless-led-when-nobody-s-home" title="IFTTT LimitlessLED" target="_blank">plugin here)

August 2014: [https://github.com/joaquincasares/python-wifi-leds" target="_blank](https://github.com/joaquincasares/python-wifi-leds)

July 2014: Trevor (from New Zealand) has written a fantastic Android  library and sample app for using the Basic4Android IDE. It is just for Dual White lights at this stage. [http://www.basic4ppc.com/android/forum/threads/limitless-led-library.42651/](http://www.basic4ppc.com/android/forum/threads/limitless-led-library.42651/)
IFTTN - If This Then Node

IFTTN is a NodeJS based server which allows you to receive actions from IFTTT. It can be used to run on a Raspberry PI in your local network to use IFTTT for further home automation and other tasks. http://sebauer.github.io/if-this-then-node/
https://ifttt.com/recipes/193847-turn-off-lights-iwy-light-milight-limitless-led-when-nobody-s-home
New Light Scheduler App for Windows 7/8/8.1/10  (uses Microsoft.NET framework)

[https://www.limitlessled.com/download/LimitlessLED_Windows_App_v2.1.11.zip">LimitlessLED_Windows_App_v2.1.11.zip)
March 2014: Added comprehensive linux Bash script commands by Vince C.

February 2014: Added Cross platform [https://github.com/shannah/CN1Sockets" target="_blank">CN1Sockets opensource library). works with Android and iOS and Java and J2ME and Blackberry and Windows Phone.

February 2014: Added Example VB.NET Full code for controlling RGBW lights.

February 2014: Added LimitlessLED Windows App for Setting wifi bridge Fixed IP, and editing the gateway address.

February 2014: Added Windows C#.NET App for control of LimitlessLED RGBW lights.

February 2014: Added Windows Phone 8 download app from the Microsoft App Store.

February 2014: Added Linux Commandline utility and opensource code for RGBW control of on, off, and brightness.

February 2014: Microsoft VB.NET Sample Code for LimitlessLED Dual White LED Music Beat Visualizer. email us for a copy.

January 2014:  Added Windows Phone example sourcecode for turning lights on and off.

January 2014: New LimitlessLED Wifi Bridge Receiver v4.0, if you received your order after 1st Jan 2014 you will have this version.
v4.0 Now contains a more advanced wifi chip that supports Wireless-N (802.11 b/g/n) and wireless b or g.

* LimitlessLED Wifi Bridge Reciever 4.0 is backwards compatible with v3.0

* Same port 8899,  you can UDP broadcast to 10.1.1.255 or 255.255.255.255 if you want all wifi bridges to receive the command on the LAN.

* Now supports Wireless-N

* Now supports wireless router outages, and auto reconnects.

* We have disabled the web admin on the new wifi bridge receiver. The Web Admin is no longer available in v4.0 of bridges using http://10.10.100.254/home.html

* If you want to configure a static ip address you can set this via mac address on your wireless router.
Jan 2014: Added <b>Arduino</b> example code: Control LimitlessLED globes with an <b>Arduino</b>.

Jan 2014: Added Color and Brightness Chart

Jan 2014: Added Python Example code for RGBW light control

Oct 2013: Added WIFI module AT commandset details for LimitlessLED Wifi Bridge version 3.0

Oct 2013: Web UI interface login details updated

Sep 2013: Released NEW LimitlessLED Wifi Bridge version 3.0

The control commands are backwards compatible with the WiFi Bridge v2.0 below.
Terms of Use: All content is copyright 2013 Limitless Designs LLC, Under no circimstances is this information to be reproduced, copied, nor distributed without written permission.

Warning: reproduction of this information without written confirmation is an offense, and is strictly enforced.

If this information is used in your programming files then you must reference the limitlessLED dev webpage using the following Text:
// Source: http://www.limitlessled.com/dev

// Copyright (2013) Limitless Designs

// If the commandset codes are utilized in any way, the LimitlessLED website url must remain in the code comments.
DETAILED DEVELOPER TECHNICAL SPECIFICATIONS

Connect iPhone/iPad direct to WiFi Bridge Receiver: Yes (WiFi adhoc AP mode)

Connect WiFi Bridge Receiver to LAN: Yes (WiFi infrastructure STA mode)
Wifi Bridge Router Web Config: http://10.10.100.254/home.html   Username: admin  Password: admin

IP Address: 10.10.100.254 (editable)

Port: 8899 (editable)

TCP/IP Mode: UDP (udp is what online games use, it is very fast and has the lowest latency)



If the app doesn't allow you to scan and find the SSID for your home network, you can use the Web User Interface to configure Wifi Bridge 3.0 STA settings on http://10.10.100.255/home.html with the username:admin  password:admin
The IP Address is now assigned automatically using DHCP. You can find the current IP address of your wifi bridge in your routers DHCP table with the same MAC address as displayed on the LimitlessLED App 3.0 first network scan screen. Or you can broadcast UDP command packets to all wifi bridges on your local network, by changing the last digit in the IP address to 255,  i.e. 10.1.1.255 

#### NEW Port Number: 8899

The Port Number most importantly has changed from 50000 to 8899
#### EXCEL VBA Fixed Code to work with and support the new Port number:

change from "Long" to "Integer"

Public Declare Function htons Lib "ws2_32" (ByVal hostshort As Integer) As Integer
#### RGB+W LimitlessLED Light Bulb Commands



All UDP Commands are 3 Bytes. First byte is from the list below, plus a fixed 2 byte suffix of 0x00 (decimal: 0) and 0x55 (decimal: 85)

i.e. to turn all RGBW COLOR LimitlessLED Smart lights to ON then send the TCP/IP UDP packet of:  0x42 0x00 0x55

                    Hexidecimal (byte)	 Decimal (integer)

    RGBW COLOR LED ALL OFF	   0x41	           65

    RGBW COLOR LED ALL ON	   0x42	           66

    DISCO SPEED SLOWER	   0x43	           67

    DISCO SPEED FASTER	   0x44	           68

    GROUP 1 ALL ON		   0x45	           69	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 1 ALL OFF		   0x46	           70

    GROUP 2 ALL ON		   0x47	           71	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 2 ALL OFF		   0x48	           72

    GROUP 3 ALL ON		   0x49	           73	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 3 ALL OFF		   0x4A	           74

    GROUP 4 ALL ON		   0x4B	           75	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 4 ALL OFF		   0x4C	           76

    DISCO MODE	           0x4D	           77

    SET COLOR TO WHITE (GROUP ALL)  0x42    100ms followed by:	0xC2

    SET COLOR TO WHITE (GROUP 1)    0x45	100ms followed by:	0xC5

    SET COLOR TO WHITE (GROUP 2)    0x47	100ms followed by:	0xC7

    SET COLOR TO WHITE (GROUP 3)    0x49	100ms followed by:	0xC9

    SET COLOR TO WHITE (GROUP 4)    0x4B	100ms followed by:	0xCB



LIMITLESSLED RGBW DIRECTLY SETTING THE BRIGHTNESS is by a 3BYTE COMMAND: (First send the Group ON for the group you want to set the brightness for. You send the group ON command 100ms before sending the 4E 1B 55)

    Byte1: 0x4E        (decimal: 78)

    Byte2: 0x02 to 0x1B (decimal range: 2 to 27) full brightness 0x1B (decimal 27)

    Byte3: Always 0x55 (decimal: 85)
    LIMITLESSLED RGBW COLOR SETTING is by a 3BYTE COMMAND: (First send the Group ON for the group you want to set the colour for. You send the group ON command 100ms before sending the 40)

    Byte1: 0x40        (decimal: 64)

    Byte2: 0x00 to 0xFF     (255 colors)   See Color Matrix Chart for the different values below.

    Byte3: Always 0x55 (decimal: 85)

Byte2: Color Matrix Chart: (thanks Stephan Schaade, http://knx-user-forum.de http://mknx.github.io/smarthome/)

note there are more colours (0-255) in between, this color chart is just steps of 16.
    
    0x00 Violet
    0x10 Royal_Blue
    0x20 Baby_Blue
    0x30 Aqua
    0x40 Mint
    0x50 Seafoam_Green
    0x60 Green
    0x70 Lime_Green
    0x80 Yellow
    0x90 Yellow_Orange
    0xA0 Orange
    0xB0 Red
    0xC0 Pink
    0xD0 Fusia
    0xE0 Lilac
    0xF0 Lavendar

Thanks Andrew Badge, here is another copy of the color chart, The last column (HTML colour code) is used for rendering onscreen previews.
Note: the names are modified to match the websafe names as well.

            table.Rows.Add("Violet",0x00,"#EE82EE");
            table.Rows.Add("RoyalBlue",0x10,"#4169E1");
            table.Rows.Add("LightSkyBlue", 0x20,"#87CEFA");
            table.Rows.Add("Aqua",0x30,"#00FFFF");
            table.Rows.Add("AquaMarine", 0x40,"#7FFFD4");
            table.Rows.Add("SeaGreen",0x50,"#2E8B57");
            table.Rows.Add("Green",0x60,"#008000");
            table.Rows.Add("LimeGreen",0x70,"#32CD32");
            table.Rows.Add("Yellow",0x80,"#FFFF00");
            table.Rows.Add("Goldenrod",0x90,"#DAA520");
            table.Rows.Add("Orange",0xA0,"#FFA500");
            table.Rows.Add("Red",0xB0,"#FF0000");
            table.Rows.Add("Pink",0xC0,"#FFC0CB");
            table.Rows.Add("Fuchsia", 0xD0,"#FF00FF");
            table.Rows.Add("Orchid",0xE0,"#DA70D6");
            table.Rows.Add("Lavender",0xF0,"#E6E6FA");



Night modes, not implemented for RGBW, but referenced here for completeness..

    NIGHT MODE ALL		        0x41	100ms followed by:	0xC1   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 1	0x46	100ms followed by:	0xC6   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 2	0x48	100ms followed by:	0xC8   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 3	0x4A	100ms followed by:	0xCA   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 4	0x4C	100ms followed by:	0xCC   (not implemented in limitlessled wifi bridge 3.0)
Special NOTE #1: The LimitlessLED bulb remembers its own Brightness setting memory separately for ColorRGB and separately for White. For example dimming Green, then switching to White full brightness, and when you go back to a specific color the brightness returns to what the ColorRGB was before. Same vice versa for the White. The previous brightness setting is remembered specifically for the White and specifically for the ColorRGB.

Special Note #2: for Java:  To convert from a decimal number to a signed byte use the 0xFF like this:  cmdBytes[0] = 85 &#038; 0xFF;

Special Note #3: for Visual Basic: To use a hexadecimal number, you can put &#038;H in front instead of 0x  for example: 0x41 would be &#038;H41

Special Note #4: If you don't know the IP address of the Wifi Bridge, You can also broadcast All Groups ON to all wifi bridges on the LAN network by using the IP address with the last number as .255  or you can use the IP address of 255.255.255.255, which will do a full LAN broadcast to all wifi bridges on this Local network only.

Special Note #5: With the new LimitlessLED Wifi Bridge 3.0  it is no longer necessary to send the 3rd byte (0x55) in the UDP packet, you can just send the 1st and 2nd bytes which will be faster, this also applies to the backward compatible RGB and DUAL White LEDs when using wifi bridge 3.0.  But to remain backwards compatible with wifi bridge 2.0 it is advisable to always send the 3rd byte 0x55 in the UDP packet for RGB and WHITE bulbs, but it is not necessary to send 3 bytes for the new RGBW bulbs(just two is ok for them).

Special Note #6: To Clear the PAIRED/SYNCED bulb from all of its Groups.. Press-And-Hold its Group ON button within 2 seconds of turning the light socket power ON.
## LimitlessLED v4.0 Wifi Bridge Control Commandset

==================================================================

==== LimitlessLED WIFI Bridge Auto Discovery ADMIN PORT 48899 ====

==================================================================

- Step 1:Send UDP message to the LAN broadcast IP address of "10.10.100.255" and port of 48899 => "Link_Wi-Fi"

- All Wifi bridges on the LAN will respond with their details. Response is "10.10.100.254, ACCF232483E8"
- Step 2 (optional for changing settings on the wifi bridge): Then send "+ok" to the LimitlessLED Wifi Bridge. Send UDP message to the response IP address returned from step 1 "10.10.100.254" => "+ok"

- Step 3 (optional for changing settings on the wifi bridge): After that you may send AT commands (ending with \r\n) to the module.


Example Source Code

    //1 second broadcasting (50ms x 20times = 1000ms = 1 second)
    UpdateStatusMessage("Broadcast UDP 20 times at 50ms intervals for LimitlessLED wifi bridges: Link_Wi-Fi");

    for (int i = 1; i <= 20; i++) {
        byte[] b = UTF8.GetBytes("Link_Wi-Fi");
        udpAdmin.Send(b, b.Length, new IPEndPoint("255.255.255.255", 48899));
        System.Threading.Thread.Sleep(50); //sleep 50ms
    }

    WriteLog("Scanning for 1second up to 1000 LimitlessLED wifi bridge responses...");
    udpAdmin.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
    while (iFoundWifiBridges < 1000) {
        receiveBytes = udpAdmin.Receive(listenEP);
        //wait here until a UDP packet has been received.

        sResponse = UTF8.GetString(receiveBytes);  //convert udp datagram bytes into a string.
        static System.Text.RegularExpressions.Regex expression = new System.Text.RegularExpressions.Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]),([A-F]|[0-9]){12},.*$");
        IsValidWifiBridgeResponse = expression.IsMatch(System.text);
    if (IsValidWifiBridgeResponse) {

    }
    }

    //Catch TimeOut Exception here, no more LimitlessLED Bridge devices responded within 1000ms.

    }


AT commandset

    +ok\r     //enter admin mode
    AT+WSCAN\r
    +ok= detected WIFIs...
    AT+WSSSID=mySSIDname\r
    +ok
    AT+WSKEY=WPA2PSK,AES,password123\r
    +ok
    AT+WMODE=STA\r
    +ok
    AT+Z\r
    +ok
    AT+Q\r   //exit admin mode



The first time the bridge is set-up you connect the phone directly to the bridge's wifi, then send the WSCAN command to find either the wifi's that the bridge can detect? If you already know the Wifi SSID, you can skip that step and just use the WSSSID command.  replace mySSIDname with the SSID you want to use.  replace password123 with your wifi password.
The LimitlessLED Wifi Bridge consists of a Wifi to Serial bridge chip LPT-100 and includes a Custom MCU designed and built to spec by Lierda, and also contains a PL1167 Low Power High Performance Single Chip 2.4GHz Transceiver used as the transmitter in the wifi bridge and used in the receiver of each bulb. The communications are secured over the air using encryption and frequency hopping.
Example VB.NET Code

    Dim listenPort As Integer = 48899
    udpSocket = New UdpClient(txtIPAddress.Text, listenPort)
    Dim b As Byte() = New Byte() {&#038;H41, &#038;H54, &#038;H2B, &#038;H57, &#038;H41, &#038;H4E, &#038;H4E, &#038;HA}
    udpSocket.Send(b, b.Length)
    Dim client As New UdpClient(48899)
    Dim groupEP As New IPEndPoint(IPAddress.Any, _PortNumber)
    Dim state As New UdpState(client, groupEP)
    ' Start async receiving
    udpSocket.BeginReceive(New AsyncCallback(AddressOf DataReceived), state)

    Private Shared Sub DataReceived(ByVal ar As IAsyncResult)
        Dim c As UdpClient = DirectCast(DirectCast(ar.AsyncState, UdpState).c, UdpClient)
        Dim wantedIpEndPoint As IPEndPoint = DirectCast(DirectCast(ar.AsyncState, UdpState).e, IPEndPoint)
        Dim receivedIpEndPoint As New IPEndPoint(IPAddress.Any, 0)
        Dim receiveBytes As [Byte]() = c.EndReceive(ar, receivedIpEndPoint)

        ' Check sender
        'Dim isRightHost As Boolean = (wantedIpEndPoint.Address.Equals(receivedIpEndPoint.Address)) OrElse wantedIpEndPoint.Address.Equals(IPAddress.Any)
        'Dim isRightPort As Boolean = (wantedIpEndPoint.Port = receivedIpEndPoint.Port) OrElse wantedIpEndPoint.Port = 0
        'If isRightHost AndAlso isRightPort Then
        ' Convert data to ASCII and print in console
        Dim receivedText As String = System.Text.Encoding.UTF8.GetString(receiveBytes)
        Debug.Print(receivedText) 'Console.Write
        'End If

        ' Restart listening for udp data packages
        c.BeginReceive(New AsyncCallback(AddressOf DataReceived), ar.AsyncState)

    End Sub




# LimitlessLED v2.0 OpenSource API for Developers
Oct 2013: Added UART config app and manual

Aug 2013: Added Python example code.

Aug 2013: Added Linux example script.

Jul 2013: Added CasualLight Voice Control of LimitlessLED Lights.

Jul 2013: Added Ruby example opensource code.

May 2013: Android AutoVoice and Tasker/Locale added for control of LimitlessLED wireless lighting.

Apr 2013: Added Links to opensource programming projects.

Mar 2013: Added Excel Macro VBA code.

Dec 2012: Added VB.NET/C#.net code.

Jul 2012: Added Android Java example code.

Jun 2012: Added iPhone/iPad xcode example.
## LimitlessLED OpenSource Projects

CasualLight Voice Control of LimitlessLED Lights.  by Joris studying at Waikato University, New Zealand.

[https://chrome.google.com/webstore/detail/casuallight/kehomlifkcmfagnefddfmiiemeaiogfj" title="Voice control LimitlessLED lights" target="_blank](https://chrome.google.com/webstore/detail/casuallight/kehomlifkcmfagnefddfmiiemeaiogfj)
Windows7/8 OpenSource LimitlessLED Application (commandline app written in Microsoft C#.Net).

[https://github.com/qyiet/LimitlessLED](https://github.com/qyiet/LimitlessLED) by Glen Balks from New Zealand.
HTML/Javascript LimitlessLED Application (ninjablocks).

https://github.com/jzGreen/ninja-limitlessLED <- no longer active (Dec 2015)

[http://www.youtube.com/watch?v=n8gRqzbA9bs&amp;feature=player_embedded">Youtube: NinjaBlocks/LimitlessLED and RFID)

[http://www.youtube.com/watch?feature=player_embedded&amp;v=AMaJZQns9Vc">Youtube: Limitlessled and ninja blocks)
cloverleaf LimitlessLED Ruby github opensource by brandon

[https://github.com/brandon-dacrib/cloverleaf/blob/master/app/modules/limitlessled-rgb.rb](https://github.com/brandon-dacrib/cloverleaf/blob/master/app/modules/limitlessled-rgb.rb)
Now you can control your LimitlessLED lights on Android using AutoVoice and Tasker/Locale.
LimitlessLED LightShow made in Perl programming language by Prof. Matt Carter from Bond University

[https://github.com/hash-bang/Lightshow](https://github.com/hash-bang/Lightshow)

provides a simple command line program to command the lighting system as well as also supports easy to program lighting macros - fake fire place lighting, time-of-day settings etc.

Matt is also working on an XBMC/BoxEE plugin for media centers so that you can control the lights from your TV.  Great work Matt, look forward to this one.
Jon Benson from Australia has built this great Tasker app.
Tasker stays in memory when launched once, so it displays almost instantly.

The current app (which simply allows sending UDP packets by acting as a plugin for Tasker/Locale) is free and will remain so.
Here is the Github webpage:

[http://hastarin.github.io/android-udpsender/](http://hastarin.github.io/android-udpsender/)
and here is the Play Store link:

[https://play.google.com/store/apps/details?id=com.hastarin.android.udpsender](https://play.google.com/store/apps/details?id=com.hastarin.android.udpsender)
To give you an idea of how it can be used, Jon has set this up so far...

* Gesture control via my launcher (Nova) to turn all lights on/off/full bright/night mode and dim/brighten lights.

* Using the Scene shown above I can turn all/individual zones on/off and control bright/dim/warm/cool.

* NOTE: A swipe up/down in the colored square controls a bright/dim ramp and left/right for warm/cool.

* An NFC tag on my bed that when tapped will turn the lights off, or keep some in night mode if I have guests.

* When my phone connects to the Wi-Fi, and it's between sunrise and sunset, my entry hall light is turned on.

## Control LimitlessLED Globes With an Arduino
source: http://www.modlog.net/?tag=limitlessled

    #include <SPI.h>        
    #include <Ethernet.h>
    #include <EthernetUdp.h>
    #include <EtherShield.h>

    byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
    IPAddress ip(255, 255, 255, 255);
    unsigned int localPort = 8899;
    EthernetUDP Udp;     

    #define pir A0

    byte lightson[] = {0x22,0x00,0x55};
    byte lightsoff[] = {0x21,0x00,0x55};
    byte dim[] = {0x24,0x00,0x55};
    byte bright[]={0x23,0x00,0x55};
    byte prev[]={0x28,0x00,0x55};


    IPAddress remoteIp1(255, 255, 255, 255);        
    unsigned int remotePort1 = 8899; 
    boolean lightstatus=true;
    boolean startup=true;

    void setup() {
        Ethernet.begin(mac,ip);
        Udp.begin(localPort);
        Serial.begin(9600);
        pinMode(pir, INPUT);
    }

    void loop() {
        if (startup==true)
        {
            delay (100);
            Udp.beginPacket(remoteIp1, remotePort1);
            Udp.write(lightson,3);
            Udp.endPacket();
            for (int i = 1; i < 20; i++)
                    {
                    
                    Udp.beginPacket(remoteIp1, remotePort1);
                    Udp.write(prev,3);
                    Udp.endPacket();
                    delay(50);
                    }   

            Udp.beginPacket(remoteIp1, remotePort1);
            Udp.write(lightsoff,3);
            Udp.endPacket();  
            startup=false;
    }

    if (lightstatus==true)  
    {
        if (digitalRead(pir) == 0)
        {
        Serial.println("Room Empty");
        lightoff();
        }

    }
    if (digitalRead(pir) == 1)
    {
        Serial.println("Motion Sensed");
        lighton();  
    }

    delay(100);
    }

    void lightoff()
    {
        Serial.println("This is dimming level " );
        for (int i = 1; i < 8; i++)
                {
                
                Udp.beginPacket(remoteIp1, remotePort1);
                Udp.write(dim,3);
                Udp.endPacket();
                delay(50);
                }   
                
        Udp.beginPacket(remoteIp1, remotePort1);
        Udp.write(lightsoff,3);
        Udp.endPacket();
        lightstatus=false;
        Serial.println("Light should be OFF" );
    }

    void lighton()
    {
        Serial.println("This is brightening level " );
        Udp.beginPacket(remoteIp1, remotePort1);
        Udp.write(lightson,3);
        Udp.endPacket();
        Serial.println("Light should be ON" );
        delay(50);
        for (int i = 1; i < 15; i++)
                {
                Udp.beginPacket(remoteIp1, remotePort1);
                Udp.write(bright,3);
                Udp.endPacket();
                delay(50);
                }   
    
        delay(10000);
        for (int i = 1; i < 10000; i++)
        {
        if (digitalRead(pir) == 1)
            {
            i==0;
            }
            delay(10);
        }   

        lightstatus=true;
    }


## LimitlessLED Wifi Bridge 2.0 UART PC Config App
No Longer available.


## VB.NET Sample Code
Only 3 lines of code to control the LimitlessLED lights.

You can use this in PowerPoint, Word, Excel, MS Access, Visual Basic apps:

   Connect to LimitlessLED Wifi Bridge Receiver
    
    Dim udpClient As New System.Net.Sockets.UdpClient("255.255.255.255", 8899)

   Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
    
    udpClient.Send(New Byte() {&#038;H38, &#038;H0, &#038;H55}, 3)
   ToDo: send as many different commands here as you like, just change the number above where you see &#038;H38

   Close Connection
   
    udpClient.Close()



## VB.NET Code


    Module globals

        'LimitlessLED VB.NET Module.  
        'Add new class item, call it globals.vb  and paste this into the whole file, overwritting everything in the globals.vb file.

        '****************************************************
        '*** USAGE from a VB.NET Form Button:
        '****************************************************
        '***
        '***    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '***
        '***        'Turn all lights off 
        '***        SendUDP(LimitlessLED_Command_WHITE_ALLOFF)
        '***
        '***        'Wait 2 seconds
        '***        Threading.Thread.Sleep(2000)
        '***
        '***        'Turn all lights on (press and hold for full brightness)
        '***        SendUDP(LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_ALL, LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_ALL_PRESSAndHOLD)
        '***
        '***        'Turn all lights to Green (which ever lights you last turned On using the wifi brige, the color command will go to)
        '***        SendUDP(LimitlessLED_Command_RGBW_SET_COLOR, RGBW_RoyalBlue)
        '***
        '***        'Set Brightness to 50% (which ever lights you last turned On using the wifi brige, the color command will go to)
        '***        SendUDP(LimitlessLED_Command_RGBW_SET_BRIGHTNESS, 50)
        '***
        '***    End Sub
        '***
        '****************************************************

        Dim _IPAddress As String = "10.1.1.255"
        Dim _PortNumber As Integer = 8899
        Dim udpSocket As System.Net.Sockets.UdpClient = New System.Net.Sockets.UdpClient(_IPAddress, _PortNumber)

        Friend LimitlessLED_Command_WHITE_ALLOFF As Byte() = New Byte() {&#038;H39, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_ALLON As Byte() = New Byte() {&#038;H35, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_BRIGHTNESS_UP As Byte() = New Byte() {&#038;H3C, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_BRIGHTNESS_DOWN As Byte() = New Byte() {&#038;H34, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_WARM_WHITE_INCREASE As Byte() = New Byte() {&#038;H3E, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_COOL_WHITE_INCREASE As Byte() = New Byte() {&#038;H3F, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_1_ALL_ON As Byte() = New Byte() {&#038;H38, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_1_ALL_OFF As Byte() = New Byte() {&#038;H3B, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_2_ALL_ON As Byte() = New Byte() {&#038;H3D, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_2_ALL_OFF As Byte() = New Byte() {&#038;H33, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_3_ALL_ON As Byte() = New Byte() {&#038;H37, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_3_ALL_OFF As Byte() = New Byte() {&#038;H3A, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_4_ALL_ON As Byte() = New Byte() {&#038;H32, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_GROUP_4_ALL_OFF As Byte() = New Byte() {&#038;H36, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_NIGHT_MODE_ALL As Byte() = New Byte() {&#038;H39, &#038;H0, &#038;H55} '100ms followed by: 0xB9
        Friend LimitlessLED_Command_WHITE_NIGHT_MODE_ALL_PRESSAndHOLD As Byte() = New Byte() {&#038;HBB, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_1 As Byte() = New Byte() {&#038;H3B, &#038;H0, &#038;H55} '100ms followed by: 0xBB
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_1_PRESSAndHOLD As Byte() = New Byte() {&#038;HBB, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_2 As Byte() = New Byte() {&#038;H33, &#038;H0, &#038;H55} '100ms followed by: 0xB3
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_2_PRESSAndHOLD As Byte() = New Byte() {&#038;HB3, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_3 As Byte() = New Byte() {&#038;H3A, &#038;H0, &#038;H55} '100ms followed by: 0xBA
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_3_PRESSAndHOLD As Byte() = New Byte() {&#038;HBA, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_4 As Byte() = New Byte() {&#038;H36, &#038;H0, &#038;H55} '100ms followed by: 0xB6
        Friend LimitlessLED_Command_WHITE_NIGHT_SAVER_MODE_GROUP_4_PRESSAndHOLD As Byte() = New Byte() {&#038;HB6, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_ALL As Byte() = New Byte() {&#038;H35, &#038;H0, &#038;H55} '100ms followed by: 0xB5
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_ALL_PRESSAndHOLD As Byte() = New Byte() {&#038;HB5, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_1 As Byte() = New Byte() {&#038;H38, &#038;H0, &#038;H55} '100ms followed by: 0xB8
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_1_PRESSAndHOLD As Byte() = New Byte() {&#038;HB8, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_2 As Byte() = New Byte() {&#038;H3D, &#038;H0, &#038;H55} '100ms followed by: 0xBD
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_2_PRESSAndHOLD As Byte() = New Byte() {&#038;HBD, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_3 As Byte() = New Byte() {&#038;H37, &#038;H0, &#038;H55} '100ms followed by: 0xB7
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_3_PRESSAndHOLD As Byte() = New Byte() {&#038;HB7, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_4 As Byte() = New Byte() {&#038;H32, &#038;H0, &#038;H55} '100ms followed by: 0xB2
        Friend LimitlessLED_Command_WHITE_FULL_BRIGHTNESS_GROUP_4_PRESSAndHOLD As Byte() = New Byte() {&#038;HB2, &#038;H0, &#038;H55}

        Friend LimitlessLED_Command_RGB_ALLOFF As Byte() = New Byte() {&#038;H21, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_ALLON As Byte() = New Byte() {&#038;H22, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_BRIGHTNESS_UP As Byte() = New Byte() {&#038;H23, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_BRIGHTNESS_DOWN As Byte() = New Byte() {&#038;H24, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_DISCO_SPEED_FASTER As Byte() = New Byte() {&#038;H25, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_DISCO_SPEED_SLOWER As Byte() = New Byte() {&#038;H26, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_DISCO_MODE_NEXT As Byte() = New Byte() {&#038;H27, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_DISCO_MODE_PREVIOUS As Byte() = New Byte() {&#038;H28, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGB_SET_COLOR As Byte() = New Byte() {&#038;H20, &#038;H0, &#038;H55}

        Friend LimitlessLED_Command_RGBW_ALLOFF As Byte() = New Byte() {&#038;H41, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_ALLON As Byte() = New Byte() {&#038;H42, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_DISCO_SPEED_SLOWER As Byte() = New Byte() {&#038;H43, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_DISCO_SPEED_FASTER As Byte() = New Byte() {&#038;H44, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_1_ALL_ON As Byte() = New Byte() {&#038;H45, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_1_ALL_OFF As Byte() = New Byte() {&#038;H46, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_2_ALL_ON As Byte() = New Byte() {&#038;H47, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_2_ALL_OFF As Byte() = New Byte() {&#038;H48, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_3_ALL_ON As Byte() = New Byte() {&#038;H49, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_3_ALL_OFF As Byte() = New Byte() {&#038;H4A, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_4_ALL_ON As Byte() = New Byte() {&#038;H4B, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_GROUP_4_ALL_OFF As Byte() = New Byte() {&#038;H4C, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_DISCO_MODE_NEXT As Byte() = New Byte() {&#038;H4D, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_WHITE_ALL As Byte() = New Byte() {&#038;H42, &#038;H0, &#038;H55} '100ms followed by: 0xC2
        Friend LimitlessLED_Command_RGBW_WHITE_ALL_PRESSAndHOLD As Byte() = New Byte() {&#038;HC2, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_1 As Byte() = New Byte() {&#038;H45, &#038;H0, &#038;H55} '100ms followed by: 0xC5
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_1_PRESSAndHOLD As Byte() = New Byte() {&#038;HC5, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_2 As Byte() = New Byte() {&#038;H47, &#038;H0, &#038;H55} '100ms followed by: 0xC7
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_2_PRESSAndHOLD As Byte() = New Byte() {&#038;HC7, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_3 As Byte() = New Byte() {&#038;H49, &#038;H0, &#038;H55} '100ms followed by: 0xC9
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_3_PRESSAndHOLD As Byte() = New Byte() {&#038;HC9, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_4 As Byte() = New Byte() {&#038;H4B, &#038;H0, &#038;H55} '100ms followed by: 0xCB
        Friend LimitlessLED_Command_RGBW_WHITE_GROUP_4_PRESSAndHOLD As Byte() = New Byte() {&#038;HCB, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_SET_BRIGHTNESS As Byte() = New Byte() {&#038;H4E, &#038;H0, &#038;H55}
        Friend LimitlessLED_Command_RGBW_SET_COLOR As Byte() = New Byte() {&#038;H40, &#038;H0, &#038;H55}

        Friend RGBW_Violet As Integer = &#038;H0
        Friend RGBW_RoyalBlue As Integer = &#038;H10
        Friend RGBW_LightSkyBlue As Integer = &#038;H20
        Friend RGBW_Aqua As Integer = &#038;H30
        Friend RGBW_AquaMarine As Integer = &#038;H40
        Friend RGBW_SeaGreen As Integer = &#038;H50
        Friend RGBW_Green As Integer = &#038;H60
        Friend RGBW_LimeGreen As Integer = &#038;H70
        Friend RGBW_Yellow As Integer = &#038;H80
        Friend RGBW_Goldenrod As Integer = &#038;H90
        Friend RGBW_Orange As Integer = &#038;HA0
        Friend RGBW_Red As Integer = &#038;HB0
        Friend RGBW_Pink As Integer = &#038;HC0
        Friend RGBW_Fuchsia As Integer = &#038;HD0
        Friend RGBW_Orchid As Integer = &#038;HE0
        Friend RGBW_Lavender As Integer = &#038;HF0

        Friend RGBW_Brightness10percent As Integer = &#038;H2
        Friend RGBW_Brightness14percent As Integer = &#038;H3
        Friend RGBW_Brightness17percent As Integer = &#038;H4
        Friend RGBW_Brightness21percent As Integer = &#038;H5
        Friend RGBW_Brightness24percent As Integer = &#038;H6
        Friend RGBW_Brightness28percent As Integer = &#038;H7
        Friend RGBW_Brightness32percent As Integer = &#038;H8
        Friend RGBW_Brightness35percent As Integer = &#038;H9
        Friend RGBW_Brightness39percent As Integer = &#038;HA
        Friend RGBW_Brightness42percent As Integer = &#038;HB
        Friend RGBW_Brightness46percent As Integer = &#038;HC
        Friend RGBW_Brightness50percent As Integer = &#038;HD
        Friend RGBW_Brightness53percent As Integer = &#038;HE
        Friend RGBW_Brightness57percent As Integer = &#038;HF
        Friend RGBW_Brightness60percent As Integer = &#038;H10
        Friend RGBW_Brightness64percent As Integer = &#038;H11
        Friend RGBW_Brightness68percent As Integer = &#038;H12
        Friend RGBW_Brightness71percent As Integer = &#038;H13
        Friend RGBW_Brightness75percent As Integer = &#038;H14
        Friend RGBW_Brightness78percent As Integer = &#038;H15
        Friend RGBW_Brightness82percent As Integer = &#038;H16
        Friend RGBW_Brightness86percent As Integer = &#038;H17
        Friend RGBW_Brightness89percent As Integer = &#038;H18
        Friend RGBW_Brightness93percent As Integer = &#038;H19
        Friend RGBW_Brightness96percent As Integer = &#038;H1A
        Friend RGBW_Brightness100percent As Integer = &#038;H1B

        Friend Sub SendUDP(ByRef dataGram() As Byte)
            Try

                'Resend the packet 5 times in case other apps or other remotes are sending UDP at the same time, like the LimitlessLED candle app is left running.
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                System.Threading.Thread.Sleep(0) 'yield the cpu to other threads, such as the system networking thread, a chance to quickly send the UDP

                'you could send the UDP command using another thread if you like, but it seemed to run slower.
                'Dispatcher.BeginInvoke(New ThreadStart(Function() udpSocket.Send(LimitlessLED_Command_WHITE_ALLON, 3)))

            Catch ex As Exception
                'implement any error handling here.
                Debug.Print(ex.Message)
            End Try
        End Sub

        Friend Sub SendUDP(ByRef dataGram() As Byte, ByRef dataGramPressAndHold() As Byte)
            Try
                'send the first command packet
                udpSocket.Send(dataGram, 3)
                System.Threading.Thread.Sleep(100)
                'send the press and hold command 100milliseconds later:
                udpSocket.Send(dataGramPressAndHold, 3)
                System.Threading.Thread.Sleep(0) 'yield the cpu to other threads, such as the system networking thread, a chance to quickly send the UDP

            Catch ex As Exception
                'implement any error handling here.
                Debug.Print(ex.Message)
            End Try


        End Sub

        Friend Sub SendUDP(ByRef dataGramZoneGroup() As Byte, ByRef dataGram() As Byte, ByRef ColorOrBrightnessPercent As Integer)
            Try

                If dataGram(0) = LimitlessLED_Command_RGBW_SET_BRIGHTNESS(0) Then 'Byte1 must be RGBW command &#038;H40

                    'Convert from Percent to brightness command

                    'BrightnessPercent steps every 3.6%: 10, 14, 17, 21, 24, 28, 32, 35, 39, 42, 46, 50, 53, 57, 60, 64, 68, 71, 75, 78, 82, 86, 89, 93, 96, 100
                    Dim BrightnessCode As Integer = Math.Floor(ColorOrBrightnessPercent / 3.6)
                    If BrightnessCode < 2 Then BrightnessCode = 2 'set the minimum brightness code (0x02)
                    If BrightnessCode > 27 Then BrightnessCode = 27 'set the maximum brightness code (0x1B)

                    'Set the array index 0,1,2   set the middle byte1 to the Brightness
                    dataGram.SetValue(BrightnessCode, 1)
                Else
                    'Set Color

                    'Set the array index 0,1,2   set the middle byte1 to the Color
                    dataGram.SetValue(ColorOrBrightnessPercent, 1)
                End If


                'Resend the packet 5 times in case other apps or other remotes are sending UDP at the same time, like the LimitlessLED candle app is left running.
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                udpSocket.Send(dataGram, 3)
                System.Threading.Thread.Sleep(0) 'yield the cpu to other threads, such as the system networking thread, a chance to quickly send the UDP

            Catch ex As Exception
                'implement any error handling here.
                Debug.Print(ex.Message)
            End Try


        End Sub

    End Module




Here is the latest opensource code for interacting with the LimitlessLED smartbulbs. Speaking with the smartthings team, they will be allowing UDP commands for devices such as LimitlessLED wifi bridges on the you home Local Area Network – exciting times ahead. You can view the latest code here [https://github.com/andrewfoster/ninja-limitlessLED/tree/master/lib](https://github.com/andrewfoster/ninja-limitlessLED/tree/master/lib)
[https://github.com/andrewfoster/ninja-limitlessLED/find/master?pr=%2Ftheojulienne%2Fninja-limitlessLED%2Fpull%2F2](https://github.com/andrewfoster/ninja-limitlessLED/find/master?pr=%2Ftheojulienne%2Fninja-limitlessLED%2Fpull%2F2)
## LimitlessLED RGB CommandSet (RGB bulbs no longer sold Jan 2014)
Terms of Use: All content is copyright 2013 Limitless Designs LLC, Under no circimstances is this information to be reproduced, copied, nor distributed without written permission.

Warning: reproduction of this information without written confirmation is an offense, and is strictly enforced.

If this information is used in your programming files then you must reference the limitlessLED dev webpage using the following Text:
// Source: http://www.limitlessled.com/dev

// Copyright (2013) Limitless Designs LLC

// If these codes are copied or utilized in any way, the LimitlessLED website link must remain attached.

# DETAILED DEVELOPER TECHNICAL SPECIFICATIONS

Connect iPhone/iPad direct to WiFi Bridge Receiver: Yes (WiFi adhoc mode)

Connect WiFi Bridge Receiver to LAN: Yes (WiFi infrastructure mode)
IP Address: 192.168.1.100 (editable)   or ip 10.10.100.255 AP, or 10.1.1.255 when on STA connected to your lan.

Port: 50000 (editable)  or port 8899 since bridge v3.0

TCP/IP Mode: UDP (udp is what online games use, it is very fast and has the lowest latency)

Username: admin (editable)

Password: 000000 (editable)
<strong>RGB COLOR LimitlessLED WIFI TCP/IP UDP COMMANDS</strong>

All UDP Commands are 3 Bytes. First byte is from the list below, plus a fixed 2 byte suffix of 0x00 (decimal: 0) and 0x55 (decimal: 85)

i.e. to turn all RGB COLOR LimitlessLED Smart lights to ON then send the TCP/IP UDP packet of:  0x22 0x00 0x55



    Command             Hexidecimal (byte)   Decimal (integer)

    RGB COLOR LED ALL OFF	   0x21	           33

    RGB COLOR LED ALL ON	   0x22	           34

    BRIGHTNESS UP	           0x23	           35

    BRIGHTNESS DOWN	           0x24	           36

    DISCO SPEED FASTER	   0x25	           37    (SYNC/PAIR RGB Bulb within 2 seconds of Wall Switch Power being turned ON)

    DISCO SPEED SLOWER	   0x26	           38

    DISCO MODE NEXT	           0x27	           39

    DISCO MODE PREVIOUS	   0x28	           40



    COLOR SETTING is by a 3BYTE COMMAND:

    Byte1: 0x20

    Byte2: 0x00 to 0xFF

    Byte3: Always 0x55 (decimal: 85)
<strong>WARM WHITE/COOL WHITE LimitlessLED Smartbulb WIFI TCP/IP UDP COMMANDS</strong>




	Hexidecimal (byte)	Decimal (integer)

    WHITE LED ALL OFF	0x39	57

    WHITE LED ALL ON	0x35	53

    BRIGHTNESS UP		0x3C	60

    BRIGHTNESS DOWN		0x34	52

    WARM WHITE INCREASE	0x3E	62

    COOL WHITE INCREASE	0x3F	63

    GROUP 1 ALL ON		0x38	56

    GROUP 1 ALL OFF		0x3B	59

    GROUP 2 ALL ON		0x3D	61

    GROUP 2 ALL OFF		0x33	51

    GROUP 3 ALL ON		0x37	55

    GROUP 3 ALL OFF		0x3A	58

    GROUP 4 ALL ON		0x32	50

    GROUP 4 ALL OFF		0x36	54

    NIGHT MODE ALL		        0x39	100ms followed by:	0xB9

    NIGHT SAVER MODE GROUP 1	0x3B	100ms followed by:	0xBB

    NIGHT SAVER MODE GROUP 2	0x33	100ms followed by:	0xB3

    NIGHT SAVER MODE GROUP 3	0x3A	100ms followed by:	0xBA

    NIGHT SAVER MODE GROUP 4	0x36	100ms followed by:	0xB6

    FULL BRIGHTNESS ALL		0x35	100ms followed by:	0xB5

    FULL BRIGHTNESS GROUP 1		0x38	100ms followed by:	0xB8

    FULL BRIGHTNESS GROUP 2		0x3D	100ms followed by:	0xBD

    FULL BRIGHTNESS GROUP 3		0x37	100ms followed by:	0xB7

    FULL BRIGHTNESS GROUP 4		0x32	100ms followed by:	0xB2



## Microsoft C#.NET example Code

        //Connect to LimitlessLED Wifi Bridge Receiver
	System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("255.255.255.255", 8899);

	//Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
	udpClient.Send(new byte[] {
		0x38,
		0x0,
		0x55
	}, 3);
	//ToDo: send as many different commands here as you like, just change the number above where you see &#038;H38

	//Close Connection
	udpClient.Close();


## Microsoft C#.NET Windows Phone 7.1/8.0/RT example Code
Example code thanks to VikingCode. WP7.1/8/WinRT don't have System.IO.Sockets, instead they make use of a 'datagram socket', and the code for that looks like this (tested, works on my RGB light) WinRT requires Private Networks (Client &#038; Server) to be selected.


    public async Task TurnOnRGBLight()
            {
                var socket = new DatagramSocket();
                using (var stream = await socket.GetOutputStreamAsync(new HostName("192.168.1.255"), "8899"))
                {
                    using (var writer = new DataWriter(stream))
                    {
                        writer.WriteBytes(new byte[] { 0x22, 0x0, 0x55 });
                        writer.StoreAsync();
                    }
                }
            }


## LimitlessLED RGBW .NET Windows App Download
Special Thanks to developer Imtiaj Meah.

[http://www.limitlessled.com/download/LimitlessLED_Windows_App_v2.1.11.zip](http://www.limitlessled.com/download/LimitlessLED_Windows_App_v2.1.11.zip)
## Microsoft C#.NET Windows Phone 8.0 App
Thanks to Andreas Overmeyer

[http://www.windowsphone.com/en-us/store/app/limitless-lightswitch/b7dfd90d-0b3f-4bcb-a464-a60460d0b072](http://www.windowsphone.com/en-us/store/app/limitless-lightswitch/b7dfd90d-0b3f-4bcb-a464-a60460d0b072)
## Example SourceCode for iPhone/iPad:

include cocoaasyncsocket AsyncUdpSocket.m
Apple iPhone code for UDP sockets  [https://github.com/robbiehanson/CocoaAsyncSocket/tree/master/Source/GCD](https://github.com/robbiehanson/CocoaAsyncSocket/tree/master/Source/GCD) 

    //Connect to WiFi Bridge Receiver
    AsyncUdpSocket *socket;
    [socket sendData:packet toHost:@"255.255.255.255" port:8899 withTimeout:-1 tag:1];


## Example SourceCode for Android
Source: Victor Murphy (Android opensource remote control apps)

    final String msg = text;
    new Thread(new Runnable() {
    public void run() {
        try {
        InetAddress serverAddr = InetAddress.getByName("255.255.255.255");
        DatagramSocket socket = new DatagramSocket();
        byte[] buf = (msg).getBytes();
        DatagramPacket packet = new DatagramPacket(buf, buf.length, serverAddr, 8899);
        socket.send(packet);
        socket.close();
        } 
        catch (UnknownHostException e) {
        Toast.makeText(getApplicationContext(),e.toString(),Toast.LENGTH_SHORT).show();
        e.printStackTrace();
        } catch (SocketException e) {
        Toast.makeText(getApplicationContext(),e.toString(),Toast.LENGTH_SHORT).show();
        e.printStackTrace();
        } catch (IOException e) {
        Toast.makeText(getApplicationContext(),e.toString(),Toast.LENGTH_SHORT).show();
        e.printStackTrace();
        }
    }
    }).start();
    }



## Example Microsoft Excel 2005/2007/2010/2013 VBA macro code


'******************* FUNCTIONS USED BY LIMITLESSLED TO SEND TCP/IP UDP NETWORK COMMANDS USING EXCEL**********
'*** Copyright (c) 2013 Limitless Designs LLC
'*** More information available at www.limitlessled.com/dev

    Type WSAData
    wVersion As Integer
    wHighVersion As Integer
    szDescription(0 To 255) As Byte
    szSystemStatus(0 To 128) As Byte
    iMaxSockets As Integer
    iMaxUdpDg As Integer
    lpVendorInfo As Long
    End Type

    Type sockaddr_in
        sin_family As Integer
        sin_port As Integer
        sin_addr As Long
        sin_zero(0 To 7) As Byte
    End Type


    Public Declare Function WSAStartup Lib "ws2_32" (ByVal wVersionRequired As Integer, ByRef lpWSAData As WSAData) As Long
    Public Declare Function WSAGetLastError Lib "ws2_32" () As Long
    Public Declare Function socket Lib "ws2_32" (ByVal af As Long, ByVal socktype As Long, ByVal protocol As Long) As Long
    Public Declare Function connect Lib "ws2_32" (ByVal sock As Long, ByRef name As sockaddr_in, ByVal namelen As Integer) As Long
    Public Declare Function send Lib "ws2_32" (ByVal sock As Long, ByRef buf As Byte, ByVal bufLen As Long, ByVal flags As Long) As Long
    Public Declare Function recv Lib "ws2_32" (ByVal sock As Long, ByRef buf As Byte, ByVal bufLen As Long, ByVal flags As Long) As Long
    Public Declare Function inet_addr Lib "ws2_32" (ByVal s As String) As Long
    Public Declare Function htons Lib "ws2_32" (ByVal hostshort As Integer) As Integer
    Public Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)
        

    'RGB: LimitlessLED Full Colour Smartbulbs
    'WWCW: LimitlessLED Warm White/Cool White Smartbulbs
    'command list from www.limitlessled.com/dev/
    Enum LimitlessLEDCommand
        RGB_ALLOFF = &#038;H21
        RGB_ALLON = &#038;H22
        RGB_BRIGHTNESS_UP = &#038;H23
        RGB_BRIGHTNESS_DOWN = &#038;H24
        RGB_DISCO_SPEED_FASTER = &#038;H25
        RGB_DISCO_SPEED_SLOWER = &#038;H26
        RGB_DISCO_MODE_NEXT = &#038;H27
        RGB_DISCO_MODE_PREVIOUS = &#038;H28
        RGB_SET_COLOR = &#038;H20
        WHITE_ALLOFF = &#038;H39
        WHITE_ALLON = &#038;H35
        WHITE_BRIGHTNESS_UP = &#038;H3C
        WHITE_BRIGHTNESS_DOWN = &#038;H34
    WHITE_WARM_WHITE_INCREASE = &#038;H3E
        WHITE_COOL_WHITE_INCREASE = &#038;H3F
        WHITE_GROUP_1_ALL_ON = &#038;H38
        WHITE_GROUP_1_ALL_OFF = &#038;H3B
        WHITE_GROUP_2_ALL_ON = &#038;H3D
        WHITE_GROUP_2_ALL_OFF = &#038;H33
        WHITE_GROUP_3_ALL_ON = &#038;H37
        WHITE_GROUP_3_ALL_OFF = &#038;H3A
        WHITE_GROUP_4_ALL_ON = &#038;H32
        WHITE_GROUP_4_ALL_OFF = &#038;H36
        WHITE_NIGHT_MODE_ALL = &#038;H39  '100ms followed by: 0xB9
        WHITE_NIGHT_MODE_ALL_PRESSAndHOLD = &#038;HBB
        WHITE_NIGHT_SAVER_MODE_GROUP_1 = &#038;H3B '100ms followed by: 0xBB
        WHITE_NIGHT_SAVER_MODE_GROUP_1_PRESSAndHOLD = &#038;HBB
        WHITE_NIGHT_SAVER_MODE_GROUP_2 = &#038;H33 '100ms followed by: 0xB3
        WHITE_NIGHT_SAVER_MODE_GROUP_2_PRESSAndHOLD = &#038;HB3
        WHITE_NIGHT_SAVER_MODE_GROUP_3 = &#038;H3A '100ms followed by: 0xBA
        WHITE_NIGHT_SAVER_MODE_GROUP_3_PRESSAndHOLD = &#038;HBA
        WHITE_NIGHT_SAVER_MODE_GROUP_4 = &#038;H36 '100ms followed by: 0xB6
        WHITE_NIGHT_SAVER_MODE_GROUP_4_PRESSAndHOLD = &#038;HB6
        WHITE_FULL_BRIGHTNESS_ALL = &#038;H35 '100ms followed by: 0xB5
        WHITE_FULL_BRIGHTNESS_ALL_PRESSAndHOLD = &#038;HB5
        WHITE_FULL_BRIGHTNESS_GROUP_1 = &#038;H38  '100ms followed by: 0xB8
        WHITE_FULL_BRIGHTNESS_GROUP_1_PRESSAndHOLD = &#038;HB8
        WHITE_FULL_BRIGHTNESS_GROUP_2 = &#038;H3D  '100ms followed by: 0xBD
        WHITE_FULL_BRIGHTNESS_GROUP_2_PRESSAndHOLD = &#038;HBD
        WHITE_FULL_BRIGHTNESS_GROUP_3 = &#038;H37  '100ms followed by: 0xB7
        WHITE_FULL_BRIGHTNESS_GROUP_3_PRESSAndHOLD = &#038;HB7
        WHITE_FULL_BRIGHTNESS_GROUP_4 = &#038;H32  '100ms followed by: 0xB2
        WHITE_FULL_BRIGHTNESS_GROUP_4_PRESSAndHOLD = &#038;HB2
    End Enum


    Sub SendLimitlessLEDCommand(cmd As LimitlessLEDCommand, Optional color As Integer)

    'Start Windows Winsock
    Dim iReturn As Long
    Dim wsaDat As WSAData
    iReturn = WSAStartup(&#038;H202, wsaDat)

    If iReturn  0 Then
        MsgBox "WSAStartup failed", 0, ""
        Exit Sub
    End If
    
    Dim sock As Long
    Dim sock1 As Long
    Dim lasterr As Long
    Dim i As Long
    Dim buf(10) As Byte
    Dim s As String
    Dim j As Integer
    Dim command(2) As Byte
    
    'http://msdn.microsoft.com/en-us/library/ms740506%28v=vs.85%29.aspx
    'sock = socket(2, 1, 6) 'TCP
    sock = socket(2, 2, 17) 'use UDP command
    
    Dim addr As sockaddr_in
    addr.sin_family = 2 'IPv4
    addr.sin_port = htons(8899) 'Default is Port 50000 (v2) 8899 (v3 v4)
    addr.sin_addr = inet_addr("255.255.255.255") 'LimitlesLED Wifi UDP Receiver Bridge IP Address
    i = connect(sock, addr, LenB(addr)) 'Connect to Wifi Bridge
    
    '*** Set up the UDP packet to command the lights:
    
    '1st Byte is for the command
    command(0) = cmd 'command list from www.limitlessled.com/dev/
    
    '2nd byte is for the color number (when necessary, otherwise it is zero)
    If cmd = &#038;H20 Then 'if command is 0x20 then a color number is also required
        command(1) = color
    Else
    command(1) = &#038;H0
    End If
    
    '3rd byte is always 0x55
    command(2) = &#038;H55 'check byte
    
    'Now SEND the command to the lights
    i = send(sock, command(0), 3, 0)

    End Sub

    'Sleep (100) ' Waits for 100 milliseconds

    Sub White_ON()
        SendLimitlessLEDCommand WHITE_ALLON
    End Sub

    Sub White_OFF()
        SendLimitlessLEDCommand WHITE_ALLOFF
    End Sub

    Sub Group1_ON()
        SendLimitlessLEDCommand WHITE_GROUP_1_ALL_ON
    End Sub

    Sub Group2_ON()
        SendLimitlessLEDCommand WHITE_GROUP_2_ALL_ON
    End Sub

    Sub Group3_ON()
        SendLimitlessLEDCommand WHITE_GROUP_3_ALL_ON
    End Sub

    Sub Group4_ON()
        SendLimitlessLEDCommand WHITE_GROUP_4_ALL_ON
    End Sub

    Sub Group1_OFF()
        SendLimitlessLEDCommand WHITE_GROUP_1_ALL_OFF
    End Sub

    Sub Group2_OFF()
        SendLimitlessLEDCommand WHITE_GROUP_2_ALL_OFF
    End Sub

    Sub Group3_OFF()
        SendLimitlessLEDCommand WHITE_GROUP_3_ALL_OFF
    End Sub

    Sub Group4_OFF()
        SendLimitlessLEDCommand WHITE_GROUP_4_ALL_OFF
    End Sub

    Sub White_IncreaseCoolWhite()
        SendLimitlessLEDCommand WHITE_COOL_WHITE_INCREASE
    End Sub

    Sub White_IncreaseBrightness()
        SendLimitlessLEDCommand WHITE_BRIGHTNESS_UP
    End Sub

    Sub White_DecreaseBrightness()
        SendLimitlessLEDCommand WHITE_BRIGHTNESS_DOWN
    End Sub

    Sub White_IncreaseWarmWhite()
        SendLimitlessLEDCommand WHITE_WARM_WHITE_INCREASE
    End Sub



## Example of Linux control
written by Shaun, updated by Vince C. from New Zealand:

[http://smileytechadventures.blogspot.co.nz/2013/07/control-limitless-led-from-linux-bash.html](http://smileytechadventures.blogspot.co.nz/2013/07/control-limitless-led-from-linux-bash.html)



        #!/bin/bash 

        if [ -z "$1" ] ; then 
            echo "You must enter a parameter: "  
            echo "  e.g. $0 allon" 
            exit 1 
        fi 

        incmd="$1" 
        ipaddress="255.255.255.255" 
        portnum="8899"

        allon="\x42\00\x55" 
        alloff="\x41\00\x55" 

        #note for disco commands these will impact the group you are currently in for example if you previously typed "./bashlight group2on" then 
        #when you turn on disco it will only affect group 2, if you want disco to apply to all then type "./bashlight allon" first

        discoon="\x4D\00\x55" 
        discofaster="\x44\00\x55" 
        discoslower="\x43\00\x55"
        discomode="\x4D\00\x55"

        group1on="\x45\00\x55"
        group1off="\x46\00\x55"

        group2on="\x47\00\x55"
        group2off="\x48\00\x55"

        group3on="\x49\00\x55"
        group3off="\x4A\00\x55"

        group4on="\x4B\00\x55"
        group4off="\x4C\00\x55"

        #Set all to white
        allwhite="\xC2\00\x55"

        #Set Group 1 to white
        group1white="\xC5\00\x55"
        group2white="\xC7\00\x55"
        group3white="\xC9\00\x55"
        group4white="\xCB\00\x55"

        #Working (range: 2 to 27 need to work out Hexidecimal eg Hexidecimal 1B is Decimal 27
        #note this will impact the group you are currently in for example if you previously typed "./bashlight group2on" then 
        #when you adjust the brightness it will only affect group 2, if you want the brightness to apply to all groups then type "./bashlight allon" first

        #Special NOTE #1: The LimitlessLED bulb remembers its own Brightness setting memory separately for ColorRGB and separately for White. 
        #For example dimming Green, then switching to White full brightness, and when you go back to a specific color the brightness returns 
        #to what the ColorRGB was before. Same vice versa for the White. The previous brightness setting is remembered specifically for the 
        #White and specifically for the ColorRGB.

        brightest="\x4E\x1B\x55"
        lowest="\x4E\x02\x55"
        half="\x4E\xd\x55"

        #Colours

        #    0x00 Violet
        #    0x10 Royal_Blue
        #    0x20 Baby_Blue
        #    0x30 Aqua
        #    0x40 Mint
        #    0x50 Seafoam_Green
        #    0x60 Green
        #    0x70 Lime_Green
        #    0x80 Yellow
        #    0x90 Yellow_Orange
        #    0xA0 Orange
        #    0xB0 Red
        #    0xC0 Pink
        #    0xD0 Fusia
        #    0xE0 Lilac
        #    0xF0 Lavendar

        # Note there are more colours (0-255) in between, this color chart is just steps of 16.
        # and they are incrementing by 10 so example mint is 40 but you could use 41 42 43 44 45 etc for the in between colours

        #First byte x40 says change colour , second byte is the colour that you want.

        #Usage note when using these commands they will impact the current active group for example if you previously typed "./bashlight group2on" then 
        #when you adjust the colour it will only affect group 2, if you want the colour to apply to all then type "./bashlight allon" first
        #or if you wanted to make only group 1 red then type "./bashlight group1on" first then "./bashlight col_red"

        col_violet="\x40\x00\x55"
        col_royal_blue="\x40\x10\x55"
        col_baby_blue="\x40\x20\x55"
        col_aqua="\x40\x30\x55"
        col_mint="\x40\x40\x55"
        col_mint2="\x40\x45\x55"
        col_seafoam_green="\x40\x50\x55"
        col_green="\x40\x60\x55"
        col_lime_green="\x40\x70\x55"
        col_yellow="\x40\x80\x55"
        col_yelloworange="\x40\x90\x55"
        col_orange="\x40\xA0\x55"
        col_red="\x40\xB0\x55"
        col_pink="\x40\xC0\x55"
        col_fusia="\x40\xD0\x55"
        col_lilac="\x40\xE0\x55"
        col_lavendar="\x40\xF0\x55"

        eval incmd=\$$incmd 

        echo -n -e "$incmd" >/dev/udp/"$ipaddress"/"$portnum"
</pre>

This takes the command line arguments, parsing it indirectly, and then passing the contents of that to the echo command to be sent directly to the linux UDP device file, which transmits the UDP commands to the wifi bridge. Your lights should already be synced to the wifi bridge first using the app.
## Linux command line utility
courtesy of http://iqjar.com



Linux command line utility which can be used to easily control the new 9W RGBW LED bulbs through simple commands like: "milight on", "milight off", "milight brightness 15", "milight color 75", etc. It also works per zones and is theoretically compatible with the wireless bridge v3.0 and v4.0 (although only tested with v3.0 so far). I'm giving the program and its sources for free to the public, as long as my website (http://iqjar.com) remains mentioned. And once again, your LimitlessLED Wi-Fi enabled LED bulbs are the best!
Binaries: [http://iqjar.com/download/jar/milight/milight_binaries.zip](http://iqjar.com/download/jar/milight/milight_binaries.zip)

Sources: [http://iqjar.com/download/jar/milight/milight_sources.zip](http://iqjar.com/download/jar/milight/milight_sources.zip)


Linux CommandLine:

    milight on 
    milight off
    milight brightness 15
    milight color 75


## Example of Python code #1
[https://github.com/Daniel0524/hal/blob/master/light_controller/change_lights.py](https://github.com/Daniel0524/hal/blob/master/light_controller/change_lights.py)
## Example of Python code #2
credits to: justakid from Australia.

credits to: [http://wiki.python.org/moin/UdpCommunication](http://wiki.python.org/moin/UdpCommunication)

    import socket

    UDP_IP = "255.255.255.255" #this is the IP of the wifi bridge, or 255.255.255.255 for UDP broadcast
    UDP_PORT = 8899

    MESSAGE1 = "\x39\x00\x55" #this turns all lights off

    print "UDP target IP:", UDP_IP #don't really need this
    print "UDP target port:", UDP_PORT #don't really need this
    print "message:", MESSAGE1 #don't really need this

    sock = socket.socket(socket.AF_INET,
    socket.SOCK_DGRAM)
    sock.sendto(MESSAGE1, (UDP_IP, UDP_PORT))


## Summary of Codes (send all 3 hex commands via port 8899 UDP to ip address of the wifi bridge receiver)
LimitlessLED White


    35 00 55 - All On

    39 00 55 - All Off

    3C 00 55 - Brightness Up

    34 00 55 - Brightness Down  (There are ten steps between min and max)

    3E 00 55 - Warmer

    3F 00 55 - Cooler  (There are ten steps between warmest and coolest)

    38 00 55 - Zone 1 On

    3B 00 55 - Zone 1 Off

    3D 00 55 - Zone 2 On

    33 00 55 - Zone 2 Off

    37 00 55 - Zone 3 On

    3A 00 55 - Zone 3 Off

    32 00 55 - Zone 4 On

    36 00 55 - Zone 4 Off

    B5 00 55 - All On Full    (Send >=100ms after All On)

    B8 00 55 - Zone 1 Full  (Send >=100ms after Zone 1 On)

    BD 00 55 - Zone 2 Full  (Send >=100ms after Zone 2 On)

    B7 00 55 - Zone 3 Full  (Send >=100ms after Zone 3 On)

    B2 00 55 - Zone 4 Full  (Send >=100ms after Zone 4 On)

    B9 00 55 - All Nightlight         (Send >=100ms after All Off)

    BB 00 55 - Zone 1 Nightlight  (Send >=100ms after Zone 1 Off)

    B3 00 55 - Zone 2 Nightlight  (Send >=100ms after Zone 2 Off)

    BA 00 55 - Zone 3 Nightlight  (Send >=100ms after Zone 3 Off)

    B6 00 55 - Zone 4 Nightlight  (Send >=100ms after Zone 4 Off)
LimitlessLED RGB

    22 00 55 - Lamps On

    21 00 55 - Lamps Off

    23 00 55 - Brightness Up

    24 00 55 - Brightness Down  (There are nine steps between min and max)

    27 00 55 - Mode Up

    28 00 55 - Mode Down  (There are 20 modes. The lowest is constant white)

    25 00 55 - Speed Up (Fast)

    26 00 55 - Speed Down (Slow)

    20 xx 55 - Set Colour to xx 
LimitlessLED RGBW

 

    41 00 55 - All Off

    42 00 55 - All On

    43 00 55 - Speed Down (One Step Slower Disco)

    44 00 55 - Speed Up (One Step Faster Disco)

    45 00 55 - Zone 1 On

    46 00 55 - Zone 1 Off

    47 00 55 - Zone 2 On

    48 00 55 - Zone 2 Off

    49 00 55 - Zone 3 On

    4A 00 55 - Zone 3 Off

    4B 00 55 - Zone 4 On

    4C 00 55 - Zone 4 Off

    4D 00 55 - One Step Disco Mode Up (20 Disco Modes)

    42 00 55 wait 100ms then send C2 00 55 - All Zones Change back to Warm White.

    45 00 55 wait 100ms then send C5 00 55 - Zone 1 Change back to Warm White.

    47 00 55 wait 100ms then send C7 00 55 - Zone 2 Change back to Warm White.

    49 00 55 wait 100ms then send C9 00 55 - Zone 3 Change back to Warm White.

    4B 00 55 wait 100ms then send CB 00 55 - Zone 4 Change back to Warm White.

    42 00 55 wait 100ms then send 4E XX 55 - Set All to Brightness XX (XX range is 0x02 to 0x1B)

    45 00 55 wait 100ms then send 4E XX 55 - Set Zone 1 to Brightness XX (XX range is 0x02 to 0x1B)

    47 00 55 wait 100ms then send 4E XX 55 - Set Zone 2 to Brightness XX (XX range is 0x02 to 0x1B)

    49 00 55 wait 100ms then send 4E XX 55 - Set Zone 3 to Brightness XX (XX range is 0x02 to 0x1B)

    4B 00 55 wait 100ms then send 4E XX 55 - Set Zone 4 to Brightness XX (XX range is 0x02 to 0x1B)

    42 00 55 wait 100ms then send 40 XX 55 - Set All to Color XX (XX range is 0x00 to 0xFF)

    45 00 55 wait 100ms then send 40 XX 55 - Set Zone 1 to Color XX (XX range is 0x00 to 0xFF)

    47 00 55 wait 100ms then send 40 XX 55 - Set Zone 2 to Color XX (XX range is 0x00 to 0xFF)

    49 00 55 wait 100ms then send 40 XX 55 - Set Zone 3 to Color XX (XX range is 0x00 to 0xFF)

    4B 00 55 wait 100ms then send 40 XX 55 - Set Zone 4 to Color XX (XX range is 0x00 to 0xFF)
## Management of Wifi Bridge 2.0 through serial connection

This module has a implementation of the Hayes AT command set. In particular:
Instead of "AT" commands, it is really "AT+" commands. The null command does require the plus character. All commands begin with the three characters "AT+".
The default escape time is 2 seconds, not the more common 1 second. In order to escape (data) transmission mode, no data or text can be sent to the serial port for 2 seconds, then three "+" (plus) characters must be sent, and then no data or text for 2 seconds. The module should then respond with the "+OK" command prompt.
Be sure the module accepts AT+ commands by sending +++ and receiving +OK. If not, keep shorting pins 2 and 5 while restarting the module, to force command mode. With the module in command mode, send AT+RSTF to reset the module to factory settings. The module should reply with +OK.
Now you can make changes to the configuration, and then you can save your changes in internal flash (AT+PMTF), and reset the module again (AT+Z).
## Example Python code for Wifi Bridge 3.0 RGBW

    #!/usr/bin/env python3
    # vim: set encoding=utf-8 tabstop=4 softtabstop=4 shiftwidth=4 expandtab
    #########################################################################
    # Copyright 2014 Stephan Schaade          http://knx-user-forum.de/
    #########################################################################
    #  This file is part of SmartHome.py.    http://mknx.github.io/smarthome/
    #
    #  SmartHome.py is free software: you can redistribute it and/or modify
    #  it under the terms of the GNU General Public License as published by
    #  the Free Software Foundation, either version 3 of the License, or
    #  (at your option) any later version.
    #
    #  SmartHome.py is distributed in the hope that it will be useful,
    #  but WITHOUT ANY WARRANTY; without even the implied warranty of
    #  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    #  GNU General Public License for more details.
    #
    #  You should have received a copy of the GNU General Public License
    #  along with SmartHome.py. If not, see http://www.gnu.org/licenses/.
    #########################################################################


    import logging
    import threading
    import socket
    import time


    logger = logging.getLogger('')




    class milight():


        def __init__(self, smarthome,udp_ip='255.255.255.255',udp_port='8899'):    # UDP Broadcast if IP not specified
            self._sh = smarthome
            self.udp_ip = udp_ip
            self.udp_port = udp_port
            self.color_map = {             # for reference and future use
                'violet': 0x00,
                'royal_blue': 0x10,
                'baby_blue': 0x20,
                'aqua': 0x30,
                'mint': 0x40,
                'seafoam_green': 0x50,
                'green': 0x60,
                'lime_green': 0x70,
                'yellow': 0x80,
                'yellow_orange': 0x90,
                'orange': 0xA0,
                'red': 0xB0,
                'pink': 0xC0,
                'fusia': 0xD0,
                'lilac': 0xE0,
                'lavendar': 0xF0
            }
            
            self.on_all = bytearray([0x42, 0x00, 0x55])
            self.on_ch1 = bytearray([0x45, 0x00, 0x55])
            self.on_ch2 = bytearray([0x47, 0x00, 0x55])
            self.on_ch3 = bytearray([0x49, 0x00, 0x55])
            self.on_ch4 = bytearray([0x4B, 0x00, 0x55])
            
            self.off_all = bytearray([0x41, 0x00, 0x55])
            self.off_ch1 = bytearray([0x46, 0x00, 0x55])
            self.off_ch2 = bytearray([0x48, 0x00, 0x55])
            self.off_ch3 = bytearray([0x4A, 0x00, 0x55])
            self.off_ch4 = bytearray([0x4C, 0x00, 0x55])
            
            self.white_ch1 = bytearray([0xC5, 0x00, 0x55])
            self.white_ch2 = bytearray([0xC7, 0x00, 0x55])
            self.white_ch3 = bytearray([0xC9, 0x00, 0x55])
            self.white_ch4 = bytearray([0xCB, 0x00, 0x55])
            
            self.brightness = bytearray([0x4E, 0x00, 0x55])
            self.color      = bytearray([0x40, 0x00, 0x55])
        
            self.max_bright = bytearray([0x4E, 0x1B, 0x55])
            self.discoon      = bytearray([0x4D, 0x00, 0x55])
            self.discoup   = bytearray([0x44, 0x00, 0x55])
            self.discodown = bytearray([0x43, 0x00, 0x55])


        def run(self):
            self.alive = True
            # if you want to create child threads, do not make them daemon = True!
            # They will not shutdown properly. (It's a python bug)
            
        def send (self,data_s) : 
            # UDP sent without further encoding
            try:
                family, type, proto, canonname, sockaddr = socket.getaddrinfo(self.udp_ip, self.udp_port)[0]
                sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
                sock.sendto(data_s, (sockaddr[0], sockaddr[1]))
                sock.close()
                del(sock)
            except Exception as e:
                logger.warning("UDP: Problem sending data to {}:{}: ".format(self.udp_ip, self.udp_port, e))
                pass
            else:
                logger.debug("UDP: Sending data to {}:{}:{} ".format(self.udp_ip, self.udp_port, data_s))


        def switch(self,group,value):   # on/off switch function  - used befor updateing brightness / color / disco
            
            if  group == 0:             # group 0 represents all groups
            if value == 0:
            data_s = self.off_all
            else:
            data_s = self.on_all
        
                    
            if group == 1:
            if value == 0:
            data_s = self.off_ch1
            else:
            data_s = self.on_ch1  
    
            if group == 2:
            if value == 0:
            data_s = self.off_ch2
            else:
            data_s = self.on_ch2 
            
            if group == 3:
            if value == 0:
            data_s = self.off_ch3
            else:
            data_s = self.on_ch3 
            
            if group == 4:
            if value == 0:
            data_s = self.off_ch4
            else:
            data_s = self.on_ch4
            
        
            self.send(data_s)           # call UDP send
        

        def dim(self,group,value):
                
            time.sleep(0.1)             # wait 100 ms
            logger.info(value)
            value = int(value/8.0)      # for compliance with KNX DPT5
            logger.info(value)
            data_s = self.brightness
            data_s[1] = value           # set Brightness
            self.send(data_s)           # call UDP to send WHITE if switched on
                
                
                
        def col(self,group,value):
            
            if group == 0:              # group 0 represents all groups
                data_s = self.on_all
                            
            if group == 1:
                data_s = self.on_ch1  
            
            if group == 2:
                data_s = self.on_ch2
            
            if group == 3:   
                data_s = self.on_ch3 
        
            if group == 4:
                data_s = self.on_ch4
            
            
        
            self.send(data_s)           # call UDP send   to switch on/off
            
            time.sleep(0.1)             # wait 100 ms
            
            data_s = self.color
            data_s[1] = value           # set Color
            self.send(data_s)           # call UDP to send WHITE if switched on            
            
            
            
            
        def white(self,group,value):
            
            time.sleep(0.1)               # wait 100 ms
            
            if value == 1:
                if group == 1:
                    data_s = self.white_ch1
                if group == 2:
                    data_s = self.white_ch2
                if group == 3:
                    data_s = self.white_ch3
                if group == 4:  
                    data_s = self.white_ch3
                self.send(data_s)        # call UDP to send WHITE if switched on
        
        def disco(self,group,value):
            value=1                      # Avoid switch off
            logger.info("disco")    
            time.sleep(0.1)               # wait 100 ms
            
            
            data_s = self.discoon
            logger.info(data_s)
            self.send(data_s)        
        
        def disco_up(self,group,value):
            value=1                      # Avoid switch off
            logger.info("disco up")      
            time.sleep(0.1)               # wait 100 ms
            
            
            data_s = self.discoup
            logger.info(data_s)
            self.send(data_s)        
            
        def disco_down(self,group,value):
            value=1
            logger.info("disco down")      # Avoid switch off
                                
            time.sleep(0.1)               # wait 100 ms
            
            
            data_s = self.discodown
            logger.info(data_s)
            self.send(data_s)              
                                                                            
        
                
        def stop(self):
            self.alive = False


        def parse_item(self, item):
            if 'milight_sw' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item
            if 'milight_dim' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item
            if 'milight_col' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item    
            if 'milight_white' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item 
            if 'milight_disco' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item              
            if 'milight_disco_up' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item              
            if 'milight_disco_down' in item.conf:
                logger.debug("parse item: {0}".format(item))
                return self.update_item                  
            
            else:
                return None


        def parse_logic(self, logic):
            if 'milight' in logic.conf:
                # self.function(logic['name'])
                pass


        def update_item(self, item, caller=None, source=None, dest=None):
            if caller != 'milight':
                logger.info("miLight update item: {0}".format(item.id()))
            
                if 'milight_sw' in item.conf:
                for channel in item.conf['milight_sw']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
            
                
                if 'milight_dim' in item.conf:
                for channel in item.conf['milight_dim']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
                    self.dim(group, item())
                
                    
                if 'milight_col' in item.conf:
                for channel in item.conf['milight_col']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    
                    self.col(group, item())
                    
                
                if 'milight_white' in item.conf:
                for channel in item.conf['milight_white']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
                    self.white(group, item())
                
                if 'milight_disco' in item.conf:
                for channel in item.conf['milight_disco']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
                    self.disco(group, item())
                    
                if 'milight_disco_up' in item.conf:
                for channel in item.conf['milight_disco_up']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
                    self.disco_up(group, item()) 
                    
                if 'milight_disco_down' in item.conf:
                for channel in item.conf['milight_disco_down']:
                    logger.info(channel)
                    group = int(channel)
                    logger.info (item())
                    self.switch(group, item())
                    self.disco_down(group, item())   
                
              

    if __name__ == '__main__':
        logging.basicConfig(level=logging.DEBUG)
        myplugin = milight('smarthome-dummy')
        myplugin.run()



## Example Rebol script from www.rebol.com

    lights: open udp://192.168.1.124:8899
    insert lights #{220055} ; turns light on
    close lights


## How to setup for use with Tasker


* Download Full Tasker For Android ($1.99) through Play store. You will also need ‘Autovoice’ (Tasker plugin), and it is worth getting ‘UDP Sender’ to test you codes. If you would like to make turn your tasker projects into apps, also get ‘Tasker App Factory’

1. Go into your router and find out what the IP address is for your Wifi Bridge (you should see this in your DHCP Table in the router. The IP Address is now assigned automatically using DHCP. You can find the current IP address of your wifi bridge in your routers DHCP table with the same MAC address as displayed on the Wifi Lights App 3.0 first network scan screen. Or you can broadcast UDP command packets to all wifi bridges on your local network, by changing the last digit in the IP address to 255, i.e. 10.1.1.255. If you can try to lock the IP address so that it will not change.

2. Once you know the IP address go into Tasker and under the TASK tab add a task. Give it a name, then add an action.

3. Either search “intent” or go into ‘Misc’ and select SEND INTENT

4. Now under ‘Action’ type – android.intent.action.SENDTO (pay attention to capitals)

5. In data field type – udp://192.168.1.6:8899/0x350055 (this is the IP address for your bridge) (Port No.) (Command Code – See list below)

6. Go to the bottom of this page and change Target field from ‘Broadcast Receiver’ to ‘Action’

7. If you now exit you should be able to press the play button and see the result. If it doesn’t work try this:

Check the IP address

Check for spelling mistakes and ensure there are no spaces

Test the code in UDP sender to make sure you have it correctly

For BRIGHTNESS, NIGHTLIGHT MODE, and COLOUR CHOOSING, you must send a prefix code (as seen in the table below) 100ms or more before sending the code you want.

Make sure TARGET is set to ‘ACTIVITY’

Try repeat the play button (it is always worth adding repeat codes in the actions to ensure that they get picked up by the bridge)

Ensure the commend code has “0x” at the beginning of Command Code



Now using the “Profiles” tab on the first page of tasker you can set voice control.

1. select ‘Add’ and choose ‘State’ Then in ‘Plugin’ choose ‘Autovoice Regonised’.

2. Under configuration select ‘Event Behaviour’ (tick) and enter your ‘Command Filter’. This might be “Lights On”

3. Press back and then select the Task to assign to your new Profiles.

4. You should now be able to use your Google Voice Search to activate that profile using your command “Lights On”.

This should be done using the google search WIDGET

This is a very basic run and the list of possibilities is endless. Watch some Tasker For Android tutorials and you will start

to see the potential for this app here are some ideas of what you could do.

- Lights turn on when your phone connect to your wifi between sunset and sunrise

- Lights turn on at set times, but only if you are home (or use sunset times by getting info from a met website daily-this can be done automatically)

- Remake the app using ‘scenes’

- Export your actions as Apps

- Much much more!...

Summary of Codes (send all 3 hex commands via port 8899 UDP to ip address of the wifi bridge receiver)


## SPI COMMANDSET 2.4Ghz RF Over-the-Air Protocol - Remote Control Commands using PL1176 (or compatible LT9800)

Control individual bulbs directly after you have synced them to the remoteID.

PL1176 2.4Ghz Transceiver Settings: 

    Register20: syncword 32 bits, trailer 4 bits, data type NRZ, FEC None      (0x48 0x00) 
    Register29: CRC on, first byte is length, FW_TERM_TX, initial CRC data 00  (0xB0 0x00)

Broadcast remote command over the Air:

    Register07: Channel 9 2411MHz (0x01 0x09)
    Preamble = ON (0xAAAAAA) 3 bytes
    SyncBytes = ON (0x147A, 0x258B) 2 bytes
    Trailer = ON (0x05) 4 bits little endian (1010)
    Length = (0x07) 1 byte
    CommandType = 0xB0 (color, white) or 0xB8 (group/all on/off) 1 byte
    RemoteID = (2 bytes) (this is securely stored into the bulb during remote-bulb sync at power on, max of 4 per bulb, syncing a 5th RemoteID drops the first one that is stored in that bulb. When clearing the bulb it removes all 4 remote ids from the bulb)
    Color = (0x00 to 0xFF) See colour chart above. 1 byte
    Brightness = (0x00 to 0xFF) See brightness chart above.  1 byte
    Command = (0x01 to 0x1A) See command list below. 1 byte
    Checksum = See checksum calc function code below. 2 Bytes
    Repeat again using Channel 40 2442MHz (Set Register 7: 0x01 0x28)
    Repeat again using Channel 71 2473MHz (Set Register 7: 0x01 0x47)
    Repeat again 5 to 40 times to ensure the bulbs receive the command. If sent a lower number of times, increase the time between sends.

Available Commands

    0x01, // All ON
    0x02, // All OFF
    0x03, // Group 1 ON
    0x04, // Group 1 OFF
    0x05, // Group 2 ON
    0x06, // Group 2 OFF
    0x07, // Group 3 ON
    0x08, // Group 3 OFF
    0x09, // Group 4 ON
    0x0A, // Group 4 OFF
    0x0B, // Disco Speed Increase
    0x0C, // Disco Speed Decrease
    0x0D, // Disco Mode
    0x11, // Set Color White - All Groups
    0x13, // Set Color White - Group 1
    0x15, // Set Color White - Group 2
    0x17, // Set Color White - Group 3
    0x19, // Set Color White - Group 4
    0x12, // Night Mode - All Groups
    0x14, // Night Mode - Group 1
    0x16, // Night Mode - Group 2
    0x18, // Night Mode - Group 3
    0x1A  // Night Mode - Group 4

//Thanks to Henryk and Erantimus for providing details and checksum code.
//Calculate Checksum - Returns 2 bytes.

    uint16 calc_crc(uint8 data[], data_length = 0x08) {
    uint16 state = 0;
    for (size i = 0; i < data_length; i++) {
        uint8 byte = data[i];
        for (int j = 0; j < 8; j++) {
        if ((byte ^ state) &#038; 0x01) {
            state = (state >> 1) ^ 0x8408;
        } else {
            state = state >> 1;
        }
        byte = byte >> 1;
        }
    }
    return state;
    }



## Code for Raspberry PI/ Raspberry PI2

Also see http://iqjar.com for RGBW bulbs.

for Dual White bulbs keep reading...
Modified iqjar.com code for Dual White Bulbs... download source code here:

https://drive.google.com/file/d/0B_1SX8Q4XAErZzBIc0pmQ09CMjg/view?usp=sharing

Modified for Dual White Bulbs by Mark Searle. 
Version 1.00 (14/07/2015) 
Works with the MiLight wireless bridge v3.0/v4.0 and with the MiLight Dual White LED bulbs.
For technical details visit: http://www.limitlessled.com/dev/
Download Source Code: https://drive.google.com/file/d/0B_1SX8Q4XAErZzBIc0pmQ09CMjg/view?usp=sharing

In src/command.cpp lines 66, 83 and 88 you may wish to try lowering the usleep value. The smaller value you can get away there the better. I can get away with smaller values but that might be because I’ve done the mod described here http://servernetworktech.com/2014/09/limitlessled-wifi-bridge-4-0-conversion-raspberry-pi/ to my bridge.


The optional ZONE argument specifies which bulb zone the command refers to.
If this argument is omitted, the command is considered to refer to all zones.
Possible values:

    ALL/0 - All zones
    1     - Zone 1
    2     - Zone 2
    3     - Zone 3
    4     - Zone 4
	
The COMMAND argument specifies the command to be sent to the given bulb zone.
Some commands require a parameter (see below)
Accepted commands:

    ON                 - Turn the bulbs in the given zone on.
    OFF                - Turn the bulbs in the given zone off.
    FB                 - Set the bulbs in the given zone to full brightness.
    NL                 - Set the bulbs in the given zone to night light mode. (0.4W)
    BU                 - Increase the brightness of the bulbs in the given zone by one increment.
    BD                 - Decrease the brightness of the bulbs in the given zone by one increment.
    TU                 - Increase the colour temperature of the bulbs in the given zone by one increment.
    TD                 - Decrease the colour temperature of the bulbs in the given zone by one increment.
    BRIGHTNESS/B VALUE - Set the brightness to integer value between 0 (dimmest) and 10 (brightest).
    TEMPERATURE/T VALUE- Set the colour temperature to integer value between 0 (warmest) and 10 (coolest).
                        note: Clumsy method must be used due to hardware limitations of the bulbs, may take a few
                        seconds to complete.  
    Note: For the commands BRIGHTNESS and TEMPERATURE only, a zone may be specified, but it will have no effect.
    Due to hardware limitations, all Dual White bulbs in all zones will accept the command.



## Python LED server, connect your raspberry PI to the circuit board inside the wifi bridge, bypassing the wifi to UART module.
http://servernetworktech.com/2014/09/limitlessled-wifi-bridge-4-0-conversion-raspberry-pi/
https://github.com/riptidewave93/RFLED-Server/blob/master/source/admin.py

File contents for led.py:

    #!/usr/bin/env python

    import socket
    import serial

    # Set LED Control server settings
    UDP_IP = '' # Leave empty for Broadcast support
    LED_PORT = 8899

    # Serial Settings
    TTL_PORT = "/dev/ttyAMA0"
    TTL_SPEED = 9600

    # Create UDP socket, bind to it
    sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
    sock.bind((UDP_IP, LED_PORT))

    while True:
        data, addr = sock.recvfrom(64) # buffer size is 64 bytes

        if data is not None:
            # print("led command: ", str(data)) # Debugging
            ser = serial.Serial(TTL_PORT, TTL_SPEED) # Connect to serial
            ser.write(data) # Write packet data out


File contents for admin.py:

    file contents for admin.py:
    #!/usr/bin/env python

    import socket

    # Set admin server settings
    UDP_IP = '' # Leave empty for Broadcast support
    ADMIN_PORT = 48899

    # Local settings of your Raspberry Pi, used for app discovery
    INT_IP = '10.0.1.61'
    INT_MAC = '111a02bf232b'

Code Starts Here

    # Create UDP socket, bind to it
    adminsock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
    adminsock.bind((UDP_IP, ADMIN_PORT))

    # Loop forever
    while True:
        admindata, adminaddr = adminsock.recvfrom(64) # buffer size is 64 bytes
        # Did we get a message?
        if admindata is not None: 
            # print("admin command: ", str(admindata)) # Debugging
            # If the client app is syncing to a unit
            if str(admindata).find("Link_Wi-Fi") != -1:
                RETURN = INT_IP + ',' + INT_MAC + ',' # Return our IP/MAC 
                # print("admin return: ", RETURN) # Debugging
                adminsock.sendto(bytes(RETURN, "utf-8"),adminaddr) # Send Response
            else:
                adminsock.sendto(bytes('+ok', "utf-8"),adminaddr) # Send OK for each packet we get
        else:
            break


